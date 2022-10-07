using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevI
{
    /// <summary>
    /// Band-stop Chebyshev-I filter
    /// </summary>
    [Serializable]
    public class BandStopFilter : IirFilter64
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order">Порядок</param>
        /// <param name="ripple">Коэф. пульсаций</param>
        public BandStopFilter(double f1, double f2, int order, double ripple = 0.1) : base(MakeTf(f1, f2, order, ripple))
        {
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order">Порядок</param>
        /// <param name="ripple">Коэф. пульсаций</param>
        private static TransferFunction MakeTf(double f1, double f2, int order, double ripple = 0.1)
        {
            return DesignFilter.IirBsTf(f1, f2, PrototypeChebyshevI.Poles(order, ripple));
        }
    }
}
