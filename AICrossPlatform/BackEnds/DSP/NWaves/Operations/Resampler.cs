using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Operations
{
    /// <summary>
    /// Class responsible for sampling rate conversion
    /// </summary>
    [Serializable]
    /// 
    public class Resampler
    {
        /// <summary>
        /// The order of FIR LP resampling filter (minimally required).
        /// This constant should be used for simple up/down ratios.
        /// </summary>
        private const int MinResamplingFilterOrder = 101;

        /// <summary>
        /// Interpolation followed by low-pass filtering
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="factor"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DiscreteSignal Interpolate(DiscreteSignal signal, int factor, FirFilter filter = null)
        {
            if (factor == 1)
            {
                return signal.Copy();
            }

            float[] output = new float[signal.Length * factor];

            int pos = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                output[pos] = factor * signal[i];
                pos += factor;
            }

            FirFilter lpFilter = filter;

            if (filter == null)
            {
                int filterSize = factor > MinResamplingFilterOrder / 2 ?
                                 (2 * factor) + 1 :
                                 MinResamplingFilterOrder;

                lpFilter = new FirFilter(DesignFilter.FirWinLp(filterSize, 0.5f / factor));
            }

            return lpFilter.ApplyTo(new DiscreteSignal(signal.SamplingRate * factor, output));
        }

        /// <summary>
        /// Decimation preceded by low-pass filtering
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="factor"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DiscreteSignal Decimate(DiscreteSignal signal, int factor, FirFilter filter = null)
        {
            if (factor == 1)
            {
                return signal.Copy();
            }

            int filterSize = factor > MinResamplingFilterOrder / 2 ?
                             (2 * factor) + 1 :
                             MinResamplingFilterOrder;

            FirFilter lpFilter = filter;

            if (filter == null)
            {
                lpFilter = new FirFilter(DesignFilter.FirWinLp(filterSize, 0.5f / factor));

                signal = lpFilter.ApplyTo(signal);
            }

            float[] output = new float[signal.Length / factor];

            int pos = 0;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = signal[pos];
                pos += factor;
            }

            return new DiscreteSignal(signal.SamplingRate / factor, output);
        }

        /// <summary>
        /// Band-limited resampling
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="newSamplingRate"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public DiscreteSignal Resample(DiscreteSignal signal,
                                       int newSamplingRate,
                                       FirFilter filter = null,
                                       int order = 15)
        {
            if (signal.SamplingRate == newSamplingRate)
            {
                return signal.Copy();
            }

            float g = (float)newSamplingRate / signal.SamplingRate;

            float[] input = signal.Samples;
            float[] output = new float[(int)(input.Length * g)];

            if (g < 1 && filter == null)
            {
                filter = new FirFilter(DesignFilter.FirWinLp(MinResamplingFilterOrder, g / 2));

                input = filter.ApplyTo(signal).Samples;
            }

            float step = 1 / g;

            for (int n = 0; n < output.Length; n++)
            {
                float x = n * step;

                for (int i = -order; i < order; i++)
                {
                    int j = (int)Math.Floor(x) - i;

                    if (j < 0 || j >= input.Length)
                    {
                        continue;
                    }

                    float t = x - j;
                    float w = (float)(0.5 * (1.0 + Math.Cos(t / order * Math.PI)));    // Hann window
                    float sinc = (float)MathUtils.Sinc(t);                             // Sinc function
                    output[n] += w * sinc * input[j];
                }
            }

            return new DiscreteSignal(newSamplingRate, output);
        }

        /// <summary>
        /// Simple resampling as the combination of interpolation and decimation.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DiscreteSignal ResampleUpDown(DiscreteSignal signal, int up, int down, FirFilter filter = null)
        {
            if (up == down)
            {
                return signal.Copy();
            }

            int newSamplingRate = signal.SamplingRate * up / down;

            if (up > 20 && down > 20)
            {
                return Resample(signal, newSamplingRate, filter);
            }

            float[] output = new float[signal.Length * up];

            int pos = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                output[pos] = up * signal[i];
                pos += up;
            }

            FirFilter lpFilter = filter;

            if (filter == null)
            {
                int factor = Math.Max(up, down);
                int filterSize = factor > MinResamplingFilterOrder / 2 ?
                                 (8 * factor) + 1 :
                                 MinResamplingFilterOrder;

                lpFilter = new FirFilter(DesignFilter.FirWinLp(filterSize, 0.5f / factor));
            }

            DiscreteSignal upsampled = lpFilter.ApplyTo(new DiscreteSignal(signal.SamplingRate * up, output));

            output = new float[upsampled.Length / down];

            pos = 0;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = upsampled[pos];
                pos += down;
            }

            return new DiscreteSignal(newSamplingRate, output);
        }
    }
}
