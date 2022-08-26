using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.DSPCore;
using System;

namespace AI.DSP.FIR
{
    /// <summary>
    /// Фильтр Баттерворта
    /// </summary>
    [Serializable]
    public class FilterLowButterworth : IFilter
    {
        private readonly ComplexVector _complexVector;
        /// <summary>
        /// Имя фильтра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фильтр Баттерворта
        /// </summary>
        /// <param name="f0">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="signalLen">Длинна реализации</param>
        /// <param name="order">Порядок фильтра</param>
        public FilterLowButterworth(int f0, int fd, int signalLen, int order = 5)
        {
            _complexVector = Filters.ButterworthLowCFH(signalLen, f0, fd, order);
            Name = string.Format("ButterworthLow. f0 : {0}, order: {1}", f0, order);
        }

        /// <summary>
        /// Фильтрация сигнала
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public Vector FilterOutp(Vector signal)
        {
            return Filters.Filter(signal, _complexVector, true);
        }
    }
}
