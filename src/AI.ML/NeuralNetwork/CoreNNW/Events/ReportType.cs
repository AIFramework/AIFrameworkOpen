using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{
    /// <summary>
    /// Тип отчета(обучение)
    /// </summary>
    [Serializable]
    public enum ReportType : byte
    {
        /// <summary>
        /// Вывод отчета в консоль
        /// </summary>
        ConsoleReport,
        /// <summary>
        /// Создание событий с отчетом об обучении
        /// </summary>
        EventReport,
        /// <summary>
        /// Не выводить отчет
        /// </summary>
        None
    }
}