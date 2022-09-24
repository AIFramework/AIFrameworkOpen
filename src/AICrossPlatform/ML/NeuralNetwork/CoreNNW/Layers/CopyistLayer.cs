using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой копирования
    /// </summary>
    [Serializable]
    public class CopyistLayer : IActivatableLayer
    {
        private readonly int _count;
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
                OutputShape = new Shape3D(value.Volume * _count);
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
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => 0;

        /// <summary>
        /// Слой копирования
        /// </summary>
        /// <param name="count">Число копий</param>
        /// <param name="act">Non-linear function</param>
        public CopyistLayer(int count, IActivation act = null)
        {
            _count = count;
            ActivationFunction = act ?? new LinearUnit();
        }
        /// <summary>
        /// Слой копирования
        /// </summary>
        /// <param name="inpShape">Размерность входа</param>
        /// <param name="count">Число копмрований</param>
        public CopyistLayer(Shape3D inpShape, int count)
        {
            _count = count;
            InputShape = inpShape;
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.Activate(ActivationFunction, g.Copyist(input, _count));
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