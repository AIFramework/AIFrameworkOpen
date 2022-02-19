using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    /// <summary>
    /// Information about training a neural network
    /// </summary>
    [Serializable]
    public class TrainInfo
    {
        /// <summary>
        /// Validation error
        /// </summary>
        public Vector ValidationLoss { get; set; }
        /// <summary>
        /// Train error
        /// </summary>
        public Vector TrainLoss { get; set; }
        /// <summary>
        /// Test error
        /// </summary>
        public float TestLoss { get; set; }

        /// <summary>
        /// Information about training a neural network
        /// </summary>
        public TrainInfo()
        {
            TrainLoss = new Vector(0);
            ValidationLoss = new Vector(0);
        }
    }
}
