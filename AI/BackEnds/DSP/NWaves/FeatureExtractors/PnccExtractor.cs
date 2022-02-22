﻿using AI.BackEnds.DSP.NWaves.FeatureExtractors.Base;
using AI.BackEnds.DSP.NWaves.FeatureExtractors.Options;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors
{
    /// <summary>
    /// Power-Normalized Cepstral Coefficients extractor
    /// </summary>
    public class PnccExtractor : FeatureExtractor
    {
        /// <summary>
        /// Descriptions (simply "pncc0", "pncc1", "pncc2", etc.)
        /// </summary>
        public override List<string> FeatureDescriptions
        {
            get
            {
                List<string> names = Enumerable.Range(0, FeatureCount).Select(i => "pncc" + i).ToList();
                if (_includeEnergy)
                {
                    names[0] = "log_En";
                }

                return names;
            }
        }

        /// <summary>
        /// Window length for median-time power (2 * M + 1)
        /// </summary>
        public int M { get; set; } = 2;

        /// <summary>
        /// Window length for spectral smoothing (2 * N + 1)
        /// </summary>
        public int N { get; set; } = 4;

        /// <summary>
        /// Lambdas used in asymmetric noise suppression formula (4)
        /// </summary>
        public float LambdaA { get; set; } = 0.999f;
        public float LambdaB { get; set; } = 0.5f;

        /// <summary>
        /// Forgetting factor in temporal masking formula
        /// </summary>
        public float LambdaT { get; set; } = 0.85f;

        /// <summary>
        /// Forgetting factor in formula (15) in [Kim & Stern, 2016]
        /// </summary>
        public float LambdaMu { get; set; } = 0.999f;

        /// <summary>
        /// Threshold for detecting excitation/non-excitation segments
        /// </summary>
        public float C { get; set; } = 2;

        /// <summary>
        /// Multiplier in formula (12) in [Kim & Stern, 2016]
        /// </summary>
        public float MuT { get; set; } = 0.2f;

        /// <summary>
        /// Filterbank matrix of dimension [filterbankSize * (fftSize/2 + 1)].
        /// By default it's gammatone filterbank.
        /// </summary>
        public float[][] FilterBank { get; }

        /// <summary>
        /// Nonlinearity coefficient (if 0 then Log10 is applied)
        /// </summary>
        protected readonly int _power;

        /// <summary>
        /// Should the first PNCC coefficient be replaced with LOG(energy)
        /// </summary>
        protected readonly bool _includeEnergy;

        /// <summary>
        /// Floor value for LOG-energy calculation
        /// </summary>
        protected readonly float _logEnergyFloor;

        /// <summary>
        /// FFT transformer
        /// </summary>
        protected readonly RealFft _fft;

        /// <summary>
        /// DCT-II transformer
        /// </summary>
        protected readonly Dct2 _dct;

        /// <summary>
        /// Internal buffer for a signal spectrum at each step
        /// </summary>
        protected readonly float[] _spectrum;

        /// <summary>
        /// Internal buffers for gammatone spectrum and its derivatives
        /// </summary>
        protected readonly float[] _gammatoneSpectrum;
        protected readonly float[] _spectrumQOut;
        protected readonly float[] _filteredSpectrumQ;
        protected readonly float[] _spectrumS;
        protected readonly float[] _smoothedSpectrumS;
        protected readonly float[] _avgSpectrumQ1;
        protected readonly float[] _avgSpectrumQ2;
        protected readonly float[] _smoothedSpectrum;

        /// <summary>
        /// Value for mean normalization
        /// </summary>
        protected float _mean = 4e07f;

        /// <summary>
        /// Ring buffer for efficient processing of consecutive spectra
        /// </summary>
        protected readonly SpectraRingBuffer _ringBuffer;

        /// <summary>
        /// Step of PNCC algorithm
        /// </summary>
        protected int _step;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">PNCC options</param>
        public PnccExtractor(PnccOptions options) : base(options)
        {
            FeatureCount = options.FeatureCount;

            int filterbankSize = options.FilterBankSize;

            if (options.FilterBank == null)
            {
                _blockSize = options.FftSize > FrameSize ? options.FftSize : MathUtils.NextPowerOfTwo(FrameSize);

                FilterBank = FilterBanks.Erb(options.FilterBankSize, _blockSize, SamplingRate, options.LowFrequency, options.HighFrequency);
            }
            else
            {
                FilterBank = options.FilterBank;
                filterbankSize = FilterBank.Length;
                _blockSize = 2 * (FilterBank[0].Length - 1);

                Guard.AgainstExceedance(FrameSize, _blockSize, "frame size", "FFT size");
            }

            _fft = new RealFft(_blockSize);
            _dct = new Dct2(filterbankSize);

            _power = options.Power;

            _includeEnergy = options.IncludeEnergy;
            _logEnergyFloor = options.LogEnergyFloor;

            _spectrum = new float[_blockSize / 2 + 1];
            _spectrumQOut = new float[filterbankSize];
            _gammatoneSpectrum = new float[filterbankSize];
            _filteredSpectrumQ = new float[filterbankSize];
            _spectrumS = new float[filterbankSize];
            _smoothedSpectrumS = new float[filterbankSize];
            _avgSpectrumQ1 = new float[filterbankSize];
            _avgSpectrumQ2 = new float[filterbankSize];
            _smoothedSpectrum = new float[filterbankSize];

            _ringBuffer = new SpectraRingBuffer(2 * M + 1, filterbankSize);

            _step = M - 1;
        }

        /// <summary>
        /// PNCC algorithm according to [Kim & Stern, 2016]:
        /// Decompose signal into overlapping (hopSize) frames of length fftSize. In each frame do:
        /// 
        ///     0) Apply window (base extractor does it)
        ///     1) Obtain power spectrum
        ///     2) Apply gammatone filters (squared)
        ///     3) Medium-time processing (asymmetric noise suppression, temporal masking, spectral smoothing)
        ///     4) Apply nonlinearity
        ///     5) Do dct-II (normalized)
        /// 
        /// </summary>
        /// <param name="block">Block of samples for analysis</param>
        /// <param name="features">List of pncc vectors</param>
        public override void ProcessFrame(float[] block, float[] features)
        {
            const float MeanPower = 1e10f;
            const float Epsilon = 2.22e-16f;

            _step++;

            // 1) calculate power spectrum

            _fft.PowerSpectrum(block, _spectrum, false);

            // 2) apply gammatone filterbank

            FilterBanks.Apply(FilterBank, _spectrum, _gammatoneSpectrum);


            // =============================================================
            // 3) medium-time processing blocks:

            // 3.1) temporal integration (zero-phase moving average filter)

            _ringBuffer.Add(_gammatoneSpectrum);

            float[] spectrumQ = _ringBuffer.AverageSpectrum;

            // 3.2) asymmetric noise suppression

            if (_step == 2 * M)
            {
                for (int j = 0; j < _spectrumQOut.Length; j++)
                {
                    _spectrumQOut[j] = spectrumQ[j] * 0.9f;
                }
            }

            // first 2*M vectors are zeros

            if (_step < 2 * M)
            {
                return;
            }

            for (int j = 0; j < _spectrumQOut.Length; j++)
            {
                if (spectrumQ[j] > _spectrumQOut[j])
                {
                    _spectrumQOut[j] = LambdaA * _spectrumQOut[j] + (1 - LambdaA) * spectrumQ[j];
                }
                else
                {
                    _spectrumQOut[j] = LambdaB * _spectrumQOut[j] + (1 - LambdaB) * spectrumQ[j];
                }
            }

            for (int j = 0; j < _filteredSpectrumQ.Length; j++)
            {
                _filteredSpectrumQ[j] = Math.Max(spectrumQ[j] - _spectrumQOut[j], 0.0f);

                if (_step == 2 * M)
                {
                    _avgSpectrumQ1[j] = 0.9f * _filteredSpectrumQ[j];
                    _avgSpectrumQ2[j] = _filteredSpectrumQ[j];
                }

                if (_filteredSpectrumQ[j] > _avgSpectrumQ1[j])
                {
                    _avgSpectrumQ1[j] = LambdaA * _avgSpectrumQ1[j] + (1 - LambdaA) * _filteredSpectrumQ[j];
                }
                else
                {
                    _avgSpectrumQ1[j] = LambdaB * _avgSpectrumQ1[j] + (1 - LambdaB) * _filteredSpectrumQ[j];
                }

                // 3.3) temporal masking

                float threshold = _filteredSpectrumQ[j];

                _avgSpectrumQ2[j] *= LambdaT;
                if (spectrumQ[j] < C * _spectrumQOut[j])
                {
                    _filteredSpectrumQ[j] = _avgSpectrumQ1[j];
                }
                else
                {
                    if (_filteredSpectrumQ[j] <= _avgSpectrumQ2[j])
                    {
                        _filteredSpectrumQ[j] = MuT * _avgSpectrumQ2[j];
                    }
                }
                _avgSpectrumQ2[j] = Math.Max(_avgSpectrumQ2[j], threshold);

                _filteredSpectrumQ[j] = Math.Max(_filteredSpectrumQ[j], _avgSpectrumQ1[j]);
            }


            // 3.4) spectral smoothing 

            for (int j = 0; j < _spectrumS.Length; j++)
            {
                _spectrumS[j] = _filteredSpectrumQ[j] / Math.Max(spectrumQ[j], Epsilon);
            }

            for (int j = 0; j < _smoothedSpectrumS.Length; j++)
            {
                _smoothedSpectrumS[j] = 0.0f;

                int total = 0;
                for (int k = Math.Max(j - N, 0);
                         k < Math.Min(j + N + 1, FilterBank.Length);
                         k++, total++)
                {
                    _smoothedSpectrumS[j] += _spectrumS[k];
                }
                _smoothedSpectrumS[j] /= total;
            }

            // 3.5) mean power normalization

            float[] centralSpectrum = _ringBuffer.CentralSpectrum;

            float sumPower = 0.0f;
            for (int j = 0; j < _smoothedSpectrum.Length; j++)
            {
                _smoothedSpectrum[j] = _smoothedSpectrumS[j] * centralSpectrum[j];
                sumPower += _smoothedSpectrum[j];
            }

            _mean = LambdaMu * _mean + (1 - LambdaMu) * sumPower;

            for (int j = 0; j < _smoothedSpectrum.Length; j++)
            {
                _smoothedSpectrum[j] /= _mean;
                _smoothedSpectrum[j] *= MeanPower;
            }

            // =============================================================

            // 4) nonlinearity (power ^ d  or  Log)

            if (_power != 0)
            {
                for (int j = 0; j < _smoothedSpectrum.Length; j++)
                {
                    _smoothedSpectrum[j] = (float)Math.Pow(_smoothedSpectrum[j], 1.0 / _power);
                }
            }
            else
            {
                for (int j = 0; j < _smoothedSpectrum.Length; j++)
                {
                    _smoothedSpectrum[j] = (float)Math.Log(_smoothedSpectrum[j] + Epsilon);
                }
            }

            // 5) dct-II (Norm = normalized)

            _dct.DirectNorm(_smoothedSpectrum, features);

            // 6) (optional) replace first coeff with log(energy) 

            if (_includeEnergy)
            {
                features[0] = (float)Math.Log(Math.Max(block.Sum(x => x * x), _logEnergyFloor));
            }

            // wow, who knows, maybe it'll happen!

            if (_step == int.MaxValue - 1)
            {
                _step = 2 * M + 1;
            }
        }

        /// <summary>
        /// Reset state
        /// </summary>
        public override void Reset()
        {
            _step = M - 1;
            _mean = 4e07f;
            _ringBuffer.Reset();
        }


        /// <summary>
        /// Helper Ring Buffer class for efficient processing of consecutive spectra
        /// </summary>
        protected class SpectraRingBuffer
        {
            protected readonly float[][] _spectra;
            protected int _count;
            protected int _capacity;
            protected int _current;

            public float[] CentralSpectrum;
            public float[] AverageSpectrum;

            public SpectraRingBuffer(int capacity, int spectrumSize)
            {
                _spectra = new float[capacity][];
                _capacity = capacity;
                _count = 0;
                _current = 0;
                AverageSpectrum = new float[spectrumSize];
            }

            public void Add(float[] spectrum)
            {
                if (_count < _capacity)
                {
                    _count++;
                }

                _spectra[_current] = spectrum;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    AverageSpectrum[j] = 0.0f;
                    for (int i = 0; i < _count; i++)
                    {
                        AverageSpectrum[j] += _spectra[i][j];
                    }
                    AverageSpectrum[j] /= _count;
                }

                CentralSpectrum = _spectra[(_current + _capacity / 2 + 1) % _capacity];

                _current = (_current + 1) % _capacity;
            }

            public void Reset()
            {
                _count = 0;
                _current = 0;

                Array.Clear(AverageSpectrum, 0, AverageSpectrum.Length);

                foreach (float[] spectrum in _spectra)
                {
                    Array.Clear(spectrum, 0, spectrum.Length);
                }
            }
        }
    }
}
