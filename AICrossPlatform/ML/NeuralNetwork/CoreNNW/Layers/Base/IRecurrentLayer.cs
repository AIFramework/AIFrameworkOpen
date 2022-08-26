namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Interface for recurrent layers
    /// </summary>
    public interface IRecurrentLayer : ILayer
    {
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        void ResetState();
    }
}