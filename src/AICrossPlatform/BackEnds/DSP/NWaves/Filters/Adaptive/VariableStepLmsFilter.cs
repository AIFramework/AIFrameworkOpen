﻿using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Adaptive
{
    /// <summary>
    /// Адаптивный фильтр (Least-Mean-Squares with variable steps)
    /// </summary>
    [Serializable]
    public class VariableStepLmsFilter : AdaptiveFilter
    {
        /// <summary>
        /// Mu
        /// </summary>
        private readonly float[] _mu;

        /// <summary>
        /// Утечка
        /// </summary>
        private readonly float _leakage;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="order"></param>
        /// <param name="mu"></param>
        /// <param name="leakage"></param>
        public VariableStepLmsFilter(int order, float[] mu = null, float leakage = 0) : base(order)
        {
            _mu = mu ?? Enumerable.Repeat(0.75f, order).ToArray();
            Guard.AgainstInequality(order, _mu.Length, "Filter order", "Steps array size");

            _leakage = leakage;
        }

        /// <summary>
        /// Входные данные процесса и целевые данные
        /// </summary>
        /// <param name="input"></param>
        /// <param name="desired"></param>
        /// <returns></returns>
        public override float Process(float input, float desired)
        {
            int offset = _delayLineOffset;

            _delayLine[offset + _kernelSize] = input;   // duplicate it for better loop performance


            float y = Process(input);

            float e = desired - y;

            for (int i = 0; i < _kernelSize; i++, offset++)
            {
                _b[i] = _b[_kernelSize + i] = ((1 - (_leakage * _mu[i])) * _b[i]) + (_mu[i] * e * _delayLine[offset]);
            }

            return y;
        }
    }
}
