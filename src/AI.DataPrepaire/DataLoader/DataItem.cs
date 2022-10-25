using AI.DataStructs.Algebraic;
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
        public string Name => _name;

        /// <summary>
        /// Данные
        /// </summary>
        public List<object> Data;

        private string _name;

        /// <summary>
        /// Столбец данных
        /// </summary>
        public DataItem(string name, List<object> data)
        {
            _name = name;
            Data = data;
        }


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

        /// <summary>
        /// Перевод данных в вектор
        /// </summary>
        /// <returns></returns>
        public Vector ToVector() 
        {
            return ToType<double>().ToArray();
        }
        
    }
}
