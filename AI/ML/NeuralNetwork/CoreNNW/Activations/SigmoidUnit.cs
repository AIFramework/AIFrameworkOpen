using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Сигмоидальная Activation function
    /// </summary>
    [Serializable]
    public class SigmoidUnit : IActivation
    {
        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 2;

        /// <summary>
        /// Сигмоидальная Activation function
        /// </summary>
        public SigmoidUnit()
        {
        }

        private float Forward(float x)
        {
            return (float)(1.0 / (1 + Math.Exp(-x)));
        }

        private float Backward(float x)
        {
            float act = Forward(x);
            return act * (1 - act);
        }
        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
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
        /// Bakward pass(производная)
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
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
        /// Имя функции
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Sigmoid";
        }
    }
}
