using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Elliptic
{
    [Serializable]

    public class BandStopFilter : IirFilter64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="order"></param>
        /// <param name="ripplePass"></param>
        /// <param name="rippleStop"></param>
        public BandStopFilter(double freq1, double freq2, int order, double ripplePass = 10, double rippleStop = 20) :
            base(MakeTf(freq1, freq2, order, ripplePass, rippleStop))
        {
        }

        /// <summary>
        /// TF generator
        /// </summary>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="order"></param>
        /// <param name="ripplePass"></param>
        /// <param name="rippleStop"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double freq1, double freq2, int order, double ripplePass = 1, double rippleStop = 20)
        {
            return DesignFilter.IirBsTf(freq1, freq2,
                                        PrototypeElliptic.Poles(order, ripplePass, rippleStop),
                                        PrototypeElliptic.Zeros(order, ripplePass, rippleStop));
        }
    }
}
