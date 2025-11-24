using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Перекрестная энтропия
    /// </summary>
    [Serializable]
    public class CrossEntropy : ILoss
    {
        private const float EPS = 1e-30f;

        /// <summary>
        /// Обратный проход
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
        public void Backward(NNValue actualOutput, NNValue targetOutput)
        {
            for (int i = 0; i < targetOutput.Data.Length; i++)
            {
                actualOutput.DifData[i] += actualOutput[i] - targetOutput[i];
            }
        }
        /// <summary>
        /// Значение ошибки
        /// </summary>
        /// <param name="actual">Реальный вызод</param>
        /// <param name="target">Целевой(идеальный) выход</param>
        public float Measure(NNValue actual, NNValue target)
        {
            float crossentropy = 0.0f;

            for (int i = 0; i < actual.Data.Length; i++)
            {

                crossentropy -= (float)((target.Data[i] * Math.Log(actual.Data[i] + EPS)) +
                                ((1 - target.Data[i]) * Math.Log(1.0 - actual.Data[i] + EPS)));
            }


            return crossentropy;
        }
    }
}
