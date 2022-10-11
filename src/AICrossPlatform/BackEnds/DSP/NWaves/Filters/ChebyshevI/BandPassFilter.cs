using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevI
{
    /// <summary>
    /// Полосовой фильтр Чебышёва первого рода
    /// </summary>
    [Serializable]
    public class BandPassFilter : IirFilter64
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="f1">Нижняя частота среза</param>
        /// <param name="f2">Верхняя частота среза</param>
        /// <param name="order">Порядок фильтра</param>
        /// <param name="ripple">Коэф. пульсаций</param>
        public BandPassFilter(double f1, double f2, int order, double ripple = 0.1) : base(MakeTf(f1, f2, order, ripple))
        {
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="order"></param>
        /// <param name="ripple">Коэф. пульсаций</param>
        /// <returns></returns>
        private static TransferFunction MakeTf(double f1, double f2, int order, double ripple = 0.1)
        {
            return DesignFilter.IirBpTf(f1, f2, PrototypeChebyshevI.Poles(order, ripple));
        }
    }
}
