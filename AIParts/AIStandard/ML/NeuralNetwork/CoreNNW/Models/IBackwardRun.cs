using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Models
{
    /// <summary>
    /// Interface for starting the calculation of derivatives by a chain rule
    /// </summary>
    public interface IBackwardRun
    {
        /// <summary>
        /// Starting the calculation of derivatives
        /// </summary>
        Action StartCalc { get; set; }
    }
}