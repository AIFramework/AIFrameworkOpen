using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.DataPrepaire.DataLoader
{

    /// <summary>
    /// Блок данных (по мотивам pandas)
    /// </summary>
    [Serializable]
    public class DataTable
    {
        private Dictionary<string, DataItem> _frame
            = new Dictionary<string, DataItem>();

        private List<string> _indexis = new List<string>();

        /// <summary>
        /// Число строк
        /// </summary>
        public int Len => _frame[_indexis[0]].Data.Count;

        /// <summary>
        /// Добавление/получение/изменение набора данных по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataItem this[string name]
        {
            get
            {
                if (!_frame.ContainsKey(name))
                    throw new Exception("Набора с таким именем не существует");
                return _frame[name];
            }

            set
            {
                if (_frame.ContainsKey(name))
                    _frame[name] = value;

                else
                {
                    _indexis.Add(name);
                    _frame.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Добавить данные
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="data">Данные</param>
        public void Add(string name, IEnumerable<dynamic> data)
        {
            DataItem dataItem = new DataItem(name, data.ToList());
            this[name] = dataItem;
        }

        /// <summary>
        /// Добавить данные
        /// </summary>
        /// <param name="dataItem"></param>
        public void Add(DataItem dataItem)
        {
            this[dataItem.Name] = dataItem;
        }

        /// <summary>
        /// Отдает имена столбцов
        /// </summary>
        public string[] GetColums()
        {
            List<string> ret = new List<string>(_frame.Count);

            foreach (var item in _indexis)
                ret.Add(item);

            return ret.ToArray();
        }

        /// <summary>
        /// Преобразование участка данных в матрицу
        /// </summary>
        public Matrix ToMatrix()
        {
            Vector[] colums = new Vector[_indexis.Count];

            for (int i = 0; i < _indexis.Count; i++)
                colums[i] = _frame[_indexis[i]].ToVector();

            return Matrix.FromVectorsAsColumns(colums);
        }

        /// <summary>
        /// Выдать подмножество определенных столбцов
        /// </summary>
        /// <param name="colums">Список столбцов</param>
        /// <returns></returns>
        public DataTable GetSubTable(IEnumerable<string> colums)
        {
            DataTable ret = new DataTable();
            foreach (var colum in colums) ret[colum] = this[colum];

            return ret;
        }

        /// <summary>
        /// Получить строку по индексу
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <returns></returns>
        public object[] GetRow(int rowIndex)
        {
            dynamic[] ret = new dynamic[_indexis.Count];

            for (int i = 0; i < _indexis.Count; i++)
                ret[i] = _frame[_indexis[i]].Data[rowIndex];

            return ret;
        }

        /// <summary>
        /// Получить строку по индексу с приведением к типу
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <returns></returns>
        public T[] GetRow<T>(int rowIndex)
        {
            var data = GetRow(rowIndex);
            T[] ret = new T[_indexis.Count];
            for (int i = 0; i < _indexis.Count; i++)
            {
                try
                {
                    ret[i] = (T)data[i];
                }
                catch
                {
                    throw new Exception($"Элемент {data[i]}, с индексом {i}, не может быть преобразован в тип {typeof(T)}");
                }
            }

            return ret;
        }

        /// <summary>
        /// Получить строку по индексу с представлением в виде вектора
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        public Vector RowToVector(int rowIndex)
        {
            return GetRow<double>(rowIndex);
        }

        /// <summary>
        /// Преобразование категорий в индексы (возвращает словарь для преобразования)
        /// </summary>
        /// <param name="colIndex">Индекс столбца</param>
        /// <returns></returns>
        public Dictionary<object, int> ColumnToCategorical(int colIndex = 0)
        {
            string name = _indexis[colIndex];
            return ColumnToCategorical(name);
        }

        /// <summary>
        /// Преобразование категорий в индексы (возвращает словарь для преобразования)
        /// </summary>
        /// <param name="colName">Имя столбца</param>
        /// <returns></returns>
        public Dictionary<object, int> ColumnToCategorical(string colName)
        {
            return this[colName].SelfCategoryToIndex();
        }

        /// <summary>
        /// Получить срез по глубине(строкам)
        /// </summary>
        /// <param name="indStart">Начальный индекс</param>
        /// <param name="len">Конечный индекс</param>
        public DataTable GetSlice(int indStart, int len)
        {
            DataTable dataTable = new DataTable();

            for (int i = 0; i < _indexis.Count; i++)
            {
                DataItem source = _frame[_indexis[i]];
                DataItem items = new DataItem(_indexis[i], new List<dynamic>(len - indStart));

                items.TypeColum = source.TypeColum;

                for (int j = indStart; j < len; j++)
                    items.Data[j] = source.Data[j];

                dataTable.Add(items);
                dataTable._indexis[i] = _indexis[i];
            }

            return dataTable;
        }


        /// <summary>
        /// Отображение таблицы строкой
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int len = Len;

            DataItem[] dataItems = new DataItem[_indexis.Count];
            for (int i = 0; i < _indexis.Count; i++)
                dataItems[i] = _frame[_indexis[i]];


            for (int i = 0; i < len; i++)
            {
                stringBuilder.Append(i);
                stringBuilder.Append("\t");

                if (i == 9 && len > 25) i = len - 10; // Чтобы было не больше 20 строк

                for (int j = 0; j < _indexis.Count; j++)
                {
                    stringBuilder.Append(dataItems[j].Data[i]);
                    stringBuilder.Append("\t\t\t");
                }

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }


    }
}
