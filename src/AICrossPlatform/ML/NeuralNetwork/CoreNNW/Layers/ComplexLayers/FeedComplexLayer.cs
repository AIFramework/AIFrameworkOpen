using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.ComplexLayers
{
    [Serializable]
    public class FeedComplexLayer : IActivatableLayer, ILearningLayer
    {
        private NNValue _al1, _al2, _b1, _b2, _g1, _g2;
        private int _slices;
        private Shape3D _inputShape;

        /// <summary>
        /// Weighting matrix
        /// </summary>
        public NNValue WRe, WIm;
        /// <summary>
        /// Hyperplane displacement vector (neuron polarization)
        /// </summary>
        public NNValue BiasRe, BiasIm;
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => (2 * (WRe.Shape.Count + BiasRe.Shape.Height)) + 6;
        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape
        {
            get => _inputShape;
            set
            {
                _inputShape = value;
                _slices = value.Depth / 2;
            }
        }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }

        /// <summary>
        /// Полносвязный слой
        /// </summary>
        /// <param name="inputShape">Input dimension</param>
        /// <param name="outputDimension">Число выходов</param>
        /// <param name="f">Activation function</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public FeedComplexLayer(Shape3D inputShape, int outputDimension, IActivation f, Random rnd)
        {
            InputShape = inputShape;
            Init(outputDimension, rnd, 1.0 / Math.Sqrt(outputDimension));

            ActivationFunction = f;
        }
        /// <summary>
        /// Полносвязный слой
        /// </summary>
        /// <param name="outputDimension">Число выходов</param>
        /// <param name="f">Activation function</param>
        public FeedComplexLayer(int outputDimension, IActivation f = null)
        {
            OutputShape = new Shape3D(outputDimension, 1, 2);

            ActivationFunction = f ?? new LinearUnit();
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue[] imReal = g.DeepSplit(input, _slices);
            NNValue valueReal = g.FeedForwardLayer(imReal[0], WRe, BiasRe, ActivationFunction);
            NNValue valueIm = g.FeedForwardLayer(imReal[1], WIm, BiasIm, ActivationFunction);
            NNValue[] data = g.ImRealCross(valueReal, valueIm, _al1, _b1, _g1, _al2, _b2, _g2);
            NNValue returnObj = g.DeepJoin(data);

            return returnObj;
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                WRe,
                WIm,
                BiasRe,
                BiasIm,
                _al1,
                _al2,
                _b1,
                _b2,
                _g1,
                _g2
            };
            return result;
        }
        /// <summary>
        /// Generating weights
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Height);
            Init(OutputShape.Height, random, std);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }

        public void OnlyUse()
        {
            WRe.OnlyUse();
            WIm.OnlyUse();
            BiasRe.OnlyUse();
            BiasIm.OnlyUse();
            _al1.OnlyUse();
            _al2.OnlyUse();
            _b1.OnlyUse();
            _b2.OnlyUse();
            _g1.OnlyUse();
            _g2.OnlyUse();
        }

        #region Приватные методы
        private void Init(int outputDimension, Random rnd, double initParamsStdDev)
        {
            OutputShape = new Shape3D(outputDimension, 1, 2);
            WRe = NNValue.Random(OutputShape.Height, InputShape.Height, initParamsStdDev, rnd);
            WIm = NNValue.Random(OutputShape.Height, InputShape.Height, initParamsStdDev, rnd);
            BiasRe = new NNValue(OutputShape.Height);
            BiasIm = new NNValue(OutputShape.Height);
            _al1 = new NNValue(rnd.NextDouble() - 0.5);
            _al2 = new NNValue(rnd.NextDouble() - 0.5);
            _b1 = new NNValue(rnd.NextDouble() - 0.5);
            _b2 = new NNValue(rnd.NextDouble() - 0.5);
            _g1 = new NNValue(rnd.NextDouble() - 0.5);
            _g2 = new NNValue(rnd.NextDouble() - 0.5);
        }
        #endregion
    }
}