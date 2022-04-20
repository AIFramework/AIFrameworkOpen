using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Neural network training dataset
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Tr. count = {TrainingInternal.Count}, Val. count = {ValidationInternal.Count}, Test. count = {TestingInternal.Count}")]
    public class DataSet : IDataSet
    {
        /// <summary>
        /// Input data dimension
        /// </summary>
        public Shape3D InputShape { get; }
        /// <summary>
        /// Output data dimension
        /// </summary>
        public Shape3D OutputShape { get; }
        /// <summary>
        /// Loss function
        /// </summary>
        public ILoss LossFunction { get; set; }
        /// <summary>
        /// Training subset internal storage
        /// </summary>
        protected List<DataSequence> TrainingInternal { get; }
        /// <summary>
        /// Training subset
        /// </summary>
        public IReadOnlyList<DataSequence> Training => TrainingInternal.AsReadOnly();
        /// <summary>
        /// Validation subset internal storage
        /// </summary>
        protected List<DataSequence> ValidationInternal { get; }
        /// <summary>
        /// Validation subset
        /// </summary>
        public IReadOnlyList<DataSequence> Validation => ValidationInternal.AsReadOnly();
        /// <summary>
        /// Tells if validation subset is present
        /// </summary>
        public bool HasValidationData => ValidationInternal != null && ValidationInternal.Count > 0;
        /// <summary>
        /// Testing subset internal storage
        /// </summary>
        protected List<DataSequence> TestingInternal { get; }
        /// <summary>
        /// Testing subset
        /// </summary>
        public IReadOnlyList<DataSequence> Testing => TestingInternal.AsReadOnly();
        /// <summary>
        /// Tells if testing subset is present
        /// </summary>
        public bool HasTestingData => TestingInternal != null && TestingInternal.Count > 0;

        /// <summary>
        /// Initialize dataset with given input and output shape and loss
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="outputShape"></param>
        /// <param name="loss"></param>
        protected DataSet(Shape inputShape, Shape outputShape, ILoss loss)
        {
            if (inputShape == null)
            {
                throw new ArgumentNullException(nameof(inputShape));
            }

            if (outputShape == null)
            {
                throw new ArgumentNullException(nameof(outputShape));
            }

            if (inputShape.Rank > 3)
            {
                throw new ArgumentException("Rank of the input shape is greater than 3", nameof(inputShape));
            }

            switch (inputShape.Rank)
            {
                case 1:
                    InputShape = new Shape3D(1, inputShape[0]);
                    break;
                case 2:
                    InputShape = new Shape3D(inputShape[1], inputShape[0]);
                    break;
                case 3:
                    InputShape = new Shape3D(inputShape[1], inputShape[0], inputShape[2]);
                    break;
                default:
                    break;
            }

            if (outputShape.Rank > 3)
            {
                throw new ArgumentException("Rank of the output shape if greater than 3", nameof(inputShape));
            }

            switch (outputShape.Rank)
            {
                case 1:
                    OutputShape = new Shape3D(1, outputShape[0]);
                    break;
                case 2:
                    OutputShape = new Shape3D(outputShape[1], outputShape[0]);
                    break;
                case 3:
                    OutputShape = new Shape3D(outputShape[1], outputShape[0], outputShape[2]);
                    break;
                default:
                    break;
            }

            LossFunction = loss;

            TrainingInternal = new List<DataSequence>();
            ValidationInternal = new List<DataSequence>();
            TestingInternal = new List<DataSequence>();
        }
    }
}