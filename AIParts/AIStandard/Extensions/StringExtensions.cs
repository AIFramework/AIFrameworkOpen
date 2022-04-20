using System.Globalization;
using System.Text;

namespace AI.Extensions
{
    public static class StringExtensions
    {
        #region ToSingleString
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
        public static string[] ToStringArray<T>(this T[] array)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString();
            }

            return output;
        }

        public static string[] ToStringArray(this float[] array, NumberFormatInfo provider)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString(provider);
            }

            return output;
        }

        public static string[] ToStringArray(this double[] array, NumberFormatInfo provider)
        {
            string[] output = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToString(provider);
            }

            return output;
        }

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
