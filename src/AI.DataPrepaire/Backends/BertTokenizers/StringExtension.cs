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
            int start = 0, index;

            while ((index = inputString.IndexOfAny(delimiters, start)) != -1)
            {
                // Если есть символы между предыдущей позицией start и текущим индексом index, добавляем их в результат.
                if (index - start > 0)
                    yield return inputString.Substring(start, index - start);

                // Добавляем найденный разделитель.
                yield return inputString.Substring(index, 1);

                // Обновляем значение start для продолжения поиска следующего разделителя.
                start = index + 1;
            }

            // Проверяем, есть ли оставшиеся символы после последнего разделителя, и если есть, добавляем их в результат.
            if (start < inputString.Length)
            {
                yield return inputString.Substring(start);
            }
        }
    }
}
