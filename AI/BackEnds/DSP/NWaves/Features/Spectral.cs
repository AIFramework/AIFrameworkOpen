using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Features
{
    /// <summary>
    /// Spectral features
    /// </summary>
    public static class Spectral
    {
        /// <summary>
        /// Spectral centroid
        /// </summary>
        /// <param name="spectrum">Magnitude spectrum</param>
        /// <param name="frequencies">Centre frequencies</param>
        /// <returns>Spectral centroid</returns>
        public static float Centroid(float[] spectrum, float[] frequencies)
        {
            float sum = 1e-10f;
            float weightedSum = 0.0f;

            for (int i = 1; i < spectrum.Length; i++)
            {
                sum += spectrum[i];
                weightedSum += frequencies[i] * spectrum[i];
            }

            return weightedSum / sum;
        }

        /// <summary>
        /// Spectral spread
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="frequencies"></param>
        /// <returns></returns>
        public static float Spread(float[] spectrum, float[] frequencies)
        {
            float centroid = Centroid(spectrum, frequencies);

            float sum = 1e-10f;
            float weightedSum = 0.0f;

            for (int i = 1; i < spectrum.Length; i++)
            {
                sum += spectrum[i];
                weightedSum += spectrum[i] * (frequencies[i] - centroid) * (frequencies[i] - centroid);
            }

            return (float)Math.Sqrt(weightedSum / sum);
        }

        /// <summary>
        /// Spectral dicrease
        /// </summary>
        /// <param name="spectrum"></param>
        /// <returns></returns>
        public static float Decrease(float[] spectrum)
        {
            float sum = 1e-10f;
            float diffSum = 0.0f;

            for (int i = 2; i < spectrum.Length; i++)
            {
                sum += spectrum[i];
                diffSum += (spectrum[i] - spectrum[1]) / (i - 1);
            }

            return diffSum / sum;
        }

        /// <summary>
        /// Spectral flatness
        /// </summary>
        /// <param name="spectrum">Magnitude spectrum</param>
        /// <param name="minLevel"></param>
        /// <returns></returns>
        public static float Flatness(float[] spectrum, float minLevel = 1e-10f)
        {
            float sum = 0.0f;
            double logSum = 0.0;

            for (int i = 1; i < spectrum.Length; i++)
            {
                float amp = Math.Max(spectrum[i], minLevel);

                sum += amp;
                logSum += Math.Log(amp);
            }

            sum /= spectrum.Length;
            logSum /= spectrum.Length;

            return sum > 1e-10 ? (float)Math.Exp(logSum) / sum : 0.0f;
        }

        /// <summary>
        /// Spectral noiseness
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="frequencies"></param>
        /// <param name="noiseFrequency"></param>
        /// <returns></returns>
        public static float Noiseness(float[] spectrum, float[] frequencies, float noiseFrequency = 3000)
        {
            float noiseSum = 0.0f;
            float totalSum = 1e-10f;

            int i = 1;
            for (; i < spectrum.Length && frequencies[i] < noiseFrequency; i++)
            {
                totalSum += spectrum[i];
            }

            for (; i < spectrum.Length; i++)
            {
                noiseSum += spectrum[i];
                totalSum += spectrum[i];
            }

            return noiseSum / totalSum;
        }

        /// <summary>
        /// Spectral rolloff frequency
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="frequencies">Centre frequencies</param>
        /// <param name="rolloffPercent"></param>
        /// <returns></returns>
        public static float Rolloff(float[] spectrum, float[] frequencies, float rolloffPercent = 0.85f)
        {
            float threshold = 0.0f;
            for (int i = 1; i < spectrum.Length; i++)
            {
                threshold += spectrum[i];
            }

            threshold *= rolloffPercent;

            float cumulativeSum = 0.0f;
            int index = 0;
            for (int i = 1; i < spectrum.Length; i++)
            {
                cumulativeSum += spectrum[i];

                if (cumulativeSum > threshold)
                {
                    index = i;
                    break;
                }
            }

            return frequencies[index];
        }

        /// <summary>
        /// Spectral crest
        /// </summary>
        /// <param name="spectrum"></param>
        /// <returns></returns>
        public static float Crest(float[] spectrum)
        {
            float sum = 0.0f;
            float max = 0.0f;

            for (int i = 1; i < spectrum.Length; i++)
            {
                float s = spectrum[i] * spectrum[i];

                sum += s;

                if (s > max)
                {
                    max = s;
                }
            }

            return sum > 1e-10 ? spectrum.Length * max / sum : 1.0f;
        }

        /// <summary>
        /// Spectral contrast (array of *bandCount* values)
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="frequencies"></param>
        /// <param name="minFrequency"></param>
        /// <param name="bandCount"></param>
        /// <returns></returns>
        public static float[] Contrast(float[] spectrum, float[] frequencies, float minFrequency = 200, int bandCount = 6)
        {
            const double alpha = 0.02;

            float[] contrasts = new float[bandCount];

            float octaveLow = minFrequency;
            float octaveHigh = 2 * octaveLow;

            for (int n = 0; n < bandCount; n++)
            {
                float[] bandSpectrum = spectrum.Where((s, i) => frequencies[i] >= octaveLow && frequencies[i] <= octaveHigh)
                                           .OrderBy(s => s)
                                           .ToArray();

                if (bandSpectrum.Length == 0)
                {
                    return contrasts;   // zeros
                }

                double selectedCount = Math.Max(alpha * bandSpectrum.Length, 1);

                double avgPeaks = 0.0;
                double avgValleys = 0.0;

                for (int i = 0; i < selectedCount; i++)
                {
                    avgValleys += bandSpectrum[i];
                    avgPeaks += bandSpectrum[bandSpectrum.Length - i - 1];
                }

                avgPeaks /= selectedCount;
                avgValleys /= selectedCount;

                contrasts[n] = (float)Math.Log10(avgPeaks / avgValleys);

                octaveLow *= 2;
                octaveHigh *= 2;
            }

            return contrasts;
        }

        /// <summary>
        /// Spectral contrast in one particular spectral band (#bandNo).
        /// This function is called from SpectralFeatureExtractor.
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="frequencies"></param>
        /// <param name="bandNo"></param>
        /// <param name="minFrequency"></param>
        /// <returns></returns>
        public static float Contrast(float[] spectrum, float[] frequencies, int bandNo, float minFrequency = 200)
        {
            const double alpha = 0.02;

            double octaveLow = minFrequency * Math.Pow(2, bandNo - 1);
            double octaveHigh = 2 * octaveLow;

            float[] bandSpectrum = spectrum.Where((s, i) => frequencies[i] >= octaveLow && frequencies[i] <= octaveHigh)
                                       .OrderBy(s => s)
                                       .ToArray();

            if (bandSpectrum.Length == 0)
            {
                return 0;
            }

            double selectedCount = Math.Max(alpha * bandSpectrum.Length, 1);

            double avgPeaks = 0.0;
            double avgValleys = 0.0;

            for (int i = 0; i < selectedCount; i++)
            {
                avgValleys += bandSpectrum[i];
                avgPeaks += bandSpectrum[bandSpectrum.Length - i - 1];
            }

            avgPeaks /= selectedCount;
            avgValleys /= selectedCount;

            return (float)Math.Log10(avgPeaks / avgValleys);
        }

        /// <summary>
        /// Shannon entropy of a spectrum (spectrum is treated as p.d.f.)
        /// </summary>
        public static float Entropy(float[] spectrum)
        {
            double entropy = 0.0;

            float sum = spectrum.Sum();

            if (sum < 1e-8)
            {
                return 0;
            }

            for (int i = 1; i < spectrum.Length; i++)
            {
                float p = spectrum[i] / sum;

                if (p > 1e-8)
                {
                    entropy += p * Math.Log(p, 2);
                }
            }

            return (float)(-entropy / Math.Log(spectrum.Length, 2));
        }
    }
}
