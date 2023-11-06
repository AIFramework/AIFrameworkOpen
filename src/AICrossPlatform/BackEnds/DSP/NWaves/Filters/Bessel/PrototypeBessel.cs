using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Bessel
{
    /// <summary>
    /// Прототип фильтра Бесселя
    /// </summary>
    [Serializable]
    public static class PrototypeBessel
    {
        /// <summary>
        /// k-th coefficient of n-th order Bessel polynomial
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Reverse(int k, int n)
        {
            return MathUtilsDSP.Factorial((2 * n) - k) /
                (Math.Pow(2, n - k) * MathUtilsDSP.Factorial(k) * MathUtilsDSP.Factorial(n - k));
        }

        /// <summary>
        /// Полюсы аналогового фильтра Бесселя
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Complex[] Poles(int order)
        {
            double[] a = Enumerable.Range(0, order + 1)
                              .Select(i => Reverse(order - i, order))
                              .ToArray();

            Complex[] poles = MathUtilsDSP.PolynomialRoots(a);

            // ...and normalize

            double norm = Math.Pow(10, -Math.Log10(a[order - 1]) / order);

            for (int i = 0; i < poles.Length; i++)
            {
                poles[i] *= norm;
            }

            return poles;
        }
    }
}
