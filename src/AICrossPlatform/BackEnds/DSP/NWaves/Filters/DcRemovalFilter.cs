using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// DC removal БИХ фильтр
    /// </summary>
    [Serializable]

    public class DcRemovalFilter : IirFilter
    {
        /// <summary>
        /// R parameter
        /// </summary>
        private readonly float _r;

        /// <summary>
        /// Линия задержки
        /// </summary>
        private float _in1;
        private float _out1;

        /// <summary>
        /// Конструктор creates simple 1st order recursive filter
        /// </summary>
        /// <param name="r">R coefficient (usually in [0.9, 1] range)</param>
        public DcRemovalFilter(double r = 0.995) : base(new[] { 1.0, -1 }, new[] { 1.0, -r })
        {
            _r = (float)r;
        }

        /// <summary>
        /// Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            float output = sample - _in1 + (_r * _out1);

            _in1 = sample;
            _out1 = output;

            return output;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _in1 = _out1 = 0;
        }
    }
}
