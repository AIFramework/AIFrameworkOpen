using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой который преобразует тензор в вектор
    /// </summary>
    [Serializable]
    public class Flatten : ILayer
    {
        private readonly float _gain = 1.0f;
        private Shape3D _inputShape;

        /// <summary>
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape
        {
            get => _inputShape;
            set
            {
                _inputShape = value;
                OutputShape = new Shape3D(_inputShape.Volume);
            }
        }

        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => 0;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        /// <summary>
        /// Слой который преобразует тензор в вектор
        /// </summary>
        /// <param name="inputShape"> Размерность и форма входного тензора </param>
        public Flatten(Shape3D inputShape)
        {
            InputShape = inputShape;
        }
        /// <summary>
        /// Слой который преобразует тензор в вектор
        /// </summary>
        /// <param name="inputShape"> Размерность и форма входного тензора </param>
        /// <param name="gain">Усиление градиента</param>
        public Flatten(Shape3D inputShape, float gain)
        {
            InputShape = inputShape;
            _gain = gain;
        }
        /// <summary>
        /// Слой который преобразует тензор в вектор
        /// </summary>
        public Flatten() { }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            Shape3D shape = new Shape3D(input.Shape.Count);
            return g.ReShape(input, shape, _gain);

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