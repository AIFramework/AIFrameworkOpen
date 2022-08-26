﻿using AI.BackEnds.DSP.NWaves.FeatureExtractors.Base;
using AI.BackEnds.DSP.NWaves.FeatureExtractors.Options;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors
{
    /// <summary>
    /// Amplitude modulation spectra (AMS) extractor
    /// </summary>
    [Serializable]
    public class AmsExtractor : FeatureExtractor
    {
        /// <summary>
        /// Feature descriptions.
        /// Initialized in constructor in the following manner (example):
        /// 
        ///     band_1_mf_0.5_Hz   band_1_mf_1.0_Hz   ...    band_1_mf_8.0_Hz
        ///     band_2_mf_0.5_Hz   band_2_mf_1.0_Hz   ...    band_2_mf_8.0_Hz
        ///     ...
        ///     band_32_mf_0.5_Hz  band_32_mf_1.0_Hz  ...    band_32_mf_8.0_Hz
        /// 
        /// </summary>
        public override List<string> FeatureDescriptions { get; }

        /// <summary>
        /// The "featuregram": the sequence of (feature) vectors;
        /// if this sequence is given, then AmsExtractor computes 
        /// modulation spectral coefficients from sequences in each 'feature channel'.
        /// </summary>
        protected readonly float[][] _featuregram;

        /// <summary>
        /// Filterbank matrix of dimension [filterCount * (fftSize/2 + 1)]
        /// </summary>
        protected readonly float[][] _filterbank;
        public float[][] Filterbank => _filterbank;

        /// <summary>
        /// Signal envelopes in different frequency bands
        /// </summary>
        protected float[][] _envelopes;
        public float[][] Envelopes => _envelopes;

        /// <summary>
        /// Size of FFT
        /// </summary>
        protected readonly int _fftSize;

        /// <summary>
        /// FFT transformer
        /// </summary>
        protected readonly RealFft _fft;

        /// <summary>
        /// FFT transformer for modulation spectrum
        /// </summary>
        protected readonly RealFft _modulationFft;

        /// <summary>
        /// Size of FFT applied to signal envelopes
        /// </summary>
        protected readonly int _modulationFftSize;

        /// <summary>
        /// Hop size for analysis of signal envelopes
        /// </summary>
        protected readonly int _modulationHopSize;

        /// <summary>
        /// Internal buffer for a signal block at each step
        /// </summary>
        protected readonly float[] _block;

        /// <summary>
        /// Internal buffer for a signal spectrum at each step
        /// </summary>
        protected readonly float[] _spectrum;

        /// <summary>
        /// Internal buffer for filtered spectrum
        /// </summary>
        protected readonly float[] _filteredSpectrum;

        /// <summary>
        /// Internal buffer for modulation spectrum analysis
        /// </summary>
        protected readonly float[] _modBlock;

        /// <summary>
        /// Modulation spectrum (in one band)
        /// </summary>
        protected readonly float[] _modSpectrum;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">AMS options</param>
        public AmsExtractor(AmsOptions options) : base(options)
        {
            _modulationFftSize = options.ModulationFftSize;
            _modulationHopSize = options.ModulationHopSize;
            _modulationFft = new RealFft(_modulationFftSize);

            _featuregram = options.Featuregram?.ToArray();

            if (_featuregram != null)
            {
                FeatureCount = _featuregram[0].Length * ((_modulationFftSize / 2) + 1);
            }
            else
            {
                if (options.FilterBank == null)
                {
                    _fftSize = options.FftSize > FrameSize ? options.FftSize : MathUtils.NextPowerOfTwo(FrameSize);

                    _filterbank = FilterBanks.Triangular(_fftSize, SamplingRate,
                                     FilterBanks.MelBands(12, SamplingRate, 100, 3200));
                }
                else
                {
                    _filterbank = options.FilterBank;
                    _fftSize = 2 * (_filterbank[0].Length - 1);

                    Guard.AgainstExceedance(FrameSize, _fftSize, "frame size", "FFT size");
                }

                _fft = new RealFft(_fftSize);

                FeatureCount = _filterbank.Length * ((_modulationFftSize / 2) + 1);

                _spectrum = new float[(_fftSize / 2) + 1];
                _filteredSpectrum = new float[_filterbank.Length];
                _block = new float[_fftSize];
            }

            _modBlock = new float[_modulationFftSize];
            _modSpectrum = new float[(_modulationFftSize / 2) + 1];

            // feature descriptions

            int length;
            if (_featuregram != null)
            {
                length = _featuregram[0].Length;
            }
            else
            {
                length = _filterbank.Length;
            }

            FeatureDescriptions = new List<string>();

            float modulationSamplingRate = (float)SamplingRate / HopSize;
            float resolution = modulationSamplingRate / _modulationFftSize;

            for (int fi = 0; fi < length; fi++)
            {
                for (int fj = 0; fj <= _modulationFftSize / 2; fj++)
                {
                    FeatureDescriptions.Add(string.Format("band_{0}_mf_{1:F2}_Hz", fi + 1, fj * resolution));
                }
            }
        }

        /// <summary>
        /// Method for computing modulation spectra.
        /// Each vector representing one modulation spectrum is a flattened version of 2D spectrum.
        /// </summary>
        /// <param name="samples">Samples for analysis</param>
        /// <param name="startSample">The number (position) of the first sample for processing</param>
        /// <param name="endSample">The number (position) of last sample for processing</param>
        /// <returns>List of flattened modulation spectra</returns>
        public override List<float[]> ComputeFrom(float[] samples, int startSample, int endSample)
        {
            Guard.AgainstInvalidRange(startSample, endSample, "starting pos", "ending pos");

            int frameSize = FrameSize;
            int hopSize = HopSize;

            List<float[]> featureVectors = new List<float[]>();

            int en = 0;
            int i = startSample;

            if (_featuregram == null)
            {
                _envelopes = new float[_filterbank.Length][];
                for (int n = 0; n < _envelopes.Length; n++)
                {
                    _envelopes[n] = new float[samples.Length / hopSize];
                }

                float prevSample = startSample > 0 ? samples[startSample - 1] : 0.0f;

                int lastSample = endSample - Math.Max(frameSize, hopSize);

                // ===================== compute local FFTs (do STFT) =======================

                for (i = startSample; i < lastSample; i += hopSize)
                {
                    // copy frameSize samples
                    samples.FastCopyTo(_block, frameSize, i);
                    // fill zeros to fftSize if frameSize < fftSize
                    for (int k = frameSize; k < _block.Length; _block[k++] = 0)
                    {
                        ;
                    }


                    // 0) pre-emphasis (if needed)

                    if (_preEmphasis > 1e-10f)
                    {
                        for (int k = 0; k < frameSize; k++)
                        {
                            float y = _block[k] - (prevSample * _preEmphasis);
                            prevSample = _block[k];
                            _block[k] = y;
                        }
                        prevSample = samples[i + hopSize - 1];
                    }

                    // 1) apply window

                    if (_window != WindowTypes.Rectangular)
                    {
                        _block.ApplyWindow(_windowSamples);
                    }

                    // 2) calculate power spectrum

                    _fft.PowerSpectrum(_block, _spectrum);

                    // 3) apply filterbank...

                    FilterBanks.Apply(_filterbank, _spectrum, _filteredSpectrum);

                    // ...and save results for future calculations

                    for (int n = 0; n < _envelopes.Length; n++)
                    {
                        _envelopes[n][en] = _filteredSpectrum[n];
                    }
                    en++;
                }
            }
            else
            {
                en = _featuregram.Length;
                _envelopes = new float[_featuregram[0].Length][];

                for (int n = 0; n < _envelopes.Length; n++)
                {
                    _envelopes[n] = new float[en];
                    for (i = 0; i < en; i++)
                    {
                        _envelopes[n][i] = _featuregram[i][n];
                    }
                }
            }

            // =========================== modulation analysis =======================

            int envelopeLength = en;

            // long-term AVG-normalization

            foreach (float[] envelope in _envelopes)
            {
                float avg = 0.0f;
                for (int k = 0; k < envelopeLength; k++)
                {
                    avg += (k >= 0) ? envelope[k] : -envelope[k];
                }
                avg /= envelopeLength;

                if (avg >= 1e-10f)   // this happens more frequently
                {
                    for (int k = 0; k < envelopeLength; k++)
                    {
                        envelope[k] /= avg;
                    }
                }
            }

            i = 0;
            while (i < envelopeLength)
            {
                float[] vector = new float[_envelopes.Length * ((_modulationFftSize / 2) + 1)];
                int offset = 0;

                foreach (float[] envelope in _envelopes)
                {
                    // copy modFftSize samples (or envelopeLength - i in the end)
                    int len = Math.Min(_modulationFftSize, envelopeLength - i);
                    envelope.FastCopyTo(_modBlock, len, i);
                    // fill zeros to modFftSize if len < modFftSize
                    for (int k = len; k < _modBlock.Length; _modBlock[k++] = 0) { }

                    _modulationFft.PowerSpectrum(_modBlock, _modSpectrum);
                    _modSpectrum.FastCopyTo(vector, _modSpectrum.Length, 0, offset);

                    offset += _modSpectrum.Length;
                }

                featureVectors.Add(vector);

                i += _modulationHopSize;
            }

            return featureVectors;
        }

        /// <summary>
        /// Get 2D modulation spectrum from its flattened version.
        /// Axes are: [short-time-frequency] x [modulation-frequency].
        /// </summary>
        /// <param name="featureVector"></param>
        /// <returns></returns>
        public float[][] MakeSpectrum2D(float[] featureVector)
        {
            int length = _filterbank?.Length ?? _featuregram[0].Length;

            float[][] spectrum = new float[length][];
            int spectrumSize = (_modulationFftSize / 2) + 1;

            int offset = 0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = featureVector.FastCopyFragment(spectrumSize, offset);
                offset += spectrumSize;
            }

            return spectrum;
        }

        /// <summary>
        /// Get sequence of short-time spectra corresponding to particular modulation frequency
        /// (by default, the most perceptually important modulation frequency of 4 Hz).
        /// </summary>
        /// <param name="featureVectors"></param>
        /// <param name="herz"></param>
        /// <returns>Short-time spectra corresponding to particular modulation frequency</returns>
        public List<float[]> VectorsAtHerz(IList<float[]> featureVectors, float herz = 4)
        {
            int length = _filterbank?.Length ?? _featuregram[0].Length;
            float modulationSamplingRate = (float)SamplingRate / HopSize;
            float resolution = modulationSamplingRate / _modulationFftSize;
            int freq = (int)Math.Round(herz / resolution);

            int spectrumSize = (_modulationFftSize / 2) + 1;

            List<float[]> freqVectors = new List<float[]>();
            foreach (float[] vector in featureVectors)
            {
                float[] spectrum = new float[length];
                for (int i = 0; i < spectrum.Length; i++)
                {
                    spectrum[i] = vector[freq + (i * spectrumSize)];
                }
                freqVectors.Add(spectrum);
            }

            return freqVectors;
        }

        /// <summary>
        /// All logic is fully implemented in ComputeFrom() method
        /// </summary>
        /// <param name="block"></param>
        /// <param name="features"></param>
        public override void ProcessFrame(float[] block, float[] features)
        {
            throw new NotImplementedException("AmsExtractor does not provide this function. Please call ComputeFrom() method");
        }

        public override void ComputeFrom(float[] samples, int startSample, int endSample, IList<float[]> vectors)
        {
            throw new NotImplementedException("AmsExtractor does not provide this function. Please call overloaded ComputeFrom() method");
        }
    }
}
