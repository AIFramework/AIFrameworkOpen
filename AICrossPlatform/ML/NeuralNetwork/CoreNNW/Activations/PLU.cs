using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Activations
{
    /// <summary>
    /// Активационная ф-я PLU (двусторонний ограничитель)
    /// </summary>
    [Serializable]
    public class PLU : IActivation
    {

        /// <summary>
        /// Random number generator setting numerator
        /// </summary>
        public float Numerator => 1;

        private readonly float _slope;
        private readonly float _max;


        /// <summary>
        /// Активационная ф-я PLU (двусторонний ограничитель)
        /// </summary>
        public PLU()
        {
            _slope = 0;
            _max = 1;
        }
        /// <summary>
        /// Активационная ф-я PLU (двусторонний ограничитель)
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
            if (x >= -_max && x <= _max)
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
            if (x >= -_max && x <= _max)
            {
                return 1.0f;
            }
            else
            {
                return _slope;
            }
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
            return "PLU";
        }
    }
}
