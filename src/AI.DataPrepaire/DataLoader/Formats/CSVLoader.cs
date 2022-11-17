using AI.NLP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace AI.DataPrepaire.DataLoader.Formats
{
    /// <summary>
    /// Загрузчик CSV файлов
    /// </summary>
    [Serializable]
    public class CSVLoader
    {
        /// <summary>
        /// Загрузка csv
        /// </summary>
        /// <returns></returns>
        public static DataTable Read(string csvPath, string separator = ",")
        {
            DataItem[] dataItems = Reader(csvPath, separator);
            DataTable dataFrame = new DataTable();

            for (int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i].Convert();
                dataFrame.Add(dataItems[i]);
            }

            return dataFrame;
        }

        /// <summary>
        /// Загрузка csv
        /// </summary>
        /// <returns></returns>
        public static DataTable Read(string csvPath, char separator = ',')
        {
            return Read(csvPath, new string(new[] { separator }));
        }

        /// <summary>
        /// Чтение CSV
        /// </summary>
        private static DataItem[] Reader(string pathToCsv, string separator) 
        {
            using (var reader = new StreamReader(pathToCsv))
            {
                // Получение заголовков
                var headers = GetValues(reader.ReadLine(), separator);
                DataItem[] dataItems = new DataItem[headers.Length];

                // Создание столбцов данных
                for (int i = 0; i < headers.Length; i++)
                    dataItems[i] = new DataItem(headers[i], new List<dynamic>());

                // Запись данных
                while (!reader.EndOfStream)
                {
                    var values = GetValues(reader.ReadLine(), separator);

                    for (int i = 0; i < headers.Length; i++)
                        dataItems[i].Data.Add(values[i]);
                }

                return dataItems;
            }

        }


        private static string[] GetValues(string text, string separator) 
        {
            return new ValuesParser(text, separator).Values.ToArray();
        }
    }


    /// <summary>
    /// Рекурсивный парсер значений таблицы
    /// </summary>
    public class ValuesParser 
    {
        /// <summary>
        /// Значения
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Рекурсивный парсер значений таблицы
        /// </summary>
        public ValuesParser(string text, string separator) 
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
