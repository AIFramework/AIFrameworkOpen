﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Fda
{
    /// <summary>
    /// Static class providing basic methods for filter design  analysis
    /// </summary>
    [Serializable]

    public static class DesignFilter
    {
        /// <summary>
        /// Method for ideal lowpass FIR filter design using sinc-window method
        /// </summary>
        /// <param name="order"></param>
        /// <param name="freq"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static double[] FirWinLp(int order, double freq, WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");

            double[] kernel = new double[order];

            int middle = order / 2;
            double freq2Pi = 2 * Math.PI * freq;

            kernel[middle] = 2 * freq;
            for (int i = 1; i <= middle; i++)
            {
                kernel[middle - i] =
                kernel[middle + i] = Math.Sin(freq2Pi * i) / (Math.PI * i);
            }

            kernel.ApplyWindow(window);

            return kernel;
        }

        /// <summary>
        /// Method for ideal highpass FIR filter design using sinc-window method
        /// </summary>
        /// <param name="order"></param>
        /// <param name="freq"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static double[] FirWinHp(int order, double freq, WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");

            double[] kernel = new double[order];

            int middle = order / 2;
            double freq2Pi = 2 * Math.PI * freq;

            kernel[middle] = 2 * (0.5 - freq);
            for (int i = 1; i <= middle; i++)
            {
                kernel[middle - i] =
                kernel[middle + i] = -Math.Sin(freq2Pi * i) / (Math.PI * i);
            }

            kernel.ApplyWindow(window);

            return kernel;
        }

        /// <summary>
        /// Method for ideal bandpass FIR filter design using sinc-window method
        /// </summary>
        /// <param name="order"></param>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static double[] FirWinBp(int order, double freq1, double freq2, WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");
            Guard.AgainstInvalidRange(freq1, freq2, "lower frequency", "upper frequency");

            double[] kernel = new double[order];

            int middle = order / 2;
            double freq12Pi = 2 * Math.PI * freq1;
            double freq22Pi = 2 * Math.PI * freq2;

            kernel[middle] = 2 * (freq2 - freq1);
            for (int i = 1; i <= middle; i++)
            {
                kernel[middle - i] =
                kernel[middle + i] = (Math.Sin(freq22Pi * i) - Math.Sin(freq12Pi * i)) / (Math.PI * i);
            }

            kernel.ApplyWindow(window);

            return kernel;
        }

        /// <summary>
        /// Method for ideal bandstop FIR filter design using sinc-window method
        /// </summary>
        /// <param name="order"></param>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static double[] FirWinBs(int order, double freq1, double freq2, WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");
            Guard.AgainstInvalidRange(freq1, freq2, "lower frequency", "upper frequency");

            double[] kernel = new double[order];

            int middle = order / 2;
            double freq12Pi = 2 * Math.PI * freq1;
            double freq22Pi = 2 * Math.PI * freq2;

            kernel[middle] = 2 * (0.5 - freq2 + freq1);
            for (int i = 1; i <= middle; i++)
            {
                kernel[middle - i] =
                kernel[middle + i] = (Math.Sin(freq12Pi * i) - Math.Sin(freq22Pi * i)) / (Math.PI * i);
            }

            kernel.ApplyWindow(window);

            return kernel;
        }

        /// <summary>
        /// Design equiripple LP FIR filter using Remez (Parks-McClellan) algorithm
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="fp">Passband edge frequency</param>
        /// <param name="fa">Stopband edge frequency</param>
        /// <param name="wp">Passband weight</param>
        /// <param name="wa">Stopband weight</param>
        /// <returns>Filter kernel</returns>
        public static double[] FirEquirippleLp(int order, double fp, double fa, double wp, double wa)
        {
            return new Remez(order, new[] { 0, fp, fa, 0.5 }, new[] { 1, 0.0 }, new[] { wp, wa }).Design();
        }

        /// <summary>
        /// Design equiripple HP FIR filter using Remez (Parks-McClellan) algorithm
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="fa">Stopband edge frequency</param>
        /// <param name="fp">Passband edge frequency</param>
        /// <param name="wa">Stopband weight</param>
        /// <param name="wp">Passband weight</param>
        /// <returns>Filter kernel</returns>
        public static double[] FirEquirippleHp(int order, double fa, double fp, double wa, double wp)
        {
            return new Remez(order, new[] { 0, fa, fp, 0.5 }, new[] { 0, 1.0 }, new[] { wa, wp }).Design();
        }

        /// <summary>
        /// Design equiripple BP FIR filter using Remez (Parks-McClellan) algorithm
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="fa1">Left stopband edge frequency</param>
        /// <param name="fp1">Passband left edge frequency</param>
        /// <param name="fp2">Passband right edge frequency</param>
        /// <param name="fa2">Right stopband edge frequency</param>
        /// <param name="wa1">Left stopband weight</param>
        /// <param name="wp">Passband weight</param>
        /// <param name="wa2">Right stopband weight</param>
        /// <returns>Filter kernel</returns>
        public static double[] FirEquirippleBp(int order, double fa1, double fp1, double fp2, double fa2, double wa1, double wp, double wa2)
        {
            return new Remez(order, new[] { 0, fa1, fp1, fp2, fa2, 0.5 }, new[] { 0, 1.0, 0 }, new[] { wa1, wp, wa2 }).Design();
        }

        /// <summary>
        /// Design equiripple BS FIR filter using Remez (Parks-McClellan) algorithm
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="fp1">Left passband edge frequency</param>
        /// <param name="fa1">Stopband left edge frequency</param>
        /// <param name="fa2">Stopband right edge frequency</param>
        /// <param name="fp2">Right passband edge frequency</param>
        /// <param name="wp1">Left passband weight</param>
        /// <param name="wa">Stopband weight</param>
        /// <param name="wp2">Right passband weight</param>
        /// <returns>Filter kernel</returns>
        public static double[] FirEquirippleBs(int order, double fp1, double fa1, double fa2, double fp2, double wp1, double wa, double wp2)
        {
            return new Remez(order, new[] { 0, fp1, fa1, fa2, fp2, 0.5 }, new[] { 1, 0.0, 1 }, new[] { wp1, wa, wp2 }).Design();
        }

        /// <summary>
        /// FIR filter design using frequency sampling method
        /// </summary>
        /// <param name="order">Filter order</param>
        /// <param name="magnitudeResponse">Magnitude response</param>
        /// <param name="phaseResponse">Phase response</param>
        /// <param name="window">Window</param>
        /// <returns>FIR filter kernel</returns>
        public static double[] Fir(int order,
                                   double[] magnitudeResponse,
                                   double[] phaseResponse = null,
                                   WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");

            int fftSize = MathUtils.NextPowerOfTwo(magnitudeResponse.Length);

            double[] real = phaseResponse == null ?
                       magnitudeResponse.PadZeros(fftSize) :
                       magnitudeResponse.Zip(phaseResponse, (m, p) => m * Math.Cos(p)).ToArray();

            double[] imag = phaseResponse == null ?
                       new double[fftSize] :
                       magnitudeResponse.Zip(phaseResponse, (m, p) => m * Math.Sin(p)).ToArray();

            Fft64 fft = new Fft64(fftSize);
            fft.Inverse(real, imag);

            double[] kernel = new double[order];

            double compensation = 2.0 / fftSize;
            int middle = order / 2;
            for (int i = 0; i <= middle; i++)
            {
                kernel[i] = real[middle - i] * compensation;
                kernel[i + middle] = real[i] * compensation;
            }

            kernel.ApplyWindow(window);

            return kernel;
        }

        /// <summary>
        /// FIR filter design using frequency sampling method
        /// </summary>
        /// <param name="order">Filter order</param>
        /// <param name="frequencyResponse">Complex frequency response</param>
        /// <param name="window">Window</param>
        /// <returns>FIR filter kernel</returns>
        public static double[] Fir(int order, ComplexDiscreteSignal frequencyResponse, WindowTypes window = WindowTypes.Blackman)
        {
            return Fir(order, frequencyResponse.Real, frequencyResponse.Imag, window);
        }

        /// <summary>
        /// FIR filter design using frequency sampling method (32-bit precision)
        /// </summary>
        /// <param name="order">Filter order</param>
        /// <param name="magnitudeResponse">Magnitude response</param>
        /// <param name="phaseResponse">Phase response</param>
        /// <param name="window">Window</param>
        /// <returns>FIR filter kernel</returns>
        public static double[] Fir(int order,
                                   float[] magnitudeResponse,
                                   float[] phaseResponse = null,
                                   WindowTypes window = WindowTypes.Blackman)
        {
            Guard.AgainstEvenNumber(order, "The order of the filter");

            int fftSize = MathUtils.NextPowerOfTwo(magnitudeResponse.Length);

            float[] real = phaseResponse == null ?
                       magnitudeResponse.PadZeros(fftSize) :
                       magnitudeResponse.Zip(phaseResponse, (m, p) => (float)(m * Math.Cos(p))).ToArray();

            float[] imag = phaseResponse == null ?
                       new float[fftSize] :
                       magnitudeResponse.Zip(phaseResponse, (m, p) => (float)(m * Math.Sin(p))).ToArray();

            Fft fft = new Fft(fftSize);
            fft.Inverse(real, imag);

            double[] kernel = new double[order];

            double compensation = 2.0 / fftSize;
            int middle = order / 2;
            for (int i = 0; i <= middle; i++)
            {
                kernel[i] = real[middle - i] * compensation;
                kernel[i + middle] = real[i] * compensation;
            }

            kernel.ApplyWindow(window);

            return kernel;
        }


        #region Convert LowPass FIR filter kernel between band forms

        /// <summary>
        /// Method for making HP filter from the linear-phase LP filter
        /// </summary>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double[] FirLpToHp(double[] kernel)
        {
            Guard.AgainstEvenNumber(kernel.Length, "The order of the filter");

            double[] kernelHp = kernel.Select(k => -k).ToArray();
            kernelHp[kernelHp.Length / 2] += 1.0;
            return kernelHp;
        }

        /// <summary>
        /// Method for making LP filter from the linear-phase HP filter
        /// (not different from FirLpToHp method)
        /// </summary>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double[] FirHpToLp(double[] kernel)
        {
            return FirLpToHp(kernel);
        }

        /// <summary>
        /// Method for making BS filter from the linear-phase BP filter
        /// (not different from FirLpToHp method)
        /// </summary>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double[] FirBpToBs(double[] kernel)
        {
            return FirLpToHp(kernel);
        }

        /// <summary>
        /// Method for making BP filter from the linear-phase BS filter
        /// (not different from FirLpToHp method)
        /// </summary>
        /// <param name="kernel"></param>
        /// <returns></returns>
        public static double[] FirBsToBp(double[] kernel)
        {
            return FirLpToHp(kernel);
        }

        #endregion


        #region design transfer functions for IIR pole filters (Butterworth, Chebyshev, etc.)


        /// <summary>
        /// Design TF for low-pass pole filter
        /// </summary>
        /// <param name="freq">Cutoff frequency in range [0, 0.5]</param>
        /// <param name="poles">Analog prototype poles</param>
        /// <param name="zeros">Analog prototype zeros</param>
        /// <returns>Transfer function</returns>
        public static TransferFunction IirLpTf(double freq, Complex[] poles, Complex[] zeros = null)
        {
            double[] pre = new double[poles.Length];
            double[] pim = new double[poles.Length];

            double warpedFreq = Math.Tan(Math.PI * freq);

            // 1) poles of analog filter (scaled)

            for (int k = 0; k < poles.Length; k++)
            {
                Complex p = warpedFreq * poles[k];
                pre[k] = p.Real;
                pim[k] = p.Imaginary;
            }

            // 2) switch to z-domain

            MathUtils.BilinearTransform(pre, pim);


            // === if zeros are also specified do the same steps 1-2 with zeros ===

            double[] zre, zim;

            if (zeros != null)
            {
                zre = new double[zeros.Length];
                zim = new double[zeros.Length];

                for (int k = 0; k < zeros.Length; k++)
                {
                    Complex z = warpedFreq * zeros[k];
                    zre[k] = z.Real;
                    zim[k] = z.Imaginary;
                }

                MathUtils.BilinearTransform(zre, zim);
            }
            // otherwise create zeros (same amount as poles) and set them all to -1
            else
            {
                zre = Enumerable.Repeat(-1.0, poles.Length).ToArray();
                zim = new double[poles.Length];
            }

            // ===



            // 3) return TF with normalized coefficients

            TransferFunction tf = new TransferFunction(new ComplexDiscreteSignal(1, zre, zim),
                                          new ComplexDiscreteSignal(1, pre, pim));
            tf.NormalizeAt(0);

            return tf;
        }

        /// <summary>
        /// Design TF for high-pass pole filter
        /// </summary>
        /// <param name="freq">Cutoff frequency in range [0, 0.5]</param>
        /// <param name="poles">Analog prototype poles</param>
        /// <param name="zeros">Analog prototype zeros</param>
        /// <returns>Transfer function</returns>
        public static TransferFunction IirHpTf(double freq, Complex[] poles, Complex[] zeros = null)
        {
            double[] pre = new double[poles.Length];
            double[] pim = new double[poles.Length];

            double warpedFreq = Math.Tan(Math.PI * freq);

            // 1) poles of analog filter (scaled)

            for (int k = 0; k < poles.Length; k++)
            {
                Complex p = warpedFreq / poles[k];
                pre[k] = p.Real;
                pim[k] = p.Imaginary;
            }

            // 2) switch to z-domain

            MathUtils.BilinearTransform(pre, pim);


            // === if zeros are also specified do the same steps 1-2 with zeros ===

            double[] zre, zim;

            if (zeros != null)
            {
                zre = new double[zeros.Length];
                zim = new double[zeros.Length];

                for (int k = 0; k < zeros.Length; k++)
                {
                    Complex z = warpedFreq / zeros[k];
                    zre[k] = z.Real;
                    zim[k] = z.Imaginary;
                }

                MathUtils.BilinearTransform(zre, zim);
            }
            // otherwise create zeros (same amount as poles) and set them all to -1
            else
            {
                zre = Enumerable.Repeat(1.0, poles.Length).ToArray();
                zim = new double[poles.Length];
            }

            // ===


            // 3) return TF with normalized coefficients

            TransferFunction tf = new TransferFunction(new ComplexDiscreteSignal(1, zre, zim),
                                          new ComplexDiscreteSignal(1, pre, pim));
            tf.NormalizeAt(Math.PI);

            return tf;
        }

        /// <summary>
        /// Design TF for band-pass pole filter
        /// </summary>
        /// <param name="freq1">Left cutoff frequency in range [0, 0.5]</param>
        /// <param name="freq2">Right cutoff frequency in range [0, 0.5]</param>
        /// <param name="poles">Analog prototype poles</param>
        /// <param name="zeros">Analog prototype zeros</param>
        /// <returns>Transfer function</returns>
        public static TransferFunction IirBpTf(double freq1, double freq2, Complex[] poles, Complex[] zeros = null)
        {
            Guard.AgainstInvalidRange(freq1, freq2, "lower frequency", "upper frequency");

            double[] pre = new double[poles.Length * 2];
            double[] pim = new double[poles.Length * 2];

            double centerFreq = 2 * Math.PI * (freq1 + freq2) / 2;

            double warpedFreq1 = Math.Tan(Math.PI * freq1);
            double warpedFreq2 = Math.Tan(Math.PI * freq2);

            double f0 = Math.Sqrt(warpedFreq1 * warpedFreq2);
            double bw = warpedFreq2 - warpedFreq1;

            // 1) poles of analog filter (scaled)

            for (int k = 0; k < poles.Length; k++)
            {
                Complex alpha = bw / 2 * poles[k];
                Complex beta = Complex.Sqrt(1 - Complex.Pow(f0 / alpha, 2));

                Complex p1 = alpha * (1 + beta);
                pre[k] = p1.Real;
                pim[k] = p1.Imaginary;

                Complex p2 = alpha * (1 - beta);
                pre[poles.Length + k] = p2.Real;
                pim[poles.Length + k] = p2.Imaginary;
            }

            // 2) switch to z-domain

            MathUtils.BilinearTransform(pre, pim);


            // === if zeros are also specified do the same steps 1-2 with zeros ===

            double[] zre, zim;

            if (zeros != null)
            {
                zre = new double[zeros.Length * 2];
                zim = new double[zeros.Length * 2];

                for (int k = 0; k < zeros.Length; k++)
                {
                    Complex alpha = bw / 2 * zeros[k];
                    Complex beta = Complex.Sqrt(1 - Complex.Pow(f0 / alpha, 2));

                    Complex z1 = alpha * (1 + beta);
                    zre[k] = z1.Real;
                    zim[k] = z1.Imaginary;

                    Complex z2 = alpha * (1 - beta);
                    zre[zeros.Length + k] = z2.Real;
                    zim[zeros.Length + k] = z2.Imaginary;
                }

                MathUtils.BilinearTransform(zre, zim);
            }
            // otherwise create zeros (same amount as poles) and set them all to [-1, -1, -1, ..., 1, 1, 1]
            else
            {
                zre = Enumerable.Repeat(-1.0, poles.Length)
                                .Concat(Enumerable.Repeat(1.0, poles.Length))
                                .ToArray();
                zim = new double[poles.Length * 2];
            }

            // ===


            // 3) return TF with normalized coefficients

            TransferFunction tf = new TransferFunction(new ComplexDiscreteSignal(1, zre, zim),
                                          new ComplexDiscreteSignal(1, pre, pim));
            tf.NormalizeAt(centerFreq);

            return tf;
        }

        /// <summary>
        /// Design TF for band-reject pole filter
        /// </summary>
        /// <param name="freq1">Left cutoff frequency in range [0, 0.5]</param>
        /// <param name="freq2">Right cutoff frequency in range [0, 0.5]</param>
        /// <param name="poles">Analog prototype poles</param>
        /// <param name="zeros">Analog prototype zeros</param>
        /// <returns>Transfer function</returns>
        public static TransferFunction IirBsTf(double freq1, double freq2, Complex[] poles, Complex[] zeros = null)
        {
            Guard.AgainstInvalidRange(freq1, freq2, "lower frequency", "upper frequency");

            // Calculation of filter coefficients is based on Neil Robertson's post:
            // https://www.dsprelated.com/showarticle/1131.php

            double[] pre = new double[poles.Length * 2];
            double[] pim = new double[poles.Length * 2];

            double f1 = Math.Tan(Math.PI * freq1);
            double f2 = Math.Tan(Math.PI * freq2);
            double f0 = Math.Sqrt(f1 * f2);
            double bw = f2 - f1;

            double centerFreq = 2 * Math.Atan(f0);


            // 1) poles and zeros of analog filter (scaled)

            for (int k = 0; k < poles.Length; k++)
            {
                Complex alpha = bw / 2 / poles[k];
                Complex beta = Complex.Sqrt(1 - Complex.Pow(f0 / alpha, 2));

                Complex p1 = alpha * (1 + beta);
                pre[k] = p1.Real;
                pim[k] = p1.Imaginary;

                Complex p2 = alpha * (1 - beta);
                pre[poles.Length + k] = p2.Real;
                pim[poles.Length + k] = p2.Imaginary;
            }

            // 2) switch to z-domain

            MathUtils.BilinearTransform(pre, pim);


            // === if zeros are also specified do the same steps 1-2 with zeros ===

            double[] zre, zim;

            if (zeros != null)
            {
                zre = new double[zeros.Length * 2];
                zim = new double[zeros.Length * 2];

                for (int k = 0; k < zeros.Length; k++)
                {
                    Complex alpha = bw / 2 / zeros[k];
                    Complex beta = Complex.Sqrt(1 - Complex.Pow(f0 / alpha, 2));

                    Complex z1 = alpha * (1 + beta);
                    zre[k] = z1.Real;
                    zim[k] = z1.Imaginary;

                    Complex z2 = alpha * (1 - beta);
                    zre[zeros.Length + k] = z2.Real;
                    zim[zeros.Length + k] = z2.Imaginary;
                }

                MathUtils.BilinearTransform(zre, zim);
            }
            // otherwise create zeros (same amount as poles) and set the following values:
            else
            {
                zre = new double[poles.Length * 2];
                zim = new double[poles.Length * 2];

                for (int k = 0; k < poles.Length; k++)
                {
                    zre[k] = Math.Cos(centerFreq);
                    zim[k] = Math.Sin(centerFreq);
                    zre[poles.Length + k] = Math.Cos(-centerFreq);
                    zim[poles.Length + k] = Math.Sin(-centerFreq);
                }
            }

            // ===


            // 3) return TF with normalized coefficients

            TransferFunction tf = new TransferFunction(new ComplexDiscreteSignal(1, zre, zim),
                                          new ComplexDiscreteSignal(1, pre, pim));
            tf.NormalizeAt(0);

            return tf;
        }

        #endregion


        #region second order sections

        /// <summary>
        /// Second-order sections to zpk.
        /// </summary>
        /// <param name="sos"></param>
        /// <returns></returns>
        public static TransferFunction SosToTf(TransferFunction[] sos)
        {
            return sos.Aggregate((tf, s) => tf * s);
        }

        /// <summary>
        /// Zpk to second-order sections.
        /// </summary>
        /// <param name="tf">Transfer function</param>
        /// <returns>Array of SOS transfer functions</returns>
        public static TransferFunction[] TfToSos(TransferFunction tf)
        {
            List<Complex> zeros = tf.Zeros.ToComplexNumbers().ToList();
            List<Complex> poles = tf.Poles.ToComplexNumbers().ToList();

            if (zeros.Count != poles.Count)
            {
                if (zeros.Count > poles.Count)
                {
                    poles.AddRange(new Complex[zeros.Count - poles.Count]);
                }

                if (zeros.Count < poles.Count)
                {
                    zeros.AddRange(new Complex[poles.Count - zeros.Count]);
                }
            }

            int sosCount = (poles.Count + 1) / 2;

            if (poles.Count % 2 == 1)
            {
                zeros.Add(Complex.Zero);
                poles.Add(Complex.Zero);
            }

            RemoveConjugated(zeros);
            RemoveConjugated(poles);

            double[] gains = new double[sosCount];
            gains[0] = tf.Gain;
            for (int i = 1; i < gains.Length; i++)
            {
                gains[i] = 1;
            }

            TransferFunction[] sos = new TransferFunction[sosCount];

            // reverse order of sections

            for (int i = sosCount - 1; i >= 0; i--)
            {
                Complex z1, z2, p1, p2;

                // Select the next pole closest to unit circle

                int pos = ClosestToUnitCircle(poles, Any);
                p1 = poles[pos];
                poles.RemoveAt(pos);

                if (IsReal(p1) && poles.All(IsComplex))
                {
                    pos = ClosestToComplexValue(zeros, p1, IsReal);     // closest to pole p1
                    z1 = zeros[pos];
                    zeros.RemoveAt(pos);

                    p2 = Complex.Zero;
                    z2 = Complex.Zero;
                }
                else
                {
                    if (IsComplex(p1) && zeros.Count(IsReal) == 1)
                    {
                        pos = ClosestToComplexValue(zeros, p1, IsComplex);
                    }
                    else
                    {
                        pos = ClosestToComplexValue(zeros, p1, Any);
                    }

                    z1 = zeros[pos];
                    zeros.RemoveAt(pos);

                    if (IsComplex(p1))
                    {
                        p2 = Complex.Conjugate(p1);

                        if (IsComplex(z1))
                        {
                            z2 = Complex.Conjugate(z1);
                        }
                        else
                        {
                            pos = ClosestToComplexValue(zeros, p1, IsReal);
                            z2 = zeros[pos];
                            zeros.RemoveAt(pos);
                        }
                    }
                    else
                    {
                        if (IsComplex(z1))
                        {
                            z2 = Complex.Conjugate(z1);

                            pos = ClosestToComplexValue(poles, z1, IsReal);
                            p2 = poles[pos];
                            poles.RemoveAt(pos);
                        }
                        else
                        {
                            pos = ClosestToUnitCircle(poles, IsReal);
                            p2 = poles[pos];
                            poles.RemoveAt(pos);

                            pos = ClosestToComplexValue(zeros, p2, IsReal);
                            z2 = zeros[pos];
                            zeros.RemoveAt(pos);
                        }
                    }
                }

                ComplexDiscreteSignal zs = new ComplexDiscreteSignal(1, new[] { z1.Real, z2.Real }, new[] { z1.Imaginary, z2.Imaginary });
                ComplexDiscreteSignal ps = new ComplexDiscreteSignal(1, new[] { p1.Real, p2.Real }, new[] { p1.Imaginary, p2.Imaginary });

                sos[i] = new TransferFunction(zs, ps, gains[i]);
            }

            return sos;
        }

        private static readonly Func<Complex, bool> Any = c => true;
        private static readonly Func<Complex, bool> IsReal = c => Math.Abs(c.Imaginary) < 1e-10;
        private static readonly Func<Complex, bool> IsComplex = c => Math.Abs(c.Imaginary) > 1e-10;

        private static int ClosestToComplexValue(List<Complex> arr, Complex value, Func<Complex, bool> condition)
        {
            int pos = 0;
            double minDistance = double.MaxValue;

            for (int i = 0; i < arr.Count; i++)
            {
                if (!condition(arr[i]))
                {
                    continue;
                }

                double distance = Complex.Abs(arr[i] - value);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    pos = i;
                }
            }

            return pos;
        }

        private static int ClosestToUnitCircle(List<Complex> arr, Func<Complex, bool> condition)
        {
            int pos = 0;
            double minDistance = double.MaxValue;

            for (int i = 0; i < arr.Count; i++)
            {
                if (!condition(arr[i]))
                {
                    continue;
                }

                double distance = Complex.Abs(Complex.Abs(arr[i]) - 1.0);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    pos = i;
                }
            }

            return pos;
        }

        /// <summary>
        /// Leave only one of two conjugated numbers in the list of complex numbers
        /// </summary>
        /// <param name="c"></param>
        private static void RemoveConjugated(List<Complex> c)
        {
            for (int i = 0; i < c.Count; i++)
            {
                if (IsReal(c[i]))
                {
                    continue;
                }

                int j = i + 1;
                for (; j < c.Count; j++)
                {
                    if (Math.Abs(c[i].Real - c[j].Real) < 1e-10 &&
                        Math.Abs(c[i].Imaginary + c[j].Imaginary) < 1e-10)
                    {
                        break;
                    }
                }

                if (j == c.Count)
                {
                    throw new ArgumentException($"Complex array does not contain conjugated pair for {c[i]}");
                }

                c.RemoveAt(j);
            }
        }

        #endregion
    }
}
