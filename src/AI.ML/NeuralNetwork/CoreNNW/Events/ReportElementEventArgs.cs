using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    /// <summary>
    /// Элемент отчета об обучении
    /// </summary>
    [Serializable]
    public class ReportElementEventArgs : EventArgs
    {
        /// <summary>
        /// Тип элемента отчета об обучении
        /// </summary>
        public ReportElementType ReportElementType { get; }
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Элемент отчета об обучении
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public ReportElementEventArgs(ReportElementType type, string message)
        {
            ReportElementType = type;
            Message = message;
        }
    }
}