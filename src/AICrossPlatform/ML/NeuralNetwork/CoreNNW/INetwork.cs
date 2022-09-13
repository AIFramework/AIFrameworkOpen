using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW
{
    /// <summary>
    /// Интерфейс нейронной сети
    /// </summary>
    public interface INetwork : ISavable
    {
        /// <summary>
        /// Inputs shape
        /// </summary>
        Shape3D InputShape { get; set; }
        /// <summary>
        /// Outputs shape
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Number of trainable parameters
        /// </summary>
        int TrainableParameters { get; }
        /// <summary>
        /// List of layers
        /// </summary>
        List<ILayer> Layers { get; set; }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Input data</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        /// <returns></returns>
        NNValue Forward(NNValue input, INNWGraph g);
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Input data</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        /// <returns></returns>
        NNValue Forward(IAlgebraicStructure input, INNWGraph g);
        /// <summary>
        /// Reset state, must be in a recurrent neural network
        /// </summary>
        void ResetState();
        /// <summary>
        /// Обучаемые параметры
        /// </summary>
        /// <returns></returns>
        List<NNValue> GetParameters();
        /// <summary>
        /// Append a new layer to the network
        /// </summary>
        /// <param name="layer">New layer</param>
        void AddNewLayer(ILayer layer);
        /// <summary>
        /// Append a new layer to the network
        /// </summary>
        /// <param name="inputShape">Input shape</param>
        /// <param name="layer">new layer</param>
        void AddNewLayer(Shape3D inputShape, ILayer layer);

        /// <summary>
        /// Только для использования (удаляет кэши важные для обучения, делает сеть в 4раза меньше)
        /// </summary>
        void OnlyUse();
    }
}