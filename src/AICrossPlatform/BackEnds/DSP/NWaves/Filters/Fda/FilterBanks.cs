﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.BiQuad;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Fda
{
    /// <summary>
    /// Static class with methods providing general shapes of filter banks:
    /// 
    ///     - triangular
    ///     - rectangular
    ///     - FIR bandpass (close to trapezoidal, slightly overlapping)
    ///     - BiQuad bandpass
    /// 
    /// ...and methods for obtaining the most widely used frequency bands:
    /// 
    ///     - Herz bands
    ///     - Mel bands (HTK and Slaney)
    ///     - Bark bands (uniform and Slaney)
    ///     - Critical bands
    ///     - ERB filterbank
    ///     - Octaves (from MPEG-7)
    /// 
    /// </summary>
    [Serializable]

    public static class FilterBanks
    {
        /// <summary>
        /// Method returns universal triangular filterbank weights based on given frequencies.
        /// </summary>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="frequencies">Array of frequency tuples (left, center, right) for each filter</param>
        /// <param name="vtln">VTLN frequency warper</param>
        /// <param name="mapper">Frequency scale mapper (e.g. herz-to-mel) used here only for proper weighting</param>
        /// <returns>Array of triangular filters</returns>
        public static float[][] Triangular(int fftSize,
                                           int samplingRate,
                                           Tuple<double, double, double>[] frequencies,
                                           VtlnWarper vtln = null,
                                           Func<double, double> mapper = null)
        {
            if (mapper == null)
            {
                mapper = x => x;
            }

            Func<double, double> warp = vtln == null ? mapper : x => mapper(vtln.Warp(x));

            double herzResolution = (double)samplingRate / fftSize;

            double[] herzFrequencies = Enumerable.Range(0, (fftSize / 2) + 1)
                                            .Select(f => f * herzResolution)
                                            .ToArray();

            int filterCount = frequencies.Length;
            float[][] filterBank = new float[filterCount][];

            for (int i = 0; i < filterCount; i++)
            {
                filterBank[i] = new float[(fftSize / 2) + 1];

                Tuple<double, double, double> tuple = frequencies[i];

                double left = tuple.Item1, center = tuple.Item2, right = tuple.Item3;

                left = warp(left);
                center = warp(center);
                right = warp(right);

                int j = 0;
                for (; mapper(herzFrequencies[j]) <= left; j++)
                {
                    ;
                }

                for (; mapper(herzFrequencies[j]) <= center; j++)
                {
                    filterBank[i][j] = (float)((mapper(herzFrequencies[j]) - left) / (center - left));
                }
                for (; j < herzFrequencies.Length && mapper(herzFrequencies[j]) < right; j++)
                {
                    filterBank[i][j] = (float)((right - mapper(herzFrequencies[j])) / (right - center));
                }
            }

            return filterBank;
        }

        /// <summary>
        /// Method returns universal rectangular filterbank weights based on given frequencies.
        /// </summary>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="frequencies">Array of frequency tuples (left, center, right) for each filter</param>
        /// <param name="vtln">VTLN frequency warper</param>
        /// <param name="mapper">Frequency scale mapper (e.g. herz-to-mel)</param>
        /// <returns>Array of rectangular filters</returns>
        public static float[][] Rectangular(int fftSize,
                                           int samplingRate,
                                           Tuple<double, double, double>[] frequencies,
                                           VtlnWarper vtln = null,
                                           Func<double, double> mapper = null)
        {
            if (mapper == null)
            {
                mapper = x => x;
            }

            Func<double, double> warp = vtln == null ? mapper : x => mapper(vtln.Warp(x));

            double herzResolution = (double)samplingRate / fftSize;

            double[] herzFrequencies = Enumerable.Range(0, (fftSize / 2) + 1)
                                            .Select(f => f * herzResolution)
                                            .ToArray();

            int filterCount = frequencies.Length;
            float[][] filterBank = new float[filterCount][];

            for (int i = 0; i < filterCount; i++)
            {
                filterBank[i] = new float[(fftSize / 2) + 1];

                Tuple<double, double, double> tuple = frequencies[i];

                double left = tuple.Item1, center = tuple.Item2, right = tuple.Item3;


                left = warp(left);
                center = warp(center);
                right = warp(right);

                int j = 0;
                for (; mapper(herzFrequencies[j]) <= left; j++)
                {
                    ;
                }

                for (; j < herzFrequencies.Length && mapper(herzFrequencies[j]) < right; j++)
                {
                    filterBank[i][j] = 1;
                }
            }

            return filterBank;
        }

        /// <summary>
        /// Method returns FIR bandpass (close to trapezoidal) filterbank based on given frequencies.
        /// </summary>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="frequencies">Array of frequency tuples (left, center, right) for each filter</param>
        /// <param name="vtln">VTLN frequency warper</param>
        /// <param name="mapper">Frequency scale mapper (e.g. herz-to-mel)</param>
        /// <returns>Array of rectangular filters</returns>
        public static float[][] Trapezoidal(int fftSize,
                                           int samplingRate,
                                           Tuple<double, double, double>[] frequencies,
                                           VtlnWarper vtln = null,
                                           Func<double, double> mapper = null)
        {
            float[][] filterBank = Rectangular(fftSize, samplingRate, frequencies, vtln, mapper);

            for (int i = 0; i < filterBank.Length; i++)
            {
                TransferFunction filterTf = new TransferFunction(DesignFilter.Fir((fftSize / 4) + 1, filterBank[i]));

                filterBank[i] = filterTf.FrequencyResponse(fftSize).Magnitude.ToFloats();

                // normalize gain to 1.0

                float maxAmp = 0.0f;
                for (int j = 0; j < filterBank[i].Length; j++)
                {
                    if (filterBank[i][j] > maxAmp)
                    {
                        maxAmp = filterBank[i][j];
                    }
                }
                for (int j = 0; j < filterBank[i].Length; j++)
                {
                    filterBank[i][j] /= maxAmp;
                }
            }

            return filterBank;
        }

        /// <summary>
        /// Method returns BiQuad bandpass overlapping filters based on given frequencies.
        /// </summary>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="frequencies">Array of frequency tuples (left, center, right) for each filter</param>
        /// <returns>Array of BiQuad bandpass filters</returns>
        public static float[][] BiQuad(int fftSize, int samplingRate, Tuple<double, double, double>[] frequencies)
        {
            double[] center = frequencies.Select(f => f.Item2).ToArray();

            int filterCount = frequencies.Length;
            float[][] filterBank = new float[filterCount][];

            for (int i = 0; i < filterCount; i++)
            {
                double freq = center[i] / samplingRate;
                BandPassFilter filter = new BandPassFilter(freq, 2.0);

                filterBank[i] = filter.Tf.FrequencyResponse(fftSize).Magnitude.ToFloats();
            }

            return filterBank;
        }

        /// <summary>
        /// This general method returns frequency tuples for uniformly spaced frequency bands on any scale.
        /// </summary>
        /// <param name="scaleMapper">The function that converts Hz to other frequency scale</param>
        /// <param name="inverseMapper">The function that converts frequency from alternative scale back to Hz</param>
        /// <param name="filterCount">Число фильтров</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each filter</returns>
        private static Tuple<double, double, double>[] UniformBands(
                                                     Func<double, double> scaleMapper,
                                                     Func<double, double> inverseMapper,
                                                     int filterCount,
                                                     int samplingRate,
                                                     double lowFreq = 0,
                                                     double highFreq = 0,
                                                     bool overlap = true)
        {
            if (lowFreq < 0)
            {
                lowFreq = 0;
            }
            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            double startingFrequency = scaleMapper(lowFreq);

            Tuple<double, double, double>[] frequencyTuples = new Tuple<double, double, double>[filterCount];

            if (overlap)
            {
                double newResolution = (scaleMapper(highFreq) - scaleMapper(lowFreq)) / (filterCount + 1);

                double[] frequencies = Enumerable.Range(0, filterCount + 2)
                                            .Select(i => inverseMapper(startingFrequency + (i * newResolution)))
                                            .ToArray();

                for (int i = 0; i < filterCount; i++)
                {
                    frequencyTuples[i] = new Tuple<double, double, double>(frequencies[i], frequencies[i + 1], frequencies[i + 2]);
                }
            }
            else
            {
                double newResolution = (scaleMapper(highFreq) - scaleMapper(lowFreq)) / filterCount;

                double[] frequencies = Enumerable.Range(0, filterCount + 1)
                                            .Select(i => inverseMapper(startingFrequency + (i * newResolution)))
                                            .ToArray();

                for (int i = 0; i < filterCount; i++)
                {
                    frequencyTuples[i] = new Tuple<double, double, double>(frequencies[i], frequencies[i + 1], frequencies[i + 2]);
                }
            }

            return frequencyTuples;
        }

        /// <summary>
        /// Method returns frequency tuples for uniformly spaced frequency bands on Herz scale.
        /// </summary>
        /// <param name="combFilterCount">Число фильтров</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each Herz filter</returns>
        public static Tuple<double, double, double>[] HerzBands(
            int combFilterCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = false)
        {
            // "x => x" means map frequency 1-to-1 (in Hz as it is)
            return UniformBands(x => x, x => x, combFilterCount, samplingRate, lowFreq, highFreq, overlap);
        }

        /// <summary>
        /// Method returns frequency tuples for uniformly spaced frequency bands on Mel scale.
        /// </summary>
        /// <param name="melFilterCount">Number of mel filters to create</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each Mel filter</returns>
        public static Tuple<double, double, double>[] MelBands(
            int melFilterCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = true)
        {
            return UniformBands(Scale.HerzToMel, Scale.MelToHerz, melFilterCount, samplingRate, lowFreq, highFreq, overlap);
        }

        /// <summary>
        /// Method returns frequency tuples for uniformly spaced frequency bands on Mel scale
        /// (according to M.Slaney's formula).
        /// </summary>
        /// <param name="melFilterCount">Number of mel filters to create</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each Mel filter</returns>
        public static Tuple<double, double, double>[] MelBandsSlaney(
            int melFilterCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = true)
        {
            return UniformBands(Scale.HerzToMelSlaney, Scale.MelToHerzSlaney, melFilterCount, samplingRate, lowFreq, highFreq, overlap);
        }

        /// <summary>
        /// Method returns frequency tuples for uniformly spaced frequency bands on Bark scale (Traunmueller, 1990).
        /// </summary>
        /// <param name="barkFilterCount">Number of bark filters to create</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each Bark filter</returns>
        public static Tuple<double, double, double>[] BarkBands(
            int barkFilterCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = true)
        {
            return UniformBands(Scale.HerzToBark, Scale.BarkToHerz, barkFilterCount, samplingRate, lowFreq, highFreq, overlap);
        }

        /// <summary>
        /// Method returns frequency tuples for uniformly spaced frequency bands on Bark scale (Wang, 1992).
        /// </summary>
        /// <param name="barkFilterCount">Number of bark filters to create</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each Bark filter</returns>
        public static Tuple<double, double, double>[] BarkBandsSlaney(
            int barkFilterCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = true)
        {
            return UniformBands(Scale.HerzToBarkSlaney, Scale.BarkToHerzSlaney, barkFilterCount, samplingRate, lowFreq, highFreq, overlap);
        }

        /// <summary>
        /// Method returns frequency tuples for critical bands.
        /// </summary>
        /// <param name="filterCount">Число фильтров</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <returns>Array of frequency tuples for each Critical Band filter</returns>
        public static Tuple<double, double, double>[] CriticalBands(
            int filterCount, int samplingRate, double lowFreq = 0, double highFreq = 0)
        {
            if (lowFreq < 0)
            {
                lowFreq = 0;
            }
            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            double[] edgeFrequencies = { 20,   100,  200,  300,  400,  510,  630,  770,  920,  1080, 1270,  1480,  1720,
                                         2000, 2320, 2700, 3150, 3700, 4400, 5300, 6400, 7700, 9500, 12000, 15500, 20500 };

            double[] centerFrequencies = { 50,   150,  250,  350,  450,  570,  700,  840,  1000, 1170, 1370,  1600,
                                           1850, 2150, 2500, 2900, 3400, 4000, 4800, 5800, 7000, 8500, 10500, 13500, 17500 };

            int startIndex = 0;
            for (int i = 0; i < centerFrequencies.Length; i++)
            {
                if (centerFrequencies[i] < lowFreq)
                {
                    continue;
                }

                startIndex = i;
                break;
            }

            int endIndex = 0;
            for (int i = centerFrequencies.Length - 1; i >= 0; i--)
            {
                if (centerFrequencies[i] > highFreq)
                {
                    continue;
                }

                endIndex = i;
                break;
            }

            filterCount = Math.Min(endIndex - startIndex + 1, filterCount);

            double[] edges = edgeFrequencies.Skip(startIndex)
                                       .Take(filterCount + 1)
                                       .ToArray();

            double[] centers = centerFrequencies.Skip(startIndex)
                                           .Take(filterCount)
                                           .ToArray();

            Tuple<double, double, double>[] frequencyTuples = new Tuple<double, double, double>[filterCount];

            for (int i = 0; i < filterCount; i++)
            {
                frequencyTuples[i] = new Tuple<double, double, double>(edges[i], centers[i], edges[i + 1]);
            }

            return frequencyTuples;
        }

        /// <summary>
        /// Method returns frequency tuples for octave bands.
        /// </summary>
        /// <param name="octaveCount">Number of octave filters to create</param>
        /// <param name="samplingRate">Assumed Частота дискретизации of a signal</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="overlap">Flag indicating that bands should overlap</param>
        /// <returns>Array of frequency tuples for each octave filter</returns>
        public static Tuple<double, double, double>[] OctaveBands(
            int octaveCount, int samplingRate, double lowFreq = 0, double highFreq = 0, bool overlap = false)
        {
            if (lowFreq < 1e-10)
            {
                lowFreq = 62.5;//Hz
            }

            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            double f1 = lowFreq;
            double f2 = lowFreq * 2;

            List<Tuple<double, double, double>> frequencyTuples = new List<Tuple<double, double, double>>();

            if (overlap)
            {
                double f3 = f2 * 2;

                for (int i = 0; i < octaveCount && f3 < highFreq; i++)
                {
                    frequencyTuples.Add(new Tuple<double, double, double>(f1, f2, f3));
                    f1 = f2;
                    f2 = f3;
                    f3 *= 2;
                }
            }
            else
            {
                for (int i = 0; i < octaveCount && f2 < highFreq; i++)
                {
                    frequencyTuples.Add(new Tuple<double, double, double>(f1, (f1 + f2) / 2, f2));
                    f1 *= 2;
                    f2 *= 2;
                }
            }

            return frequencyTuples.ToArray();
        }

        /// <summary>
        /// Method creates overlapping triangular mel filters (as suggested by Malcolm Slaney).
        /// </summary>
        /// <param name="filterCount">Number of mel filters</param>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="normalizeGain">True if gain should be normalized; false if all filters should have same height 1.0</param>
        /// <param name="vtln">VTLN frequency warper</param>
        /// <returns>Array of mel filters</returns>
        public static float[][] MelBankSlaney(
            int filterCount, int fftSize, int samplingRate, double lowFreq = 0, double highFreq = 0, bool normalizeGain = true, VtlnWarper vtln = null)
        {
            if (lowFreq < 0)
            {
                lowFreq = 0;
            }
            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            Tuple<double, double, double>[] frequencies = UniformBands(Scale.HerzToMelSlaney, Scale.MelToHerzSlaney, filterCount, samplingRate, lowFreq, highFreq, true);

            float[][] filterBank = Triangular(fftSize, samplingRate, frequencies, vtln);

            if (normalizeGain)
            {
                Normalize(filterCount, frequencies, filterBank);
            }

            return filterBank;
        }

        /// <summary>
        /// Method creates overlapping trapezoidal bark filters (as suggested by Malcolm Slaney).
        /// </summary>
        /// <param name="filterCount"></param>
        /// <param name="fftSize"></param>
        /// <param name="samplingRate"></param>
        /// <param name="lowFreq"></param>
        /// <param name="highFreq"></param>
        /// <param name="width">Constant width of each band in Bark</param>
        /// <returns></returns>
        public static float[][] BarkBankSlaney(
            int filterCount, int fftSize, int samplingRate, double lowFreq = 0, double highFreq = 0, double width = 1)
        {
            if (lowFreq < 0)
            {
                lowFreq = 0;
            }
            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            double lowBark = Scale.HerzToBarkSlaney(lowFreq);
            double highBark = Scale.HerzToBarkSlaney(highFreq) - lowBark;

            double herzResolution = (double)samplingRate / fftSize;
            double step = highBark / (filterCount - 1);

            double[] binBarks = Enumerable.Range(0, (fftSize / 2) + 1)
                                     .Select(i => Scale.HerzToBarkSlaney(i * herzResolution))
                                     .ToArray();

            float[][] filterBank = new float[filterCount][];

            double midBark = lowBark;

            for (int i = 0; i < filterCount; i++, midBark += step)
            {
                filterBank[i] = new float[(fftSize / 2) + 1];

                for (int j = 0; j < filterBank[i].Length; j++)
                {
                    double lof = binBarks[j] - midBark - 0.5;
                    double hif = binBarks[j] - midBark + 0.5;

                    filterBank[i][j] = (float)Math.Pow(10, Math.Min(0, Math.Min(hif, -2.5 * lof) / width));
                }
            }

            return filterBank;
        }

        /// <summary>
        /// Method creates overlapping ERB filters (ported from Malcolm Slaney's MATLAB code).
        /// </summary>
        /// <param name="erbFilterCount">Number of ERB filters</param>
        /// <param name="fftSize">Assumed size of FFT</param>
        /// <param name="samplingRate">Assumed Частота дискретизации</param>
        /// <param name="lowFreq">Lower bound of the frequency range</param>
        /// <param name="highFreq">Upper bound of the frequency range</param>
        /// <param name="normalizeGain">True if gain should be normalized; false if all filters should have same height 1.0</param>
        /// <returns>Array of ERB filters</returns>
        public static float[][] Erb(
            int erbFilterCount, int fftSize, int samplingRate, double lowFreq = 0, double highFreq = 0, bool normalizeGain = true)
        {
            if (lowFreq < 0)
            {
                lowFreq = 0;
            }
            if (highFreq <= lowFreq)
            {
                highFreq = samplingRate / 2.0;
            }

            const double earQ = 9.26449;
            const double minBw = 24.7;
            const double bw = earQ * minBw;
            const int order = 1;

            double t = 1.0 / samplingRate;

            double[] frequencies = new double[erbFilterCount];
            for (int i = 1; i <= erbFilterCount; i++)
            {
                frequencies[erbFilterCount - i] =
                    -bw + (Math.Exp(i * (-Math.Log(highFreq + bw) + Math.Log(lowFreq + bw)) / erbFilterCount) * (highFreq + bw));
            }

            Complex[] ucirc = new Complex[(fftSize / 2) + 1];
            for (int i = 0; i < ucirc.Length; i++)
            {
                ucirc[i] = Complex.Exp(2 * Complex.ImaginaryOne * i * Math.PI / fftSize);
            }

            double rootPos = Math.Sqrt(3 + Math.Pow(2, 1.5));
            double rootNeg = Math.Sqrt(3 - Math.Pow(2, 1.5));

            Fft fft = new Fft(fftSize);

            float[][] erbFilterBank = new float[erbFilterCount][];

            for (int i = 0; i < erbFilterCount; i++)
            {
                double cf = frequencies[i];
                double erb = Math.Pow(Math.Pow(cf / earQ, order) + Math.Pow(minBw, order), 1.0 / order);
                double b = 1.019 * 2 * Math.PI * erb;

                double theta = 2 * cf * Math.PI * t;
                Complex itheta = Complex.Exp(2 * Complex.ImaginaryOne * theta);

                double a0 = t;
                double a2 = 0.0;
                double b0 = 1.0;
                double b1 = -2 * Math.Cos(theta) / Math.Exp(b * t);
                double b2 = Math.Exp(-2 * b * t);

                double common = -t * Math.Exp(-b * t);

                double k1 = Math.Cos(theta) + (rootPos * Math.Sin(theta));
                double k2 = Math.Cos(theta) - (rootPos * Math.Sin(theta));
                double k3 = Math.Cos(theta) + (rootNeg * Math.Sin(theta));
                double k4 = Math.Cos(theta) - (rootNeg * Math.Sin(theta));

                double a11 = common * k1;
                double a12 = common * k2;
                double a13 = common * k3;
                double a14 = common * k4;

                Complex gainArg = Complex.Exp((Complex.ImaginaryOne * theta) - (b * t));

                float gain = (float)Complex.Abs(
                                    (itheta - (gainArg * k1)) *
                                    (itheta - (gainArg * k2)) *
                                    (itheta - (gainArg * k3)) *
                                    (itheta - (gainArg * k4)) *
                                    Complex.Pow(t * Math.Exp(b * t) / ((-1.0 / Math.Exp(b * t)) + 1 + (itheta * (1 - Math.Exp(b * t)))), 4.0));

                IirFilter filter1 = new IirFilter(new[] { a0, a11, a2 }, new[] { b0, b1, b2 });
                IirFilter filter2 = new IirFilter(new[] { a0, a12, a2 }, new[] { b0, b1, b2 });
                IirFilter filter3 = new IirFilter(new[] { a0, a13, a2 }, new[] { b0, b1, b2 });
                IirFilter filter4 = new IirFilter(new[] { a0, a14, a2 }, new[] { b0, b1, b2 });

                DiscreteSignal ir = new DiscreteSignal(1, fftSize);
                ir[0] = 1.0f;

                FilterChain chain = new FilterChain(new[] { filter1, filter2, filter3, filter4 });

                DiscreteSignal kernel = chain.ApplyTo(ir);
                kernel.Attenuate(gain);

                erbFilterBank[i] = fft.PowerSpectrum(kernel, false).Samples;
            }

            // normalize gain (by default)

            if (!normalizeGain)
            {
                return erbFilterBank;
            }

            foreach (float[] filter in erbFilterBank)
            {
                double sum = 0.0;
                for (int j = 0; j < filter.Length; j++)
                {
                    sum += Math.Abs(filter[j] * filter[j]);
                }

                double weight = Math.Sqrt(sum * samplingRate / fftSize);

                for (int j = 0; j < filter.Length; j++)
                {
                    filter[j] = (float)(filter[j] / weight);
                }
            }

            return erbFilterBank;
        }

        /// <summary>
        /// Normalize weights (so that energies in each band are approx. equal)
        /// </summary>
        /// <param name="filterCount"></param>
        /// <param name="frequencies"></param>
        /// <param name="filterBank"></param>
        public static void Normalize(int filterCount, Tuple<double, double, double>[] frequencies, float[][] filterBank)
        {
            for (int i = 0; i < filterCount; i++)
            {
                Tuple<double, double, double> tuple = frequencies[i];

                double left = tuple.Item1, right = tuple.Item2;

                for (int j = 0; j < filterBank[i].Length; j++)
                {
                    filterBank[i][j] *= 2 / (float)(right - left);
                }
            }
        }

        /// <summary>
        /// Method applies filters to spectrum and fills resulting filtered spectrum.
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrum"></param>
        /// <param name="filtered"></param>
        public static void Apply(float[][] filterbank, float[] spectrum, float[] filtered)
        {
            for (int i = 0; i < filterbank.Length; i++)
            {
                float en = 0.0f;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    en += filterbank[i][j] * spectrum[j];
                }

                filtered[i] = en;
            }
        }

        /// <summary>
        /// Method applies filters to sequence of spectra
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrogram"></param>
        public static float[][] Apply(float[][] filterbank, IList<float[]> spectrogram)
        {
            float[][] filtered = new float[spectrogram.Count][];

            for (int k = 0; k < filtered.Length; k++)
            {
                filtered[k] = new float[filterbank.Length];
            }

            for (int i = 0; i < filterbank.Length; i++)
            {
                for (int k = 0; k < filtered.Length; k++)
                {
                    float en = 0.0f;

                    for (int j = 0; j < spectrogram[i].Length; j++)
                    {
                        en += filterbank[i][j] * spectrogram[k][j];
                    }

                    filtered[k][i] = en;
                }
            }

            return filtered;
        }

        /// <summary>
        /// Method applies filters to spectrum and then does Ln() on resulting spectrum.
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrum"></param>
        /// <param name="filtered"></param>
        /// <param name="floor">log floor</param>
        public static void ApplyAndLog(float[][] filterbank, float[] spectrum, float[] filtered, float floor = float.Epsilon)
        {
            for (int i = 0; i < filterbank.Length; i++)
            {
                float en = 0.0f;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    en += filterbank[i][j] * spectrum[j];
                }

                filtered[i] = (float)Math.Log(Math.Max(en, floor));
            }
        }

        /// <summary>
        /// Method applies filters to spectrum and then does Log10() on resulting spectrum.
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrum"></param>
        /// <param name="filtered"></param>
        /// <param name="floor">log floor</param>
        public static void ApplyAndLog10(float[][] filterbank, float[] spectrum, float[] filtered, float floor = float.Epsilon)
        {
            for (int i = 0; i < filterbank.Length; i++)
            {
                float en = 0.0f;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    en += filterbank[i][j] * spectrum[j];
                }

                filtered[i] = (float)Math.Log10(Math.Max(en, floor));
            }
        }

        /// <summary>
        /// Method applies filters to spectrum and then does 10*Log10() on resulting spectrum
        /// (added to compare MFCC coefficients with librosa results)
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrum"></param>
        /// <param name="filtered"></param>
        /// <param name="minLevel"></param>
        public static void ApplyAndToDecibel(float[][] filterbank, float[] spectrum, float[] filtered, float minLevel = 1e-10f)
        {
            for (int i = 0; i < filterbank.Length; i++)
            {
                float en = 0.0f;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    en += filterbank[i][j] * spectrum[j];
                }

                filtered[i] = (float)Scale.ToDecibelPower(Math.Max(en, minLevel));
            }
        }

        /// <summary>
        /// Method applies filters to spectrum and then does Pow(x, power) on resulting spectrum.
        /// In PLP: power=1/3 (cubic root).
        /// </summary>
        /// <param name="filterbank"></param>
        /// <param name="spectrum"></param>
        /// <param name="filtered"></param>
        /// <param name="power"></param>
        public static void ApplyAndPow(float[][] filterbank, float[] spectrum, float[] filtered, double power = 1.0 / 3)
        {
            for (int i = 0; i < filterbank.Length; i++)
            {
                float en = 0.0f;

                for (int j = 0; j < spectrum.Length; j++)
                {
                    en += filterbank[i][j] * spectrum[j];
                }

                filtered[i] = (float)Math.Pow(en, power);
            }
        }
    }
}
