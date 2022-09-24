using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Полносвязный слой
    /// </summary>
    [Serializable]
    public class FeedForwardLayer : IActivatableLayer, ILearningLayer
    {
        /// <summary>
        /// Матрица весов
        /// </summary>
        public NNValue W { get; set; }
        /// <summary>
        /// Вектор смещения гиперплоскости (поляризация нейронов)
        /// </summary>
        public NNValue Bias { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; }
        /// <summary>
        /// Добавление к знаменателю
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Количество параметров обучения
        /// </summary>
        public int TrainableParameters => W.Shape.Count + Bias.Shape.Height;
        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }

        /// <summary>
        /// Полносвязный слой
        /// </summary>
        /// <param name="inputDimension">Размерность входа</param>
        /// <param name="outputDimension">Размерность выхода</param>
        /// <param name="f">Активационная функция</param>
        /// <param name="rnd">Генератор псевдослучайных чисел</param>
        public FeedForwardLayer(int inputDimension, int outputDimension, IActivation f, Random rnd)
        {
            double initParamsStdDev = 1.0 / Math.Sqrt(outputDimension);
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            W = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
            Bias = new NNValue(outputDimension);
            ActivationFunction = f;
        }
        /// <summary>
        /// Полносвязный слой
        /// </summary>
        /// <param name="inputShape">Размерность входа</param>
        /// <param name="outputDimension">Размерность выхода</param>
        /// <param name="f">Активационная функция</param>
        /// <param name="rnd">Генератор псевдослучайных чисел</param>
        public FeedForwardLayer(Shape3D inputShape, int outputDimension, IActivation f, Random rnd)
        {
            double initParamsStdDev = 1.0 / Math.Sqrt(outputDimension);
            InputShape = inputShape;
            OutputShape = new Shape3D(outputDimension);
            W = NNValue.Random(OutputShape.Height, InputShape.Height, initParamsStdDev, rnd);
            Bias = new NNValue(outputDimension);
            ActivationFunction = f;
        }
        /// <summary>
        /// Полносвязный слой
        /// </summary>
        /// <param name="outputDimension">Размерность выхода</param>
        /// <param name="f">Активационная функция</param>
        public FeedForwardLayer(int outputDimension, IActivation f = null)
        {
            OutputShape = new Shape3D(outputDimension);

            ActivationFunction = f ?? new LinearUnit();
        }

        /// <summary>
        /// Прямой проход слоя
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автодифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue sum = g.Add(g.Mul(W, input), Bias);
            NNValue returnObj = g.Activate(ActivationFunction, sum);
            return returnObj;
        }
        /// <summary>
        /// Получение обучаемых параметров
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue> { W, Bias };
        }
        /// <summary>
        /// Генерация новых коэффициентов
        /// </summary>
        /// <param name="random">Генератор псевдослучайных чисел</param>
        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Height);
            W = NNValue.Random(OutputShape.Height, InputShape.Height, std, random);
            Bias = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            W.OnlyUse();
            Bias.OnlyUse();
        }
    }
}