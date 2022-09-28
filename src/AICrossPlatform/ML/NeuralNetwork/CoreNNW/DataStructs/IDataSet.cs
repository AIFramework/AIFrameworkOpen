using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Набор данных interface
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
        /// Validation subset
        /// </summary>
        IReadOnlyList<DataSequence> Validation { get; }
        /// <summary>
        /// Tells if validation subset is present
        /// </summary>
        bool HasValidationData { get; }
        /// <summary>
        /// Testing subset
        /// </summary>
        IReadOnlyList<DataSequence> Testing { get; }
        /// <summary>
        /// Tells if testing subset is present
        /// </summary>
        bool HasTestingData { get; }
    }
}