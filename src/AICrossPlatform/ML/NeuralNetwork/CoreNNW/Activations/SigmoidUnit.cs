using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Сигмоидальная функция активации [ y(x) = a*sigm(x+eps)]
    /// </summary>
    [Serializable]
    public class SigmoidUnit : IActivation
    {
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 2;


        /// <summary>
        /// Параметр смещения y(x) = a*sigm(b*x+eps), по-умолчанию равен eps = 0
        /// </summary>
        public float Epsilon { get; set; } = 0f;

        /// <summary>
        /// Параметр масштаба y(x) = a*sigm(b*x+eps), по-умолчанию равен a = 1
        /// </summary>
        public float Alpha { get; set; } = 1f;

        /// <summary>
        /// Параметр наклона y(x) = a*sigm(b*x+eps)
        /// </summary>
        public double Beta { get; set; } = 1f;

        /// <summary>
        /// Сигмоидальная функция активации [ y(x) = a*sigm(b*x+eps)]
        /// </summary>
        public SigmoidUnit()
        {
        }

        private float Forward(float x)
        {
            return Alpha*(float)(1.0 / (1 + Math.Exp(-(Beta * x -Epsilon))));
        }

        private float Backward(float x)
        {
            float act = Forward(x); // Правильно т.к. тензор возвращается до прохождения прямого слоя
            float dif = act * (1 - act);
            return dif;
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
                valueMatrix.Data[i] = Backward(x.Data[i]);
            }

            return valueMatrix;
        }
        /// <summary>
        /// Имя функции
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Sigmoid";
        }
    }
}
