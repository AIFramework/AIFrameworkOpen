using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Метод-расширение для строки, который разбивает ее на подстроки с использованием заданных разделителей,
    /// при этом разделители также включаются в результат.
    /// </summary>
    [Serializable]
    public static class StringExtensions
    {
        /// <summary>
        /// Разбивает строку на подстроки с использованием заданных разделителей,
        /// при этом разделители также включаются в результат.
        /// </summary>
        /// <param name="inputString">Исходная строка.</param>
        /// <param name="delimiters">Массив символов-разделителей.</param>
        /// <returns>Перечисление подстрок, включая разделители.</returns>
        public static IEnumerable<string> SplitAndKeep(this string inputString, params char[] delimiters)
        {
            int start = 0, index = 0;
            int length = inputString.Length;

            while (index < length)
            {
                index = inputString.IndexOfAny(delimiters, start);

                if (index == -1)
                {
                    yield return inputString.Substring(start);
                    yield break;
                }

                if (index > start)
                {
                    yield return inputString.Substring(start, index - start);
                }

                yield return inputString.Substring(index, 1);

                start = ++index;
            }
        }
    }
}
