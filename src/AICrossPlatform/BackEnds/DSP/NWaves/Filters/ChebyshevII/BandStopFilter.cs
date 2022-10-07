using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevII
{
    /// <summary>
    /// Band-stop Chebyshev-II filter
    /// </summary>
    [Serializable]

    public class BandStopFilter : IirFilter64
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        /// <param name="ripple">Коэф. пульсаций</param>
        public BandStopFilter(double f1, double f2, int order, double ripple = 0.1) : base(MakeTf(f1, f2, order, ripple))
        {
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="order"></param>
        /// <param name="ripple">Коэф. пульсаций</param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double freq1, double freq2, int order, double ripple = 0.1)
        {
            return DesignFilter.IirBsTf(freq1, freq2,
                                        PrototypeChebyshevII.Poles(order, ripple),
                                        PrototypeChebyshevII.Zeros(order));
        }
    }
}
