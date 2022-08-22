using AI.BackEnds.DSP.NWaves.Signals;
using System;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Class for Goertzel algorithm
    /// </summary>
    [Serializable]
    /// 
    public class Goertzel
    {
        /// <summary>
        /// Size of FFT
        /// </summary>
        private readonly int _fftSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fftSize">Size of FFT</param>
        public Goertzel(int fftSize)
        {
            _fftSize = fftSize;
        }

        /// <summary>
        /// Very simple implementation of Goertzel algorithm
        /// </summary>
        /// <param name="input">Input array</param>
        /// <param name="n">Number of the frequency component</param>
        /// <returns>nth component of a complex spectrum</returns>
        public Complex Direct(float[] input, int n)
        {
            float f = (float)(2 * Math.Cos(2 * Math.PI * n / _fftSize));

            float s1 = 0, s2 = 0, s = 0;

            for (int i = 0; i < _fftSize; i++)
            {
                s = input[i] + (s1 * f) - s2;

                s2 = s1;
                s1 = s;
            }

            Complex c = Complex.FromPolarCoordinates(1, 2 * Math.PI * n / _fftSize);

            c *= s;
            c -= s1;

            return c;
        }

        /// <summary>
        /// Overloaded method for DiscreteSignal
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="n">Number of the frequency component</param>
        /// <returns>nth component of a complex spectrum</returns>
        public Complex Direct(DiscreteSignal input, int n)
        {
            return Direct(input.Samples, n);
        }
    }
}
