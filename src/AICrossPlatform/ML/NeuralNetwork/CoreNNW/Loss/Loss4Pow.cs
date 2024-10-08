﻿using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Loss
{
    /// <summary>
    /// Ошибка 4я степень, для контрастирования выбросов
    /// </summary>
    [Serializable]
    public class Loss4Pow : ILoss
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
                actualOutput.DifData[i] += errDelta + (8 * errDelta * errDelta * errDelta);
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
                float sqr = errDelta * errDelta;
                sum += 0.5f * (sqr + (4 * sqr * sqr));
            }
            return sum;
        }
    }
}

