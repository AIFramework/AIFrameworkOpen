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
        public static DataTable Read(string csvPath, char separator = ',')
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
        /// Чтение CSV
        /// </summary>
        private static DataItem[] Reader(string pathToCsv, char separator) 
        {
            using (var reader = new StreamReader(pathToCsv))
            {
                // Получение заголовков
                var headers = reader.ReadLine().Split(separator);
                DataItem[] dataItems = new DataItem[headers.Length];

                // Создание столбцов данных
                for (int i = 0; i < headers.Length; i++)
                    dataItems[i] = new DataItem(headers[i], new List<object>());

                // Запись данных
                while (!reader.EndOfStream)
                {
                    var values = reader.ReadLine().Split(separator);

                    for (int i = 0; i < headers.Length; i++)
                        dataItems[i].Data.Add(values[i]);
                }

                return dataItems;
            }

        }
    }
}
