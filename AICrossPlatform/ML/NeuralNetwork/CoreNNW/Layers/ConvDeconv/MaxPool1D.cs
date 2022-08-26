using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// One-dimensional subsampling layer
    /// </summary>
    [Serializable]
    public class MaxPool1D : ILayer
    {
        private readonly MaxPooling _maxPool;

        /// <summary>
        /// Dimension and shape of the input tensor
        /// </summary>
        public Shape3D InputShape { get => _maxPool.InputShape; set => _maxPool.InputShape = value; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape => _maxPool.OutputShape;
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => _maxPool.TrainableParameters;
        /// <summary>
        /// Adding a value to the denominator under the root when initializing the weights
        /// </summary>
        public double AddDenInSqrt => _maxPool.AddDenInSqrt;

        /// <summary>
        /// Subsampling (Maxpooling 1D)
        /// </summary>
        /// <param name="inputShape">Dimension and shape of the input tensor</param>
        /// <param name="k">How many times to compress out</param>
        public MaxPool1D(Shape3D inputShape, int k = 2)
        {
            _maxPool = new MaxPooling(inputShape, k, 1);
        }
        /// <summary>
        /// Subsampling (Maxpooling 1D)
        /// </summary>
        /// <param name="k">How many times to compress out</param>
        public MaxPool1D(int k = 2)
        {
            _maxPool = new MaxPooling(k, 1);
        }

        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            return _maxPool.Forward(input, g);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }

        public void OnlyUse()
        {
        }
    }
}