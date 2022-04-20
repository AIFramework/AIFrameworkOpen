using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Data aggregation layer (Calculates average)
    /// </summary>
    [Serializable]
    public class Agregate : ILearningLayer
    {
        private NNValue _parametrs;
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
                _parametrs = NNValue.Uniform(1, value.Volume, 1.0 / value.Volume);
            }
        }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => 0;
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt => 0;

        /// <summary>
        /// Data aggregation layer (Calculates average)
        /// </summary>
        public Agregate()
        {
            OutputShape = new Shape3D(1);
        }
        /// <summary>
        /// Data aggregation layer (Calculates average)
        /// </summary>
        public Agregate(Shape3D inputShape)
        {
            InputShape = inputShape;
            OutputShape = new Shape3D(1);
            _parametrs = NNValue.Uniform(1, inputShape.Volume, 1.0 / inputShape.Volume);
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            return g.ScalarProduct(_parametrs, input);
        }
        /// <summary>
        /// Generating weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        public void InitWeights(Random random)
        {
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue>() { _parametrs };
        }

        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
            _parametrs.OnlyUse();
        }
    }
}