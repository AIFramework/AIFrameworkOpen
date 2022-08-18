﻿using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Class providing methods for various cepstrum transforms:
    /// 
    ///     1) Complex cepstrum (direct and inverse)
    ///     2) Real cepstrum
    ///     3) Power cepstrum
    ///     4) Phase cepstrum
    ///     
    /// 1) and 2) are analogous to MATLAB cceps/icceps and rceps, respectively.
    /// 
    /// </summary>
    public class CepstralTransform
    {
        /// <summary>
        /// Size of cepstrum
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// FFT transformer
        /// </summary>
        private readonly Fft _fft;

        /// <summary>
        /// Logarithm base (E or 10)
        /// </summary>
        private readonly double _logBase;

        /// <summary>
        /// Intermediate buffer storing real parts of spectrum
        /// </summary>
        private readonly float[] _realSpectrum;

        /// <summary>
        /// Intermediate buffer storing imaginary parts of spectrum
        /// </summary>
        private readonly float[] _imagSpectrum;

        /// <summary>
        /// Intermediate buffer storing unwrapped phase
        /// </summary>
        private readonly double[] _unwrapped;

        /// <summary>
        /// Constructor with necessary parameters
        /// </summary>
        /// <param name="cepstrumSize"></param>
        /// <param name="fftSize"></param>
        /// <param name="logBase"></param>
        public CepstralTransform(int cepstrumSize, int fftSize = 0, double logBase = Math.E)
        {
            Size = cepstrumSize;

            if (cepstrumSize > fftSize)
            {
                fftSize = MathUtils.NextPowerOfTwo(cepstrumSize);
            }

            _fft = new Fft(fftSize);

            _logBase = logBase;

            _realSpectrum = new float[fftSize];
            _imagSpectrum = new float[fftSize];
            _unwrapped = new double[fftSize];
        }

        /// <summary>
        /// Direct complex cepstral transform:
        /// 
        /// Real{IFFT(log(abs(FFT(x)) + unwrapped_phase))}
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cepstrum"></param>
        /// <returns></returns>
        public double Direct(float[] input, float[] cepstrum)
        {
            Array.Clear(_realSpectrum, 0, _realSpectrum.Length);
            Array.Clear(_imagSpectrum, 0, _imagSpectrum.Length);

            input.FastCopyTo(_realSpectrum, input.Length);

            // complex fft

            _fft.Direct(_realSpectrum, _imagSpectrum);

            // complex logarithm of magnitude spectrum

            // the most difficult part is phase unwrapping which is slightly different from MathUtils.Unwrap

            double offset = 0.0;
            _unwrapped[0] = 0.0;

            double prevPhase = Math.Atan2(_imagSpectrum[0], _realSpectrum[0]);

            for (int n = 1; n < _unwrapped.Length; n++)
            {
                double phase = Math.Atan2(_imagSpectrum[n], _realSpectrum[n]);

                double delta = phase - prevPhase;

                if (delta > Math.PI)
                {
                    offset -= 2 * Math.PI;
                }
                else if (delta < -Math.PI)
                {
                    offset += 2 * Math.PI;
                }

                _unwrapped[n] = phase + offset;
                prevPhase = phase;
            }

            int mid = _realSpectrum.Length / 2;
            double delay = Math.Round(_unwrapped[mid] / Math.PI);

            for (int i = 0; i < _realSpectrum.Length; i++)
            {
                _unwrapped[i] -= Math.PI * delay * i / mid;

                double mag = Math.Sqrt((_realSpectrum[i] * _realSpectrum[i]) + (_imagSpectrum[i] * _imagSpectrum[i]));

                _realSpectrum[i] = (float)Math.Log(mag + float.Epsilon, _logBase);
                _imagSpectrum[i] = (float)_unwrapped[i];
            }

            // complex ifft

            _fft.Inverse(_realSpectrum, _imagSpectrum);

            // take truncated part

            _realSpectrum.FastCopyTo(cepstrum, Size);

            // normalize

            for (int i = 0; i < cepstrum.Length; i++)
            {
                cepstrum[i] /= _fft.Size;
            }

            return delay;
        }

        /// <summary>
        /// Direct complex cepstral transform
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal Direct(DiscreteSignal signal)
        {
            float[] cepstrum = new float[Size];
            Direct(signal.Samples, cepstrum);
            return new DiscreteSignal(signal.SamplingRate, cepstrum);
        }

        /// <summary>
        /// Inverse complex cepstral transform
        /// </summary>
        /// <param name="cepstrum"></param>
        /// <param name="output"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public void Inverse(float[] cepstrum, float[] output, double delay = 0)
        {
            Array.Clear(_realSpectrum, 0, _realSpectrum.Length);
            Array.Clear(_imagSpectrum, 0, _imagSpectrum.Length);

            cepstrum.FastCopyTo(_realSpectrum, cepstrum.Length);

            // complex fft

            _fft.Direct(_realSpectrum, _imagSpectrum);

            // complex exp() of spectrum

            int mid = _realSpectrum.Length / 2;

            for (int i = 0; i < _realSpectrum.Length; i++)
            {
                float mag = _realSpectrum[i];
                double phase = _imagSpectrum[i] + (Math.PI * delay * i / mid);

                _realSpectrum[i] = (float)(Math.Pow(_logBase, mag) * Math.Cos(phase));
                _imagSpectrum[i] = (float)(Math.Pow(_logBase, mag) * Math.Sin(phase));
            }

            // complex ifft

            _fft.Inverse(_realSpectrum, _imagSpectrum);

            // take truncated part

            _realSpectrum.FastCopyTo(output, output.Length);

            // normalize

            for (int i = 0; i < output.Length; i++)
            {
                output[i] /= _fft.Size;
            }
        }

        /// <summary>
        /// Inverse complex cepstral transform
        /// </summary>
        /// <param name="cepstrum"></param>
        /// <returns></returns>
        public DiscreteSignal Inverse(DiscreteSignal cepstrum)
        {
            float[] output = new float[_realSpectrum.Length];
            Inverse(cepstrum.Samples, output);
            return new DiscreteSignal(cepstrum.SamplingRate, output);
        }

        /// <summary>
        /// Real cepstrum:
        /// 
        /// real{IFFT(log(abs(FFT(x))))}
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cepstrum"></param>
        public void RealCepstrum(float[] input, float[] cepstrum)
        {
            Array.Clear(_realSpectrum, 0, _realSpectrum.Length);
            Array.Clear(_imagSpectrum, 0, _imagSpectrum.Length);

            input.FastCopyTo(_realSpectrum, input.Length);

            // complex fft

            _fft.Direct(_realSpectrum, _imagSpectrum);

            // logarithm of magnitude spectrum

            for (int i = 0; i < _realSpectrum.Length; i++)
            {
                double mag = Math.Sqrt((_realSpectrum[i] * _realSpectrum[i]) + (_imagSpectrum[i] * _imagSpectrum[i]));

                _realSpectrum[i] = (float)Math.Log(mag + float.Epsilon, _logBase);
                _imagSpectrum[i] = 0.0f;
            }

            // complex ifft

            _fft.Inverse(_realSpectrum, _imagSpectrum);

            // take truncated part

            _realSpectrum.FastCopyTo(cepstrum, Size);

            // normalize

            for (int i = 0; i < cepstrum.Length; i++)
            {
                cepstrum[i] /= _fft.Size;
            }
        }

        /// <summary>
        /// Wiki:
        /// power_cepstrum = 4 * real_cepstrum ^ 2
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cepstrum"></param>
        public void PowerCepstrum(float[] input, float[] cepstrum)
        {
            RealCepstrum(input, cepstrum);

            for (int i = 0; i < cepstrum.Length; i++)
            {
                float pc = 4 * cepstrum[i] * cepstrum[i];

                cepstrum[i] = pc;
            }
        }

        /// <summary>
        /// Wiki:
        /// phase_cepstrum = (complex_cepstrum - reversed_complex_cepstrum) ^ 2
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cepstrum"></param>
        public void PhaseCepstrum(float[] input, float[] cepstrum)
        {
            Direct(input, cepstrum);

            // use this free memory block for storing reversed cepstrum
            cepstrum.FastCopyTo(_realSpectrum, cepstrum.Length);

            for (int i = 0; i < cepstrum.Length; i++)
            {
                float pc = cepstrum[i] - _realSpectrum[cepstrum.Length - 1 - i];

                cepstrum[i] = pc * pc;
            }
        }
    }
}
