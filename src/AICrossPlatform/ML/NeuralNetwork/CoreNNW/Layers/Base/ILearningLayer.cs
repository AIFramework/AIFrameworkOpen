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
        /// Initialize layer weights
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        void InitWeights(Random random);
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        List<NNValue> GetParameters();
    }
}