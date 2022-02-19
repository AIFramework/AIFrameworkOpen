using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// SQN Активация
    /// </summary>
    [Serializable]
    public class SqnlUnit : IActivation
    {

        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        public NNValue Forward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                if (x[i] > 2)
                {
                    valueMatrix[i] = 1;
                }
                else if (x[i] < -2)
                {
                    valueMatrix[i] = -1;
                }
                else if (x[i] < 0)
                {
                    valueMatrix[i] = x[i] + x[i] * x[i] / 4;
                }
                else
                {
                    valueMatrix[i] = x[i] - x[i] * x[i] / 4;
                }
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
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = 1 + x[i] / 2;
            }

            return valueMatrix;
        }

        /// <summary>
        /// Activation function name
        /// </summary>
        public override string ToString()
        {
            return "Sqn";
        }
    }
}
