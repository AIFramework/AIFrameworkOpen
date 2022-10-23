using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.DataLoader
{
    /// <summary>
    /// Столбец данных
    /// </summary>
    [Serializable]
    public class DataItem
    {
        /// <summary>
        /// Имя колонки
        /// </summary>
        public string Name;

        /// <summary>
        /// Данные
        /// </summary>
        public List<object> Data;

        /// <summary>
        /// Преобразовать в определенный тип
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> ToType<T>()
        {
            List<T> list = new List<T>(Data.Count);

            for (int i = 0; i < Data.Count; i++)
            {
                try
                {
                    list[i] = (T)Data[i];
                }
                catch
                {
                    throw new Exception($"Элемент {Data[i]}, с индексом {i}, не может быть преобразован в тип {typeof(T)}");
                }
            }

            return list;
        }
        
    }
}
