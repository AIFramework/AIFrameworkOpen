using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Fully connected forward propagation layer
    /// </summary>
    [Serializable]
    public class FeedForwardLayer : IActivatableLayer, ILearningLayer
    {
        /// <summary>
        /// Weighting matrix
        /// </summary>
        public NNValue W { get; set; }
        /// <summary>
        /// Hyperplane displacement vector (neuron polarization)
        /// </summary>
        public NNValue Bias { get; set; }
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => W.Shape.Count + Bias.Shape.Height;
        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }

        /// <summary>
        /// Fully connected layer
        /// </summary>
        /// <param name="inputDimension">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="f">Activation function</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public FeedForwardLayer(int inputDimension, int outputDimension, IActivation f, Random rnd)
        {
            double initParamsStdDev = 1.0 / Math.Sqrt(outputDimension);
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            W = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
            Bias = new NNValue(outputDimension);
            ActivationFunction = f;
        }
        /// <summary>
        /// Fully connected layer
        /// </summary>
        /// <param name="inputShape">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="f">Activation function</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public FeedForwardLayer(Shape3D inputShape, int outputDimension, IActivation f, Random rnd)
        {
            double initParamsStdDev = 1.0 / Math.Sqrt(outputDimension);
            InputShape = inputShape;
            OutputShape = new Shape3D(outputDimension);
            W = NNValue.Random(OutputShape.Height, InputShape.Height, initParamsStdDev, rnd);
            Bias = new NNValue(outputDimension);
            ActivationFunction = f;
        }
        /// <summary>
        /// Fully connected layer
        /// </summary>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="f">Activation function</param>
        public FeedForwardLayer(int outputDimension, IActivation f = null)
        {
            OutputShape = new Shape3D(outputDimension);

            ActivationFunction = f ?? new LinearUnit();
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue sum = g.Add(g.Mul(W, input), Bias);
            NNValue returnObj = g.Activate(ActivationFunction, sum);
            return returnObj;
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue> { W, Bias };
        }
        /// <summary>
        /// Generating weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Height);
            W = NNValue.Random(OutputShape.Height, InputShape.Height, std, random);
            Bias = new NNValue(OutputShape.Height);
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
            W.OnlyUse();
            Bias.OnlyUse();
        }
    }
}