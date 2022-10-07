﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations.Tsm
{
    /// <summary>
    /// Waveform-Synchronized Overlap-Add
    /// </summary>
    [Serializable]
    public class Wsola : IFilter
    {
        /// <summary>
        /// Коэффициент растяжения
        /// </summary>
        protected readonly double _stretch;

        /// <summary>
        /// Window size
        /// </summary>
        private int _windowSize;

        /// <summary>
        /// Hop size at analysis stage (STFT decomposition)
        /// </summary>
        private int _hopAnalysis;

        /// <summary>
        /// Hop size at synthesis stage (STFT merging)
        /// </summary>
        private int _hopSynthesis;

        /// <summary>
        /// Maximum length of the fragment for search of the most similar waveform
        /// </summary>
        private int _maxDelta;

        /// <summary>
        /// True if parameters were set by user (not by default)
        /// </summary>
        private readonly bool _userParameters;

        /// <summary>
        /// Internal convolver
        /// (will be used for evaluating auto-correlation if the window size is too big)
        /// </summary>
        private Convolver _convolver;

        /// <summary>
        /// Cross-correlation signal
        /// </summary>
        private float[] _cc;


        /// <summary>
        /// Конструктор with detailed WSOLA settings
        /// </summary>
        /// <param name="stretch">Коэффициент растяжения</param>
        /// <param name="windowSize"></param>
        /// <param name="hopAnalysis"></param>
        /// <param name="maxDelta"></param>
        public Wsola(double stretch, int windowSize, int hopAnalysis, int maxDelta = 0)
        {
            _stretch = stretch;
            _windowSize = Math.Max(windowSize, 32);
            _hopAnalysis = Math.Max(hopAnalysis, 10);
            _hopSynthesis = (int)(_hopAnalysis * stretch);
            _maxDelta = maxDelta > 2 ? maxDelta : _hopSynthesis;

            _userParameters = true;

            PrepareConvolver();
        }

        /// <summary>
        /// Конструктор with smart parameter autoderivation 
        /// </summary>
        /// <param name="stretch"></param>
        public Wsola(double stretch)
        {
            _stretch = stretch;

            // IMO these are good parameters for different Коэффициент растяженияs

            if (_stretch > 1.5)        // parameters are for 22.05 kHz Частота дискретизации, so they will be adjusted for an Входной сигнал
            {
                _windowSize = 1024;     // 46,4 ms
                _hopAnalysis = 128;     //  5,8 ms
            }
            else if (_stretch > 1.1)
            {
                _windowSize = 1536;     // 69,7 ms
                _hopAnalysis = 256;     // 10,6 ms
            }
            else if (_stretch > 0.6)
            {
                _windowSize = 1536;     // 69,7 ms
                _hopAnalysis = 690;     // 31,3 ms
            }
            else
            {
                _windowSize = 1024;     // 46,4 ms
                _hopAnalysis = 896;     // 40,6 ms
            }

            _hopSynthesis = (int)(_hopAnalysis * stretch);
            _maxDelta = _hopSynthesis;

            PrepareConvolver();
        }

        /// <summary>
        /// For large window sizes prepare the internal convolver
        /// </summary>
        private void PrepareConvolver()
        {
            int fftSize = MathUtils.NextPowerOfTwo(_windowSize + _maxDelta - 1);

            if (fftSize >= 512)
            {
                _convolver = new Convolver(fftSize);
                _cc = new float[fftSize];
            }
        }

        /// <summary>
        /// WSOLA algorithm
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyTo(DiscreteSignal signal,
                                      FilteringMethod method = FilteringMethod.Auto)
        {
            // adjust default parameters for a new Частота дискретизации

            if (signal.SamplingRate != 22050 && !_userParameters)
            {
                float factor = (float)signal.SamplingRate / 22050;

                _windowSize = (int)(_windowSize * factor);
                _hopAnalysis = (int)(_hopAnalysis * factor);
                _hopSynthesis = (int)(_hopAnalysis * _stretch);
                _maxDelta = (int)(_maxDelta * factor);

                PrepareConvolver();
            }

            float[] window = Window.OfType(WindowTypes.Hann, _windowSize);
            float gain = _hopSynthesis / window.Select(w => w * w).Sum() * 0.75f;

            // and now WSOLA:

            float[] input = signal.Samples;
            float[] output = new float[(int)(_stretch * (input.Length + _windowSize))];

            float[] current = new float[_windowSize + _maxDelta];
            float[] prev = new float[_windowSize];

            int posSynthesis = 0;

            for (int posAnalysis = 0;
                     posAnalysis + _windowSize + _maxDelta + _hopSynthesis < input.Length;
                     posAnalysis += _hopAnalysis,
                     posSynthesis += _hopSynthesis)
            {
                int delta = 0;

                if (posAnalysis > _maxDelta / 2)
                {
                    input.FastCopyTo(current, _windowSize + _maxDelta, posAnalysis - (_maxDelta / 2));

                    delta = WaveformSimilarityPos(current, prev, _maxDelta);
                }
                else
                {
                    input.FastCopyTo(current, _windowSize + _maxDelta, posAnalysis);
                }

                int size = Math.Min(_windowSize, output.Length - posSynthesis);

                for (int j = 0; j < size; j++)
                {
                    output[posSynthesis + j] += current[delta + j] * window[j];
                }

                for (int j = 0; j < _hopSynthesis; j++)
                {
                    output[posSynthesis + j] *= gain;
                }

                input.FastCopyTo(prev, _windowSize, posAnalysis + delta - (_maxDelta / 2) + _hopSynthesis);
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Position of the best found waveform similarity
        /// </summary>
        /// <param name="current"></param>
        /// <param name="prev"></param>
        /// <param name="maxDelta"></param>
        /// <returns></returns>
        public int WaveformSimilarityPos(float[] current, float[] prev, int maxDelta)
        {
            int optimalShift = 0;
            float maxCorrelation = 0.0f;

            // for small window sizes cross-correlate directly:

            if (_convolver == null)
            {
                for (int i = 0; i < maxDelta; i++)
                {
                    float xcorr = 0.0f;

                    for (int j = 0; j < prev.Length; j++)
                    {
                        xcorr += current[i + j] * prev[j];
                    }

                    if (xcorr > maxCorrelation)
                    {
                        maxCorrelation = xcorr;
                        optimalShift = i;
                    }
                }
            }
            // for very large window sizes better use FFT convolution:
            else
            {
                _convolver.CrossCorrelate(current, prev, _cc);

                for (int i = prev.Length - 1, j = 0; i < prev.Length + _maxDelta - 1; i++, j++)
                {
                    if (_cc[i] > maxCorrelation)
                    {
                        maxCorrelation = _cc[i];
                        optimalShift = j;
                    }
                }
            }

            return optimalShift;
        }
    }
}
