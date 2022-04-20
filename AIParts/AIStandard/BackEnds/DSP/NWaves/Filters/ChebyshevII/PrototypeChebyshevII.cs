using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.ChebyshevII
{
    public static class PrototypeChebyshevII
    {
        public static Complex[] Poles(int order, double ripple = 0.1)
        {
            double eps = Math.Sqrt(Math.Pow(10, ripple / 10) - 1);
            double s = MathUtils.Asinh(1 / eps) / order;
            double sinh = Math.Sinh(s);
            double cosh = Math.Cosh(s);

            Complex[] poles = new Complex[order];

            for (int k = 0; k < order; k++)
            {
                double theta = Math.PI * (2 * k + 1) / (2 * order);
                double re = -sinh * Math.Sin(theta);
                double im = cosh * Math.Cos(theta);
                poles[k] = 1 / new Complex(re, im);
            }

            return poles;
        }

        public static Complex[] Zeros(int order)
        {
            Complex[] zeros = new Complex[order];

            for (int k = 0; k < order; k++)
            {
                double theta = Math.PI * (2 * k + 1) / (2 * order);
                zeros[k] = new Complex(0, -1 / Math.Cos(theta));
            }

            return zeros;
        }
    }
}
