using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;

namespace AI.BackEnds.DSP.NWaves.Filters.Butterworth
{
    /// <summary>
    /// Class for Butterworth IIR BS filter.
    /// </summary>
    public class BandStopFilter : IirFilter64
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        public BandStopFilter(double f1, double f2, int order) : base(MakeTf(f1, f2, order))
        {
        }

        /// <summary>
        /// TF generator
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double f1, double f2, int order)
        {
            return DesignFilter.IirBsTf(f1, f2, PrototypeButterworth.Poles(order));
        }
    }
}
