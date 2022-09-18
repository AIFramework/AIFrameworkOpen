using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Структура фильтра сверточной сети
    /// </summary>
    [Serializable]
    public class FilterStruct
    {
        /// <summary>
        /// Ширина фильтра
        /// </summary>
        public int FilterW { get; set; }
        /// <summary>
        /// Высота фильтра
        /// </summary>
        public int FilterH { get; set; }
        /// <summary>
        /// Число фильтров
        /// </summary>
        public int FilterCount { get; set; }
        /// <summary>
        /// Число параметров фильтра
        /// </summary>
        public int Volume => FilterCount * FilterH * FilterW;

        /// <summary>
        /// Структура фильтра сверточной сети
        /// </summary>
        public FilterStruct(int h = 3, int w = 3, int count = 4)
        {
            FilterW = w;
            FilterH = h;
            FilterCount = count;
        }
    }
}