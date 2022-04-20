﻿using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Signals
{
    /// <summary>
    /// Any finite complex DT signal is simply two arrays of data (real and imaginary parts)
    /// sampled at certain sampling rate.
    /// 
    /// This arrays of samples can be:
    ///     - delayed (shifted) by positive or negative number of samples
    ///     - superimposed with another arrays of samples (another signal)
    ///     - concatenated with another arrays of samples (another signal)
    ///     - repeated N times
    /// 
    /// Note.
    /// Method implementations are LINQ-less and do Buffer.BlockCopy() for better performance.
    /// </summary>
    public static class ComplexDiscreteSignalExtensions
    {
        /// Method delays the signal
        ///     either by shifting it to the right (positive, e.g. Delay(1000))
        ///         or by shifting it to the left (negative, e.g. Delay(-1000))
        /// <param name="signal"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Delay(this ComplexDiscreteSignal signal, int delay)
        {
            int length = signal.Length;

            if (delay <= 0)
            {
                delay = -delay;

                Guard.AgainstInvalidRange(delay, length, "Delay", "signal length");

                return new ComplexDiscreteSignal(
                                signal.SamplingRate,
                                signal.Real.FastCopyFragment(length - delay, delay),
                                signal.Imag.FastCopyFragment(length - delay, delay));
            }

            return new ComplexDiscreteSignal(
                            signal.SamplingRate,
                            signal.Real.FastCopyFragment(length, destinationOffset: delay),
                            signal.Imag.FastCopyFragment(length, destinationOffset: delay));
        }

        /// <summary>
        /// Method superimposes two signals.
        /// If sizes are different then the smaller signal is broadcasted 
        /// to fit the size of the larger signal.
        /// </summary>
        /// <param name="signal1">Object signal</param>
        /// <param name="signal2">Argument signal</param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Superimpose(this ComplexDiscreteSignal signal1, ComplexDiscreteSignal signal2)
        {
            Guard.AgainstInequality(signal1.SamplingRate, signal2.SamplingRate,
                                        "Sampling rate of signal1", "sampling rate of signal2");

            ComplexDiscreteSignal superimposed;

            if (signal1.Length > signal2.Length)
            {
                superimposed = signal1.Copy();

                for (int i = 0; i < signal2.Length; i++)
                {
                    superimposed.Real[i] += signal2.Real[i];
                    superimposed.Imag[i] += signal2.Imag[i];
                }
            }
            else
            {
                superimposed = signal2.Copy();

                for (int i = 0; i < signal1.Length; i++)
                {
                    superimposed.Real[i] += signal1.Real[i];
                    superimposed.Imag[i] += signal1.Imag[i];
                }
            }

            return superimposed;
        }

        /// <summary>
        /// Method concatenates two signals.
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Concatenate(this ComplexDiscreteSignal signal1, ComplexDiscreteSignal signal2)
        {
            Guard.AgainstInequality(signal1.SamplingRate, signal2.SamplingRate,
                                        "Sampling rate of signal1", "sampling rate of signal2");

            return new ComplexDiscreteSignal(
                            signal1.SamplingRate,
                            signal1.Real.MergeWithArray(signal2.Real),
                            signal1.Imag.MergeWithArray(signal2.Imag));
        }

        /// <summary>
        /// Method returns repeated n times copy of the signal
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Repeat(this ComplexDiscreteSignal signal, int times)
        {
            Guard.AgainstNonPositive(times, "Number of repeat times");

            return new ComplexDiscreteSignal(
                            signal.SamplingRate,
                            signal.Real.RepeatArray(times),
                            signal.Imag.RepeatArray(times));
        }

        /// <summary>
        /// In-place signal amplification by coeff
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="coeff"></param>
        public static void Amplify(this ComplexDiscreteSignal signal, double coeff)
        {
            for (int i = 0; i < signal.Length; i++)
            {
                signal.Real[i] *= coeff;
                signal.Imag[i] *= coeff;
            }
        }

        /// <summary>
        /// In-place signal attenuation by coeff
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="coeff"></param>
        public static void Attenuate(this ComplexDiscreteSignal signal, double coeff)
        {
            Guard.AgainstNonPositive(coeff, "Attenuation coefficient");

            signal.Amplify(1 / coeff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal First(this ComplexDiscreteSignal signal, int sampleCount)
        {
            Guard.AgainstNonPositive(sampleCount, "Number of samples");
            Guard.AgainstExceedance(sampleCount, signal.Length, "Number of samples", "signal length");

            return new ComplexDiscreteSignal(
                            signal.SamplingRate,
                            signal.Real.FastCopyFragment(sampleCount),
                            signal.Imag.FastCopyFragment(sampleCount));
        }

        /// <summary>
        /// More or less efficient LINQ-less version.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Last(this ComplexDiscreteSignal signal, int sampleCount)
        {
            Guard.AgainstNonPositive(sampleCount, "Number of samples");
            Guard.AgainstExceedance(sampleCount, signal.Length, "Number of samples", "signal length");

            return new ComplexDiscreteSignal(
                            signal.SamplingRate,
                            signal.Real.FastCopyFragment(sampleCount, signal.Length - sampleCount),
                            signal.Imag.FastCopyFragment(sampleCount, signal.Imag.Length - sampleCount));
        }

        /// <summary>
        /// Method creates new zero-padded complex discrete signal from the current signal.
        /// </summary>
        /// <param name="signal">Signal</param>
        /// <param name="newLength">The length of a zero-padded signal.
        /// By default array is zero-padded to have length of next power of 2.</param>
        /// <returns>Zero padded complex discrete signal</returns>
        public static ComplexDiscreteSignal ZeroPadded(this ComplexDiscreteSignal signal, int newLength)
        {
            if (newLength <= 0)
            {
                newLength = MathUtils.NextPowerOfTwo(signal.Length);
            }

            return new ComplexDiscreteSignal(
                            signal.SamplingRate,
                            signal.Real.PadZeros(newLength),
                            signal.Imag.PadZeros(newLength));
        }

        /// <summary>
        /// Method performs the complex multiplication of two signals
        /// (with normalization by length)
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Multiply(
            this ComplexDiscreteSignal signal1, ComplexDiscreteSignal signal2)
        {
            Guard.AgainstInequality(signal1.SamplingRate, signal2.SamplingRate,
                                        "Sampling rate of signal1", "sampling rate of signal2");

            int length = signal1.Length;

            double[] real = new double[length];
            double[] imag = new double[length];

            double[] real1 = signal1.Real;
            double[] imag1 = signal1.Imag;
            double[] real2 = signal2.Real;
            double[] imag2 = signal2.Imag;

            for (int i = 0; i < length; i++)
            {
                real[i] = real1[i] * real2[i] - imag1[i] * imag2[i];
                imag[i] = real1[i] * imag2[i] + imag1[i] * real2[i];
            }

            return new ComplexDiscreteSignal(signal1.SamplingRate, real, imag);
        }

        /// <summary>
        /// Method performs the complex division of two signals
        /// (with normalization by length)
        /// </summary>
        /// <param name="signal1"></param>
        /// <param name="signal2"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal Divide(
            this ComplexDiscreteSignal signal1, ComplexDiscreteSignal signal2)
        {
            Guard.AgainstInequality(signal1.SamplingRate, signal2.SamplingRate,
                                        "Sampling rate of signal1", "sampling rate of signal2");

            int length = signal1.Length;

            double[] real = new double[length];
            double[] imag = new double[length];

            double[] real1 = signal1.Real;
            double[] imag1 = signal1.Imag;
            double[] real2 = signal2.Real;
            double[] imag2 = signal2.Imag;

            for (int i = 0; i < length; i++)
            {
                double den = imag1[i] * imag1[i] + imag2[i] * imag2[i];
                real[i] = (real1[i] * real2[i] + imag1[i] * imag2[i]) / den;
                imag[i] = (real2[i] * imag1[i] - imag2[i] * real1[i]) / den;
            }

            return new ComplexDiscreteSignal(signal1.SamplingRate, real, imag);
        }

        /// <summary>
        /// Just another way for calling Unwrap() function
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static double[] Unwrap(this double[] phase, double tolerance = Math.PI)
        {
            return MathUtils.Unwrap(phase, tolerance);
        }

        /// <summary>
        /// Magnitude of complex numbers given in tuple of float arrays (re and im)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public static float[] Magnitude(this Tuple<float[], float[]> signal)
        {
            Tuple<float[], float[]> tuple = signal;

            float[] real = tuple.Item1, imag = tuple.Item2;

            float[] magnitude = new float[real.Length];

            for (int i = 0; i < magnitude.Length; i++)
            {
                magnitude[i] = (float)Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
            }

            return magnitude;
        }

        /// <summary>
        /// Phase of complex numbers given in tuple of float arrays (re and im)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public static float[] Phase(this Tuple<float[], float[]> signal)
        {
            Tuple<float[], float[]> tuple = signal;

            float[] real = tuple.Item1, imag = tuple.Item2;

            float[] magnitude = new float[real.Length];

            for (int i = 0; i < magnitude.Length; i++)
            {
                magnitude[i] = (float)Math.Atan2(imag[i], real[i]);
            }

            return magnitude;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public static IEnumerable<Complex> ToComplexNumbers(this ComplexDiscreteSignal signal)
        {
            for (int i = 0; i < signal.Length; i++)
            {
                yield return new Complex(signal.Real[i], signal.Imag[i]);
            }
        }
    }
}
