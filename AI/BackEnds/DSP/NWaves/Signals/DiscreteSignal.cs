﻿using System;
using System.Collections.Generic;
using System.Linq;
using AI.BackEnds.DSP.NWaves.Utils;

namespace AI.BackEnds.DSP.NWaves.Signals
{
    /// <summary>
    /// Base class for finite real-valued discrete-time signals.
    /// 
    /// In general, any finite DT signal is simply an array of data sampled at certain sampling rate.
    /// 
    /// See also DiscreteSignalExtensions for additional functionality of DT signals.
    /// 
    /// Note. 
    /// Method implementations are LINQ-less for better performance.
    /// 
    /// In the earliest versions of NWaves there was also an ISignal interface, however it was refactored out.
    /// </summary>
    public class DiscreteSignal
    {
        /// <summary>
        /// Number of samples per unit of time (1 second)
        /// </summary>
        public int SamplingRate { get; }

        /// <summary>
        /// Real-valued array of samples
        /// </summary>
        public float[] Samples { get; }

        /// <summary>
        /// Length of the signal
        /// </summary>
        public int Length => Samples.Length;

        /// <summary>
        /// Duration of the signal (in sec)
        /// </summary>
        public double Duration => (double)Samples.Length / SamplingRate;

        /// <summary>
        /// The most efficient constructor for initializing discrete signals
        /// </summary>
        /// <param name="samplingRate">Sampling rate</param>
        /// <param name="samples">Array of samples</param>
        /// <param name="allocateNew">Set to true if new memory should be allocated for data</param>
        public DiscreteSignal(int samplingRate, float[] samples, bool allocateNew = false)
        {
            Guard.AgainstNonPositive(samplingRate, "Sampling rate");

            SamplingRate = samplingRate;
            Samples = allocateNew ? samples.FastCopy() : samples;
        }

        /// <summary>
        /// Constructor for creating a signal from collection of samples
        /// </summary>
        /// <param name="samplingRate">Sampling rate</param>
        /// <param name="samples">Collection of samples</param>
        public DiscreteSignal(int samplingRate, IEnumerable<float> samples)
            : this(samplingRate, samples?.ToArray())
        {
        }

        /// <summary>
        /// Constructor for creating a signal of specified length filled with specified values
        /// </summary>
        /// <param name="samplingRate">Sampling rate</param>
        /// <param name="length">Number of samples</param>
        /// <param name="value">Value of each sample</param>
        public DiscreteSignal(int samplingRate, int length, float value = 0.0f)
        {
            Guard.AgainstNonPositive(samplingRate, "Sampling rate");

            SamplingRate = samplingRate;

            var samples = new float[length];
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i] = value;
            }

            Samples = samples;
        }

        /// <summary>
        /// Constructor for creating a signal from collection of integer samples
        /// </summary>
        /// <param name="samplingRate">Sampling rate</param>
        /// <param name="samples">Collection of integer samples</param>
        /// <param name="normalizeFactor">Some normalization coefficient</param>
        public DiscreteSignal(int samplingRate, IEnumerable<int> samples, float normalizeFactor = 1.0f)
        {
            Guard.AgainstNonPositive(samplingRate, "Sampling rate");

            SamplingRate = samplingRate;
            
            var intSamples = samples.ToArray();
            var floatSamples = new float[intSamples.Length];
            for (var i = 0; i < intSamples.Length; i++)
            {
                floatSamples[i] = intSamples[i] / normalizeFactor;
            }

            Samples = floatSamples;
        }

        /// <summary>
        /// Method for creating deep copy of the signal
        /// </summary>
        /// <returns>Copy of the signal</returns>
        public DiscreteSignal Copy()
        {
            return new DiscreteSignal(SamplingRate, Samples, true);
        }

        /// <summary>
        /// Sample indexer
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Sample by index</returns>
        public float this[int index]
        {
            get => Samples[index];
            set => Samples[index] = value;
        }

        /// <summary>
        /// Slice the signal (Python-style)
        /// 
        ///     var middle = signal[900, 1200];
        /// 
        /// Implementaion is LINQ-less, since Skip() would be less efficient:
        ///     return new DiscreteSignal(SamplingRate, Samples.Skip(startPos).Take(endPos - startPos));
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns>Slice of the signal</returns>
        /// <exception>Overflow possible if endPos is less than startPos</exception>
        public DiscreteSignal this[int startPos, int endPos]
        {
            get
            {
                Guard.AgainstInvalidRange(startPos, endPos, "Left index", "Right index");

                return new DiscreteSignal(SamplingRate, Samples.FastCopyFragment(endPos - startPos, startPos));
            }
        }

        #region overloaded operators

        /// <summary>
        /// Overloaded binary plus (superimpose signals)
        /// </summary>
        /// <param name="s1">Left signal</param>
        /// <param name="s2">Right signal</param>
        /// <returns>Superimposed signal</returns>
        public static DiscreteSignal operator +(DiscreteSignal s1, DiscreteSignal s2)
        {
            return s1.Superimpose(s2);
        }

        /// <summary>
        /// Overloaded unary minus (negated signal)
        /// </summary>
        /// <param name="s">Signal</param>
        /// <returns>Negated signal</returns>
        public static DiscreteSignal operator -(DiscreteSignal s)
        {
            return new DiscreteSignal(s.SamplingRate, s.Samples.Select(x => -x));
        }

        /// <summary>
        /// Overloaded binary minus (difference signal)
        /// </summary>
        /// <param name="s1">Left signal</param>
        /// <param name="s2">Right signal</param>
        /// <returns>Difference signal</returns>
        public static DiscreteSignal operator -(DiscreteSignal s1, DiscreteSignal s2)
        {
            return s1.Subtract(s2);
        }

        /// <summary>
        /// Overloaded + (add constant)
        /// </summary>
        /// <param name="s">Signal</param>
        /// <param name="constant">Constant to add to each sample</param>
        /// <returns>Modified signal</returns>
        public static DiscreteSignal operator +(DiscreteSignal s, float constant)
        {
            return new DiscreteSignal(s.SamplingRate, s.Samples.Select(x => x + constant));
        }

        /// <summary>
        /// Overloaded - (subtract constant)
        /// </summary>
        /// <param name="s">Signal</param>
        /// <param name="constant">Constant to subtract from each sample</param>
        /// <returns>Modified signal</returns>
        public static DiscreteSignal operator -(DiscreteSignal s, float constant)
        {
            return new DiscreteSignal(s.SamplingRate, s.Samples.Select(x => x - constant));
        }

        /// <summary>
        /// Overloaded * (signal amplification/attenuation)
        /// </summary>
        /// <param name="s">Signal</param>
        /// <param name="coeff">Amplification coefficient</param>
        /// <returns>Amplified signal</returns>
        public static DiscreteSignal operator *(DiscreteSignal s, float coeff)
        {
            var signal = s.Copy();
            signal.Amplify(coeff);
            return signal;
        }

        #endregion

        #region time-domain characteristics

        /// <summary>
        /// Energy of a signal fragment
        /// </summary>
        /// <param name="startPos">Starting sample</param>
        /// <param name="endPos">Ending sample (exclusive)</param>
        /// <returns>Energy</returns>
        public float Energy(int startPos, int endPos)
        {
            var total = 0.0f;
            for (var i = startPos; i < endPos; i++)
            {
                total += Samples[i] * Samples[i];
            }

            return total / (endPos - startPos);
        }

        /// <summary>
        /// Energy of entire signal
        /// </summary>
        /// <returns>Energy</returns>
        public float Energy() => Energy(0, Length);

        /// <summary>
        /// RMS of a signal fragment
        /// </summary>
        /// <param name="startPos">Starting sample</param>
        /// <param name="endPos">Ending sample (exclusive)</param>
        /// <returns>RMS</returns>
        public float Rms(int startPos, int endPos)
        {
            return (float)Math.Sqrt(Energy(startPos, endPos));
        }

        /// <summary>
        /// RMS of entire signal
        /// </summary>
        /// <returns>RMS</returns>
        public float Rms() => (float)Math.Sqrt(Energy(0, Length));

        /// <summary>
        /// Zero-crossing rate of a signal fragment
        /// </summary>
        /// <param name="startPos">Starting sample</param>
        /// <param name="endPos">Ending sample (exclusive)</param>
        /// <returns>Zero-crossing rate</returns>
        public float ZeroCrossingRate(int startPos, int endPos)
        {
            const float disbalance = 1e-4f;

            var prevSample = Samples[startPos] + disbalance;

            var rate = 0;
            for (var i = startPos + 1; i < endPos; i++)
            {
                var sample = Samples[i] + disbalance;

                if ((sample >= 0) != (prevSample >= 0))
                {
                    rate++;
                }

                prevSample = sample;
            }

            return (float)rate / (endPos - startPos - 1);
        }

        /// <summary>
        /// Zero-crossing rate of entire signal
        /// </summary>
        /// <returns>Zero-crossing rate</returns>
        public float ZeroCrossingRate() => ZeroCrossingRate(0, Length);

        /// <summary>
        /// Shannon entropy of a signal fragment
        /// (computed from bins distributed uniformly between the minimum and maximum values of samples)
        /// </summary>
        /// <param name="startPos">Starting sample</param>
        /// <param name="endPos">Ending sample (exclusive)</param>
        /// <returns>Shannon entropy</returns>
        public float Entropy(int startPos, int endPos, int binCount = 32)
        {
            var len = endPos - startPos;

            if (len < binCount)
            {
                binCount = len;
            }

            var bins = new int[binCount+1];

            var min = Samples[0];
            var max = Samples[0];
            for (var i = startPos; i < endPos; i++)
            {
                var sample = Math.Abs(Samples[i]);

                if (sample < min)
                {
                    min = sample;
                }
                if (sample > max)
                {
                    max = sample;
                }
            }

            if (max - min < 1e-8f)
            {
                return 0;
            }

            var binLength = (max - min) / binCount;

            for (var i = startPos; i < endPos; i++)
            {
                bins[(int)((Math.Abs(Samples[i]) - min) / binLength)]++;
            }

            var entropy = 0.0;
            for (var i = 0; i < binCount; i++)
            {
                var p = (float) bins[i] / (endPos - startPos);

                if (p > 1e-8f)
                {
                    entropy += p * Math.Log(p, 2);
                }
            }

            return (float)(-entropy / Math.Log(binCount, 2));
        }

        /// <summary>
        /// Entropy of entire signal
        /// </summary>
        /// <returns>Entropy</returns>
        public float Entropy() => Entropy(0, Length);

        #endregion
    }
}
