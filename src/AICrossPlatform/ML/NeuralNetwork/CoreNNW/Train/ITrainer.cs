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
        /// <param name="data">Data set</param>
        /// <param name="minLoss">Minimal loss value</param>
        /// <returns>Loss</returns>
        void Train(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss);
        /// <summary>
        /// Асинхронное обучение нейронной сети
        /// </summary>
        /// <param name="epochesToPass">Число эпох</param>
        /// <param name="batchSize">Размер подвыборки</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="data">Data set</param>
        /// <param name="minLoss">Minimal loss value</param>
        /// <returns>Loss</returns>
        Task TrainAsync(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss, CancellationToken cancellationToken = default);
    }
}
