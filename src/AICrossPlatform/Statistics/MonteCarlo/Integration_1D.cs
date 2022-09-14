using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Statistics.MonteCarlo
{
    /// <summary>
    /// Расчет интеграла методом Монте-Карло
    /// </summary>
    [Serializable]
    public class Integration
    {
        /// <summary>
        /// Взятие одномерного интеграла
        /// </summary>
        /// <param name="func">Подъинтегральная функция</param>
        /// <param name="a">Нижний предел интегрирования</param>
        /// <param name="b">Верхний предел интегрирования</param>
        /// <param name="n">Число точек для расчета интеграла</param>
        /// <param name="iter">Число итераций расчета</param>
        /// <param name="seed">Зерно</param>
        public static double CalcIntegral1D(Func<double, double> func, double a, double b, int n = 50000, int iter = 20, int? seed = null) 
        {
            Random random = seed == null? new Random() : new Random(seed.Value);
            double integ = 0;

            for (int i = 0; i < iter; i++)
                integ += Cl1D(func, a, b, n, random);


            return integ/iter;
        }


        private static double Cl1D(Func<double, double> func, double a, double b, int n, Random random) 
        {
            Vector samles = (b - a) * Statistic.UniformDistribution(n, random) + a;
            samles = samles.Transform(func);
            return (b - a) * samles.Mean();
        }
    }
}
