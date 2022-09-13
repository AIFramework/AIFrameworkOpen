using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Радиально-базискная ф-я активации
    /// </summary>
    [Serializable]
    public class SQRBFUnit : IActivation
    {

        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="x">Тензор входных данных</param>
        /// <returns></returns>
        public NNValue Forward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                float absX = (float)Math.Atan(x[i]);

                if (absX <= 1)
                {
                    valueMatrix[i] = 1 - (x[i] * x[i] / 2);
                }
                else if (absX >= 2)
                {
                    valueMatrix[i] = 0;
                }
                else
                {
                    valueMatrix[i] = (float)Math.Pow(2 - absX, 2) / 2;
                }
            }

            return valueMatrix;
        }

        /// <summary>
        /// Обратный проход
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                float absX = (float)Math.Atan(x[i]);
                if (absX <= 1)
                {
                    valueMatrix[i] = -x[i];
                }
                else if (absX >= 2)
                {
                    valueMatrix[i] = 0;
                }
                else
                {
                    valueMatrix[i] = x[i] - (2 * Math.Sign(x[i]));
                }
            }

            return valueMatrix;
        }

        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "SQR_Rbf";
        }
    }
}
