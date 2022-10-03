using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.ConvDeconv
{
    /// <summary>
    /// Одномерный апсемплинг с бикубической интерполяцией
    /// </summary>
    [Serializable]
    public class UpSampling1D : ILayer
    {
        private readonly UpSampling2DBicubic _upsampling2DBicubic;

        /// <summary>
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape { get => _upsampling2DBicubic.InputShape; set => _upsampling2DBicubic.InputShape = value; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape => _upsampling2DBicubic.OutputShape;
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => _upsampling2DBicubic.TrainableParameters;
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt => _upsampling2DBicubic.AddDenInSqrt;

        /// <summary>
        /// Одномерный апсемплинг с бикубической интерполяцией
        /// </summary>
        public UpSampling1D(int k = 2)
        {
            _upsampling2DBicubic = new UpSampling2DBicubic(k, 1);
        }
        /// <summary>
        /// Одномерный апсемплинг с бикубической интерполяцией
        /// </summary>
        public UpSampling1D(Shape3D inputShape, int k = 2)
        {
            _upsampling2DBicubic = new UpSampling2DBicubic(inputShape, k, 1);
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return _upsampling2DBicubic.Forward(input, g);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }

        /// <summary>
        /// Только использование
        /// </summary>
        public void OnlyUse()
        {
        }
    }
}