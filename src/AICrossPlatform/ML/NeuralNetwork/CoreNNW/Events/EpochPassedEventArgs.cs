using AI.ML.NeuralNetwork.CoreNNW.Utilities;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Events
{

    /// <summary>
    /// Параметры события эпохи обучения
    /// </summary>
    [Serializable]
    public class EpochPassedEventArgs
    {
        /// <summary>
        /// Параметры события обучения
        /// </summary>
        public TrainingEventArgs TrainingArgs { get; set; }
        /// <summary>
        /// Номер эпохи обучения
        /// </summary>
        public int CurrentEpochNumber { get; set; }
        /// <summary>
        /// Ошибка на тренировочном наборе данных
        /// </summary>
        public float TrainingLoss { get; set; }
        /// <summary>
        /// Ошибка на валидационном наборе данных
        /// </summary>
        public float? ValidationLoss { get; set; }
        /// <summary>
        /// Имеется ли функция ошибки на валидации
        /// </summary>
        public bool HasValidationLoss => ValidationLoss != null;
        /// <summary>
        /// Качество на валидационной выборке
        /// </summary>
        public float? ValidationMetrics { get; set; }
        /// <summary>
        /// Метрика
        /// </summary>
        public Metrics? Metrics { get; set; }
        /// <summary>
        /// Имеется ли валидационная метрика
        /// </summary>
        public bool HasValidationMetrics => ValidationMetrics != null;
        /// <summary>
        /// Ошибка на тестовой выборке
        /// </summary>
        public float? TestLoss { get; set; }
        /// <summary>
        /// Имеется ли функция ошибки на тестовой выборке
        /// </summary>
        public bool HasTestLoss => TestLoss != null;
    }
}