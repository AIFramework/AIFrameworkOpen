using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevI
{
    /// <summary>
    /// Прототип фильтра Чебшёва первого рода
    /// </summary>
    public static class PrototypeChebyshevI
    {
        /// <summary>
        /// Полюса передаточной функции
        /// </summary>
        /// <param name="order">Порядок</param>
        /// <param name="ripple">Коэф. пульсаций</param>
        public static Complex[] Poles(int order, double ripple = 0.1)
        {
            double eps = Math.Sqrt(Math.Pow(10, ripple / 10) - 1);
            double s = MathUtilsDSP.Asinh(1 / eps) / order;
            double sinh = Math.Sinh(s);
            double cosh = Math.Cosh(s);

            Complex[] poles = new Complex[order];

            for (int k = 0; k < order; k++)
            {
                double theta = Math.PI * ((2 * k) + 1) / (2 * order);
                double re = -sinh * Math.Sin(theta);
                double im = cosh * Math.Cos(theta);
                poles[k] = new Complex(re, im);
            }

            return poles;
        }
    }
}
