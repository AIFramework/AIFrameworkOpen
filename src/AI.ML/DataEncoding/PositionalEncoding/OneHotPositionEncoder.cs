using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{

    /// <summary>
    /// Кодирование позиции в вектор например, позиция 1 кодируется [0, 1, 0, 0], позиция 2 [0, 0, 1, 0]
    /// </summary>
    [Serializable]
    public class OneHotPositionEncoder : IPositionEncoding
    {
        /// <summary>
        /// Размерность вектора
        /// </summary>
        public int Dim { get; set; }

        /// <summary>
        /// Кодирование позиции в вектор например, позиция 1 кодируется [0, 1, 0, 0], позиция 2 [0, 0, 1, 0]
        /// </summary>
        public OneHotPositionEncoder(int dim)
        {
            Dim = dim;

        }

        /// <summary>
        /// Получить код
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector GetCode(int position)
        {
            Vector outp = new Vector(Dim)
            {
                [position > Dim - 1 ? Dim - 1 : position] = 1
            };
            return outp;
        }

        /// <summary>
        /// Получить код
        /// </summary>
        public Vector GetCode(double position)
        {
            return GetCode((int)position);
        }
    }
}
