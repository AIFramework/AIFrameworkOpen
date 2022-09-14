using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{

    /// <summary>
    /// Аргументы обучения
    /// </summary>
    [Serializable]
    public class TrainingEventArgs : EventArgs
    {
        /// <summary>
        /// Число эпох
        /// </summary>
        public int TrainingEpoches { get; }
        /// <summary>
        /// Размер подвыборки
        /// </summary>
        public int BatchSize { get; }
        /// <summary>
        /// Скорость обучения
        /// </summary>
        public float LearningRate { get; }
        /// <summary>
        /// Нейронная сеть
        /// </summary>
        public INetwork Model { get; }
        /// <summary>
        /// Обучающая выборка
        /// </summary>
        public IDataSet Data { get; }
        /// <summary>
        /// Минимальная ошибка
        /// </summary>
        public float MinLoss { get; }

        /// <summary>
        /// Аргументы обучения
        /// </summary>
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