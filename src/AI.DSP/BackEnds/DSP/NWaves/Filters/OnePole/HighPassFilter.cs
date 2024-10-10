using System;

namespace AI.BackEnds.DSP.NWaves.Filters.OnePole
{
    /// <summary>
    /// Класс для полюсно-нулевого фильтра верхних частот
    /// </summary>
    [Serializable]

    public class HighPassFilter : OnePoleFilter
    {
        /// <summary>
        /// Частота
        /// </summary>
        public double Freq { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="freq"></param>
        public HighPassFilter(double freq)
        {
            SetCoefficients(freq);
        }

        /// <summary>
        /// Установить коэффициенты фильтра
        /// </summary>
        /// <param name="freq"></param>
        private void SetCoefficients(double freq)
        {
            _a[0] = 1;
            _a[1] = (float)Math.Exp(-2 * Math.PI * (0.5 - freq));

            _b[0] = 1 - _a[1];
        }

        /// <summary>
        /// Изменить параметры фильтра (с сохранением его состояния)
        /// </summary>
        /// <param name="freq"></param>
        public void Change(double freq)
        {
            SetCoefficients(freq);
        }
    }
}
