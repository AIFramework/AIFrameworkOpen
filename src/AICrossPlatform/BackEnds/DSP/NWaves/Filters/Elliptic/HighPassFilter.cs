using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Elliptic
{

    /// <summary>
    /// Фильтр верхних частот
    /// </summary>
    [Serializable]

    public class HighPassFilter : IirFilter64
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order"></param>
        /// <param name="ripplePass"></param>
        /// <param name="rippleStop"></param>
        public HighPassFilter(double freq, int order, double ripplePass = 1, double rippleStop = 20) :
            base(MakeTf(freq, order, ripplePass, rippleStop))
        {
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="order"></param>
        /// <param name="ripplePass"></param>
        /// <param name="rippleStop"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double freq, int order, double ripplePass = 1, double rippleStop = 20)
        {
            return DesignFilter.IirHpTf(freq,
                                        PrototypeElliptic.Poles(order, ripplePass, rippleStop),
                                        PrototypeElliptic.Zeros(order, ripplePass, rippleStop));
        }
    }
}
