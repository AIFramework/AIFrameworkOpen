using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Dog.Tools
{
    /// <summary>
    /// Проверка схожести чисел
    /// </summary>
    [Serializable]
    public static class Simillary
    {
        /// <summary>
        /// Преобразование массива байт в вектор
        /// </summary>
        /// <param name="bools">Массив байт</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        public static Vector Bools2Vect(bool[] bools, double min = -1, double max = 1) 
        {
            Vector vector = new Vector(bools.Length) + min;

            for (int i = 0; i < bools.Length; i++)
                if (bools[i]) vector[i] = max;
            
            return vector;
        }

        /// <summary>
        /// Коэффициент корреляции Пирсона между вектором семантического образа и индексом
        /// </summary>
        /// <param name="semantic">Семантический образ</param>
        /// <param name="index">Индекс</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns></returns>
        public static double CorrelationVectorInt(Vector semantic, int index, double min = -1, double max = 1)
        {
            bool[] bools = index.DecimalToGrayBits(semantic.Count);
            Vector index_vector = Bools2Vect(bools, min, max);
            return Statistic.CorrelationCoefficient(semantic, index_vector);
        }

        /// <summary>
        /// Нечеткая принадлежность вектора индексу (множеству)
        /// </summary>
        /// <param name="semantic">Семантический образ</param>
        /// <param name="index">Индекс</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        public static double FuzzyMembership(Vector semantic, int index, double min = -1, double max = 1)
        {
            double cor = 4.0 * CorrelationVectorInt(semantic, index, min, max);
            return 1.0 / (1.0 + Math.Exp(-cor));
        }

        /// <summary>
        /// Коэффициент корреляции Пирсона между двумя индексами и индексом
        /// </summary>
        /// <param name="index1">Индекс 1</param>
        /// <param name="index2">Индекс 2</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns></returns>
        public static double CorrelationIntInt(int index1, int index2, double min = -1, double max = 1)
        {
            int count = Math.Max(index1.DecimalToBinaryBits().Length,
                index2.DecimalToBinaryBits().Length);

            count = Math.Max(2, count);

            bool[] bools1 = index1.DecimalToGrayBits(count);
            bool[] bools2 = index2.DecimalToGrayBits(count);
            Vector index_vector1 = Bools2Vect(bools1, min, max);
            Vector index_vector2 = Bools2Vect(bools2, min, max);
            return Statistic.CorrelationCoefficient(index_vector1, index_vector2);
        }


        /// <summary>
        /// Коэффициент корреляции Пирсона между двумя индексами и индексом
        /// </summary>
        /// <param name="index1">Индекс 1</param>
        /// <param name="index2">Индекс 2</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns></returns>
        public static double CosIntInt(int index1, int index2, double min = -1, double max = 1)
        {
            int count = Math.Max(index1.DecimalToBinaryBits().Length,
                index2.DecimalToBinaryBits().Length);

            count = Math.Max(2, count);

            bool[] bools1 = index1.DecimalToGrayBits(count);
            bool[] bools2 = index2.DecimalToGrayBits(count);
            Vector index_vector1 = Bools2Vect(bools1, min, max);
            Vector index_vector2 = Bools2Vect(bools2, min, max);
            return AnalyticGeometryFunctions.Cos(index_vector1, index_vector2);
        }

        /// <summary>
        /// Нечеткое сходствой двух чисел (множеств)
        /// </summary>
        /// <param name="index1">Индекс 1</param>
        /// <param name="index2">Индекс 2</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        public static double FuzzyMembershipIntInt(int index1, int index2, double min = -1, double max = 1)
        {
            double cor = 4.0 * CosIntInt(index1, index2, min, max);
            return 1.0 / (1.0 + Math.Exp(-cor));
        }
    }
}
