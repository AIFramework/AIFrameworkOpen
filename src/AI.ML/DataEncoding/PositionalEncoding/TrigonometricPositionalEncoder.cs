using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Позиционное (индексное) кодирование на основе ортогональных тригонометрических функций
    /// </summary>
    [Serializable]
    public class TrigonometricPositionalEncoder : IPositionEncoding
    {
        /// <summary>
        /// Размерность вектора выхода
        /// </summary>
        public int Dim { get; set; }

        private readonly Vector time;
        private readonly double coef;

        /// <summary>
        /// Позиционное (индексное) кодирование на основе ортогональных тригонометрических функций
        /// </summary>
        /// <param name="dim">Размерность вектора выхода</param>
        public TrigonometricPositionalEncoder(int dim = 512)
        {
            if (dim % 2 == 1)
            {
                throw new Exception("Размерность не делится на 2 без остатка");
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
        /// Получение кода позиции вектора (индекса)
        /// </summary>
        public Vector GetCode(int position)
        {
            return GetCode((double)position);
        }

        /// <summary>
        /// Получение кода позиции вектора (Время)
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
