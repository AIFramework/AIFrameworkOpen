using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// Class providing recursive implementation of N-sample MA filter:
    /// 
    ///     y[n] = x[n] / N - x[n - N] / N + y[n - 1]
    /// 
    /// i.e. 
    ///     B = [1/N, 0, 0, 0, 0, ... , 0, -1/N]
    ///     A = [1, -1]
    /// 
    /// </summary>
    /// 
    [Serializable]

    public class MovingAverageRecursiveFilter : IirFilter
    {
        /// <summary>
        /// Размер фильтра: number of samples for averaging
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Линия задержки
        /// </summary>
        private float _out1;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="size">size of the filter</param>
        public MovingAverageRecursiveFilter(int size = 9) : base(MakeNumerator(size), new[] { 1f, -1 })
        {
            Size = size;
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static float[] MakeNumerator(int size)
        {
            float[] b = new float[size + 1];

            b[0] = 1f / size;
            b[size] = -b[0];

            return b;
        }

        /// <summary>
        /// Применить фильтр by fast recursive strategy
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
            int size = Size;

            float[] output = new float[input.Length];

            float b0 = _b[0];
            float bs = _b[Size];

            output[0] = input[0] * b0;

            for (int n = 1, k = 0; n < size; n++, k++)
            {
                output[n] = (input[n] * b0) + output[k];
            }

            for (int n = size, k = size - 1, delay = 0; n < input.Length; n++, k++, delay++)
            {
                output[n] = (input[delay] * bs) + (input[n] * b0) + output[k];
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            float b0 = _b[0];
            float bs = _b[Size];

            float output = (b0 * sample) + (bs * _delayLineB[_delayLineOffsetB]) + _out1;

            _delayLineB[_delayLineOffsetB] = sample;
            _out1 = output;

            if (--_delayLineOffsetB < 1)
            {
                _delayLineOffsetB = _numeratorSize - 1;
            }

            return output;
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            _out1 = 0;
            base.Reset();
        }
    }
}
