using AI.ML.NeuralNetwork.CoreNNW.Activations;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Layer with Активационная функция
    /// </summary>
    public interface IActivatableLayer : ILayer
    {
        /// <summary>
        /// Активационная функция
        /// </summary>
        IActivation ActivationFunction { get; set; }
    }
}