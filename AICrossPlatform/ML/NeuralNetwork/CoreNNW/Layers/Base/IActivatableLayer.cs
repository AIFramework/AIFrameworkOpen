using AI.ML.NeuralNetwork.CoreNNW.Activations;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Layer with activation function
    /// </summary>
    public interface IActivatableLayer : ILayer
    {
        /// <summary>
        /// Activation function
        /// </summary>
        IActivation ActivationFunction { get; set; }
    }
}