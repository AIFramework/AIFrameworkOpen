namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Интерфейс актив. функций
    /// </summary>
    public interface IActivation
    {
        /// <summary>
        /// Initializer numerator
        /// </summary>
        float Numerator { get; }
        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        NNValue Forward(NNValue x);
        /// <summary>
        /// Bakward pass
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        NNValue Backward(NNValue x);
    }
}
