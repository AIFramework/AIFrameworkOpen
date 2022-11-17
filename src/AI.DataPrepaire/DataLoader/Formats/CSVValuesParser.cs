using AI.NLP;
using System;
using System.Collections.Generic;
using System.Text;


namespace AI.DataPrepaire.DataLoader.Formats
{
    /// <summary>
    /// Рекурсивный парсер значений таблицы
    /// </summary>
    [Serializable]
    public class CSVValuesParser 
    {
        /// <summary>
        /// Значения
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Рекурсивный парсер значений таблицы
        /// </summary>
        public CSVValuesParser(string text, string separator) 
        {
            Values = new List<string>();
            VParser(text, separator);
        }

        // Парсер
        private void VParser(string text, string separator)
        {
            if (text.Length == 0) return;
            string[] parts = new string[0];
            string new_sep;


            string dat;
            if (text[0] == '"')
            {
                new_sep = "\"" + separator;
                parts = text.Split(new_sep);
                dat = parts[0].Trim('"');
            }
            else
            {
                new_sep = separator;
                parts = text.Split(new_sep);
                dat = parts[0];
            }

            dat = dat.Replace("\\n", "\n");
            Values.Add(dat);


            StringBuilder stringBuilder = new StringBuilder();

            // Объединение
            for (int i = 1; i < parts.Length-1; i++)
            {
                stringBuilder.Append(parts[i]);
                stringBuilder.Append(new_sep);
            }

            if(parts.Length > 1)
                stringBuilder.Append(parts[parts.Length-1]);

            VParser(stringBuilder.ToString(), separator); // Рекурсия
        }
    }

}
