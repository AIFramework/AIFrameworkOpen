using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


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
            return new CSVValuesParser(text, separator).Values.ToArray();
        }
    }

}
