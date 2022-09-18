using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Long short-term memory(LSTM) layer peephole connection
    /// </summary>
    [Serializable]
    public class LSTMLayerPeepholeConnection : ILearningLayer, IRecurrentLayer
    {
        #region Поля
        private static readonly SigmoidUnit s_sigmoidActivation = new SigmoidUnit();
        private static readonly TanhUnit s_tanhActivation = new TanhUnit();

        private NNValue inpGate;
        private NNValue outpGate;
        private NNValue forgetG;
        private NNValue writeG;

        private NNValue _forgetBias;
        private NNValue _outputBias;
        private NNValue _inputBias;
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
        public int TrainableParameters => CountParams();

        /// <summary>
        /// Long short-term memory(LSTM) layer peephole connection
        /// </summary>
        /// <param name="inputDimension">Размерность входа</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public LSTMLayerPeepholeConnection(int inputDimension, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            Init(outputDimension, initParamsStdDev, rnd);
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer peephole connection
        /// </summary>
        /// <param name="inputShape">Размерность входа</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public LSTMLayerPeepholeConnection(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = inputShape;
            Init(outputDimension, initParamsStdDev, rnd);
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer peephole connection
        /// </summary>
        /// <param name="outputDimension">Output dimension</param>
        public LSTMLayerPeepholeConnection(int outputDimension)
        {
            OutputShape = new Shape3D(outputDimension);
            ResetState();
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue conc = g.ConcatinateVectors(input, _hiddenContext);
            NNValue peephole = g.ConcatinateVectors(new NNValue[] { input, _cellContext, _hiddenContext });

            //input gate
            NNValue iSum = g.MulMV(inpGate, peephole);
            NNValue i = g.Activate(s_sigmoidActivation, g.Add(iSum, _inputBias));

            //forget gate
            NNValue fSum = g.MulMV(forgetG, peephole);
            NNValue forgetGate = g.Activate(s_sigmoidActivation, g.Add(fSum, _forgetBias));

            //output gate
            NNValue oSum = g.MulMV(outpGate, peephole);
            NNValue outputGate = g.Activate(s_sigmoidActivation, g.Add(oSum, _outputBias));

            //write operation on cells
            NNValue cSum = g.MulMV(writeG, conc);
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
        /// Сброс состояния нейронной сети
        /// </summary>
        public void ResetState()
        {
            _hiddenContext = new NNValue(OutputShape.Height);
            _cellContext = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                inpGate,
                forgetG,
                _inputBias,
                outpGate,
                writeG,
                _forgetBias,
                _outputBias,
                _cellWriteBias
            };
            return result;
        }
        /// <summary>
        /// Генерация случ. весов для сети
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
            return LayerHelper.GetLayerDescription("LSTMLayerPC", InputShape, OutputShape, "sigm/tanh", TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            outpGate.OnlyUse();
            inpGate.OnlyUse();
            forgetG.OnlyUse();
            writeG.OnlyUse();

            _inputBias.OnlyUse();
            _forgetBias.OnlyUse();
            _outputBias.OnlyUse();
            _cellWriteBias.OnlyUse();
        }

        #region Приватные методы
        private void Init(int outputDimension, double initParamsStdDev, Random rnd)
        {
            ResetState();
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            int inputDimension = InputShape.Height;
            int con = inputDimension + outputDimension;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            outpGate = NNValue.Random(outputDimension, con + outputDimension, initParamsStdDev, rnd);
            inpGate = NNValue.Random(outputDimension, con + outputDimension, initParamsStdDev, rnd);
            forgetG = NNValue.Random(outputDimension, con + outputDimension, initParamsStdDev, rnd);
            writeG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);

            _inputBias = new NNValue(outputDimension);
            _forgetBias = new NNValue(outputDimension);
            _outputBias = new NNValue(outputDimension);
            _cellWriteBias = new NNValue(outputDimension);
        }

        private int CountParams()
        {
            int res = 0;
            List<NNValue> p = GetParameters();

            foreach (NNValue item in p)
            {
                res += item.Shape.Count;
            }

            return res;
        }
        #endregion
    }
}