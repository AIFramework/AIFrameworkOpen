namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    public enum Metrics : byte
    {
        Precision,
        Recall,
        F1,
        Accuracy,
        MAE,
        MAPE,
        MSE,
        RMSE,
        /// <summary>
        /// RMLE (target[i]>-1, output[i]>-1 for all i \in [0; N-1])
        /// </summary>
        RMSLE,
        R2
    }
}
