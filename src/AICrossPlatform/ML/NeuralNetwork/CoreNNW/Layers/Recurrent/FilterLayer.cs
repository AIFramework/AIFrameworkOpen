using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Обучаемый банк фильтров
    /// </summary>
    [Serializable]
    public class FilterLayer : IActivatableLayer, ILearningLayer, IRecurrentLayer
    {
        private int _count;
        private NNValue[] _valueOutps, _values;
        private readonly int _aLen, _bLen;
        private Shape3D _inputShape;

        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape
        {
            get => _inputShape;
            set
            {
                _inputShape = value;
                OutputShape = value;
                _count = value.Volume;
                FilterCells = new FilterCell[_count];
            }
        }
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
        /// Teachable parameters
        /// </summary>
        public int TrainableParameters => (_aLen + _bLen + 1) * _count;
        /// <summary>
        /// Фильтры
        /// </summary>
        public FilterCell[] FilterCells { get; set; }

        /// <summary>
        /// Обучаемый банк фильтров
        /// </summary>
        /// <param name="activation">Активационная функция</param>
        /// <param name="aL">Коэф. а</param>
        /// <param name="bL">Коэф. б</param>
        public FilterLayer(IActivation activation, int aL = 12, int bL = 13)
        {
            _aLen = aL;
            _bLen = bL;
            ActivationFunction = activation;
        }
        /// <summary>
        /// Обучаемый банк фильтров
        /// </summary>
        /// <param name="aL">Коэф. а</param>
        /// <param name="bL">Коэф. б</param>
        public FilterLayer(int aL = 12, int bL = 13)
        {
            _aLen = aL;
            _bLen = bL;
            ActivationFunction = new LinearUnit();
        }
        /// <summary>
        /// Обучаемый банк фильтров
        /// </summary>
        /// <param name="countF"></param>
        /// <param name="aL"></param>
        /// <param name="bL"></param>
        /// <param name="random"></param>
        public FilterLayer(int countF, int aL, int bL, Random random)
        {
            _aLen = aL;
            _bLen = bL;
            InputShape = new Shape3D(countF);
            InitWeights(random);
            ResetState();
        }

        /// <summary>
        /// Прохождение банка
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            _values = g.DeConcatinateOne(input);
            _valueOutps = new NNValue[_count];

            for (int i = 0; i < input.Shape.Count; i++)
            {
                _valueOutps[i] = FilterCells[i].Forward(_values[i], g);
            }

            return g.Activate(ActivationFunction, g.ConcatinateVectors(_valueOutps));
        }
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
            for (int i = 0; i < _count; i++)
            {
                FilterCells[i] = new FilterCell(_aLen, _bLen, random);
            }
        }
        /// <summary>
        /// Получение обучаемых параметров
        /// </summary>
        /// <returns></returns>
        public List<NNValue> GetParameters()
        {
            List<NNValue> param = new List<NNValue>();

            for (int i = 0; i < _count; i++)
            {
                param.AddRange(FilterCells[i].GetParameters());
            }

            return param;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            for (int i = 0; i < _count; i++)
            {
                FilterCells[i].ResetState();
            }
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
        /// Только для использования
        /// </summary>
        public void OnlyUse()
        {
            for (int i = 0; i < FilterCells.Length; i++)
            {
                FilterCells[i].OnlyUse();
            }

            for (int i = 0; i < _values.Length; i++)
            {
                _values[i].OnlyUse();
            }

            for (int i = 0; i < _valueOutps.Length; i++)
            {
                _valueOutps[i].OnlyUse();
            }
        }
    }
}