using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Operations
{
    /// <summary>
    /// Class that implements Spectral Вычитание algorithm according to
    /// 
    /// [1979] M. Berouti, R. Schwartz, J. Makhoul
    /// "Enhancement of Speech Corrupted by Acoustic Noise".
    /// 
    /// </summary>
    [Serializable]
    public class SpectralSubtractor : OverlapAddFilter
    {
        // Algorithm parameters
        /// <summary>
        /// 
        /// </summary>
        public float Beta { get; set; } = 0.009f;
        /// <summary>
        /// 
        /// </summary>
        public float AlphaMin { get; set; } = 2f;
        /// <summary>
        /// 
        /// </summary>
        public float AlphaMax { get; set; } = 5f;
        /// <summary>
        /// 
        /// </summary>
        public float SnrMin { get; set; } = -5f;
        /// <summary>
        /// 
        /// </summary>
        public float SnrMax { get; set; } = 20f;

        /// <summary>
        /// Noise estimate
        /// </summary>
        private readonly float[] _noiseEstimate;

        // Internal buffers for noise estimation

        private readonly float[] _noiseBuf;
        private readonly float[] _noiseSpectrum;
        private readonly float[] _noiseAcc;

        /// <summary>
        /// Constructor from float[] noise
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="fftSize"></param>
        /// <param name="hopSize"></param>
        public SpectralSubtractor(float[] noise, int fftSize = 1024, int hopSize = 128) : base(hopSize, fftSize)
        {
            _noiseEstimate = new float[(_fftSize / 2) + 1];
            _noiseBuf = new float[_fftSize];
            _noiseSpectrum = new float[(_fftSize / 2) + 1];
            _noiseAcc = new float[(_fftSize / 2) + 2];

            EstimateNoise(noise);
        }

        /// <summary>
        /// Constructor from DiscreteSignal noise
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="fftSize"></param>
        /// <param name="hopSize"></param>
        public SpectralSubtractor(DiscreteSignal noise, int fftSize = 1024, int hopSize = 128)
            : this(noise.Samples, fftSize, hopSize)
        {
        }

        /// <summary>
        /// Process one spectrum at each STFT step
        /// </summary>
        /// <param name="re">Real parts of input spectrum</param>
        /// <param name="im">Imaginary parts of input spectrum</param>
        /// <param name="filteredRe">Real parts of output spectrum</param>
        /// <param name="filteredIm">Imaginary parts of output spectrum</param>
        public override void ProcessSpectrum(float[] re, float[] im,
                                             float[] filteredRe, float[] filteredIm)
        {
            float k = (AlphaMin - AlphaMax) / (SnrMax - SnrMin);
            float b = AlphaMax - (k * SnrMin);

            for (int j = 1; j <= _fftSize / 2; j++)
            {
                float power = (re[j] * re[j]) + (im[j] * im[j]);
                double phase = Math.Atan2(im[j], re[j]);

                float noisePower = _noiseEstimate[j];

                double snr = 10 * Math.Log10(power / noisePower);
                double alpha = Math.Max(Math.Min((k * snr) + b, AlphaMax), AlphaMin);

                double diff = power - (alpha * noisePower);

                double mag = Math.Sqrt(Math.Max(diff, Beta * noisePower));

                filteredRe[j] = (float)(mag * Math.Cos(phase));
                filteredIm[j] = (float)(mag * Math.Sin(phase));
            }
        }

        /// <summary>
        /// Estimate noise power spectrum
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void EstimateNoise(float[] noise, int startPos = 0, int endPos = -1)
        {
            if (endPos < 0)
            {
                endPos = noise.Length + endPos + 1;
            }

            int numFrames = 0;

            for (int pos = startPos; pos + _fftSize < endPos; pos += _hopSize, numFrames++)
            {
                noise.FastCopyTo(_noiseBuf, _fftSize, pos);

                _fft.PowerSpectrum(_noiseBuf, _noiseSpectrum, false);

                for (int j = 1; j <= _fftSize / 2; j++)
                {
                    _noiseAcc[j] += _noiseSpectrum[j];
                }
            }

            // (including smoothing)

            for (int j = 1; j <= _fftSize / 2; j++)
            {
                _noiseEstimate[j] = (_noiseAcc[j - 1] + _noiseAcc[j] + _noiseAcc[j + 1]) / (3 * numFrames);
            }
        }

        /// <summary>
        /// Estimate noise power spectrum
        /// </summary>
        /// <param name="noise"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void EstimateNoise(DiscreteSignal noise, int startPos = 0, int endPos = -1)
        {
            EstimateNoise(noise.Samples, startPos, endPos);
        }
    }
}
