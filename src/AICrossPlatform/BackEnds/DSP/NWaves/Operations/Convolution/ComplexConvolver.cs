﻿using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Operations.Convolution
{
    /// <summary>
    /// Class responsible for complex-valued convolution.
    /// 
    /// ComplexConvolver does not participate in heavy calculations,
    /// so it does not contain internal buffers.
    /// 
    /// Memory is allocated for each operation ad-hoc.
    /// 
    /// </summary>
    [Serializable]
    public class ComplexConvolver
    {
        /// <summary>
        /// Fast convolution via FFT for general complex-valued case
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="kernel"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public ComplexDiscreteSignal Convolve(ComplexDiscreteSignal signal, ComplexDiscreteSignal kernel, int fftSize = 0)
        {
            int length = signal.Length + kernel.Length - 1;

            if (fftSize == 0)
            {
                fftSize = MathUtils.NextPowerOfTwo(length);
            }

            Fft64 fft = new Fft64(fftSize);

            signal = signal.ZeroPadded(fftSize);
            kernel = kernel.ZeroPadded(fftSize);

            // 1) do FFT of both signals

            fft.Direct(signal.Real, signal.Imag);
            fft.Direct(kernel.Real, kernel.Imag);

            // 2) do complex multiplication of spectra

            ComplexDiscreteSignal spectrum = signal.Multiply(kernel);

            // 3) do inverse FFT of resulting spectrum

            fft.Inverse(spectrum.Real, spectrum.Imag);

            // 3a) normalize

            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum.Real[i] /= fftSize;
                spectrum.Imag[i] /= fftSize;
            }

            // 4) return resulting meaningful part of the signal (truncate size to N + M - 1)

            return new ComplexDiscreteSignal(signal.SamplingRate, spectrum.Real, spectrum.Imag).First(length);
        }

        /// <summary>
        /// Fast cross-correlation via FFT
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="kernel"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public ComplexDiscreteSignal CrossCorrelate(ComplexDiscreteSignal signal, ComplexDiscreteSignal kernel, int fftSize = 0)
        {
            ComplexDiscreteSignal reversedKernel =
                new ComplexDiscreteSignal(kernel.SamplingRate, kernel.Real.Reverse(), kernel.Imag.Reverse());

            return Convolve(signal, reversedKernel, fftSize);
        }

        /// <summary>
        /// Fast deconvolution via FFT for general complex-valued case.
        ///  
        /// NOTE!
        /// 
        /// Deconvolution is an experimental feature.
        /// It's problematic due to division by zero.
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="kernel"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public ComplexDiscreteSignal Deconvolve(ComplexDiscreteSignal signal, ComplexDiscreteSignal kernel, int fftSize = 0)
        {
            // first, try to divide polynomials

            Complex[][] div = MathUtils.DividePolynomial(signal.Real.Zip(signal.Imag, (r, i) => new Complex(r, i)).ToArray(),
                                                   kernel.Real.Zip(kernel.Imag, (r, i) => new Complex(r, i)).ToArray());

            Complex[] quotient = div[0];
            Complex[] remainder = div[1];
            if (remainder.All(d => Math.Abs(d.Real) < 1e-10) &&
                remainder.All(d => Math.Abs(d.Imaginary) < 1e-10))
            {
                return new ComplexDiscreteSignal(signal.SamplingRate,
                                                 quotient.Select(q => q.Real),
                                                 quotient.Select(q => q.Imaginary));
            }

            // ... deconvolve via FFT

            int length = signal.Length - kernel.Length + 1;

            if (fftSize == 0)
            {
                fftSize = MathUtils.NextPowerOfTwo(signal.Length);
            }

            Fft64 fft = new Fft64(fftSize);

            signal = signal.ZeroPadded(fftSize);
            kernel = kernel.ZeroPadded(fftSize);

            // 1) do FFT of both signals

            fft.Direct(signal.Real, signal.Imag);
            fft.Direct(kernel.Real, kernel.Imag);

            for (int i = 0; i < fftSize; i++)
            {
                signal.Real[i] += 1e-10;
                signal.Imag[i] += 1e-10;
                kernel.Real[i] += 1e-10;
                kernel.Imag[i] += 1e-10;
            }

            // 2) do complex division of spectra

            ComplexDiscreteSignal spectrum = signal.Divide(kernel);

            // 3) do inverse FFT of resulting spectrum

            fft.Inverse(spectrum.Real, spectrum.Imag);

            // 4) return resulting meaningful part of the signal (truncate to N - M + 1)

            return new ComplexDiscreteSignal(signal.SamplingRate,
                                             spectrum.Real.FastCopyFragment(length),
                                             spectrum.Imag.FastCopyFragment(length));
        }
    }
}
