﻿using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Adaptive
{
    /// <summary>
    /// Адаптивный фильтр (Least-Mean-Fourth алгоритм)
    /// </summary>
    [Serializable]
    public class LmfFilter : AdaptiveFilter
    {
        /// <summary>
        /// Mu
        /// </summary>
        private readonly float _mu;

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
        public LmfFilter(int order, float mu = 0.75f, float leakage = 0) : base(order)
        {
            _mu = mu;
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
                _b[i] = _b[_kernelSize + i] = ((1 - (_leakage * _mu)) * _b[i]) + (4 * _mu * e * e * e * _delayLine[offset]);
            }

            return y;
        }
    }
}
