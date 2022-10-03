using AI.ML.NeuralNetwork.CoreNNW.Activations;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Слой с активационной функции
    /// </summary>
    public interface IActivatableLayer : ILayer
    {
        /// <summary>
        /// Активационная функция
        /// </summary>
        IActivation ActivationFunction { get; set; }
    }
}