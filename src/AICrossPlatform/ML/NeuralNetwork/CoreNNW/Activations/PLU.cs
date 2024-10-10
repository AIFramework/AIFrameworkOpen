using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активационная функция PLU (двусторонний ограничитель)
    /// </summary>
    [Serializable]
    public class PLU : IActivation
    {

        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public float Numerator => 1;

        private readonly float _slope;
        private readonly float _max;


        /// <summary>
        /// Активационная функция PLU (двусторонний ограничитель)
        /// </summary>
        public PLU()
        {
            _slope = 0;
            _max = 1;
        }
        /// <summary>
        /// Активационная функция PLU (двусторонний ограничитель)
        /// </summary>
        /// <param name="slope">Наклон за линейным участком</param>
        /// <param name="max">Максимальное значение линейного участка</param>
        public PLU(float slope, float max = 1)
        {
            _max = max;
            _slope = slope;
        }

        private float Forward(float x)
        {
            return (x >= -_max && x <= _max) ? x : _slope * x;
        }

        private float Backward(float x)
        {
            return (x >= -_max && x <= _max) ? 1.0f : _slope;
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
        /// Активационная функция name
        /// </summary>
        public override string ToString()
        {
            return "PLU";
        }
    }
}
