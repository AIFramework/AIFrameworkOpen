using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Convolutional network filter structure
    /// </summary>
    [Serializable]
    public class FilterStruct
    {
        /// <summary>
        /// Filter width
        /// </summary>
        public int FilterW { get; set; }
        /// <summary>
        /// Filter height
        /// </summary>
        public int FilterH { get; set; }
        /// <summary>
        /// Number of filters
        /// </summary>
        public int FilterCount { get; set; }
        /// <summary>
        /// Product of height by width and number of filters
        /// </summary>
        public int Volume => FilterCount * FilterH * FilterW;

        public FilterStruct(int h = 3, int w = 3, int count = 4)
        {
            FilterW = w;
            FilterH = h;
            FilterCount = count;
        }
    }
}