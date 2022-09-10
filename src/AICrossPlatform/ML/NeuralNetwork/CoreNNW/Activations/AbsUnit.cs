using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Activation function в виде модуля
    /// </summary>
    [Serializable]
    public class AbsUnit : IActivation
    {
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;

        private float Forward(float x)
        {
            return Math.Abs(x);
        }

        private float Backward(float x)
        {
            if (x >= 0)
            {
                return 1.0f;
            }
            else
            {
                return -1.0f;
            }
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="x">Input data tensor</param>
        public NNValue Forward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Data.Length;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = Forward(x.Data[i]);
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
                valueMatrix.Data[i] = Backward(x.Data[i]);
            }

            return valueMatrix;
        }

        /// <summary>
        /// Activation function name
        /// </summary>
        public override string ToString()
        {
            return "Abs";
        }
    }
}