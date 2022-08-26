using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Радиально-базисная активационная ф-я
    /// </summary>
    [Serializable]
    public class GaussianRbfUnit : IActivation
    {

        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Input data tensor</param>
        /// <returns></returns>
        public NNValue Forward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                valueMatrix[i] = (float)Math.Exp(-Math.Pow(x[i], 2));
            }

            return valueMatrix;
        }

        /// <summary>
        /// Bakward pass
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                double d = x[i];
                valueMatrix.Data[i] = (float)(-2.0 * d * Math.Exp(-d * d));
            }

            return valueMatrix;
        }

        /// <summary>
        /// Activation function name
        /// </summary>
        public override string ToString()
        {
            return "RBF";
        }
    }
}
