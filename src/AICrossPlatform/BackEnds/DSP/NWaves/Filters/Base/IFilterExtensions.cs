using AI.BackEnds.DSP.NWaves.Signals;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Методы расширения для фильтров
    /// </summary>
    [Serializable]
    public static class FilterExtensions
    {
        /// <summary>
        /// Метод реализует онлайн-фильтрацию для дискретного сигнала
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="input">Входной сигнал</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public static DiscreteSignal Process(this IOnlineFilter filter,
                                                  DiscreteSignal input)
        {
            float[] output = new float[input.Length];
            filter.Process(input.Samples, output, output.Length);
            return new DiscreteSignal(input.SamplingRate, output);
        }

        /// <summary>
        /// Метод реализует онлайн-фильтрацию (frame-by-frame)
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="input">Блок(фрейм) входного сигнала</param>
        /// <param name="output">Блок(фрейм) отфильтрованного сигнала</param>
        /// <param name="count">Number of samples to filter</param>
        /// <param name="inputPos">Input starting position</param>
        /// <param name="outputPos">Output starting position</param>
        public static void Process(this IOnlineFilter filter,
                                   float[] input,
                                   float[] output,
                                   int count = 0,
                                   int inputPos = 0,
                                   int outputPos = 0)
        {
            if (count <= 0)
            {
                count = input.Length;
            }

            int endPos = inputPos + count;

            for (int n = inputPos, m = outputPos; n < endPos; n++, m++)
            {
                output[m] = filter.Process(input[n]);
            }
        }

        /// <summary>
        /// NOTE. For educational purposes and for testing online filtering.
        /// 
        /// Implementation of Фильтрация всего сигнала in time domain frame-by-frame.
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="frameSize"></param>
        /// <param name="filter"></param>
        /// <returns></returns>        
        public static DiscreteSignal ProcessChunks(this IOnlineFilter filter,
                                                        DiscreteSignal signal,
                                                        int frameSize = 4096)
        {
            float[] input = signal.Samples;
            float[] output = new float[input.Length];

            int i = 0;
            for (; i + frameSize < input.Length; i += frameSize)
            {
                filter.Process(input, output, frameSize, i, i);
            }

            // process last chunk
            filter.Process(input, output, input.Length - i, i, i);

            return new DiscreteSignal(signal.SamplingRate, output);
        }
    }
}
