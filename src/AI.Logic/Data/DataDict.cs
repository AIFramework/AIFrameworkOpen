using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Logic.Data
{
    /// <summary>
    /// Элемент данных
    /// </summary>
    [Serializable]
    public class DataDict<ImageType, ElementType>
    {
        /// <summary>
        /// Данные задачи
        /// </summary>
        public Dictionary<ImageType, ElementType> TaskData { get; set; } = new Dictionary<ImageType, ElementType>();
    }
}
