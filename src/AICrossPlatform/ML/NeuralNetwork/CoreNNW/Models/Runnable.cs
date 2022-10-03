using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Models
{
    /// <summary>
    /// Класс для запуска расчета производных по цепному правилу
    /// </summary>
    [Serializable]
    public class Runnable : IBackwardRun
    {
        /// <summary>
        /// Запуск расчета
        /// </summary>
        public Action StartCalc { get; set; }
    }
}