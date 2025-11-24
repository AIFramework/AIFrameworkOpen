using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    /// <summary>
    /// Метрики
    /// </summary>
    [Serializable]
    public enum Metrics : byte
    {
        /// <summary>
        /// Точность - для оценки классификации 
        /// </summary>
        Precision,
        /// <summary>
        /// Полнота - для оценки классификации 
        /// </summary>
        Recall,
        /// <summary>
        /// Среднее геометрическое точности и полноты (f1) - для оценки классификации 
        /// </summary>
        F1,
        /// <summary>
        /// Точность по всем классам (в случае дисбаланса классов малоинформативна) - для оценки классификации 
        /// </summary>
        Accuracy,
        /// <summary>
        /// MAE - для оценки регрессии 
        /// </summary>
        MAE,
        /// <summary>
        /// MAPE - для оценки регрессии 
        /// </summary>
        MAPE,
        /// <summary>
        /// Средний квадрат ошибки - для оценки регрессии 
        /// </summary>
        MSE,
        /// <summary>
        /// Средне квадратичное значение ошибки - для оценки регрессии 
        /// </summary>
        RMSE,
        /// <summary>
        /// RMLE (target[i]>-1, output[i]>-1 for all i \in [0; N-1]) - для оценки регрессии 
        /// </summary>
        RMSLE,
        /// <summary>
        /// Коэффициент корреляции Пирсона в квадрате - для оценки регрессии 
        /// </summary>
        R2
    }
}
