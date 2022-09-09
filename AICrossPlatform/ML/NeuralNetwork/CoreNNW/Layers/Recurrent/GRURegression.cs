using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой GRU
    /// </summary>
    [Serializable]
    public class GRURegression : ILearningLayer, IRecurrentLayer
    {
        private static readonly SigmoidUnit s_sigmoidActivation = new SigmoidUnit();
        private static readonly TanhUnit s_tanhActivation = new TanhUnit();

        private NNValue _direct;
        private NNValue _directBias;
        private NNValue _hmix;
        private NNValue _hHmix;
        private NNValue _bmix;
        private NNValue _hnew;
        private NNValue _hHnew;
        private NNValue _bnew;
        private NNValue _hreset;
        private NNValue _hHreset;
        private NNValue _breset;
        private NNValue _context;

        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Обучаемые параметры
        /// </summary>
        public int TrainableParameters => 4 * OutputShape.Height * (InputShape.Height + OutputShape.Height);

        /// <summary>
        /// Слой GRU
        /// </summary>
        /// <param name="inputDimension">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public GRURegression(int inputDimension, int outputDimension, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            Init(outputDimension, rnd);
        }
        /// <summary>
        /// Слой GRU
        /// </summary>
        /// <param name="inputShape">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public GRURegression(Shape3D inputShape, int outputDimension, Random rnd)
        {
            InputShape = inputShape;
            Init(outputDimension, rnd);
        }
        /// <summary>
        /// Слой GRU
        /// </summary>
        /// <param name="outputDimension">Output dimension</param>
        public GRURegression(int outputDimension)
        {
            OutputShape = new Shape3D(outputDimension);
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue outputGru = g.GRULayer(input,
                 _hmix, _hHmix, _bmix, _hnew, _hHnew,
                 _bnew, _hreset, _hHreset, _breset
                 , _context, s_sigmoidActivation, s_sigmoidActivation, s_tanhActivation);

            _context = outputGru;

            NNValue outputDense = g.FeedforwardLinLayer(input, _direct, _directBias);
            NNValue output = g.AddN(outputGru, outputDense);

            return output;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            _context = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                _hmix,
                _hHmix,
                _bmix,
                _hnew,
                _hHnew,
                _bnew,
                _hreset,
                _hHreset,
                _breset,
                _direct,
                _directBias
            };
            return result;
        }
        /// <summary>
        /// Generating weight coefficients of a neural network layer
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public void InitWeights(Random random)
        {
            Init(OutputShape.Height, random);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "sigm/tanh", TrainableParameters);
        }

        public void OnlyUse()
        {
            _hmix.OnlyUse();
            _hHmix.OnlyUse();
            _bmix.OnlyUse();
            _hnew.OnlyUse();
            _hHnew.OnlyUse();
            _bnew.OnlyUse();
            _hreset.OnlyUse();
            _hHreset.OnlyUse();
            _breset.OnlyUse();
            _direct.OnlyUse();
            _directBias.OnlyUse();
        }

        #region Приватные методы
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Init(int outputDimension, Random rnd)
        {
            double initParamsStdDev = 1.0 / Math.Sqrt(outputDimension);
            int inputDimension = InputShape.Height;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            _direct = NNValue.Uniform(1, inputDimension, 1.0 / inputDimension); // Dense layer
            _directBias = new NNValue(1); // Dense layer bias
            _hmix = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
            _hHmix = NNValue.Random(outputDimension, outputDimension, initParamsStdDev, rnd);
            _bmix = new NNValue(outputDimension);
            _hnew = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
            _hHnew = NNValue.Random(outputDimension, outputDimension, initParamsStdDev, rnd);
            _bnew = new NNValue(outputDimension);
            _hreset = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
            _hHreset = NNValue.Random(outputDimension, outputDimension, initParamsStdDev, rnd);
            _breset = new NNValue(outputDimension);
        }
        #endregion
    }
}