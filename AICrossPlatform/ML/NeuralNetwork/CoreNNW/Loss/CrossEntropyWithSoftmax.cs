using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Calculation of the error for the cross entropy provided that the output is Softmax
    /// </summary>
    [Serializable]
    public class CrossEntropyWithSoftmax : ILoss
    {
        /// <summary>
        /// Backward pass (taking derivative)
        /// </summary>
        /// <param name="actualOutput">Output value (actual)</param>
        /// <param name="targetOutput">Target value (ideal)</param>
        public void Backward(NNValue actualOutput, NNValue targetOutput)
        {
            for (int i = 0; i < targetOutput.Data.Length; i++)
            {
                float errDelta = actualOutput.Data[i] - targetOutput.Data[i];
                actualOutput.DifData[i] += errDelta;
            }
        }

        /// <summary>
        /// Error value
        /// </summary>
        /// <param name="actualOutput">Output value (actual)</param>
        /// <param name="targetOutput">Target value (ideal)</param>
        public float Measure(NNValue actualOutput, NNValue targetOutput)
        {
            float crossentropy = 0.0f;

            for (int i = 0; i < actualOutput.Data.Length; i++)
            {
                crossentropy += targetOutput.Data[i] * (float)Math.Log(actualOutput.Data[i] + 1e-15);
            }

            return -crossentropy;
        }
    }
}

