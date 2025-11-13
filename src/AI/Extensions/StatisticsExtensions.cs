using System;
using System.Linq;

namespace AI.Extensions
{
    /// <summary>
    /// Расширение для статистик
    /// </summary>
    public static class StatisticsExtensions
    {
        #region Mean
        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this double[] array)
        {
            return array.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static float Mean(this float[] array)
        {
            return array.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this int[] array)
        {
            return array.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this short[] array)
        {
            double[] arr = array.Cast<double>().ToArray();
            return arr.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this byte[] array)
        {
            double[] arr = array.Cast<double>().ToArray();
            return arr.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this decimal[] array)
        {
            double[] arr = array.Cast<double>().ToArray();
            return arr.Average();
        }

        /// <summary>
        /// Среднее значение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Mean(this long[] array)
        {
            double[] arr = array.Cast<double>().ToArray();
            return arr.Average();
        }
        #endregion

        #region Disp

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this double[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }

            return disp / (array.Length - 1.0);
        }
        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>

        public static double Disp(this float[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this short[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this int[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this decimal[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow((double)array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this long[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }

        /// <summary>
        /// Дисперсия (\sigma^2) в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Disp(this byte[] array)
        {
            double mean = array.Mean();
            double disp = 0;

            for (int i = 0; i < array.Length; i++)
            {
                disp += Math.Pow(array[i] - mean, 2);
            }


            return disp / (array.Length - 1.0);
        }
        #endregion

        #region STD
        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this double[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this byte[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this short[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this int[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this long[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this float[] array)
        {
            return Math.Sqrt(array.Disp());
        }

        /// <summary>
        /// Среднеквадратичное отклонение в массиве
        /// </summary>
        /// <param name="array">Массив</param>
        /// <returns></returns>
        public static double Std(this decimal[] array)
        {
            return Math.Sqrt(array.Disp());
        }
        #endregion
    }
}