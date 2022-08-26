using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Dataset interface
    /// </summary>
    public interface IDataSet
    {
        /// <summary>
        /// Input data dimension
        /// </summary>
        Shape3D InputShape { get; }
        /// <summary>
        /// Output data dimension
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Loss function
        /// </summary>
        ILoss LossFunction { get; set; }
        /// <summary>
        /// Training subset
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