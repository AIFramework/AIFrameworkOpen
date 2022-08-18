using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Multiscale position coding
    /// </summary>
    [Serializable]
    public class MultiscaleEncoder : IPositionEncoding
    {
        /// <summary>
        /// Dimension
        /// </summary>
        public int Dim { get; set; }

        private readonly int[] dims;

        /// <summary>
        /// Multiscale position coding, it is assumed that the dimension can be represented as 2^N
        /// </summary>
        /// <param name="dim">Dimension</param>
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
        /// Getting a multi-scale position code, the code is represented by a vector
        /// </summary>
        /// <param name="position">Position</param>
        public Vector GetCode(int position)
        {
            Vector outp = new Vector(0);

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
        /// Getting a multi-scale position code, the code is represented by a vector
        /// </summary>
        /// <param name="position">Position</param>
        public Vector GetCode(double position)
        {
            return GetCode((int)position);
        }
    }
}
