using System.Globalization;
using System.Text;

namespace AI.Extensions
{
    /// <summary>
    /// Со строками
    /// </summary>
    public static class StringExtensions
    {
        #region ToSingleString
        /// <summary>
        /// Преобразовать массив в строку
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="array">Массив</param>
        /// <param name="separator">Разделитель между элементами массива</param>
        /// <returns></returns>
        public static string ToSingleString<T>(this T[] array, string separator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0, max = array.Length; i < max; i++)
            {
                stringBuilder.Append(array[i]);
                stringBuilder.Append(separator);
            }

            return stringBuilder.ToString().Trim(' ');
        }
        /// <summary>
        /// Преобразовать массив в строку
        /// </summary>
        public static string ToSingleString(this float[] array, NumberFormatInfo provider, string separator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0, max = array.Length; i < max; i++)
            {
                stringBuilder.Append(array[i].ToString(provider));
                stringBuilder.Append(separator);
            }

            return stringBuilder.ToString().Trim(' ');
        }

        /// <summary>
        /// Преобразовать массив в строку
        /// </summary>
        public static string ToSingleString(this double[] array, NumberFormatInfo provider, string separator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0, max = array.Length; i < max; i++)
            {
                stringBuilder.Append(array[i].ToString(provider));
                stringBuilder.Append(separator);
            }

            return stringBuilder.ToString().Trim(' ');
        }

        /// <summary>
        /// Преобразовать массив в строку
        /// </summary>
        public static string ToSingleString(this decimal[] array, NumberFormatInfo provider, string separator = " ")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0, max = array.Length; i < max; i++)
            {
                stringBuilder.Append(array[i].ToString(provider));
                stringBuilder.Append(separator);
            }

            return stringBuilder.ToString().Trim(' ');
        }
        #endregion

        #region ToStringArray
        /// <summary>
        /// Преобразовать массив в масссив строк
        /// </summary>
        public static string[] ToStringArray<T>(this T[] array)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i]!.ToString();
            }

            return output;
        }

        /// <summary>
        /// Преобразовать массив в масссив строк
        /// </summary>
        public static string[] ToStringArray(this float[] array, NumberFormatInfo provider)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString(provider);
            }

            return output;
        }

        /// <summary>
        /// Преобразовать массив в масссив строк
        /// </summary>
        public static string[] ToStringArray(this double[] array, NumberFormatInfo provider)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString(provider);
            }

            return output;
        }

        /// <summary>
        /// Преобразовать массив в масссив строк
        /// </summary>
        public static string[] ToStringArray(this decimal[] array, NumberFormatInfo provider)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString(provider);
            }

            return output;
        }
        #endregion
    }
}
