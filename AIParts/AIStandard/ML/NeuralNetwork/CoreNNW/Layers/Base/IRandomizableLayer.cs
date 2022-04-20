using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Layer that requires randomizer for work
    /// </summary>
    public interface IRandomizableLayer : ILayer
    {
        /// <summary>
        /// Random object for the layer
        /// </summary>
        Random Random { set; }
    }
}
