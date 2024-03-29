﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;
using System.Collections.Generic;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// Hilbert filter
    /// </summary>
    [Serializable]

    public class HilbertFilter : FirFilter
    {
        /// <summary>
        /// Размер фильтра
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="size">size of the filter</param>
        public HilbertFilter(int size = 128) : base(MakeKernel(size))
        {
            Size = size;
        }

        /// <summary>
        /// Генератор ядра
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<double> MakeKernel(int size)
        {
            double[] kernel = new double[size];

            kernel[0] = 0;
            for (int i = 1; i < size; i++)
            {
                kernel[i] = 2 * Math.Pow(Math.Sin(Math.PI * i / 2), 2) / (Math.PI * i);
            }

            return kernel;
        }
    }
}
