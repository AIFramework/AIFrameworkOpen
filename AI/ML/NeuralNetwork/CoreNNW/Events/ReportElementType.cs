using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    [Serializable]
    public class ReportElementType : IEquatable<ReportElementType>
    {
        public static readonly ReportElementType TrainingStarted = new ReportElementType("TrainingStarted");
        public static readonly ReportElementType TrainingFinished = new ReportElementType("TrainingFinished");
        public static readonly ReportElementType TrainingStopped = new ReportElementType("TrainingStopped");
        public static readonly ReportElementType TrainingCancelled = new ReportElementType("TrainingCancelled");
        public static readonly ReportElementType EpochPassed = new ReportElementType("EpochPassed");
        public static readonly ReportElementType CheckPointSaved = new ReportElementType("CheckPointSaved");

        public string Value { get; set; }

        public ReportElementType(string value)
        {
            Value = value;
        }

        public static bool operator ==(ReportElementType left, ReportElementType right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(ReportElementType left, ReportElementType right)
        {
            return left.Value != right.Value;
        }

        #region Технические методы
        public override string ToString()
        {
            return $"ReportElementType[{Value}]";
        }

        public override bool Equals(object obj)
        {
            if (obj is ReportElementType elementType)
            {
                return Value == elementType.Value;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ReportElementType other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
    }
}