﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Filters.Base64;

namespace AI.BackEnds.DSP.NWaves.Filters.Bessel
{
    /// <summary>
    /// Low-pass Bessel filter
    /// </summary>
    public class LowPassFilter : IirFilter64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order"></param>
        /// <param name="ripple"></param>
        public LowPassFilter(double freq, int order) : base(MakeTf(freq, order))
        {
        }

        /// <summary>
        /// TF generator
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double freq, int order)
        {
            return DesignFilter.IirLpTf(freq, PrototypeBessel.Poles(order));
        }
    }
}
