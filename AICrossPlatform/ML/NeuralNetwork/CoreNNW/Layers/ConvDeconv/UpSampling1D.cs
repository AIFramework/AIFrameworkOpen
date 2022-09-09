using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.ConvDeconv
{
    /// <summary>
    /// One-dimensional upsampling with bicubic interpolation
    /// </summary>
    [Serializable]
    public class UpSampling1D : ILayer
    {
        private readonly UpSampling2DBicubic _upsampling2DBicubic;

        /// <summary>
        /// Dimension and shape of the input tensor
        /// </summary>
        public Shape3D InputShape { get => _upsampling2DBicubic.InputShape; set => _upsampling2DBicubic.InputShape = value; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape => _upsampling2DBicubic.OutputShape;
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => _upsampling2DBicubic.TrainableParameters;
        /// <summary>
        /// Adding a value to the denominator under the root when initializing the weights
        /// </summary>
        public double AddDenInSqrt => _upsampling2DBicubic.AddDenInSqrt;

        /// <summary>
        /// One-dimensional upsampling with bicubic interpolation
        /// </summary>
        public UpSampling1D(int k = 2)
        {
            _upsampling2DBicubic = new UpSampling2DBicubic(k, 1);
        }
        /// <summary>
        /// One-dimensional upsampling with bicubic interpolation
        /// </summary>
        public UpSampling1D(Shape3D inputShape, int k = 2)
        {
            _upsampling2DBicubic = new UpSampling2DBicubic(inputShape, k, 1);
        }

        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            return _upsampling2DBicubic.Forward(input, g);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, "None", TrainableParameters);
        }

        public void OnlyUse()
        {
        }
    }
}