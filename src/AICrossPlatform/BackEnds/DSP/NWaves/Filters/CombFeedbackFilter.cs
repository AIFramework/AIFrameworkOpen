﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// Гребенчатый фильтр с обратной связью:
    /// 
    ///     y[n] = b0 * x[n] - am * y[n - m]
    /// 
    /// </summary>
    [Serializable]

    public class CombFeedbackFilter : IirFilter
    {
        /// <summary>
        /// Задержка на m шагов
        /// </summary>
        private readonly int _delay;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="m">Задержка</param>
        /// <param name="b0">Coefficient b0</param>
        /// <param name="am">Coefficient am</param>
        public CombFeedbackFilter(int m, double b0 = 1.0, double am = 0.6) : base(new float[1], new float[m + 1])
        {
            _a[0] = 1;
            _a[m] = (float)am;
            _b[0] = (float)b0;

            _delay = m;
        }

        /// <summary>
        /// Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            float b0 = _b[0];
            float am = _a[_delay];

            float output = (b0 * sample) - (am * _delayLineA[_delayLineOffsetA]);

            _delayLineA[_delayLineOffsetA] = output;

            if (--_delayLineOffsetA < 1)
            {
                _delayLineOffsetA = _denominatorSize - 1;
            }

            return output;
        }

        /// <summary>
        /// Применить фильтр
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override DiscreteSignal ApplyTo(DiscreteSignal signal,
                                               FilteringMethod method = FilteringMethod.Auto)
        {
            if (method != FilteringMethod.Auto)
            {
                return base.ApplyTo(signal, method);
            }

            float[] input = signal.Samples;
            float[] output = new float[input.Length];

            float b0 = _b[0];
            float am = _a[_delay];

            for (int i = 0; i < _delay; i++)
            {
                output[i] = b0 * input[i];
            }
            for (int i = _delay, j = 0; i < signal.Length; i++, j++)
            {
                output[i] = (b0 * input[i]) - (am * output[j]);
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Изменить состояние (сохранение состояния)
        /// </summary>
        /// <param name="b0"></param>
        /// <param name="am"></param>
        public void Change(double b0, double am)
        {
            _b[0] = (float)b0;
            _a[_delay] = (float)am;
        }
    }
}
