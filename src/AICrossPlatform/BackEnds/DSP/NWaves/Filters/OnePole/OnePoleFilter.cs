using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.OnePole
{
    /// <summary>
    /// Базовый класс полюсно-нулевых фильтров
    /// </summary>
    [Serializable]

    public class OnePoleFilter : IirFilter
    {
        /// <summary>
        /// Линия задержки
        /// </summary>
        private float _prev;

        /// <summary>
        /// Конструктор
        /// </summary>
        protected OnePoleFilter() : base(new[] { 1.0 }, new[] { 1.0, 0 })
        {
        }

        /// <summary>
        /// Конструктор for user defined coefficients
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public OnePoleFilter(double b, double a) : base(new[] { b }, new[] { 1.0, a })
        {
        }

        /// <summary>
        /// Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            float output = (_b[0] * sample) - (_a[1] * _prev);
            _prev = output;

            return output;
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            _prev = 0;
        }
    }
}
