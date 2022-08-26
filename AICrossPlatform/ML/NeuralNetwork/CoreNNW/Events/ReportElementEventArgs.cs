using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    [Serializable]
    public class ReportElementEventArgs : EventArgs
    {
        public ReportElementType ReportElementType { get; }
        public string Message { get; }

        public ReportElementEventArgs(ReportElementType type, string message)
        {
            ReportElementType = type;
            Message = message;
        }
    }
}