using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Линейный слой без смещения
    /// </summary>
    [Serializable]
    public class LinearLayer : ILearningLayer
    {
        private NNValue _w;

        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => _w.Shape.Count;

        /// <summary>
        /// Линейный слой без смещения
        /// </summary>
        public LinearLayer(int inputDimension, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);
            _w = NNValue.Random(outputDimension, inputDimension, initParamsStdDev, rnd);
        }
        /// <summary>
        /// Линейный слой без смещения
        /// </summary>
        public LinearLayer(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = inputShape;
            OutputShape = new Shape3D(outputDimension);
            _w = NNValue.Random(outputDimension, inputShape.Height, initParamsStdDev, rnd);
        }
        /// <summary>
        /// Линейный слой без смещения
        /// </summary>
        public LinearLayer(int outputDimension)
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
            NNValue returnObj = g.Mul(_w, input);
            return returnObj;
        }
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue> { _w };
        }
        /// <summary>
        /// Генерация весовых коэффициентов
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public void InitWeights(Random random)
        {
            Init(OutputShape.Height, 1.0 / Math.Sqrt(OutputShape.Volume), random);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            _w.OnlyUse();
        }

        #region Приватные методы
        private void Init(int outputDimension, double initParamsStdDev, Random rng)
        {
            OutputShape = new Shape3D(outputDimension);
            _w = NNValue.Random(outputDimension, InputShape.Height, initParamsStdDev, rng);
        }
        #endregion
    }
}