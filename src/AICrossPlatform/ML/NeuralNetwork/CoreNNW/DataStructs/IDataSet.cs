using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Интерфейс набор обучающих данных
    /// </summary>
    public interface IDataSet
    {
        /// <summary>
        /// Размерность входных данных
        /// </summary>
        Shape3D InputShape { get; }
        /// <summary>
        /// Размерность выходных данных
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Функция ошибки
        /// </summary>
        ILoss LossFunction { get; set; }
        /// <summary>
        /// Обучающая выборка
        /// </summary>
        IReadOnlyList<DataSequence> Training { get; }
        /// <summary>
        /// Валидационная подвыборка
        /// </summary>
        IReadOnlyList<DataSequence> Validation { get; }
        /// <summary>
        /// Есть ли валидационная подвыборка
        /// </summary>
        bool HasValidationData { get; }
        /// <summary>
        /// Тестовая подвыборка
        /// </summary>
        IReadOnlyList<DataSequence> Testing { get; }
        /// <summary>
        /// Есть ли тестовая подвыборка
        /// </summary>
        bool HasTestingData { get; }
    }
}