using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Одномерный слой подвыборки
    /// </summary>
    [Serializable]
    public class MaxPool1D : ILayer
    {
        private readonly MaxPooling _maxPool;

        /// <summary>
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape { get => _maxPool.InputShape; set => _maxPool.InputShape = value; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape => _maxPool.OutputShape;
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => _maxPool.TrainableParameters;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt => _maxPool.AddDenInSqrt;

        /// <summary>
        /// Одномерный слой подвыборки
        /// </summary>
        /// <param name="inputShape"> Размерность и форма входного тензора </param>
        /// <param name="k">Восколько раз сжать карту признаков</param>
        public MaxPool1D(Shape3D inputShape, int k = 2)
        {
            _maxPool = new MaxPooling(inputShape, k, 1);
        }
        /// <summary>
        /// Одномерный слой подвыборки
        /// </summary>
        /// <param name="k">Восколько раз сжать карту признаков</param>
        public MaxPool1D(int k = 2)
        {
            _maxPool = new MaxPooling(k, 1);
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return _maxPool.Forward(input, g);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }

        public void OnlyUse()
        {
        }
    }
}