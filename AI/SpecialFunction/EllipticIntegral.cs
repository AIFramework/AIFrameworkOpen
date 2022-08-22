using AI.HightLevelFunctions;
using System;

namespace AI.SpecialFunction
{

    /// <summary>
    /// Эллиптические интегралы
    /// </summary>
    [Serializable]
    public class EllipticIntegral
    {
        /// <summary>
        /// Полный эллиптический интеграл первого рода
        /// </summary>
        /// <param name="k">Значение</param>
        public static double CompleteEllipticIntegral_I(double k)
        {


            double pi2 = Math.PI;
            pi2 = pi2 / 2.0;

            double bas = k * k / (1 - (k * k));


            if (k < 0.65)
            {
                return (1 + (0.25 * bas) - (0.125 * bas * k * k)) * pi2;
            }
            else if (k >= 1.0)
            {
                return double.PositiveInfinity;
            }
            else
            {
                double outp = 1 + (0.25 * bas) - (0.125 * bas * k * k);


                for (int i = 3; i < 6; i++)
                {
                    int i2 = 2 * i;
                    outp += Math.Pow(FunctionsForEachElements.Factorial(i2) / (Math.Pow(FunctionsForEachElements.Factorial(i), 2) * Math.Pow(2, i2)), 2) * Math.Pow(k, i2);
                }

                return outp * pi2;
            }

        }

        /// <summary>
        /// Полный комплементарный эллиптический интеграл первого рода
        /// </summary>
        /// <param name="k">Значение</param>
        public static double CompleteComplementarEllipticIntegral_I(double k)
        {
            double comlementarAgr = k * k;
            comlementarAgr = Math.Sqrt(1 - comlementarAgr);
            return CompleteEllipticIntegral_I(comlementarAgr);
        }

    }
}
