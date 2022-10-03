using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Models
{
    /// <summary>
    /// Интерфейс для запуска расчета производных по цепному правилу
    /// </summary>
    public interface IBackwardRun
    {
        /// <summary>
        /// Запуск расчета производных
        /// </summary>
        Action StartCalc { get; set; }
    }
}