using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активационная ф-я гиперболический тангенс
    /// y(x) = a*th(b*x)
    /// </summary>
    [Serializable]
    public class TanhUnit : IActivation
    {
        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 1;

        /// <summary>
        /// Параметр наклона y(x) = a*th(b*x), по-умолчанию равен b = 1, но есть рекомендация 2/3 [С. Хайкин. "Нейронные сети 2е изд. исп." стр. 247-248]
        /// </summary>
        public float Beta { get; set; } = 1f;

        /// <summary>
        /// Параметр масштаба y(x) = a*th(b*x), по-умолчанию равен a = 1, но есть рекомендация 1.7159 [С. Хайкин. "Нейронные сети 2е изд. исп." стр. 247-248]
        /// </summary>
        public float Alpha { get; set; } = 1f;

        /// <summary>
        /// Активационная ф-я гиперболический тангенс
        /// y(x) = a*th(b*x)
        /// </summary>
        public TanhUnit() { }

        private float Forward(float x)
        {
            return Alpha * (float)Math.Tanh(Beta*x);
        }

        private float Backward(float x)
        {
            //float coshx = (float)Math.Cosh(x);
            //float denom = (float)(Math.Cosh(2 * x) + 1);
            //return 4 * coshx * coshx / (denom * denom);
            float th = Alpha * (float)Math.Tanh(Beta * x);
            return 1 - th * th;
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="x">Input data tensor</param>
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
        /// Bakward pass
        /// </summary>
        /// <param name="x">Input data tensor</param>
        public NNValue Backward(NNValue x)
        {
            NNValue valueMatrix = new NNValue(x.Shape);
            int len = x.Data.Length;

            for (int i = 0; i < len; i++)
            {
                valueMatrix.Data[i] = Backward(x.Data[i]);
            }

            return valueMatrix;
        }

        /// <summary>
        /// Activation function name
        /// </summary>
        public override string ToString()
        {
            return "Tanh";
        }
    }
}
