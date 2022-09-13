using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Long short-term memory(LSTM) layer (inpGate = 1.0 - forgetGate)
    /// </summary>
    [Serializable]
    public class LSTMLayerL1 : ILearningLayer, IRecurrentLayer
    {
        #region Поля
        private static readonly SigmoidUnit s_sigmoidActivation = new SigmoidUnit();
        private static readonly TanhUnit s_tanhActivation = new TanhUnit();

        private NNValue _outpGate;
        private NNValue _forgetG;
        private NNValue _writeG;

        private NNValue _forgetBias;
        private NNValue _outputBias;
        private NNValue _cellWriteBias;

        private NNValue _hiddenContext;
        private NNValue _cellContext;
        #endregion

        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
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
        public int TrainableParameters => 3 * OutputShape.Height * (InputShape.Height + OutputShape.Height);

        /// <summary>
        /// Long short-term memory(LSTM) layer (inpGate = 1.0 - forgetGate)
        /// </summary>
        /// <param name="inputDimension">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public LSTMLayerL1(int inputDimension, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            Init(outputDimension, initParamsStdDev, rnd);
            ResetState();
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer (inpGate = 1.0 - forgetGate)
        /// </summary>
        /// <param name="inputShape">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public LSTMLayerL1(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = inputShape;
            Init(outputDimension, initParamsStdDev, rnd);
            ResetState(); // Запуск НС
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer
        /// </summary>
        /// <param name="outputDimension">Output dimension</param>
        public LSTMLayerL1(int outputDimension)
        {
            OutputShape = new Shape3D(outputDimension);
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue conc = g.ConcatinateVectors(input, _hiddenContext);

            //forget gate
            NNValue fSum = g.MulMV(_forgetG, conc);
            NNValue forgetGate = g.Activate(s_sigmoidActivation, g.Add(fSum, _forgetBias));

            //input gate
            NNValue i = g.OneMinus(forgetGate);

            //output gate
            NNValue oSum = g.MulMV(_outpGate, conc);
            NNValue outputGate = g.Activate(s_sigmoidActivation, g.Add(oSum, _outputBias));

            //write operation on cells
            NNValue cSum = g.MulMV(_writeG, conc);
            NNValue cellInput = g.Activate(s_tanhActivation, g.Add(cSum, _cellWriteBias));

            //compute new cell activation
            NNValue retainCell = g.AdamarMul(forgetGate, _cellContext);
            NNValue writeCell = g.AdamarMul(i, cellInput);
            NNValue cellAct = g.Add(retainCell, writeCell);

            //compute hidden state as gated, saturated cell activations
            NNValue output = g.AdamarMul(outputGate, g.Activate(s_tanhActivation, cellAct));

            //rollover activations for next iteration
            _hiddenContext = output;
            _cellContext = cellAct;

            return output;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            _hiddenContext = new NNValue(OutputShape.Height);
            _cellContext = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                _forgetG,
                _outpGate,
                _writeG,
                _forgetBias,
                _outputBias,
                _cellWriteBias
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
            double std = 1.0 / Math.Sqrt(OutputShape.Volume * InputShape.Volume);
            Init(OutputShape.Height, std, random);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "sigm/tanh", TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            _outpGate.OnlyUse();
            _forgetG.OnlyUse();
            _writeG.OnlyUse();

            _forgetBias.OnlyUse();
            _outputBias.OnlyUse();
            _cellWriteBias.OnlyUse();
        }

        #region Приватные методы
        private void Init(int outputDimension, double initParamsStdDev, Random rnd)
        {
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            int inputDimension = InputShape.Height;
            int con = inputDimension + outputDimension;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            _outpGate = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);
            _forgetG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);
            _writeG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);

            _forgetBias = new NNValue(outputDimension);
            _outputBias = new NNValue(outputDimension);
            _cellWriteBias = new NNValue(outputDimension);

            ResetState();
        }
        #endregion
    }
}