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
        /// Обратный проход
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
        public void Backward(NNValue actualOutput, NNValue targetOutput)
        {
            for (int i = 0; i < targetOutput.Data.Length; i++)
            {
                float errDelta = actualOutput.Data[i] - targetOutput.Data[i];
                actualOutput.DifData[i] += errDelta;
            }
        }

        /// <summary>
        /// Значение ошибки
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
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

