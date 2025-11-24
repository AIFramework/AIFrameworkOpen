using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой изменения формы тензора
    /// </summary>
    [Serializable]
    public class ReShape : ILayer
    {
        private readonly float _gain = 1.0f;

        /// <summary>
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число тренировочных параметров
        /// </summary>
        public int TrainableParameters => 0;
        /// <summary>
        /// Добавление в знаменатель
        /// </summary>
        public double AddDenInSqrt { get; set; }

        /// <summary>
        /// Слой изменения формы тензора
        /// </summary>
        /// <param name="inputShape">Начальная форма</param>
        /// <param name="newShape">Новая форма</param>
        public ReShape(Shape3D inputShape, Shape3D newShape)
        {
            InputShape = inputShape;
            OutputShape = newShape;
        }
        /// <summary>
        /// Слой изменения формы тензора
        /// </summary>
        /// <param name="inputShape">Начальная форма</param>
        /// <param name="newShape">Новая форма</param>
        /// <param name="gain">Усиление градиента</param>
        public ReShape(Shape3D inputShape, Shape3D newShape, float gain = 1.0f)
        {
            InputShape = inputShape;
            OutputShape = newShape;
            _gain = gain;
        }
        /// <summary>
        /// Слой изменения формы тензора
        /// </summary>
        /// <param name="newShape">Новая форма</param>
        public ReShape(Shape3D newShape)
        {
            OutputShape = newShape;
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return g.ReShape(input, OutputShape, _gain);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }

        /// <summary>
        /// Только использование, без обучения
        /// </summary>
        public void OnlyUse()
        {
        }
    }
}