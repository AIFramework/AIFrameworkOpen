using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Fast implementation of DCT-IV via FFT
    /// </summary>
    public class FastDct4 : IDct
    {
        /// <summary>
        /// Internal FFT transformer
        /// </summary>
        private readonly Fft _fft;

        /// <summary>
        /// Internal temporary buffer
        /// </summary>
        private readonly float[] _temp;
        private readonly float[] _tempRe;
        private readonly float[] _tempIm;

        /// <summary>
        /// DCT size
        /// </summary>
        public int Size => 2 * _fft.Size;


        public FastDct4(int dctSize)
        {
            int halfSize = dctSize / 2;
            _fft = new Fft(halfSize);
            _temp = new float[halfSize];
            _tempRe = new float[halfSize];
            _tempIm = new float[halfSize];
        }

        public void Direct(float[] input, float[] output)
        {
            Array.Clear(output, 0, output.Length);

            int N = Size;

            // mutiply by exp(-j * pi * n / N):

            for (int m = 0; m < _temp.Length; m++)
            {
                float re = input[2 * m];
                float im = input[N - 1 - 2 * m];
                double cos = Math.Cos(Math.PI * m / N);
                double sin = Math.Sin(-Math.PI * m / N);

                _temp[m] = 2 * (float)(re * cos - im * sin);
                output[m] = 2 * (float)(re * sin + im * cos);
            };

            _fft.Direct(_temp, output);

            // mutiply by exp(-j * pi * (2n + 0.5) / 2N):

            for (int m = 0; m < _temp.Length; m++)
            {
                float re = _temp[m];
                float im = output[m];
                double cos = Math.Cos(0.5 * Math.PI * (2 * m + 0.5) / N);
                double sin = Math.Sin(-0.5 * Math.PI * (2 * m + 0.5) / N);

                _tempRe[m] = (float)(re * cos - im * sin);
                _tempIm[m] = (float)(re * sin + im * cos);
            };

            for (int m = 0, k = 0; m < N; m += 2, k++)
            {
                output[m] = _tempRe[k];
            }
            for (int m = 1, k = (N - 2) / 2; m < N; m += 2, k--)
            {
                output[m] = -_tempIm[k];
            }
        }

        public void DirectNorm(float[] input, float[] output)
        {
            Direct(input, output);

            float norm = (float)(0.5 * Math.Sqrt(2.0 / Size));

            for (int i = 0; i < Size; output[i++] *= norm)
            {
                ;
            }
        }

        public void Inverse(float[] input, float[] output)
        {
            Direct(input, output);
        }

        public void InverseNorm(float[] input, float[] output)
        {
            DirectNorm(input, output);
        }
    }
}
