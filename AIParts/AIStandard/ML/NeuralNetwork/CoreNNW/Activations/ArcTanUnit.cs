using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Activation function
    /// </summary>
    [Serializable]
    public class ArcTanUnit : IActivation
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
                valueMatrix[i] = (float)Math.Atan(x[i]);
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
                float z = x[i];
                z *= z;
                z += 1;

                valueMatrix.Data[i] = 1.0f / z;
            }

            return valueMatrix;
        }
        /// <summary>
        /// Activation function name
        /// </summary>
        public override string ToString()
        {
            return "ATan";
        }

    }
}
