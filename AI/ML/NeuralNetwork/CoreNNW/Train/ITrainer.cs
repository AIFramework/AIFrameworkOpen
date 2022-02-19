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
        /// Network training
        /// </summary>
        /// <param name="epochesToPass">Number of epochs</param>
        /// /// <param name="batchSize">Batch size</param>
        /// <param name="learningRate">Learning rate</param>
        /// <param name="network">Neural network</param>
        /// <param name="data">Data set</param>
        /// <param name="minLoss">Minimal loss value</param>
        /// <returns>Loss</returns>
        void Train(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss);
        /// <summary>
        /// Network async training
        /// </summary>
        /// <param name="epochesToPass">Number of epochs</param>
        /// <param name="batchSize">Batch size</param>
        /// <param name="learningRate">Learning rate</param>
        /// <param name="network">Neural network</param>
        /// <param name="data">Data set</param>
        /// <param name="minLoss">Minimal loss value</param>
        /// <returns>Loss</returns>
        Task TrainAsync(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss, CancellationToken cancellationToken = default);
    }
}
