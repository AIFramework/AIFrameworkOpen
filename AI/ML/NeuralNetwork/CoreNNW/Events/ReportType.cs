using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    /// <summary>
    /// Report type
    /// </summary>
    /// 
    [Serializable]
    public enum ReportType : byte
    {
        ConsoleReport,
        EventReport,
        None
    }
}