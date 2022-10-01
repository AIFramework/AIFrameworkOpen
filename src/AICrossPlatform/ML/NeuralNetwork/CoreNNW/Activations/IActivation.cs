namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Интерфейс актив. функций
    /// </summary>
    public interface IActivation
    {
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        float Numerator { get; }
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        NNValue Forward(NNValue x);
        /// <summary>
        /// Обратный проход
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        NNValue Backward(NNValue x);
    }
}
