using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.ConvDeconv
{
    /// <summary>
    /// Одномерный сверточный слой
    /// </summary>
    [Serializable]
    public class Conv1D : IActivatableLayer, ILearningLayer
    {
        private readonly int _filters, _core;
        private readonly ConvolutionalLayer _convolution;

        /// <summary>
        /// Сохраняется ли размер ввода
        /// </summary>
        public bool IsSame { get => _convolution.IsSame; set => _convolution.IsSame = value; }
        /// <summary>
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape { get => _convolution.InputShape; set => _convolution.InputShape = value; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape => _convolution.OutputShape;
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => _convolution.TrainableParameters;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt => _convolution.AddDenInSqrt;
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get => _convolution.ActivationFunction; set => _convolution.ActivationFunction = value; }

        /// <summary>
        /// Одномерный сверточный слой
        /// </summary>
        public Conv1D(Shape3D inputShape, int core, int filters, Random rnd, IActivation activation = null)
        {
            _filters = filters;
            _core = core;
            _convolution = new ConvolutionalLayer(inputShape: inputShape, new FilterStruct() { FilterCount = filters, FilterH = core, FilterW = 1 }, activation, rnd);
            ActivationFunction = activation ?? new LinearUnit();
        }
        /// <summary>
        /// Одномерный сверточный слой
        /// </summary>
        public Conv1D(int core, int filters, IActivation activation = null)
        {
            _filters = filters;
            _core = core;
            _convolution = new ConvolutionalLayer(activation, filters, core, 1);
            ActivationFunction = activation ?? new LinearUnit();
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return _convolution.Forward(input, g);
        }
        /// <summary>
        /// Инициализация слоя
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
            FilterStruct fs = new FilterStruct() { FilterCount = _filters, FilterH = _core, FilterW = 1 };
            _convolution.Init(fs, ActivationFunction, random);
        }
        /// <summary>
        /// Обучаемые параметры
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return _convolution.GetParameters();
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
            _convolution.OnlyUse();
        }
    }
}