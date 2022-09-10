﻿using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой для создания и обучения векторов встранивания
    /// </summary>
    [Serializable]
    public class EmbedingLayer : ILearningLayer, IRecurrentLayer,IActivatableLayer
    {
        private NNValue[] _vectors; // Вектора встраивания (эмбединги)
        private readonly HashSet<int> _usedKeys = new HashSet<int>();
        private readonly int _countV;
        [NonSerialized]
        private readonly bool _isPreTrained;

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
        public int TrainableParameters => _vectors.Length * _vectors[0].Shape.Volume;
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt => 0;

        /// <summary>
        /// Активационная ф-я
        /// </summary>
        public IActivation ActivationFunction { get; set; } = new LinearUnit();
        
        /// <summary>
        /// Layer for training and creating embedding vectors
        /// </summary>
        public EmbedingLayer(int countVectors, int dim)
        {
            _countV = countVectors;
            OutputShape = new Shape3D(dim);
            _isPreTrained = false;
        }
        /// <summary>
        /// Layer for training and creating embedding vectors (initialization with pre-trained vectors)
        /// </summary>
        public EmbedingLayer(Vector[] vectors)
        {
            if (vectors == null)
            {
                throw new ArgumentException("vectors is null");
            }

            if (vectors.Length == 0)
            {
                throw new ArgumentException("vectors is empty");
            }

            if (vectors[0] == null)
            {
                throw new ArgumentException("vectors[0] is null");
            }

            _countV = vectors.Length;
            OutputShape = new Shape3D(vectors[0].Count);
            _vectors = new NNValue[vectors.Length];

            for (int i = 0; i < _countV; i++)
            {
                _vectors[i] = new NNValue(vectors[i]);
            }

            _isPreTrained = true;
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            int index = (int)input[0];

            if (_vectors == null)
            {
                throw new Exception("Vectors are not initialized");
            }

            if (index > _vectors.Length - 1)
            {
                throw new Exception("Index is greater than the maximum index in the vector array");
            }

            if (!_usedKeys.Contains(index) && g.IsBackward)
            {
                _usedKeys.Add(index);
            }

            return g.Activate(ActivationFunction, _vectors[index]);
        }
        /// <summary>
        /// Initialize layer weights
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
            if (!_isPreTrained)
            {
                Init(OutputShape.Height, random);
            }
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> data = new List<NNValue>();
            foreach (int item in _usedKeys)
            {
                data.Add(_vectors[item]);
            }
            return data;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            _usedKeys.Clear();
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            for (int i = 0; i < _vectors.Length; i++)
            {
                _vectors[i].OnlyUse();
            }
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "sigm/tanh", TrainableParameters);
        }

        #region Приватные методы
        private void Init(int dim, Random random)
        {
            _vectors = new NNValue[_countV];


            for (int i = 0; i < _countV; i++)
            {
                _vectors[i] = NNValue.Random(dim, 1, 1.0 / dim, random);
            }

        }
        #endregion
    }
}