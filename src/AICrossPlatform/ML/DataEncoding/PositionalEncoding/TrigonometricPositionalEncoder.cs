using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Position (index) coding based on orthogonal trigonometric functions
    /// </summary>
    [Serializable]
    public class TrigonometricPositionalEncoder : IPositionEncoding
    {
        /// <summary>
        /// Вектор выхода dimension
        /// </summary>
        public int Dim { get; set; }

        private readonly Vector time;
        private readonly double coef;

        /// <summary>
        /// Position (index) coding based on orthogonal trigonometric functions
        /// </summary>
        /// <param name="dim">Вектор выхода dimension</param>
        public TrigonometricPositionalEncoder(int dim = 512)
        {
            if (dim % 2 == 1)
            {
                throw new Exception("dim is not divisible by 2 without remainder");
            }

            Dim = dim;

            time = new Vector(dim / 2);
            double c = 1.0 / time.Count;

            for (int i = 0; i < time.Count; i++)
            {
                time[i] = i * c;
            }

            coef = 2 * Math.PI;
        }


        /// <summary>
        /// Getting the vector position code(index)
        /// </summary>
        public Vector GetCode(int position)
        {
            return GetCode((double)position);
        }

        /// <summary>
        /// Getting the vector position code(time)
        /// </summary>
        public Vector GetCode(double position)
        {
            Vector sin = time.Transform(x => Math.Sin(x * position * coef));
            Vector cos = time.Transform(x => Math.Cos(x * position * coef));
            sin.AddRange(cos);

            return sin;
        }
    }
}
