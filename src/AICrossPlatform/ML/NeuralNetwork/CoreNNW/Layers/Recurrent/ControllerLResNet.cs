﻿using AI.DataStructs.Shapes;
using AI.ML.DataEncoding.PositionalEncoding;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Облегченный контроллер
    /// </summary>
    [Serializable]
    public class ControllerLResNet : ILearningLayer, IRecurrentLayer
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
        public int TrainableParameters => GetParameters().Count;

        /// <summary>
        /// Число признаков для кодирования кода позиции
        /// </summary>
        public int FeatureCodeLen
        {
            get { return _featureCodeLen; }
        }

        /// <summary>
        /// Метод кодирования позиции
        /// </summary>
        public IPositionEncoding PositionEncoder
        {
            get { return _pEnc; }
            set
            {
                _pEnc = value;
                GenCodePositionMatrix();
            }

        }

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

        private readonly IActivation forgetGateActivation = new SigmoidUnit();
        private readonly IActivation outputGateActivation = new SigmoidUnit();
        private readonly IActivation inpGateActivation = new SigmoidUnit();
        private readonly IActivation cellInputActivation = new TanhUnit();
        private readonly IActivation cellOutputActivation = new TanhUnit();

        // Число признаков для кодирования кода позиции
        private IPositionEncoding _pEnc = new MultiscaleEncoder(128);
        // Метод кодирования позиции
        private readonly int _featureCodeLen = 16;
        // Выделение признаков позиции
        private NNValue _poseFeatures;
        // Номер входа
        private int _position = 0;
        #endregion

        /// <summary>
        /// Облегченный контроллер
        /// </summary>
        /// <param name="inputDimension">Размерность входа</param>
        /// <param name="initParamsStdDev">Среднеквадратичное отклонение</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        /// <param name="posSize">Длинна вектора, для кодирования позиции</param>
        public ControllerLResNet(int inputDimension, double initParamsStdDev, Random rnd, int posSize = 16)
        {
            _featureCodeLen = posSize;
            InputShape = new Shape3D(inputDimension);
            OutputShape = InputShape;
            Init(InputShape, initParamsStdDev, rnd);
            ResetState();
        }
        /// <summary>
        /// Облегченный контроллер
        /// </summary>
        /// <param name="inputShape">Размерность входа</param>
        /// <param name="initParamsStdDev">Среднеквадратичное отклонение</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        /// <param name="posSize">Длинна вектора, для кодирования позиции</param>
        public ControllerLResNet(Shape3D inputShape, double initParamsStdDev, Random rnd, int posSize = 16)
        {
            _featureCodeLen = posSize;
            Init(inputShape, initParamsStdDev, rnd);
            ResetState(); // Запуск НС
        }

        /// <summary>
        /// Облегченный контроллер
        /// </summary>
        /// <param name="posSize">Длинна вектора, для кодирования позиции</param>
        public ControllerLResNet(int posSize = 16)
        {
            _featureCodeLen = posSize;
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue positionCode = new NNValue(_pEnc.GetCode(_position++)); // Позиционный код
            NNValue positionFeatures = g.MulMV(_poseFeatures, positionCode);


            NNValue conc1 = g.ConcatinateVectors(input, positionFeatures);
            NNValue conc = g.ConcatinateVectors(conc1, _hiddenContext);
            NNValue commandLine = g.MulMV(commandGet, conc);


            //Вентиль забывания
            NNValue fSum = g.AdamarMul(forgetG, commandLine);
            NNValue forgetGate = g.Activate(forgetGateActivation, g.Add(fSum, forgetBias));

            //Вентиль входа
            NNValue iSum = g.AdamarMul(forgetG, commandLine);
            NNValue i = g.Activate(cellInputActivation, g.Add(iSum, inputBias));

            //Вентиль выхода
            NNValue oSum = g.AdamarMul(outpGate, commandLine);
            NNValue outputGate = g.Activate(outputGateActivation, g.Add(oSum, outputBias));

            //Операция записи в ячейки
            NNValue cSum = g.MulMV(writeG, conc);
            NNValue cellInput = g.Activate(cellInputActivation, g.Add(cSum, cellWriteBias));

            // Вычисляем активацию новой ячейки
            NNValue retainCell = g.AdamarMul(forgetGate, _cellContext);
            NNValue writeCell = g.AdamarMul(i, cellInput);
            NNValue cellAct = g.Add(retainCell, writeCell);

            //compute hidden state as gated, saturated cell activations
            NNValue output = g.Add(
                g.AdamarMul(outputGate,
                g.Activate(cellOutputActivation, cellAct)), input);

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
            _position = 0;
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
                forgetG,
                outpGate,
                writeG,
                forgetBias,
                outputBias,
                cellWriteBias,
                commandGet,
                inputBias,
                inpG,
                _poseFeatures
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
            double std = 1.0 / InputShape.Count;
            Init(inpShape, std, random);
        }
        private void Init(Shape3D inputShape, double initParamsStdDev, Random rnd)
        {
            int outputDimension = inputShape!.Volume;
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            int inputDimension = inputShape.Height;
            int con = inputDimension + outputDimension + _featureCodeLen;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            commandGet = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);
            outpGate = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            forgetG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            inpG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            writeG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);

            forgetBias = new NNValue(outputDimension);
            outputBias = new NNValue(outputDimension);
            cellWriteBias = new NNValue(outputDimension);
            inputBias = new NNValue(outputDimension);
            GenCodePositionMatrix();
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
            return string.Format("LstmLayerL1     \t|Вход: {0} |Выходы: {1} |Нелинейная функция активации: {3} |Число обучаемых параметров: {2}", InputShape, OutputShape, TrainableParameters, "Сигмоида/тангенс");
        }

        /// <summary>
        /// Генерация весовых коэффициентов
        /// </summary>
        /// <param name="random">ГПСЧ</param>
        public void InitWeights(Random random)
        {
            double std = 1.0 / InputShape.Volume;
            Init(InputShape, std, random);
        }


        // Создание матрицы кодирования позиции
        private void GenCodePositionMatrix()
        {
            Random random = new Random(1);
            double std = 1.0 / Math.Sqrt(_featureCodeLen * _pEnc.Dim);
            _poseFeatures = NNValue.Random(_featureCodeLen, _pEnc.Dim, std, random);
        }
    }
}

