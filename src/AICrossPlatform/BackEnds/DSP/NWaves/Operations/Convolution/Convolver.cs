using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations.Convolution
{
    /// <summary>
    /// Class responsible for real-valued convolution
    /// </summary>
    [Serializable]
    public class Convolver
    {
        /// <summary>
        /// Размер блока (число отсчетов) для преобразования Фурье
        /// </summary>
        private int _fftSize;

        /// <summary>
        /// Метод вычисления БПФ
        /// </summary>
        private RealFft _fft;

        // internal reusable buffers
        private float[] _real1;
        private float[] _imag1;
        private float[] _real2;
        private float[] _imag2;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fftSize">FFT size</param>
        public Convolver(int fftSize = 0)
        {
            if (fftSize > 0)
            {
                PrepareMemory(fftSize);
            }
        }

        /// <summary>
        /// Prepare all necessary arrays for calculations
        /// </summary>
        /// <param name="fftSize"></param>
        private void PrepareMemory(int fftSize)
        {
            _fftSize = fftSize;
            _fft = new RealFft(_fftSize);

            _real1 = new float[_fftSize];
            _imag1 = new float[_fftSize];
            _real2 = new float[_fftSize];
            _imag2 = new float[_fftSize];
        }

        /// <summary>
        /// Свертка
        /// </summary>
        /// <param name="signal">Signal of length N</param>
        /// <param name="kernel">Kernel of length M</param>
        /// <returns>Convolution signal of length N + M - 1</returns>
        public DiscreteSignal Convolve(DiscreteSignal signal, DiscreteSignal kernel)
        {
            int length = signal.Length + kernel.Length - 1;

            if (_fft == null)
            {
                PrepareMemory(MathUtils.NextPowerOfTwo(length));
            }

            float[] output = new float[_fftSize];

            Convolve(signal.Samples, kernel.Samples, output);

            return new DiscreteSignal(signal.SamplingRate, output).First(length);
        }

        /// <summary>
        /// Fast convolution via FFT for arrays of samples (maximally in-place).
        /// This version is best suited for block processing when memory needs to be reused.
        /// Input arrays must have size equal to the Размер блока БПФ.
        /// Размер блока (число отсчетов) для преобразования Фурье MUST be set properly in constructor!
        /// </summary>
        /// <param name="input">Real parts of the 1st signal (zero-padded)</param>
        /// <param name="kernel">Real parts of the 2nd signal (zero-padded)</param>
        /// <param name="output">Real parts of resulting convolution (zero-padded)</param>
        public void Convolve(float[] input, float[] kernel, float[] output)
        {
            Array.Clear(_real1, 0, _fftSize);
            Array.Clear(_real2, 0, _fftSize);

            input.FastCopyTo(_real1, input.Length);
            kernel.FastCopyTo(_real2, kernel.Length);

            // 1) do FFT of both signals

            _fft.Direct(_real1, _real1, _imag1);
            _fft.Direct(_real2, _real2, _imag2);

            // 2) do complex multiplication of spectra and normalize

            for (int i = 0; i <= _fftSize / 2; i++)
            {
                float re = (_real1[i] * _real2[i]) - (_imag1[i] * _imag2[i]);
                float im = (_real1[i] * _imag2[i]) + (_imag1[i] * _real2[i]);
                _real1[i] = re / _fftSize;
                _imag1[i] = im / _fftSize;
            }

            // 3) do Обратное преобразование Фурье (ОБПФ) of resulting spectrum

            _fft.Inverse(_real1, _imag1, output);
        }

        /// <summary>
        /// Fast cross-correlation via FFT
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        /// <returns></returns>
        public DiscreteSignal CrossCorrelate(DiscreteSignal signal1, DiscreteSignal signal2)
        {
            DiscreteSignal reversedKernel = new DiscreteSignal(signal2.SamplingRate, signal2.Samples.Reverse());

            return Convolve(signal1, reversedKernel);
        }

        /// <summary>
        /// Fast cross-correlation via FFT for arrays of samples (maximally in-place).
        /// This version is best suited for block processing when memory needs to be reused.
        /// Input arrays must have size equal to the Размер блока БПФ.
        /// Размер блока (число отсчетов) для преобразования Фурье MUST be set properly in constructor!
        /// </summary>
        /// <param name="input1">Real parts of the 1st signal (zero-padded)</param>
        /// <param name="input2">Real parts of the 2nd signal (zero-padded)</param>
        /// <param name="output">Real parts of resulting cross-correlation (zero-padded if center == 0)</param>
        /// (if it is set then resulting array has length of CENTER
        public void CrossCorrelate(float[] input1, float[] input2, float[] output)
        {
            // reverse second signal

            int kernelLength = input2.Length - 1;

            for (int i = 0; i < kernelLength / 2; i++)
            {
                float tmp = input2[i];
                input2[i] = input2[kernelLength - i];
                input2[kernelLength - i] = tmp;
            }

            Convolve(input1, input2, output);
        }
    }
}
