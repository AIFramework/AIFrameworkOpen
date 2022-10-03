namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Интерфейс рекуррентного слоя
    /// </summary>
    public interface IRecurrentLayer : ILayer
    {
        /// <summary>
        /// Сброс состояния нейронной сети
        /// </summary>
        void ResetState();
    }
}