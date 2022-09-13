using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активационная функция
    /// </summary>
    [Serializable]
    public class ArcTanUnit : IActivation
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
                valueMatrix[i] = (float)Math.Atan(x[i]);
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
                float z = x[i];
                z *= z;
                z += 1;

                valueMatrix.Data[i] = 1.0f / z;
            }

            return valueMatrix;
        }
        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "ATan";
        }

    }
}
