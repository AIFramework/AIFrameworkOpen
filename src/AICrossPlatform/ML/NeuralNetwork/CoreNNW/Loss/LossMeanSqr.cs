using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Средний квадрат отклонения
    /// </summary>
    [Serializable]
    public class LossMeanSqrSqrt : ILoss
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
                actualOutput.DifData[i] += errDelta / (float)Math.Sqrt(actualOutput.Shape.Count);
            }
        }
        /// <summary>
        /// Значение ошибки
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
        public float Measure(NNValue actualOutput, NNValue targetOutput)
        {
            float sum = 0;
            for (int i = 0; i < targetOutput.Shape.Count; i++)
            {
                float errDelta = actualOutput.Data[i] - targetOutput.Data[i];
                sum += 0.5f * errDelta * errDelta;
            }
            return sum / (float)Math.Sqrt(actualOutput.Shape.Count);
        }
    }
}
