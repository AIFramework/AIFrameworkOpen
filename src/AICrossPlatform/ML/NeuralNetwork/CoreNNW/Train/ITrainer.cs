using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using System.Threading;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    /// <summary>
    /// Интерфейс учителя
    /// </summary>
    public interface ITrainer
    {
        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="epochesToPass">Число эпох</param>
        /// /// <param name="batchSize">Размер подвыборки</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="data">Выборка</param>
        /// <param name="minLoss">Минимальное значение ошибки</param>
        /// <returns>Ошибка</returns>
        void Train(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss);
        /// <summary>
        /// Асинхронное обучение нейронной сети
        /// </summary>
        /// <param name="epochesToPass">Число эпох</param>
        /// <param name="batchSize">Размер подвыборки</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="data">Выборка</param>
        /// <param name="minLoss">Минимальное значение ошибки</param>
        /// <param name="cancellationToken">Токен завершения</param>
        /// <returns>Ошибка</returns>
        Task TrainAsync(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss, CancellationToken cancellationToken = default);
    }
}
