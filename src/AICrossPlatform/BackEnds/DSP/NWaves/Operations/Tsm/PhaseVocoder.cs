﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations.Tsm
{
    /// <summary>
    /// Обычный фазовый вокодер
    /// </summary>
    [Serializable]
    public class PhaseVocoder : IFilter
    {
        /// <summary>
        /// Размер прыжка на этапе анализа (разложение STFT)
        /// </summary>
        protected readonly int _hopAnalysis;

        /// <summary>
        /// Размер прыжка на этапе анализа (объединение STFT)
        /// </summary>
        protected readonly int _hopSynthesis;

        /// <summary>
        /// Размер блока БПФ для анализа и синтеза
        /// </summary>
        protected readonly int _fftSize;

        /// <summary>
        /// Коэффициент растяжения
        /// </summary>
        protected readonly double _stretch;

        /// <summary>
        /// Внутренний алгоритм для выполнения БПФ
        /// </summary>
        protected readonly RealFft _fft;

        /// <summary>
        ///Весовое окно
        /// </summary>
        protected readonly float[] _window;

        /// <summary>
        /// Коэффициент нормализации ISTFT
        /// </summary>
        protected readonly float _gain;

        /// <summary>
        /// Линейно разнесенные частоты
        /// </summary>
        protected readonly double[] _omega;

        /// <summary>
        /// Внутренний буфер для реальных частей анализируемого блока
        /// </summary>
        protected readonly float[] _re;

        /// <summary>
        /// Внутренний буфер для мнимых частей анализируемого блока
        /// </summary>
        protected readonly float[] _im;

        /// <summary>
        /// Массив фаз, вычисленный на предыдущем шаге
        /// </summary>
        protected readonly double[] _prevPhase;

        /// <summary>
        /// Массив новых синтезированных фаз
        /// </summary>
        protected readonly double[] _phaseTotal;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="stretch"></param>
        /// <param name="hopAnalysis"></param>
        /// <param name="fftSize"></param>
        public PhaseVocoder(double stretch, int hopAnalysis, int fftSize = 0)
        {
            _stretch = stretch;
            _hopAnalysis = hopAnalysis;
            _hopSynthesis = (int)(hopAnalysis * stretch);
            _fftSize = (fftSize > 0) ? fftSize : 8 * Math.Max(_hopAnalysis, _hopSynthesis);

            _fft = new RealFft(_fftSize);

            _window = Window.OfType(WindowTypes.Hann, _fftSize);

            _gain = 1 / (_fftSize * _window.Select(w => w * w).Sum() / _hopSynthesis);

            _omega = Enumerable.Range(0, (_fftSize / 2) + 1)
                               .Select(f => 2 * Math.PI * f / _fftSize)
                               .ToArray();

            _re = new float[_fftSize];
            _im = new float[_fftSize];

            _prevPhase = new double[(_fftSize / 2) + 1];
            _phaseTotal = new double[(_fftSize / 2) + 1];
        }

        /// <summary>
        /// Алгоритм фазового вокодера
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyTo(DiscreteSignal signal,
                                      FilteringMethod method = FilteringMethod.Auto)
        {
            float[] input = signal.Samples;
            float[] output = new float[(int)(input.Length * _stretch) + _fftSize];

            int posSynthesis = 0;
            for (int posAnalysis = 0; posAnalysis + _fftSize < input.Length; posAnalysis += _hopAnalysis)
            {
                input.FastCopyTo(_re, _fftSize, posAnalysis);

                // analysis ==================================================

                _re.ApplyWindow(_window);
                _fft.Direct(_re, _re, _im);

                // processing ================================================

                ProcessSpectrum();

                // synthesis =================================================

                _fft.Inverse(_re, _im, _re);

                for (int j = 0; j < _re.Length; j++)
                {
                    output[posSynthesis + j] += _re[j] * _window[j];
                }

                for (int j = 0; j < _hopSynthesis; j++)
                {
                    output[posSynthesis + j] *= _gain;
                }

                posSynthesis += _hopSynthesis;
            }

            for (; posSynthesis < output.Length; posSynthesis++)
            {
                output[posSynthesis] *= _gain;
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Обработка одного спектра на каждом шаге STFT.
        /// </summary>
        public virtual void ProcessSpectrum()
        {
            for (int j = 1; j <= _fftSize / 2; j++)
            {
                double mag = Math.Sqrt((_re[j] * _re[j]) + (_im[j] * _im[j]));
                double phase = Math.Atan2(_im[j], _re[j]);

                double delta = phase - _prevPhase[j];

                double deltaUnwrapped = delta - (_hopAnalysis * _omega[j]);
                double deltaWrapped = MathUtilsDSP.Mod(deltaUnwrapped + Math.PI, 2 * Math.PI) - Math.PI;

                double freq = _omega[j] + (deltaWrapped / _hopAnalysis);

                _phaseTotal[j] += _hopSynthesis * freq;
                _prevPhase[j] = phase;

                _re[j] = (float)(mag * Math.Cos(_phaseTotal[j]));
                _im[j] = (float)(mag * Math.Sin(_phaseTotal[j]));
            }
        }

        /// <summary>
        /// Перезапуск фазового вокодера
        /// </summary>
        public virtual void Reset()
        {
            Array.Clear(_phaseTotal, 0, _phaseTotal.Length);
            Array.Clear(_prevPhase, 0, _prevPhase.Length);
        }
    }
}
