using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активация ReLU
    /// </summary>
    [Serializable]
    public class ReLU : IActivation
    {

        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 2;

        private readonly float _slope;


        /// <summary>
        /// Активация ReLU
        /// </summary>
        public ReLU()
        {
            _slope = 0;
        }

        /// <summary>
        /// Активация ReLU
        /// </summary>
        /// <param name="slope">Наклон при x меньше 0 </param>
        public ReLU(float slope)
        {
            _slope = slope;
        }

        /// <summary>
        /// Активация ReLU
        /// </summary>
        /// <param name="slope">Наклон при x меньше 0 </param>
        public ReLU(double slope)
        {
            _slope = (float)slope;
        }

        private float Forward(float x)
        {
            if (x >= 0)
            {
                return x;
            }
            else
            {
                return x * _slope;
            }
        }

        private float Backward(float x)
        {

            if (x >= 0)
            {
                return 1.0f;
            }
            else
            {
                return _slope;
            }
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="x">Тензор входных данных</param>
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
        /// Обратный проход
        /// </summary>
        /// <param name="x">Тензор входных данных</param>
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
        /// Имя актив ф.-ии
        /// </summary>
        public override string ToString()
        {
            return "ReLu:" + _slope;
        }
    }
}
