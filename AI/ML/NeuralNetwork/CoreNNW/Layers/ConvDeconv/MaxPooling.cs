using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Макспуллинг
    /// </summary>
    [Serializable]
    public class MaxPooling : ILayer
    {
        private readonly int _h, _w;
        private Shape3D _inputShape;

        /// <summary>
        /// Dimension and shape of the input tensor
        /// </summary>
        public Shape3D InputShape
        {
            get => _inputShape;
            set
            {
                _inputShape = value;
                OutputShape = new Shape3D(value.Height / _h, value.Width / _w, value.Depth);
            }
        }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => 0;
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }

        /// <summary>
        /// Subsampling (Maxpooling 2D)
        /// </summary>
        /// <param name="inputShape">Dimension and shape of the input tensor</param>
        /// <param name="h">How many times to compress out in height</param>
        /// <param name="w">How many times to compress in width</param>
        public MaxPooling(Shape3D inputShape, int h = 2, int w = 2)
        {
            _h = h;
            _w = w;
            InputShape = inputShape;
        }
        /// <summary>
        /// Subsampling (Maxpooling 2D) 
        /// </summary>
        /// <param name="h">How many times to compress out in height</param>
        /// <param name="w">How many times to compress in width</param>
        public MaxPooling(int h = 2, int w = 2)
        {
            _h = h;
            _w = w;
        }

        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            NNValue res = g.MaxPooling(input, _h, _w);
            return res;
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