using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Class providing methods for Discrete Cosine Transform of type-II.
    /// See https://en.wikipedia.org/wiki/Discrete_cosine_transform
    /// </summary>
    [Serializable]
    /// 
    public class Dct2 : IDct
    {
        /// <summary>
        /// DCT precalculated cosine matrix
        /// </summary>
        private readonly float[][] _dctMtx;

        /// <summary>
        /// IDCT precalculated cosine matrix
        /// </summary>
        private readonly float[][] _dctMtxInv;

        /// <summary>
        /// Size of DCT
        /// </summary>
        public int Size => _dctSize;
        private readonly int _dctSize;

        /// <summary>
        /// Precalculate DCT matrices
        /// </summary>
        /// <param name="dctSize"></param>
        public Dct2(int dctSize)
        {
            _dctSize = dctSize;
            _dctMtx = new float[dctSize][];
            _dctMtxInv = new float[dctSize][];

            // Precalculate dct and idct matrices

            double m = Math.PI / (dctSize << 1);

            for (int k = 0; k < dctSize; k++)
            {
                _dctMtx[k] = new float[dctSize];

                for (int n = 0; n < dctSize; n++)
                {
                    _dctMtx[k][n] = 2 * (float)Math.Cos(((n << 1) + 1) * k * m);
                }
            }

            for (int k = 0; k < dctSize; k++)
            {
                _dctMtxInv[k] = new float[dctSize];

                for (int n = 0; n < dctSize; n++)
                {
                    _dctMtxInv[k][n] = 2 * (float)Math.Cos(((k << 1) + 1) * n * m);
                }
            }
        }

        /// <summary>
        /// DCT-II (without normalization)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Direct(float[] input, float[] output)
        {
            for (int k = 0; k < output.Length; k++)
            {
                output[k] = 0.0f;

                for (int n = 0; n < input.Length; n++)
                {
                    output[k] += input[n] * _dctMtx[k][n];
                }
            }
        }

        /// <summary>
        /// DCT-II (with normalization)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void DirectNorm(float[] input, float[] output)
        {
            float norm0 = (float)Math.Sqrt(0.5);
            float norm = (float)Math.Sqrt(0.5 / _dctSize);

            for (int k = 0; k < output.Length; k++)
            {
                output[k] = 0.0f;

                for (int n = 0; n < input.Length; n++)
                {
                    output[k] += input[n] * _dctMtx[k][n];
                }

                output[k] *= norm;
            }

            output[0] *= norm0;
        }

        /// <summary>
        /// IDCT-II (without normalization)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Inverse(float[] input, float[] output)
        {
            for (int k = 0; k < output.Length; k++)
            {
                output[k] = input[0];

                for (int n = 1; n < input.Length; n++)
                {
                    output[k] += input[n] * _dctMtxInv[k][n];
                }
            }
        }

        /// <summary>
        /// IDCT-II (with normalization)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void InverseNorm(float[] input, float[] output)
        {
            float norm0 = (float)Math.Sqrt(0.5);
            float norm = (float)Math.Sqrt(0.5 / _dctSize);

            for (int k = 0; k < output.Length; k++)
            {
                output[k] = input[0] * _dctMtxInv[k][0] * norm0;

                for (int n = 1; n < input.Length; n++)
                {
                    output[k] += input[n] * _dctMtxInv[k][n];
                }

                output[k] *= norm;
            }
        }
    }
}
