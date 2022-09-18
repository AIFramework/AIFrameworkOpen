namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Interface for recurrent layers
    /// </summary>
    public interface IRecurrentLayer : ILayer
    {
        /// <summary>
        /// Сброс состояния нейронной сети
        /// </summary>
        void ResetState();
    }
}