using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Adaptive
{
    /// <summary>
    /// Адаптивный фильтр (нормализованный Least-Mean-Squares алгоритм + смещение)
    /// </summary>
    [Serializable]
    public class NlmsFilter : AdaptiveFilter
    {
        /// <summary>
        /// Mu
        /// </summary>
        private readonly float _mu;

        /// <summary>
        /// Смещение
        /// </summary>
        private readonly float _eps;

        /// <summary>
        /// Утечка
        /// </summary>
        private readonly float _leakage;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="order"></param>
        /// <param name="mu"></param>
        /// <param name="eps"></param>
        /// <param name="leakage"></param>
        public NlmsFilter(int order, float mu = 0.75f, float eps = 1, float leakage = 0) : base(order)
        {
            _mu = mu;
            _eps = eps;
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

            float norm = _eps + _delayLine.Sum(x => x * x);

            for (int i = 0; i < _kernelSize; i++, offset++)
            {
                _b[i] = _b[_kernelSize + i] = ((1 - (_leakage * _mu)) * _b[i]) + (_mu * e * _delayLine[offset] / norm);
            }

            return y;
        }
    }
}
