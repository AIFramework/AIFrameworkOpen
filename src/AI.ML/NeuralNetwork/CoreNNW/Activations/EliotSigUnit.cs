using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активационная функция EliotSig
    /// </summary>
    [Serializable]
    public class EliotSigUnit : IActivation
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
                valueMatrix[i] = x[i] / (1 + Math.Abs(x[i]));
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
                float z = (float)(1.0 + Math.Abs(x[i]));
                z *= z;
                valueMatrix.Data[i] = 1.0f / z;
            }

            return valueMatrix;
        }

        /// <summary>
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "EliotSigm";
        }
    }
}
