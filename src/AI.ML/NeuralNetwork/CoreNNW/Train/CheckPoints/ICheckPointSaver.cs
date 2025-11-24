namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{
    /// <summary>
    /// Интерфейс создания чекпоинтов
    /// </summary>
    public interface ICheckPointSaver
    {
        /// <summary>
        /// Сохранение нейронной сети
        /// </summary>
        /// <param name="net">Нейросеть</param>
        /// <param name="val">Метрика на валидационной выборке</param>
        void Save(INetwork net, float val);
    }
}