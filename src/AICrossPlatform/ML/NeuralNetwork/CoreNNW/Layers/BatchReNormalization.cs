using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    ///  BatchReNormalization (Нормализация входов при онлайн-обучении)
    /// </summary>
    [Serializable]
    public class BatchReNormalization : ILearningLayer
    {
        private NNValue _beta, _gamma;
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
                _beta = new NNValue(value);
                _gamma = NNValue.Ones(value);
            }
        }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => _beta == null ? 0 : 2 * _beta.Shape.Count;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt => 0;

        /// <summary>
        ///  BatchReNormalization (Нормализация входов при онлайн-обучении)
        /// </summary>
        public BatchReNormalization()
        {
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.Add(g.AdamarMul(_gamma, input), _beta);
        }
        /// <summary>
        /// Генерация весовых коэффициентов
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public void InitWeights(Random random)
        {
        }
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        public List<NNValue> GetParameters()
        {
            return new List<NNValue>()
            {
                _beta, _gamma
            };
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription("BatchReNorm", InputShape, OutputShape, "None", TrainableParameters);
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            _beta.OnlyUse();
            _gamma.OnlyUse();
        }
    }
}