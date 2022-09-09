namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Loss function interface
    /// </summary>
    public interface ILoss
    {
        /// <summary>
        /// Backward pass (taking derivative)
        /// </summary>
        /// <param name="actualOutput">Output value (actual)</param>
        /// <param name="targetOutput">Target value (ideal)</param>
        void Backward(NNValue actualOutput, NNValue targetOutput);
        /// <summary>
        /// Error value
        /// </summary>
        /// <param name="actualOutput">Output value (actual)</param>
        /// <param name="targetOutput">Target value (ideal)</param>
        float Measure(NNValue actualOutput, NNValue targetOutput);
    }
}
