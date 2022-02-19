using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Models
{
    /// <summary>
    /// Class for starting the calculation of derivatives by a chain rule
    /// </summary>
    [Serializable]
    public class Runnable : IBackwardRun
    {
        /// <summary>
        /// Class for starting the calculation of derivatives by a chain rule
        /// </summary>
        public Action StartCalc { get; set; }
    }
}