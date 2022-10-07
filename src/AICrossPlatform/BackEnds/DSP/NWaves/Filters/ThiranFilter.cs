using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// N-th order Thiran allpass interpolation filter for delay 'Delta' (samples)
    /// 
    /// N = 13
    /// Delta = 13 + 0.4
    /// 
    /// https://ccrma.stanford.edu/~jos/pasp/Thiran_Allpass_Interpolators.html
    /// </summary>
    [Serializable]

    public class ThiranFilter : IirFilter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="order"></param>
        /// <param name="delta"></param>
        public ThiranFilter(int order, double delta) : base(MakeTf(order, delta))
        {
        }

        /// <summary>
        /// Создание передаточной функции
        /// </summary>
        /// <param name="order"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private static TransferFunction MakeTf(int order, double delta)
        {
            System.Collections.Generic.IEnumerable<double> a = Enumerable.Range(0, order + 1).Select(i => ThiranCoefficient(i, order, delta));
            System.Collections.Generic.IEnumerable<double> b = a.Reverse();

            return new TransferFunction(b.ToArray(), a.ToArray());
        }

        /// <summary>
        /// k-th coefficient in TF denominator
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        private static double ThiranCoefficient(int k, int n, double delta)
        {
            double a = 1.0;

            for (int i = 0; i <= n; i++)
            {
                a *= (delta - n + i) / (delta - n + k + i);
            }

            a *= Math.Pow(-1, k) * MathUtils.BinomialCoefficient(k, n);

            return a;
        }
    }
}
