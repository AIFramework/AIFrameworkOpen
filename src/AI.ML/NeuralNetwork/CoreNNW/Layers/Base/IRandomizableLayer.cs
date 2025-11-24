using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Слой, для работы которого требуется рандомизатор
    /// </summary>
    public interface IRandomizableLayer : ILayer
    {
        /// <summary>
        /// Генератор псевдослучайных чисел
        /// </summary>
        Random Random { set; }
    }
}
