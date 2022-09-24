using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Нейросетевой контроллер
    /// </summary>
    [Serializable]
    public class NeuralСontroller : ILearningLayer, IRecurrentLayer
    {
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation function { get; set; }
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
        public int TrainableParameters => 3;

        #region поля
        private NNValue outpGate;
        private NNValue forgetG;
        private NNValue writeG;
        private NNValue inpG;
        private NNValue commandGet;

        private NNValue forgetBias;
        private NNValue outputBias;
        private NNValue cellWriteBias;
        private NNValue inputBias;

        private NNValue _hiddenContext;
        private NNValue _cellContext;
        private NNValue commandLineCont;

        private readonly IActivation forgetGateActivation = new SigmoidUnit();
        private readonly IActivation outputGateActivation = new SigmoidUnit();
        private readonly IActivation commandActivation = new TanhUnit();
        private readonly IActivation cellInputActivation = new TanhUnit();
        private readonly IActivation cellOutputActivation = new TanhUnit();
        #endregion

        /// <summary>
        /// Нейросетевой контроллер
        /// </summary>
        /// <param name="inputDimension">Размерность входа</param>
        /// <param name="outputDimension">Размерность выхода</param>
        /// <param name="initParamsStdDev">Среднеквадратичное отклонение</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public NeuralСontroller(int inputDimension, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            Init(InputShape, outputDimension, initParamsStdDev, rnd);
            ResetState();
        }
        /// <summary>
        /// Neural network microcontroller
        /// </summary>
        /// <param name="inputShape">Размерность входа</param>
        /// <param name="outputDimension">Размерность выхода</param>
        /// <param name="initParamsStdDev">Среднеквадратичное отклонение</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public NeuralСontroller(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            Init(inputShape, outputDimension, initParamsStdDev, rnd);
            ResetState(); // Запуск НС
        }
        /// <summary>
        /// Neural network microcontroller
        /// </summary>
        /// <param name="outputDimension">Размерность выхода</param>
        public NeuralСontroller(int outputDimension)
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
            NNValue comIn = g.ConcatinateVectors(conc, commandLineCont);
            NNValue commandLine = g.MulMV(commandGet, comIn);


            //forget gate
            NNValue fSum = g.AdamarMul(forgetG, commandLine);
            NNValue forgetGate = g.Activate(forgetGateActivation, g.Add(fSum, forgetBias));

            //input gate
            NNValue iSum = g.AdamarMul(inpG, commandLine);
            NNValue i = g.Activate(cellInputActivation, g.Add(iSum, inputBias));

            //output gate
            NNValue oSum = g.AdamarMul(outpGate, commandLine);
            NNValue outputGate = g.Activate(outputGateActivation, g.Add(oSum, outputBias));

            //write operation on cells
            NNValue cSum = g.MulMV(writeG, conc);
            NNValue cellInput = g.Activate(cellInputActivation, g.Add(cSum, cellWriteBias));

            //compute new cell activation
            NNValue retainCell = g.AdamarMul(forgetGate, _cellContext);
            NNValue writeCell = g.AdamarMul(i, cellInput);
            NNValue cellAct = g.Add(retainCell, writeCell);

            //compute hidden state as gated, saturated cell activations
            NNValue output = g.AdamarMul(outputGate, g.Activate(cellOutputActivation, cellAct));

            //rollover activations for next iteration
            _hiddenContext = output;
            _cellContext = cellAct;
            commandLineCont = g.Activate(commandActivation, commandLine);

            return output;
        }
        /// <summary>
        /// Сброс состояния нейронной сети
        /// </summary>
        public void ResetState()
        {
            _hiddenContext = new NNValue(OutputShape.Height);
            _cellContext = new NNValue(OutputShape.Height);
            commandLineCont = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                forgetG,
                outpGate,
                writeG,
                forgetBias,
                outputBias,
                cellWriteBias,
                commandGet,
                inputBias,
                inpG
            };
            return result;
        }
        /// <summary>
        /// Генерация случ. весов для сети
        /// </summary>
        /// <param name="inpShape"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public void Generate(Shape3D inpShape, Random random)
        {
            InputShape = inpShape;
            double std = 1.0 / Math.Sqrt(OutputShape.Count * InputShape.Count);
            Init(inpShape, OutputShape.Height, std, random);
        }
        private void Init(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            int inputDimension = inputShape.Height;
            int con = inputDimension + outputDimension;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            commandGet = NNValue.Random(outputDimension, con + outputDimension, initParamsStdDev, rnd);
            outpGate = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            forgetG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            inpG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            writeG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);

            forgetBias = new NNValue(outputDimension);
            outputBias = new NNValue(outputDimension);
            cellWriteBias = new NNValue(outputDimension);
            inputBias = new NNValue(outputDimension);

            ResetState();
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            outpGate.OnlyUse();
            forgetG.OnlyUse();
            writeG.OnlyUse();
            commandGet.OnlyUse();
            inpG.OnlyUse();

            forgetBias.OnlyUse();
            outputBias.OnlyUse();
            cellWriteBias.OnlyUse();
            inputBias.OnlyUse();
        }

        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("LstmLayerL1     \t|inp: {0} |outp: {1} |Non lin. activate: {3} |TrainParams: {2}", InputShape, OutputShape, TrainableParameters, "sigm/tanh");
        }

        /// <summary>
        /// Инициализация весовых коэффициентов случайными числами
        /// </summary>
        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Volume * InputShape.Volume);
            Init(InputShape, OutputShape.Height, std, random);
        }
    }
}
