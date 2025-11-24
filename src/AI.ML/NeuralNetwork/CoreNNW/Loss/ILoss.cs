namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Интерфейс функции ошибки
    /// </summary>
    public interface ILoss
    {
        /// <summary>
        /// Обратный проход
        /// </summary>
        /// <param name="actualOutput"></param>
        /// <param name="targetOutput"></param>
        void Backward(NNValue actualOutput, NNValue targetOutput);
        /// <summary>
        /// Значение ошибки
        /// </summary>
        float Measure(NNValue actualOutput, NNValue targetOutput);
    }
}
