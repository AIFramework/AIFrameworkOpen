using AI.ML.NeuralNetwork.CoreNNW.Utilities;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    public class EpochPassedEventArgs
    {
        public TrainingEventArgs TrainingArgs { get; set; }
        public int CurrentEpochNumber { get; set; }

        public float TrainingLoss { get; set; }

        public float? ValidationLoss { get; set; }
        public bool HasValidationLoss => ValidationLoss != null;

        public float? ValidationMetrics { get; set; }
        public Metrics? Metrics { get; set; }
        public bool HasValidationMetrics => ValidationMetrics != null;

        public float? TestLoss { get; set; }
        public bool HasTestLoss => TestLoss != null;
    }
}