using System;

namespace AI.Extensions
{
    /// <summary>
    /// Расширения для конвертации массивов
    /// </summary>
    public static class ArraysExtensions
    {
        #region ToDoubleArray
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this float[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = array[i];

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this int[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = array[i];

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this long[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = array[i];

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this decimal[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = (double)array[i];

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this short[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = array[i];

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[] ToDoubleArray(this byte[] array)
        {
            double[] dArr = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            dArr[i] = array[i];

            return dArr;
        }
        #endregion

        #region ToDoubleArray2D
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubleArray2D(this float[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                dArr[i, j] = array[i, j];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubleArray2D(this int[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                dArr[i, j] = array[i, j];   
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubleArray2D(this long[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                    dArr[i, j] = array[i, j];
                   
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubleArray2D(this decimal[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    dArr[i, j] = (double)array[i, j];
                }
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubleArray2D(this short[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    dArr[i, j] = array[i, j];
                }
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа double
        /// </summary>
        public static double[,] ToDoubArray2D(this byte[,] array)
        {
            double[,] dArr = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    dArr[i, j] = array[i, j];
                }
            }

            return dArr;
        }
        #endregion

        #region ToFloatArray
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this double[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = (float)array[i];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this int[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = array[i];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this decimal[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = (float)array[i];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this long[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = array[i];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this short[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = array[i];
            }

            return dArr;
        }
        /// <summary>
        /// Конвертация в массив элементов типа float
        /// </summary>
        public static float[] ToFloatArray(this byte[] array)
        {
            float[] dArr = new float[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                dArr[i] = array[i];
            }

            return dArr;
        }
        #endregion

        #region Transform
        /// <summary>
        /// Поэлементное преобразование над элементами исходного массива с созданием выходного массифа нужного типа
        /// </summary>
        /// <typeparam name="TI">Тип фходного массива</typeparam>
        /// <typeparam name="TO">Тип выходного массива</typeparam>
        /// <param name="array">Входной массив</param>
        /// <param name="function">Функция преобразования</param>
        /// <returns></returns>
        public static TO[] Transform<TI, TO>(this TI[] array, Func<TI, TO> function)
        {
            TO[] output = new TO[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = function(array[i]);
            }

            return output;
        }
        #endregion

        #region ElementWiseEqual
        /// <summary>
        /// Поэлементное сравнение 2х массивов
        /// </summary>
        /// <typeparam name="T">Тип массивов</typeparam>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static bool ElementWiseEqual<T>(this T[] arr1, T[] arr2)
        {
            bool a1n = arr1 == null;
            bool a2n = arr2 == null;

            if (a1n && a2n)
            {
                return true;
            }
            else if ((a1n && !a2n) || (!a1n && a2n))
            {
                return false;
            }
            else
            {
                if (arr1!.Length != arr2!.Length)
                {
                    return false;
                }

                for (int i = 0; i < arr1.Length; i++)
                {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                    if (arr1[i].Equals(arr2[i]))
                    {
                        continue;
                    }
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                    return false;
                }

                return true;
            }
        }
        #endregion
    }
}
