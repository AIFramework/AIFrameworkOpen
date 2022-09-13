﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevI
{
    /// <summary>
    /// High-pass Chebyshev-I filter
    /// </summary>
    [Serializable]

    public class HighPassFilter : IirFilter64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order"></param>
        /// <param name="ripple"></param>
        public HighPassFilter(double freq, int order, double ripple = 0.1) : base(MakeTf(freq, order, ripple))
        {
        }

        /// <summary>
        /// TF generator
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order">Порядок</param>
        /// <param name="ripple">Коэф. пульсаций</param>
        private static TransferFunction MakeTf(double freq, int order, double ripple = 0.1)
        {
            return DesignFilter.IirHpTf(freq, PrototypeChebyshevI.Poles(order, ripple));
        }
    }
}
