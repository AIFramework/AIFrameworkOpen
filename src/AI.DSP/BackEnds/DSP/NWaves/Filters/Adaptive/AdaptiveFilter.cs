using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Adaptive
{
    /// <summary>
    /// Базовый абстрактный класс адаптивных фильтров
    /// </summary>
    [Serializable]
    public abstract class AdaptiveFilter : FirFilter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="order"></param>
        public AdaptiveFilter(int order) : base(new float[order])
        {
            Array.Resize(ref _delayLine, _kernelSize * 2);  // trick for better performance
        }

        /// <summary>
        /// Начальные веса
        /// </summary>
        /// <param name="weights"></param>
        public void Init(float[] weights)
        {
            Guard.AgainstInequality(_kernelSize, weights.Length, "Порядок фильтра", "размерности массива весов");
            ChangeKernel(weights);
        }

        /// <summary>
        /// Обработка одного образца входного сигнала и одного образца полезного сигнала
        /// </summary>
        /// <param name="input"></param>
        /// <param name="desired"></param>
        /// <returns></returns>
        public abstract float Process(float input, float desired);
    }
}
