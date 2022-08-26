using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    public interface IReporter
    {
        ReportType ReportType { get; set; }

        event EventHandler<ReportElementEventArgs> ReportElementCreated;
    }
}