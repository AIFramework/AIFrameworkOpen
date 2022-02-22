using System;

namespace AI.BackEnds.DSP.NWaves.Features
{
    /// <summary>
    /// Harmonic features
    /// </summary>
    public static class Harmonic
    {
        /// <summary>
        /// Simple algorithm for detecting harmonic peaks in spectrum
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Array for peak positions</param>
        /// <param name="peakFrequencies">Array for peak frequencies</param>
        /// <param name="samplingRate">Sampling rate</param>
        /// <param name="pitch">Pitch is given if it is known</param>
        public static void Peaks(float[] spectrum, int[] peaks, float[] peakFrequencies, int samplingRate, float pitch = -1)
        {
            if (pitch < 0)
            {
                pitch = Pitch.FromSpectralPeaks(spectrum, samplingRate);
            }

            float resolution = (float)samplingRate / (2 * (spectrum.Length - 1));

            int region = (int)(pitch / (2 * resolution));

            peaks[0] = (int)(pitch / resolution);
            peakFrequencies[0] = pitch;

            for (int i = 0; i < peaks.Length; i++)
            {
                int candidate = (i + 1) * peaks[0];

                if (candidate >= spectrum.Length)
                {
                    peaks[i] = spectrum.Length - 1;
                    peakFrequencies[i] = resolution * (spectrum.Length - 1);
                    continue;
                }

                int c = candidate;
                for (int j = -region; j < region; j++)
                {
                    if (c + j - 1 > 0 &&
                        c + j + 1 < spectrum.Length &&
                        spectrum[c + j] > spectrum[c + j - 1] &&
                        spectrum[c + j] > spectrum[c + j + 1] &&
                        spectrum[c + j] > spectrum[candidate])
                    {
                        candidate = c + j;
                    }
                }

                peaks[i] = candidate;
                peakFrequencies[i] = resolution * candidate;
            }
        }

        /// <summary>
        /// Harmonic centroid
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Peak positions</param>
        /// <param name="peakFrequencies">Peak frequencies</param>
        /// <returns>Harmonic centroid</returns>
        public static float Centroid(float[] spectrum, int[] peaks, float[] peakFrequencies)
        {
            if (peaks[0] == 0)
            {
                return 0;
            }

            float sum = 1e-10f;
            float weightedSum = 0.0f;

            for (int i = 0; i < peaks.Length; i++)
            {
                int p = peaks[i];
                sum += spectrum[p];
                weightedSum += peakFrequencies[i] * spectrum[p];
            }

            return weightedSum / sum;
        }

        /// <summary>
        /// Harmonic spread
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Peak positions</param>
        /// <param name="peakFrequencies">Peak frequencies</param>
        /// <returns>Harmonic spread</returns>
        public static float Spread(float[] spectrum, int[] peaks, float[] peakFrequencies)
        {
            if (peaks[0] == 0)
            {
                return 0;
            }

            float centroid = Centroid(spectrum, peaks, peakFrequencies);

            float sum = 1e-10f;
            float weightedSum = 0.0f;

            for (int i = 0; i < peaks.Length; i++)
            {
                int p = peaks[i];
                sum += spectrum[p];
                weightedSum += spectrum[p] * (peakFrequencies[i] - centroid) * (peakFrequencies[i] - centroid);
            }

            return (float)Math.Sqrt(weightedSum / sum);
        }

        /// <summary>
        /// Inharmonicity
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Peak positions</param>
        /// <param name="peakFrequencies">Peak frequencies</param>
        /// <returns>Inharmonicity</returns>
        public static float Inharmonicity(float[] spectrum, int[] peaks, float[] peakFrequencies)
        {
            if (peaks[0] == 0)
            {
                return 0;
            }

            float f0 = peakFrequencies[0];

            float squaredSum = 1e-10f;
            float sum = 0.0f;

            for (int i = 0; i < peaks.Length; i++)
            {
                int p = peaks[i];
                float sqr = spectrum[p] * spectrum[p];

                sum += (peakFrequencies[i] - (i + 1) * f0) * sqr;
                squaredSum += sqr;
            }

            return 2 * sum / (f0 * squaredSum);
        }

        /// <summary>
        /// Harmonic Odd-to-Even Ratio
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Peak positions</param>
        /// <returns>Odd-to-Even Ratio</returns>
        public static float OddToEvenRatio(float[] spectrum, int[] peaks)
        {
            if (peaks[0] == 0)
            {
                return 0;
            }

            float oddSum = 1e-10f;
            float evenSum = 1e-10f;

            for (int i = 0; i < peaks.Length; i += 2)
            {
                evenSum += spectrum[peaks[i]];
            }

            for (int i = 1; i < peaks.Length; i += 2)
            {
                oddSum += spectrum[peaks[i]];
            }

            return oddSum / evenSum;
        }

        /// <summary>
        /// Tristimulus (nth component)
        /// </summary>
        /// <param name="spectrum">Spectrum</param>
        /// <param name="peaks">Peak positions</param>
        /// <param name="n">Tristimulus component: 1, 2 or 3</param>
        /// <returns>Tristimulus</returns>
        public static float Tristimulus(float[] spectrum, int[] peaks, int n)
        {
            if (peaks[0] == 0)
            {
                return 0;
            }

            float sum = 1e-10f;

            for (int i = 0; i < peaks.Length; i++)
            {
                sum += spectrum[peaks[i]];
            }

            if (n == 1)
            {
                return spectrum[peaks[0]] / sum;
            }
            else if (n == 2)
            {
                return (spectrum[peaks[1]] + spectrum[peaks[2]] + spectrum[peaks[3]]) / sum;
            }
            else
            {
                return (sum - spectrum[peaks[0]] - spectrum[peaks[1]] - spectrum[peaks[2]] - spectrum[peaks[3]]) / sum;
            }
        }
    }
}
