using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.ConvDeconv
{
    /// <summary>
    /// One-dimensional convolutional layer
    /// </summary>
    [Serializable]
    public class Conv1D : IActivatableLayer, ILearningLayer
    {
        private readonly int _filters, _core;
        private readonly ConvolutionalLayer _convolution;

        /// <summary>
        /// Whether the dimension of the input is preserved
        /// </summary>
        public bool IsSame { get => _convolution.IsSame; set => _convolution.IsSame = value; }
        /// <summary>
        /// Dimension and shape of the input tensor
        /// </summary>
        public Shape3D InputShape { get => _convolution.InputShape; set => _convolution.InputShape = value; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape => _convolution.OutputShape;
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => _convolution.TrainableParameters;
        /// <summary>
        /// Adding a value to the denominator under the root when initializing the weights
        /// </summary>
        public double AddDenInSqrt => _convolution.AddDenInSqrt;
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get => _convolution.ActivationFunction; set => _convolution.ActivationFunction = value; }

        /// <summary>
        /// One-dimensional convolutional layer
        /// </summary>
        public Conv1D(Shape3D inputShape, int core, int filters, Random rnd, IActivation activation = null)
        {
            _filters = filters;
            _core = core;
            _convolution = new ConvolutionalLayer(inputShape: inputShape, new FilterStruct() { FilterCount = filters, FilterH = core, FilterW = 1 }, activation, rnd);
            ActivationFunction = activation ?? new LinearUnit();
        }
        /// <summary>
        /// One-dimensional convolutional layer
        /// </summary>
        public Conv1D(int core, int filters, IActivation activation = null)
        {
            _filters = filters;
            _core = core;
            _convolution = new ConvolutionalLayer(activation, filters, core, 1);
            ActivationFunction = activation ?? new LinearUnit();
        }

        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            return _convolution.Forward(input, g);
        }
        /// <summary>
        /// Initialize layer weights
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
            FilterStruct fs = new FilterStruct() { FilterCount = _filters, FilterH = _core, FilterW = 1 };
            _convolution.Init(fs, ActivationFunction, random);
        }
        /// <summary>
        /// Generating a neural network layer
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return _convolution.GetParameters();
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }

        public void OnlyUse()
        {
            _convolution.OnlyUse();
        }
    }
}