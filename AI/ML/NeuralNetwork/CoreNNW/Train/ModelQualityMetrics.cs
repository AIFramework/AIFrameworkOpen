using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    [Serializable]
    internal class ModelQualityMetrics
    {
        public float TrainLoss { get; set; }
        public float ValLoss { get; set; }
        public float ValMetric { get; set; }
        public string Report { get; set; }
    }
}