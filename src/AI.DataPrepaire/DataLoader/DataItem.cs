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
    }
}
