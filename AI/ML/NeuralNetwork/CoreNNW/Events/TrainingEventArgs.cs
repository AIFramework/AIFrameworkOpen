using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    [Serializable]
    public class TrainingEventArgs : EventArgs
    {
        /// <summary>
        /// Total epoches that need to be passed
        /// </summary>
        public int TrainingEpoches { get; }
        /// <summary>
        /// Batch size
        /// </summary>
        public int BatchSize { get; }
        /// <summary>
        /// Learning rate
        /// </summary>
        public float LearningRate { get; }
        /// <summary>
        /// Neural network
        /// </summary>
        public INetwork Model { get; }
        /// <summary>
        /// Training dataset
        /// </summary>
        public IDataSet Data { get; }
        /// <summary>
        /// Minimal loss
        /// </summary>
        public float MinLoss { get; }

        public TrainingEventArgs(int trainingEpoches, int batchSize, float learningRate, INetwork model, IDataSet data, float minLoss)
        {
            TrainingEpoches = trainingEpoches;
            BatchSize = batchSize;
            LearningRate = learningRate;
            Model = model;
            Data = data;
            MinLoss = minLoss;
        }
    }
}