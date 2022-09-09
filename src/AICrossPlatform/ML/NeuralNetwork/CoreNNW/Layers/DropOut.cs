using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// DropOut layer
    /// </summary>
    [Serializable]
    public class DropOut : IRandomizableLayer
    {
        private readonly float _q, _nomalizer;
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
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => 0;
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt => 0;
        /// <summary>
        /// Random object for the layer
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// Dropout layer
        /// </summary>
        /// <param name="dropProb">Probability of disconnection with a neuron</param>
        public DropOut(float dropProb = 0.5f)
        {
            _q = 1.0f - dropProb;
            _nomalizer = 1.0f / _q;
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.DropOut(input, _q, _nomalizer, Random);
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }
    }
}