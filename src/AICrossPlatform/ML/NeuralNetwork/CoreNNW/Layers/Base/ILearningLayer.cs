using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Обучаемый слой
    /// </summary>
    public interface ILearningLayer : ILayer
    {
        /// <summary>
        /// Инициализация слоя
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        void InitWeights(Random random);
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        List<NNValue> GetParameters();
    }
}