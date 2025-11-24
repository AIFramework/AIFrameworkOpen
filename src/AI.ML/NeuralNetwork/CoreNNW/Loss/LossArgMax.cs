using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Аргмакс (нереализован)
    /// </summary>
    [Serializable]
    public class LossArgMax : ILoss
    {
        /// <summary>
        /// Обратный проход
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
        public void Backward(NNValue actualOutput, NNValue targetOutput)
        {
            throw new NotImplementedException();

        }
        /// <summary>
        /// Значение ошибки
        /// </summary>
        /// <param name="actualOutput">Реальный вызод</param>
        /// <param name="targetOutput">Целевой(идеальный) выход</param>
        public float Measure(NNValue actualOutput, NNValue targetOutput)
        {
            if (actualOutput.Data.Length != targetOutput.Data.Length)
            {
                throw new Exception("mismatch");
            }
            float maxActual = float.PositiveInfinity;
            float maxTarget = float.NegativeInfinity;
            int indxMaxActual = -1;
            int indxMaxTarget = -1;
            for (int i = 0; i < actualOutput.Data.Length; i++)
            {
                if (actualOutput.Data[i] > maxActual)
                {
                    maxActual = actualOutput.Data[i];
                    indxMaxActual = i;
                }
                if (targetOutput.Data[i] > maxTarget)
                {
                    maxTarget = targetOutput.Data[i];
                    indxMaxTarget = i;
                }
            }
            if (indxMaxActual == indxMaxTarget)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
