using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Нелинейный активационный слой
    /// </summary>
    [Serializable]
    public class ActivationLayer : IActivatableLayer
    {
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
            }
        }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число обучаемых параметров (0)
        /// </summary>
        public int TrainableParameters { get; private set; } = 0;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов (0)
        /// </summary>
        public double AddDenInSqrt { get; private set; } = 0;
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        /// <summary>
        /// Слой активации
        /// </summary>
        /// <param name="shapeInp">Размерность входа</param>
        /// <param name="activation">Активационная функция</param>
        public ActivationLayer(Shape3D shapeInp, IActivation activation)
        {
            ActivationFunction = activation;
            InputShape = shapeInp;
        }
        /// <summary>
        /// Слой активации
        /// </summary>
        /// <param name="activation">Активационная функция</param>
        public ActivationLayer(IActivation activation)
        {
            ActivationFunction = activation;
        }

        /// <summary>
        /// Прямой проход слоя
        /// </summary>
        /// <param name="input">Входной тензор</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        /// <returns></returns>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.Activate(ActivationFunction, input);
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
        }
    }
}