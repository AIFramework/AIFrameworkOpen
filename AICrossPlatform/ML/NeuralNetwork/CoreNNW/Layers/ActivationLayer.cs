using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Nonlinear layer
    /// </summary>
    [Serializable]
    public class ActivationLayer : IActivatableLayer
    {
        private Shape3D _inputShape;

        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape
        {
            get => _inputShape;
            set
            {
                _inputShape = value;
                OutputShape = value;
            }
        }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of training parameters (0)
        /// </summary>
        public int TrainableParameters { get; private set; } = 0;
        /// <summary>
        /// Adding under the root (0)
        /// </summary>
        public double AddDenInSqrt { get; private set; } = 0;
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        /// <summary>
        /// Activation layer
        /// </summary>
        /// <param name="shapeInp">Input dimension</param>
        /// <param name="activation">Activation function</param>
        public ActivationLayer(Shape3D shapeInp, IActivation activation)
        {
            ActivationFunction = activation;
            InputShape = shapeInp;
        }
        /// <summary>
        /// Activation layer
        /// </summary>
        /// <param name="activation">Activation function</param>
        public ActivationLayer(IActivation activation)
        {
            ActivationFunction = activation;
        }

        /// <summary>
        /// Forward pass слоя
        /// </summary>
        /// <param name="input">Входной тензор</param>
        /// <param name="g">Graph of automatic differentiation</param>
        /// <returns></returns>
        public NNValue Forward(NNValue input, IGraph g)
        {
            return g.Activate(ActivationFunction, input);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
        }
    }
}