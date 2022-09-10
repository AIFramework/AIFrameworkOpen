using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Upsampling with bicubic interpolation
    /// </summary>
    [Serializable]
    public class UpSampling2DBicubic : ILayer
    {
        private readonly int _h, _w;
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
                OutputShape = new Shape3D(value.Height * _h, value.Width * _w, value.Depth);
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
        /// Adding a value to the denominator under the root when initializing the weights
        /// </summary>
        public double AddDenInSqrt { get; private set; }

        /// <summary>
        /// Upsampling with bicubic interpolation
        /// </summary>
        /// <param name="inputShape"> Размерность и форма входного тензора </param>
        public UpSampling2DBicubic(Shape3D inputShape, int h = 2, int w = 2)
        {
            _h = h;
            _w = w;
            InputShape = inputShape;
        }
        /// <summary>
        /// Upsampling with bicubic interpolation
        /// </summary>
        public UpSampling2DBicubic(int h = 2, int w = 2)
        {
            _h = h;
            _w = w;
        }

        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue res = g.Upsampling2DBicubic(input, _h, _w);
            return res;
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription("UpSampling2DBC", InputShape, OutputShape, "None", TrainableParameters);
        }

        public void OnlyUse()
        {
        }
    }
}