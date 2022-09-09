using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Простая рекурентная сеть
    /// </summary>
    [Serializable]
    public class RNNLayer : IActivatableLayer, ILearningLayer, IRecurrentLayer
    {
        private NNValue _w;
        private NNValue _b;
        private NNValue _context;

        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => OutputShape.Height * (InputShape.Height + OutputShape.Height + 1);
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        /// <summary>
        /// Простая рекурентная сеть
        /// </summary>
        public RNNLayer(int inputDimension, int outputDimension, IActivation hiddenUnit, double initParamsStdDev, Random rng)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            ActivationFunction = hiddenUnit;
            _w = NNValue.Random(outputDimension, inputDimension + outputDimension, initParamsStdDev, rng);
            _b = new NNValue(outputDimension);
            ResetState();
        }
        /// <summary>
        /// Простая рекурентная сеть
        /// </summary>
        public RNNLayer(Shape3D inputShape, int outputDimension, IActivation hiddenUnit, double initParamsStdDev, Random rng)
        {
            int inputDimension = inputShape.Height;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            ActivationFunction = hiddenUnit;
            _w = NNValue.Random(outputDimension, inputDimension + outputDimension, initParamsStdDev, rng);
            _b = new NNValue(outputDimension);
            ResetState();
        }
        /// <summary>
        /// Простая рекурентная сеть
        /// </summary>
        public RNNLayer(int outputDimension, IActivation hiddenUnit = null)
        {
            OutputShape = new Shape3D(outputDimension);

            ActivationFunction = hiddenUnit ?? new LinearUnit();
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue concat = g.ConcatinateVectors(input, _context);
            NNValue sum = g.Mul(_w, concat); sum = g.Add(sum, _b);
            NNValue output = g.Activate(ActivationFunction, sum);

            //rollover activations for next iteration
            _context = output;

            if (float.IsNaN(_context[0])) ResetState();

            return output;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            _context = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue> { _w, _b };
        }
        /// <summary>
        /// Generating weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Volume);
            Init(OutputShape.Height, ActivationFunction, std, random);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
            _w.OnlyUse();
            _b.OnlyUse();
            _context.OnlyUse();
        }

        #region Приватные методы
        private void Init(int outputDimension, IActivation hiddenUnit, double initParamsStdDev, Random rng)
        {
            int inputDimension = InputShape.Height;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            ActivationFunction = hiddenUnit;
            _w = NNValue.Random(outputDimension, inputDimension + outputDimension, initParamsStdDev, rng);
            _b = new NNValue(outputDimension);
            ResetState();
        }
        #endregion
    }
}