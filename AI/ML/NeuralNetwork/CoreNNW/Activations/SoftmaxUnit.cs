using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Софтмакс активация
    /// </summary>
    [Serializable]
    public class SoftmaxUnit : IActivation
    {
        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 1; // Todo


        private static readonly float maxActivate = 1e+3f;
        private static readonly float eps = 1e-15f;

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Input data tensor</param>
        /// <returns></returns>
        public NNValue Forward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Data.Length;
            float summ = 0;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = (float)Math.Exp(x.Data[i]);
                valueMatrix.Data[i] = valueMatrix.Data[i] > maxActivate ? maxActivate : valueMatrix.Data[i];
                summ += valueMatrix.Data[i];
            }

            summ += eps;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] /= summ;
            }

            return valueMatrix;
        }
        /// <summary>
        /// Bakward pass
        /// </summary>
        /// <param name="x">Input data tensor</param>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Data.Length;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = 1;
            }

            return valueMatrix;
        }
        /// <summary>
        /// Имя функции активации
        /// </summary>
        public override string ToString()
        {
            return "Softmax";
        }
    }
}
