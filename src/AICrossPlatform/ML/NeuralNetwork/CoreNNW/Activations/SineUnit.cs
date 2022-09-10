using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Синусоидальная активационная фукция
    /// </summary>
    [Serializable]
    public class SineUnit : IActivation
    {
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;



        /// <summary>
        /// Синусоидальная активационная фукция
        /// </summary>
        public SineUnit()
        {

        }

        private float Forward(float x)
        {
            return (float)Math.Sin(x);
        }

        private float Backward(float x)
        {
            return (float)Math.Cos(x);
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
            return "Sin";
        }
    }
}
