using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    ///  BatchReNormalization (Normalization of inputs during online learning)
    /// </summary>
    [Serializable]
    public class BatchReNormalization : ILearningLayer
    {
        private NNValue _beta, _gamma;
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
                _beta = new NNValue(value);
                _gamma = NNValue.Ones(value);
            }
        }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => _beta == null ? 0 : 2 * _beta.Shape.Count;
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt => 0;

        /// <summary>
        ///  BatchReNormalization (Normalization of inputs during online learning)
        /// </summary>
        public BatchReNormalization()
        {
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.Add(g.AdamarMul(_gamma, input), _beta);
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
            return new List<NNValue>()
            {
                _beta, _gamma
            };
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription("BatchReNorm", InputShape, OutputShape, "None", TrainableParameters);
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
            _beta.OnlyUse();
            _gamma.OnlyUse();
        }
    }
}