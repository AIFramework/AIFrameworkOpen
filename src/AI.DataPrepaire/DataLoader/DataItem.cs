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
        /// Тип данных
        /// </summary>
        public TypeData TypeColum = TypeData.UnDef;

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

        /// <summary>
        /// Определяет тип
        /// </summary>
        private void TypeDetected() 
        {
            try {
                try {
                    double.Parse((string)Data[0], AISettings.GetProvider());
                    TypeColum = TypeData.DigitP;
                }
                catch {
                    double.Parse((string)Data[0], AISettings.GetProviderComa());
                    TypeColum = TypeData.DigitC;
                }

            }
            catch
            {
                if (Data[0] is string) TypeColum = TypeData.String;
                else TypeColum = TypeData.UnDef;
            }
        }

        /// <summary>
        /// Преобразовывает данные (Служебный метод)
        /// </summary>
        public void Convert() 
        {
            TypeDetected();

            if (TypeColum == TypeData.DigitP)
                for (int i = 0; i < Data.Count; i++)
                    Data[i] = double.Parse(((string)Data[i]).Replace("\"", ""), AISettings.GetProvider());

            if (TypeColum == TypeData.DigitC)
                for (int i = 0; i < Data.Count; i++)
                    Data[i] = double.Parse(((string)Data[i]).Replace("\"", ""), AISettings.GetProviderComa());

        }

        /// <summary>
        /// Изменение столбца
        /// </summary>
        /// <param name="transformFunc">Функция трансформации</param>
        public void TransformSelf(Func<object, object> transformFunc)
        {
            for (int i = 0; i < Data.Count; i++)
                Data[i] = transformFunc(Data[i]);
        }
        
    }

    /// <summary>
    /// Тип данных
    /// </summary>
    public enum TypeData 
    {
        /// <summary>
        /// Число строка(Разделитель запятая)
        /// </summary>
        DigitC,

        /// <summary>
        /// Число строка (Разделитель точка)
        /// </summary>
        DigitP,
        /// <summary>
        /// Строка
        /// </summary>
        String,
        /// <summary>
        /// Неопределен
        /// </summary>
        UnDef
    }
}
