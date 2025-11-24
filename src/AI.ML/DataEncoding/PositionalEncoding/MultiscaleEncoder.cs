using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Многомасштабное кодирование позиции, предполагается, что размерность может быть представлена ​​как 2^N
    /// </summary>
    [Serializable]
    public class MultiscaleEncoder : IPositionEncoding
    {
        /// <summary>
        /// Размерность
        /// </summary>
        public int Dim { get; set; }

        private readonly int[] dims;

        /// <summary>
        /// Многомасштабное кодирование позиции, предполагается, что размерность может быть представлена ​​как 2^N
        /// </summary>
        /// <param name="dim">Размерность</param>
        public MultiscaleEncoder(int dim)
        {
            Dim = dim;
            double k = Math.Log(dim, 2);

            if (k % 1 != 0)
            {
                throw new Exception("dim != 2^N");
            }

            dims = new int[(int)k];

            for (int i = 0; i < dims.Length; i++)
            {
                dims[i] = dim / (1 << (i + 1));
            }

            dims[0] += 1;
        }


        /// <summary>
        /// Получение многомасштабного кода положения, код представляется вектором
        /// </summary>
        /// <param name="position">Позиция</param>
        public Vector GetCode(int position)
        {
            Vector outp = new Vector();

            for (int i = 0; i < dims.Length; i++)
            {
                Vector d = new Vector(dims[i])
                {
                    [position % dims[i]] = 1
                };
                outp.AddRange(d);
            }

            return outp;
        }

        /// <summary>
        /// Получение многомасштабного кода положения, код представляется вектором
        /// </summary>
        /// <param name="position">Позиция</param>
        public Vector GetCode(double position)
        {
            return GetCode((int)position);
        }
    }
}
