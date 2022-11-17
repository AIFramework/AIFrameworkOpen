using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.NLP
{
    /// <summary>
    /// Методы расширения для строк
    /// </summary>
    [Serializable]
    public static class StringExtention
    {
        /// <summary>
        /// Разделение по строке
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="strSpliter">Строка-разделитель</param>
        public static string[] Split(this string text, string strSpliter) 
        {
            return text.Split(new[] { strSpliter }, StringSplitOptions.None);
        }

        /// <summary>
        /// Удаление символов
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="delStrs">Подстроки, которые будут удалены</param>
        public static string Remove(this string text, string[] delStrs) 
        {
            string ret = text;

            for (int i = 0; i < delStrs.Length; i++)
                text.Replace(delStrs[i], "");

            return ret;
        }

        /// <summary>
        /// Замена с использованием регулярных выражений
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="pattern">Патерн для замены</param>
        /// <param name="new_string">На что заменить патерн</param>
        public static string ReReplace(this string text, string pattern, string new_string) 
        {
            return Regex.Replace(text, pattern, new_string);
        }

        /// <summary>
        /// Преобразование с использованием регулярных выражений
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="pattern">Патерн для преобразования</param>
        /// <param name="transformer">Функция преобразования текста совпадающего с патерном</param>
        public static string ReTransform(this string text, string pattern, Func<string, string> transformer) 
        {
            return Regex.Replace(text, pattern, x => transformer(x.Value));
        }
    }
}
