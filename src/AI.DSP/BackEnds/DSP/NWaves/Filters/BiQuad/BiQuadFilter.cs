using AI.BackEnds.DSP.NWaves.Filters.Base64;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.BiQuad
{
    /// <summary>
    /// BiQuad filter base class
    /// </summary>
    [Serializable]
    public class BiQuadFilter : IirFilter64
    {
        /// <summary>
        /// Линия задержки
        /// </summary>
        private double _in1;
        private double _in2;
        private double _out1;
        private double _out2;

        /// <summary>
        /// Конструктор
        /// </summary>
        protected BiQuadFilter() : base(new[] { 1.0, 0, 0 }, new[] { 1.0, 0, 0 })
        {
        }

        /// <summary>
        /// Конструктор использующий коэффициенты передаточной функции
        /// </summary>
        /// <param name="b0"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="a0"></param>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        public BiQuadFilter(double b0, double b1, double b2,
                            double a0, double a1, double a2) :
            base(new[] { b0, b1, b2 }, new[] { a0, a1, a2 })
        {
        }

        /// <summary>
        /// Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override double Process(double sample)
        {
            double output = (_b[0] * sample) + (_b[1] * _in1) + (_b[2] * _in2) - (_a[1] * _out1) - (_a[2] * _out2);

            _in2 = _in1;
            _in1 = sample;
            _out2 = _out1;
            _out1 = output;

            return output;
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            _in1 = _in2 = _out1 = _out2 = 0;
        }
    }
}
