using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevI
{
    /// <summary>
    /// Band-pass Chebyshev-I filter
    /// </summary>
    [Serializable]
    public class BandPassFilter : IirFilter64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        public BandPassFilter(double f1, double f2, int order, double ripple = 0.1) : base(MakeTf(f1, f2, order, ripple))
        {
        }

        /// <summary>
        /// TF generator
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double f1, double f2, int order, double ripple = 0.1)
        {
            return DesignFilter.IirBpTf(f1, f2, PrototypeChebyshevI.Poles(order, ripple));
        }
    }
}
