﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Signals;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Polyphase
{
    /// <summary>
    /// System of polyphase filters
    /// </summary>
    [Serializable]

    public class PolyphaseSystem : IFilter, IOnlineFilter
    {
        /// <summary>
        /// Polyphase filters with Передаточная функция E(z^k).
        /// 
        /// Example:
        /// h = [1, 2, 3, 4, 3, 2, 1],  k = 3
        /// 
        /// e0 = [1, 0, 0, 4, 0, 0, 1]
        /// e1 = [0, 2, 0, 0, 3, 0, 0]
        /// e2 = [0, 0, 3, 0, 0, 2, 0]
        /// </summary>
        public FirFilter[] Filters { get; private set; }

        /// <summary>
        /// Polyphase filters with Передаточная функция E(z) used for multi-rate processing.
        /// 
        /// h = [1, 2, 3, 4, 3, 2, 1],  k = 3
        /// 
        /// e0 = [1, 4, 1]
        /// e1 = [2, 3, 0]
        /// e2 = [3, 2, 0]
        /// </summary>
        public FirFilter[] MultirateFilters { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="filterCount"></param>
        /// <param name="type">1 or 2</param>
        public PolyphaseSystem(double[] kernel, int filterCount, int type = 1)
        {
            Filters = new FirFilter[filterCount];
            MultirateFilters = new FirFilter[filterCount];

            int len = (kernel.Length + 1) / filterCount;

            for (int i = 0; i < Filters.Length; i++)
            {
                double[] filterKernel = new double[kernel.Length];
                double[] mrFilterKernel = new double[len];

                for (int j = 0; j < len; j++)
                {
                    int kernelPos = i + (filterCount * j);

                    if (kernelPos < kernel.Length)
                    {
                        filterKernel[kernelPos] = kernel[kernelPos];
                        mrFilterKernel[j] = kernel[kernelPos];
                    }
                }

                Filters[i] = new FirFilter(filterKernel);
                MultirateFilters[i] = new FirFilter(mrFilterKernel);
            }

            // type-II -> reverse

            if (type == 2)
            {
                for (int i = 0; i < Filters.Length / 2; i++)
                {
                    FirFilter tmp = Filters[i];
                    Filters[i] = Filters[filterCount - 1 - i];
                    Filters[filterCount - 1 - i] = tmp;

                    tmp = MultirateFilters[i];
                    MultirateFilters[i] = MultirateFilters[filterCount - 1 - i];
                    MultirateFilters[filterCount - 1 - i] = tmp;
                }
            }
        }

        /// <summary>
        /// Polyphase decimation (for type-I systems)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal Decimate(DiscreteSignal signal)
        {
            int resampledRate = signal.SamplingRate / MultirateFilters.Length;
            int resampledLength = signal.Length / MultirateFilters.Length;
            DiscreteSignal resampled = new DiscreteSignal(resampledRate, resampledLength);

            float acc = 0f;

            // process first K samples separately

            for (int i = MultirateFilters.Length - 1; i >= 1; i--)
            {
                acc += MultirateFilters[i].Process(0);
            }
            acc += MultirateFilters[0].Process(signal[0]);
            resampled[0] = acc;

            // rest of the samples are processed very simply by each filter

            int si = 1;
            for (int i = 1; i < resampled.Length; i++)
            {
                acc = 0f;

                for (int j = MultirateFilters.Length - 1; j >= 0; j--)
                {
                    acc += MultirateFilters[j].Process(signal[si++]);
                }

                resampled[i] = acc;
            }

            return resampled;
        }

        /// <summary>
        /// Polyphase interpolation (for type-II systems)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal Interpolate(DiscreteSignal signal)
        {
            int k = MultirateFilters.Length;
            int resampledRate = signal.SamplingRate * k;
            int resampledLength = signal.Length * k;
            DiscreteSignal resampled = new DiscreteSignal(resampledRate, resampledLength);

            int ri = 0;
            for (int i = 0; i < signal.Length; i++)
            {
                for (int j = MultirateFilters.Length - 1; j >= 0; j--)
                {
                    resampled[ri++] = k * MultirateFilters[j].Process(signal[i]);
                }
            }

            return resampled;
        }


        #region FIR Filtering (for educational purposes)

        /// <summary>
        /// Online processing.
        /// Inefficient, but helps understanding how polyphase filters work
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public float Process(float sample)
        {
            float output = 0f;

            foreach (FirFilter filter in Filters)
            {
                output += filter.Process(sample);
            }

            return output;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            foreach (FirFilter filter in Filters)
            {
                filter.Reset();
            }

            foreach (FirFilter filter in MultirateFilters)
            {
                filter.Reset();
            }
        }

        /// <summary>
        /// Offline processing
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method">Метод фильтрации</param>
        /// <returns></returns>
        public DiscreteSignal ApplyTo(DiscreteSignal signal, FilteringMethod method = FilteringMethod.Auto)
        {
            return new DiscreteSignal(signal.SamplingRate, signal.Samples.Select(s => Process(s)));
        }

        #endregion
    }
}
