using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    /// <summary>
    /// Интерфейс отчета
    /// </summary>
    public interface IReporter
    {
        /// <summary>
        /// Тип отчета
        /// </summary>
        ReportType ReportType { get; set; }
        /// <summary>
        /// Событие отчета
        /// </summary>
        event EventHandler<ReportElementEventArgs> ReportElementCreated;
    }
}