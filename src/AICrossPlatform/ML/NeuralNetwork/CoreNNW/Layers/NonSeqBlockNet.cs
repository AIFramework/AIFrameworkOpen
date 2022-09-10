using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Непоследовательный блок
    /// </summary>
    [Serializable]
    public class NonSeqBlockNet : IActivatableLayer, ILearningLayer, IRecurrentLayer
    {
        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters
        {
            get
            {
                int sum = 0;
                foreach (var layer in _layers)
                    if (layer is ILearningLayer)
                        sum += (layer as ILearningLayer).TrainableParameters;

                return sum;
            }
        }
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        private readonly Func<NNValue, INNWGraph, NNValue> _forward;
        private readonly List<ILayer> _layers;

        /// <summary>
        /// Непоследовательный блок
        /// </summary>
        public NonSeqBlockNet(Shape3D inputDimension, Shape3D outputDimension, List<ILayer> layers, Func<NNValue, INNWGraph, NNValue> forward)
        {
            InputShape = inputDimension;
            OutputShape = outputDimension;
            _forward = forward;
            _layers = layers;
            ResetState();
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return _forward(input, g);
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            foreach (var layer in _layers)
            {
                if (layer is IRecurrentLayer)
                    (layer as IRecurrentLayer).ResetState();
            }
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> block_params = new List<NNValue>();

            foreach (var layer in _layers)
            {
                if (layer is ILearningLayer)
                {
                    block_params.AddRange(
                        (layer as ILearningLayer).GetParameters());
                }
            }

            return block_params;
        }
        /// <summary>
        /// Generating weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        public void InitWeights(Random random)
        {
            foreach (var layer in _layers)
                if (layer is ILearningLayer)
                    (layer as ILearningLayer).InitWeights(random);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            foreach (var layer in _layers)
            {
                layer.OnlyUse();
            }
        }
    }
}
