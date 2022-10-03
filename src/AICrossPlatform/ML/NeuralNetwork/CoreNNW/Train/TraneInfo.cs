using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    /// <summary>
    /// Информация об обучении нейронной сети
    /// </summary>
    [Serializable]
    public class TrainInfo
    {
        /// <summary>
        /// Ошибка валидации
        /// </summary>
        public Vector ValidationLoss { get; set; }
        /// <summary>
        /// Ошибка обучения
        /// </summary>
        public Vector TrainLoss { get; set; }
        /// <summary>
        /// Ошибка на тестовой выборке
        /// </summary>
        public float TestLoss { get; set; }

        /// <summary>
        /// Данные обучения нейронной сети
        /// </summary>
        public TrainInfo()
        {
            TrainLoss = new Vector(0);
            ValidationLoss = new Vector(0);
        }
    }
}
