using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Security;


namespace AI.DataPrepaire.DataLoader.Formats
{
    /// <summary>
    /// Загрузчик CSV файлов
    /// </summary>
    [Serializable]
    public static class CSVLoader
    {
        /// <summary>
        /// Загрузка csv
        /// </summary>
        /// <returns></returns>
        public static DataTable Read(string csvPath, string separator = ",")
        {
            DataItem[] dataItems = Reader(csvPath, separator);
            return ToTable(dataItems);
        }

        /// <summary>
        /// Загрузка csv из потока
        /// </summary>
        /// <returns></returns>
        public static DataTable Read(StreamReader csvStream, char separator = ',')
        {
            return Read(csvStream, new string(new[] { separator }));
        }


        /// <summary>
        /// Загрузка csv из потока
        /// </summary>
        /// <returns></returns>
        public static DataTable Read(StreamReader csvStream, string separator = ",")
        {
            DataItem[] dataItems = Reader(csvStream, separator);
            return ToTable(dataItems);
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
                return Reader(reader, separator);
            }

        }

        /// <summary>
        /// Получение таблицы
        /// </summary>
        /// <param name="dataItems"></param>
        /// <returns></returns>
        private static DataTable ToTable(DataItem[] dataItems) 
        {
            DataTable dataFrame = new DataTable();

            for (int i = 0; i < dataItems.Length; i++)
            {
                dataItems[i].Convert();
                dataFrame.Add(dataItems[i]);
            }

            return dataFrame;
        }

        // Работа с потоком
        private static DataItem[] Reader(StreamReader csvStream, string separator) 
        {
            // Получение заголовков
            var headers = GetValues(csvStream.ReadLine(), separator);
            DataItem[] dataItems = new DataItem[headers.Length];

            // Создание столбцов данных
            for (int i = 0; i < headers.Length; i++)
                dataItems[i] = new DataItem(headers[i], new List<dynamic>());

            // Запись данных
            while (!csvStream.EndOfStream)
            {
                var values = GetValues(csvStream.ReadLine(), separator);

                for (int i = 0; i < headers.Length; i++)
                    dataItems[i].Data.Add(values[i]);
            }

            return dataItems;
        }


        private static string[] GetValues(string text, string separator) 
        {
            return new CSVValuesParser(text, separator).Values.ToArray();
        }
    }

}
