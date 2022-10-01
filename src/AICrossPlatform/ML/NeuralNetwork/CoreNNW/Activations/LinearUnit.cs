using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Линейная активационная функция
    /// </summary>
    [Serializable]
    public class LinearUnit : IActivation
    {

        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Линейная активационная функция
        /// </summary>
        public LinearUnit()
        {

        }
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="x">Аргумента</param>
        public float Forward(float x)
        {
            return x;
        }

        /// <summary>
        /// Прямой проход
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
        /// Обратный проход(производная)
        /// </summary>
        /// <param name="x">Тензор аргумента</param>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Data.Length;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = 1.0f;
            }

            return valueMatrix;
        }

        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "Linear";
        }

    }
}