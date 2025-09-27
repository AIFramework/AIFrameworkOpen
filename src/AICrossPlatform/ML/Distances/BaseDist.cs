using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.ML.Distances
{
    /// <summary>
    /// Базовые функции измерения расстояний
    /// </summary>
    [Serializable]
    public static class BaseDist
    {
        /// <summary>
        /// Lp семейство расстояний
        /// </summary>
        public static double LpDist(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B, int p)
        {
            if (A.Shape.Count != B.Shape.Count)
            {
                throw new InvalidOperationException("Размерности не совпадают");
            }

            double res = 0, sempl;
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                sempl = Math.Pow(A.Data[i] - B.Data[i], p);
                res += Math.Abs(sempl);
            }

            res = Math.Pow(res, 1.0 / p);

            return res;
        }
        /// <summary>
        /// Евклидово расстояние
        /// </summary>
        public static double EuclideanDistance(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B)
        {
            return Math.Sqrt(SquareEucl(A, B));
        }

        /// <summary>
        /// Евклидово расстояние
        /// </summary>
        public static double EuclideanDistanceC(ComplexVector A, ComplexVector B)
        {
            return Math.Sqrt(Complex.Abs(SquareEucl(A, B)));
        }


        /// <summary>
        /// Расстояние L-infinity
        /// </summary>
        public static double LinfDist(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B)
        {
            return LinfDist((Vector)A.Data, (Vector)B.Data);
        }
        /// <summary>
        /// Косинусное сходство
        /// </summary>
        public static double Cos(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            return ((Vector)a.Data).Cos(b.Data);
        }
        /// <summary>
        /// Косинусное расстояние
        /// </summary>
        public static double CosDistRelu(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double eps = 1e-8;
            double cos = ((Vector)a.Data).Cos(b.Data);
            return 1.0 - (cos < eps ? eps : cos);
        }

        /// <summary>
        /// Косинусное расстояние
        /// </summary>
        public static double CosDist(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double cos = ((Vector)a.Data).Cos(b.Data);
            return -cos;
        }
        /// <summary>
        /// Квадрат эвклидова расстояния
        /// </summary>
        public static double SquareEucl(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
            {
                double dif = a.Data[i] - b.Data[i];
                sum += dif * dif;
            }


            return sum;
        }


        /// <summary>
        /// Квадрат эвклидова расстояния
        /// </summary>
        public static Complex SquareEucl(ComplexVector a, ComplexVector b)
        {
            Complex sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
            {
                Complex dif = a[i] - b[i];
                sum += dif * dif;
            }


            return sum;
        }

        /// <summary>
        /// L2
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public static double L2(ComplexVector cv)
        {
            Complex sum = 0;

            for (int i = 0; i < cv.Count; i++)
                sum += cv[i] * cv[i];

            return Math.Sqrt(Complex.Abs(sum));
        }

        /// <summary>
        /// L2
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double L2(IAlgebraicStructure<double> a)
        {
            double sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
                sum += a.Data[i] * a.Data[i];

            return sum;
        }

        /// <summary>
        /// Манхетонское расстояние
        /// </summary>
        public static double ManhattanDistance(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
            {
                sum += Math.Abs(a.Data[i] - b.Data[i]);
            }

            return sum;
        }

        #region Схожесть на множествах

        /// <summary>
        /// Коэфициент Жаккара
        /// </summary>
        /// <typeparam name="T">Тип данных множества</typeparam>
        /// <param name="set1">Множество 1</param>
        /// <param name="set2">Множество 2</param>
        public static double JaccardCoef<T>(HashSet<T> set1, HashSet<T> set2)
        {
            double numerator = set1.Intersect(set2).Count();
            double denumerator = set1.Union(set2).Count();
            return numerator / denumerator;
        }
        /// <summary>
        /// Коэфициент Жаккара с нормировкой на минимальное множество
        /// </summary>
        /// <typeparam name="T">Тип данных множества</typeparam>
        /// <param name="set1">Множество 1</param>
        /// <param name="set2">Множество 2</param>
        public static double JaccardCoefMin<T>(HashSet<T> set1, HashSet<T> set2)
        {
            double numerator = set1.Intersect(set2).Count();
            double denumerator = Math.Min(set1.Count(), set2.Count());
            return numerator / denumerator;
        }

        #endregion
    }
}
