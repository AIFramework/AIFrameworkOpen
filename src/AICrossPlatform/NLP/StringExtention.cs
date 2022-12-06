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
        /// Объединение строк
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="sep"></param>
        /// <returns></returns>
        public static string Concatinate(this string[] strings, string sep = "\n")
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                stringBuilder.Append(strings[i]);
                
                if(i< strings.Length-1)
                    stringBuilder.Append(sep);
            }

            return stringBuilder.ToString();
        }


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


        /// <summary>
        /// Находит разность между строками, например "привет" - "ве" = "прит"
        /// </summary>
        /// <param name="text1">Строка из которой вычитаем</param>
        /// <param name="text2">Строка которую вычитаем</param>
        public static string Diff(this string text1, string text2)
        {
            int len = text1.Length-text2.Length;
          
            if (len < 0) throw new Exception("Вычитаемое больше уменьшаемого");
            if (!text1.Contains(text2)) throw new Exception("Уменьшаемое не содержит вычитаемое");
            char[] result = new char[len];

            for (int start = 0, j = 0; start < text1.Length-text2.Length && j < text1.Length; start++, j++)
            {
                bool isCont = false;

                if (text1[start] == text2[0])
                {
                    isCont = true;

                    for (int i = 0; i < text2.Length; i++)
                        if (text1[start + i] != text2[i])
                        {
                            isCont = false;
                            break;
                        }
                }

                j = isCont ? j + text2.Length : j; // Пропуск вычитания

                result[start] = text1[j];

            }


            return new string(result);
        }
    }
}
