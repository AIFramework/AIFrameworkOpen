using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// Class providing non-recursive implementation of N-sample MA filter.
    /// 
    /// Actually MA filter belongs to FIR filters (so it's inherited from FirFilter);
    /// however it can be realized also (and more efficiently) as a recursive filter (see below).
    /// </summary>
    [Serializable]

    public class MovingAverageFilter : FirFilter
    {
        /// <summary>
        /// Размер фильтра: number of samples for averaging
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="size">size of the filter</param>
        public MovingAverageFilter(int size = 9) : base(MakeKernel(size))
        {
            Size = size;
        }

        /// <summary>
        /// Генератор ядра
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static IEnumerable<float> MakeKernel(int size)
        {
            return Enumerable.Repeat(1f / size, size);
        }
    }
}
