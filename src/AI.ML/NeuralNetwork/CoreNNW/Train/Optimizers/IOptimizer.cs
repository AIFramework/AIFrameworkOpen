namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Оптимизатор
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// Обновление параметров
        /// </summary>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="gradClip"> Максимальное значение градиента</param>
        /// <param name="gradGain">Усиление градиента (множитель)</param>
        /// <param name="L1">L1 регуляризация</param>
        /// <param name="L2">L2 регуляризация</param>
        void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float gradGain);

        /// <summary>
        /// Сброс параметров обучения
        /// </summary>
        void Reset();
    }
}
