using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;

namespace AI.BackEnds.DSP.NWaves.Features
{
    /// <summary>
    /// Class for pitch estimation and tracking
    /// </summary>
    public static class Pitch
    {
        #region time-domain methods

        /// <summary>
        /// Pitch estimation by autocorrelation method
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="samplingRate"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static float FromAutoCorrelation(float[] signal,
                                                int samplingRate,
                                                int startPos = 0,
                                                int endPos = -1,
                                                float low = 80,
                                                float high = 400)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            int pitch1 = (int)(1.0 * samplingRate / high);    // 2,5 ms = 400Hz
            int pitch2 = (int)(1.0 * samplingRate / low);     // 12,5 ms = 80Hz

            float[] block = new DiscreteSignal(samplingRate, signal)[startPos, endPos].Samples;

            int fftSize = MathUtils.NextPowerOfTwo(2 * block.Length - 1);

            float[] cc = new float[fftSize];

            new Convolver(fftSize).CrossCorrelate(block, block.FastCopy(), cc);

            int start = pitch1 + block.Length - 1;
            int end = Math.Min(start + pitch2, cc.Length);

            float max = start < cc.Length ? cc[start] : 0;

            int peakIndex = start;
            for (int k = start; k < end; k++)
            {
                if (cc[k] > max)
                {
                    max = cc[k];
                    peakIndex = k - block.Length + 1;
                }
            }

            return max > 1.0f ? (float)samplingRate / peakIndex : 0;
        }

        /// <summary>
        /// Pitch estimation by autocorrelation method (overloaded for DiscreteSignal)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static float FromAutoCorrelation(DiscreteSignal signal,
                                                int startPos = 0,
                                                int endPos = -1,
                                                float low = 80,
                                                float high = 400)
        {
            return FromAutoCorrelation(signal.Samples, signal.SamplingRate, startPos, endPos, low, high);
        }

        /// <summary>
        /// Pitch estimation from zero crossing rate (based on Schmitt trigger)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="samplingRate"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="lowSchmittThreshold"></param>
        /// <param name="highSchmittThreshold"></param>
        /// <returns></returns>
        public static float FromZeroCrossingsSchmitt(float[] signal,
                                                     int samplingRate,
                                                     int startPos = 0,
                                                     int endPos = -1,
                                                     float lowSchmittThreshold = -1e10f,
                                                     float highSchmittThreshold = 1e10f)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            float maxPositive = 0.0f;
            float minNegative = 0.0f;

            for (int i = startPos; i < endPos; i++)
            {
                if (signal[i] > 0 && signal[i] > maxPositive)
                {
                    maxPositive = signal[i];
                }

                if (signal[i] < 0 && signal[i] < minNegative)
                {
                    minNegative = signal[i];
                }
            }

            float highThreshold = highSchmittThreshold < 1e9f ? highSchmittThreshold : 0.75f * maxPositive;
            float lowThreshold = lowSchmittThreshold > -1e9f ? lowSchmittThreshold : 0.75f * minNegative;

            int zcr = 0;
            int firstCrossed = endPos;
            int lastCrossed = startPos;

            // Schmitt trigger:

            bool isCurrentHigh = false;

            int j = startPos;
            for (; j < endPos - 1; j++)
            {
                if (signal[j] < highThreshold && signal[j + 1] >= highThreshold && !isCurrentHigh)
                {
                    isCurrentHigh = true;
                    firstCrossed = j;
                    break;
                }
                if (signal[j] > lowThreshold && signal[j + 1] <= lowThreshold && isCurrentHigh)
                {
                    isCurrentHigh = false;
                    firstCrossed = j;
                    break;
                }
            }

            for (; j < endPos - 1; j++)
            {
                if (signal[j] < highThreshold && signal[j + 1] >= highThreshold && !isCurrentHigh)
                {
                    zcr++;
                    isCurrentHigh = true;
                    lastCrossed = j;
                }
                if (signal[j] > lowThreshold && signal[j + 1] <= lowThreshold && isCurrentHigh)
                {
                    zcr++;
                    isCurrentHigh = false;
                    lastCrossed = j;
                }
            }

            return zcr > 0 && lastCrossed > firstCrossed ? (float)zcr * samplingRate / 2 / (lastCrossed - firstCrossed) : 0;
        }

        /// <summary>
        /// Pitch estimation from zero crossing rate (overloaded for DiscreteSignal)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="lowSchmittThreshold"></param>
        /// <param name="highSchmittThreshold"></param>
        /// <returns></returns>
        public static float FromZeroCrossingsSchmitt(DiscreteSignal signal,
                                                     int startPos = 0,
                                                     int endPos = -1,
                                                     float lowSchmittThreshold = -1e10f,
                                                     float highSchmittThreshold = 1e10f)
        {
            return FromZeroCrossingsSchmitt(signal.Samples,
                                            signal.SamplingRate,
                                            startPos,
                                            endPos,
                                            lowSchmittThreshold,
                                            highSchmittThreshold);
        }

        /// <summary>
        /// YIN algorithm for pitch estimation
        /// De Cheveigné, A., Kawahara, H. YIN, a fundamental frequency estimator for speech and music.
        /// The Journal of the Acoustical Society of America, 111(4). - 2002.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="samplingRate"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="cmdfThreshold"></param>
        /// <returns></returns>
        public static float FromYin(float[] signal,
                                    int samplingRate,
                                    int startPos = 0,
                                    int endPos = -1,
                                    float low = 80,
                                    float high = 400,
                                    float cmdfThreshold = 0.25f)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            int pitch1 = (int)(1.0 * samplingRate / high);    // 2,5 ms = 400Hz
            int pitch2 = (int)(1.0 * samplingRate / low);     // 12,5 ms = 80Hz

            int length = (endPos - startPos) / 2;

            // cumulative mean difference function (CMDF):

            float[] cmdf = new float[length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    float diff = signal[j + startPos] - signal[i + j + startPos];
                    cmdf[i] += diff * diff;
                }
            }

            cmdf[0] = 1;

            float sum = 0.0f;
            for (int i = 1; i < length; i++)
            {
                sum += cmdf[i];
                cmdf[i] *= i / sum;
            }

            // adjust t according to some threshold:

            int t = pitch1;     // focusing on range [pitch1 .. pitch2]

            for (; t < pitch2; t++)
            {
                if (cmdf[t] < cmdfThreshold)
                {
                    while (t + 1 < pitch2 && cmdf[t + 1] < cmdf[t])
                    {
                        t++;
                    }

                    break;
                }
            }

            // no pitch

            if (t == pitch2 || cmdf[t] >= cmdfThreshold)
            {
                return 0.0f;
            }

            // parabolic interpolation:

            int x1 = t < 1 ? t : t - 1;
            int x2 = t + 1 < length ? t + 1 : t;

            if (t == x1)
            {
                if (cmdf[t] > cmdf[x2])
                {
                    t = x2;
                }
            }
            else if (t == x2)
            {
                if (cmdf[t] > cmdf[x1])
                {
                    t = x1;
                }
            }
            else
            {
                t = (int)(t + (cmdf[x2] - cmdf[x1]) / (2 * cmdf[t] - cmdf[x2] - cmdf[x1]) / 2);
            }

            return samplingRate / t;
        }

        /// <summary>
        /// YIN algorithm for pitch estimation (overloaded for DiscreteSignal)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="cmdfThreshold"></param>
        /// <returns></returns>
        public static float FromYin(DiscreteSignal signal,
                                    int startPos = 0,
                                    int endPos = -1,
                                    float low = 80,
                                    float high = 400,
                                    float cmdfThreshold = 0.2f)
        {
            return FromYin(signal.Samples, signal.SamplingRate, startPos, endPos, low, high, cmdfThreshold);
        }

        #endregion

        #region frequency-domain methods

        /// <summary>
        /// Pitch estimation: Harmonic Sum Spectrum
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public static float FromHss(DiscreteSignal signal,
                                    int startPos = 0,
                                    int endPos = -1,
                                    float low = 80,
                                    float high = 400,
                                    int fftSize = 0)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            if (startPos != 0 || endPos != signal.Length)
            {
                signal = signal[startPos, endPos];
            }

            signal.ApplyWindow(WindowTypes.Hann);

            int size = fftSize > 0 ? fftSize : MathUtils.NextPowerOfTwo(signal.Length);
            RealFft fft = new RealFft(size);

            float[] spectrum = fft.PowerSpectrum(signal, false).Samples;

            return FromHss(spectrum, signal.SamplingRate, low, high);
        }

        /// <summary>
        /// Pitch estimation: Harmonic Sum Spectrum (given pre-computed spectrum)
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="samplingRate"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static float FromHss(float[] spectrum,
                                    int samplingRate,
                                    float low = 80,
                                    float high = 400)
        {
            float[] sumSpectrum = spectrum.FastCopy();

            int fftSize = (spectrum.Length - 1) * 2;

            int startIdx = (int)(low * fftSize / samplingRate) + 1;
            int endIdx = (int)(high * fftSize / samplingRate) + 1;
            int decimations = Math.Min(spectrum.Length / endIdx, 10);

            int hssIndex = 0;
            float maxHss = 0.0f;

            for (int j = startIdx; j < endIdx; j++)
            {
                sumSpectrum[j] *= 1.5f;         // slightly emphasize 1st component

                for (int k = 2; k < decimations; k++)
                {
                    sumSpectrum[j] += (spectrum[j * k - 1] + spectrum[j * k] + spectrum[j * k + 1]) / 3;
                }

                if (sumSpectrum[j] > maxHss)
                {
                    maxHss = sumSpectrum[j];
                    hssIndex = j;
                }
            }

            return (float)hssIndex * samplingRate / fftSize;
        }

        /// <summary>
        /// Pitch estimation: Harmonic Product Spectrum
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public static float FromHps(DiscreteSignal signal,
                                    int startPos = 0,
                                    int endPos = -1,
                                    float low = 80,
                                    float high = 400,
                                    int fftSize = 0)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            if (startPos != 0 || endPos != signal.Length)
            {
                signal = signal[startPos, endPos];
            }

            signal.ApplyWindow(WindowTypes.Hann);

            int size = fftSize > 0 ? fftSize : MathUtils.NextPowerOfTwo(signal.Length);
            RealFft fft = new RealFft(size);

            float[] spectrum = fft.PowerSpectrum(signal, false).Samples;

            return FromHps(spectrum, signal.SamplingRate, low, high);
        }

        /// <summary>
        /// Pitch estimation: Harmonic Product Spectrum (given pre-computed spectrum)
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="samplingRate"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static float FromHps(float[] spectrum,
                                    int samplingRate,
                                    float low = 80,
                                    float high = 400)
        {
            float[] sumSpectrum = spectrum.FastCopy();

            int fftSize = (spectrum.Length - 1) * 2;

            int startIdx = (int)(low * fftSize / samplingRate) + 1;
            int endIdx = (int)(high * fftSize / samplingRate) + 1;
            int decimations = Math.Min(spectrum.Length / endIdx, 10);

            int hpsIndex = 0;
            float maxHps = 0.0f;

            for (int j = startIdx; j < endIdx; j++)
            {
                for (int k = 2; k < decimations; k++)
                {
                    sumSpectrum[j] *= (spectrum[j * k - 1] + spectrum[j * k] + spectrum[j * k + 1]) / 3;
                }

                if (sumSpectrum[j] > maxHps)
                {
                    maxHps = sumSpectrum[j];
                    hpsIndex = j;
                }
            }

            return (float)hpsIndex * samplingRate / fftSize;
        }

        /// <summary>
        /// Pitch estimation: from spectral peaks
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public static float FromSpectralPeaks(DiscreteSignal signal,
                                              int startPos = 0,
                                              int endPos = -1,
                                              float low = 80,
                                              float high = 400,
                                              int fftSize = 0)
        {
            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            if (startPos != 0 || endPos != signal.Length)
            {
                signal = signal[startPos, endPos];
            }

            signal.ApplyWindow(WindowTypes.Hann);

            int size = fftSize > 0 ? fftSize : MathUtils.NextPowerOfTwo(signal.Length);
            RealFft fft = new RealFft(size);

            float[] spectrum = fft.PowerSpectrum(signal, false).Samples;

            return FromSpectralPeaks(spectrum, signal.SamplingRate, low, high);
        }

        /// <summary>
        /// Pitch estimation: from spectral peaks (given pre-computed spectrum)
        /// </summary>
        /// <param name="spectrum"></param>
        /// <param name="samplingRate"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static float FromSpectralPeaks(float[] spectrum,
                                              int samplingRate,
                                              float low = 80,
                                              float high = 400)
        {
            int fftSize = (spectrum.Length - 1) * 2;

            int startIdx = (int)(low * fftSize / samplingRate) + 1;
            int endIdx = (int)(high * fftSize / samplingRate) + 1;

            for (int k = startIdx + 1; k < endIdx; k++)
            {
                if (spectrum[k] > spectrum[k - 1] && spectrum[k] > spectrum[k - 2] &&
                    spectrum[k] > spectrum[k + 1] && spectrum[k] > spectrum[k + 2])
                {
                    return (float)k * samplingRate / fftSize;
                }
            }

            return (float)startIdx * samplingRate / fftSize;
        }

        /// <summary>
        /// Pitch estimation from signal cepstrum
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="cepstrumSize"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public static float FromCepstrum(DiscreteSignal signal,
                                         int startPos = 0,
                                         int endPos = -1,
                                         float low = 80,
                                         float high = 400,
                                         int cepstrumSize = 256,
                                         int fftSize = 1024)
        {
            int samplingRate = signal.SamplingRate;

            if (endPos == -1)
            {
                endPos = signal.Length;
            }

            if (startPos != 0 || endPos != signal.Length)
            {
                signal = signal[startPos, endPos];
            }

            int pitch1 = (int)(1.0 * samplingRate / high);                              // 2,5 ms = 400Hz
            int pitch2 = Math.Min(cepstrumSize - 1, (int)(1.0 * samplingRate / low));   // 12,5 ms = 80Hz

            float[] cepstrum = new float[cepstrumSize];

            CepstralTransform cepstralTransform = new CepstralTransform(cepstrumSize, fftSize);
            cepstralTransform.RealCepstrum(signal.Samples, cepstrum);

            float max = cepstrum[pitch1];
            int peakIndex = pitch1;
            for (int k = pitch1 + 1; k <= pitch2; k++)
            {
                if (cepstrum[k] > max)
                {
                    max = cepstrum[k];
                    peakIndex = k;
                }
            }

            return (float)samplingRate / peakIndex;
        }

        #endregion
    }
}
