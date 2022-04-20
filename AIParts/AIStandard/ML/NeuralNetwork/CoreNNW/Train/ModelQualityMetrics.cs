namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    internal class ModelQualityMetrics
    {
        public float TrainLoss { get; set; }
        public float ValLoss { get; set; }
        public float ValMetric { get; set; }
        public string Report { get; set; }
    }
}