﻿using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// SQN Активация
    /// </summary>
    [Serializable]
    public class SqnlUnit : IActivation
    {
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Прямой проход
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
                    valueMatrix[i] = x[i] + (x[i] * x[i] / 4);
                }
                else
                {
                    valueMatrix[i] = x[i] - (x[i] * x[i] / 4);
                }
            }

            return valueMatrix;
        }
        /// <summary>
        /// Обратный проход(производная)
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = 1 + (x[i] / 2);
            }

            return valueMatrix;
        }

        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "Sqn";
        }
    }
}
