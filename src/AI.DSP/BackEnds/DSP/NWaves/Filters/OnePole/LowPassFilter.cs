using System;

namespace AI.BackEnds.DSP.NWaves.Filters.OnePole
{
    /// <summary>
    /// Класс для полюсно-нулевого фильтра нижних частот
    /// </summary>
    [Serializable]

    public class LowPassFilter : OnePoleFilter
    {
        /// <summary>
        /// Частота
        /// </summary>
        public double Freq { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="freq"></param>
        public LowPassFilter(double freq)
        {
            SetCoefficients(freq);
        }

        /// <summary>
        /// Установить коэффициенты фильтра
        /// </summary>
        /// <param name="freq"></param>
        private void SetCoefficients(double freq)
        {
            Freq = freq;

            _a[0] = 1;
            _a[1] = (float)-Math.Exp(-2 * Math.PI * freq);

            _b[0] = 1 + _a[1];
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
