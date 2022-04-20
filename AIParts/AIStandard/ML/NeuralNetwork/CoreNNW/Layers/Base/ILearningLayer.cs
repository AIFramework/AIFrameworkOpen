using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Trainable layer
    /// </summary>
    public interface ILearningLayer : ILayer
    {
        /// <summary>
        /// Initialize layer weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        void InitWeights(Random random);
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        List<NNValue> GetParameters();
    }
}