using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Class providing methods for direct and inverse Fast Fourier Transforms
    /// and postprocessing: magnitude spectrum, power spectrum, logpower spectrum.
    /// </summary>
    public class Fft64
    {
        /// <summary>
        /// The size of FFT
        /// </summary>
        public int Size => _fftSize;
        private readonly int _fftSize;

        /// <summary>
        /// Precomputed cosines
        /// </summary>
        private readonly double[] _cosTbl;

        /// <summary>
        /// Precomputed sines
        /// </summary>
        private readonly double[] _sinTbl;

        /// <summary>
        /// Constructor accepting the size of FFT
        /// </summary>
        /// <param name="fftSize">Size of FFT</param>
        public Fft64(int fftSize = 512)
        {
            Guard.AgainstNotPowerOfTwo(fftSize, "FFT size");

            _fftSize = fftSize;

            int tblSize = (int)Math.Log(fftSize, 2);

            _cosTbl = new double[tblSize];
            _sinTbl = new double[tblSize];

            for (int i = 1, pos = 0; i < _fftSize; i *= 2, pos++)
            {
                _cosTbl[pos] = Math.Cos(2 * Math.PI * i / _fftSize);
                _sinTbl[pos] = Math.Sin(2 * Math.PI * i / _fftSize);
            }
        }

        /// <summary>
        /// Fast Fourier Transform algorithm
        /// </summary>
        /// <param name="re">Array of real parts</param>
        /// <param name="im">Array of imaginary parts</param>
        public void Direct(double[] re, double[] im)
        {
            int L = _fftSize;
            int M = _fftSize >> 1;
            int S = _fftSize - 1;
            int ti = 0;
            while (L >= 2)
            {
                int l = L >> 1;
                double u1 = 1.0;
                double u2 = 0.0;
                double c = _cosTbl[ti];
                double s = -_sinTbl[ti];
                ti++;
                for (int j = 0; j < l; j++)
                {
                    for (int i = j; i < _fftSize; i += L)
                    {
                        int p = i + l;
                        double t1 = re[i] + re[p];
                        double t2 = im[i] + im[p];
                        double t3 = re[i] - re[p];
                        double t4 = im[i] - im[p];
                        re[p] = t3 * u1 - t4 * u2;
                        im[p] = t4 * u1 + t3 * u2;
                        re[i] = t1;
                        im[i] = t2;
                    }
                    double u3 = u1 * c - u2 * s;
                    u2 = u2 * c + u1 * s;
                    u1 = u3;
                }
                L >>= 1;
            }
            for (int i = 0, j = 0; i < S; i++)
            {
                if (i > j)
                {
                    double t1 = re[j];
                    double t2 = im[j];
                    re[j] = re[i];
                    im[j] = im[i];
                    re[i] = t1;
                    im[i] = t2;
                }
                int k = M;
                while (j >= k)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
        }

        /// <summary>
        /// Inverse Fast Fourier Transform algorithm
        /// </summary>
        /// <param name="re">Array of real parts</param>
        /// <param name="im">Array of imaginary parts</param>
        public void Inverse(double[] re, double[] im)
        {
            int L = _fftSize;
            int M = _fftSize >> 1;
            int S = _fftSize - 1;
            int ti = 0;
            while (L >= 2)
            {
                int l = L >> 1;
                double u1 = 1.0;
                double u2 = 0.0;
                double c = _cosTbl[ti];
                double s = _sinTbl[ti];
                ti++;
                for (int j = 0; j < l; j++)
                {
                    for (int i = j; i < _fftSize; i += L)
                    {
                        int p = i + l;
                        double t1 = re[i] + re[p];
                        double t2 = im[i] + im[p];
                        double t3 = re[i] - re[p];
                        double t4 = im[i] - im[p];
                        re[p] = t3 * u1 - t4 * u2;
                        im[p] = t4 * u1 + t3 * u2;
                        re[i] = t1;
                        im[i] = t2;
                    }
                    double u3 = u1 * c - u2 * s;
                    u2 = u2 * c + u1 * s;
                    u1 = u3;
                }
                L >>= 1;
            }
            for (int i = 0, j = 0; i < S; i++)
            {
                if (i > j)
                {
                    double t1 = re[j];
                    double t2 = im[j];
                    re[j] = re[i];
                    im[j] = im[i];
                    re[i] = t1;
                    im[i] = t2;
                }
                int k = M;
                while (j >= k)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
        }

        /// <summary>
        /// Inverse Fast Fourier Transform algorithm (with normalization by FFT size)
        /// </summary>
        /// <param name="re">Array of real parts</param>
        /// <param name="im">Array of imaginary parts</param>
        public void InverseNorm(double[] re, double[] im)
        {
            Inverse(re, im);

            for (int i = 0; i < _fftSize; i++)
            {
                re[i] /= _fftSize;
                im[i] /= _fftSize;
            }
        }
    }
}
