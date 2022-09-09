using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Models;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Layer interface
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// Input dimension
        /// </summary>
        Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        int TrainableParameters { get; }
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        double AddDenInSqrt { get; }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        NNValue Forward(NNValue input, INNWGraph g);
        /// <summary>
        /// Only use mode
        /// </summary>
        void OnlyUse();
    }
}