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
        /// Dimension and shape of the input tensor
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
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        /// <summary>
        /// Слой который преобразует тензор в вектор
        /// </summary>
        /// <param name="inputShape">Dimension and shape of the input tensor</param>
        public Flatten(Shape3D inputShape)
        {
            InputShape = inputShape;
        }
        /// <summary>
        /// Слой который преобразует тензор в вектор
        /// </summary>
        /// <param name="inputShape">Dimension and shape of the input tensor</param>
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
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            Shape3D shape = new Shape3D(input.Shape.Count);
            return g.ReShape(input, shape, _gain);

        }
        /// <summary>
        /// Layer description
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