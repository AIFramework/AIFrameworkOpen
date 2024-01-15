using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;

namespace AI.DataPrepaire.DataLoader.NNWBlockLoader
{
    /// <summary>
    /// Интерфейс нейросетевого блока отображения вектора в вектор
    /// </summary>
    public interface INNWBlockV2V
    {
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        Vector Forward(Vector input);

        /// <summary>
        /// Перевод модуля в слой нейронной сети
        /// </summary>
        ILayer ToLayer();

    }
}
