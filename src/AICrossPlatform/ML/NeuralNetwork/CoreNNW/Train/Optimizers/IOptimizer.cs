namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Optimizer
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// Updating parameters 
        /// </summary>
        /// <param name="network"> Neural network</param>
        /// <param name="learningRate"> Learning rate</param>
        /// <param name="gradClip"> Maximum gradient value</param>
        /// <param name="gradGain">Gradient enhancement factor</param>
        /// <param name="L1">L1 regularization</param>
        /// <param name="L2">L2 regularization</param>
        void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float gradGain);

        /// <summary>
        /// Resetting Teaching Parameters
        /// </summary>
        void Reset();
    }
}
