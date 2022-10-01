using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Радиально-базисная активационная ф-я
    /// </summary>
    [Serializable]
    public class GaussianRbfUnit : IActivation
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
                valueMatrix[i] = (float)Math.Exp(-Math.Pow(x[i], 2));

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
                double d = x[i];
                valueMatrix.Data[i] = (float)(-2.0 * d * Math.Exp(-d * d));
            }

            return valueMatrix;
        }

        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "RBF";
        }
    }
}
