﻿using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Hilbert Transform
    /// </summary>
    public class HilbertTransform
    {
        /// <summary>
        /// Size (length) of Hilbert Transform
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Fft transformer, single precision
        /// </summary>
        private readonly Fft _fft32;

        /// <summary>
        /// Fft transformer, double precision
        /// </summary>
        private readonly Fft64 _fft64;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Size of Hilbert Transform</param>
        /// <param name="doublePrecision"></param>
        public HilbertTransform(int size = 1024, bool doublePrecision = true)
        {
            Size = size;

            if (doublePrecision)
            {
                _fft64 = new Fft64(size);
            }
            else
            {
                _fft32 = new Fft(size);
            }
        }

        /// <summary>
        /// Compute complex analytic signal, double precision
        /// </summary>
        /// <param name="samples">Array of samples</param>
        /// <param name="norm">Normalize by fft size</param>
        /// <returns>Complex analytic signal</returns>
        public ComplexDiscreteSignal AnalyticSignal(double[] samples, bool norm = true)
        {
            ComplexDiscreteSignal analyticSignal = new ComplexDiscreteSignal(1, samples);

            double[] re = analyticSignal.Real;
            double[] im = analyticSignal.Imag;

            _fft64.Direct(re, im);

            for (int i = 1; i < re.Length / 2; i++)
            {
                re[i] *= 2;
                im[i] *= 2;
            }

            for (int i = re.Length / 2 + 1; i < re.Length; i++)
            {
                re[i] = 0.0;
                im[i] = 0.0;
            }

            _fft64.Inverse(re, im);

            if (norm)
            {
                analyticSignal.Attenuate(re.Length);
            }

            return analyticSignal;
        }

        /// <summary>
        /// Compute complex analytic signal, single precision
        /// </summary>
        /// <param name="samples">Array of samples</param>
        /// <param name="norm">Normalize by fft size</param>
        /// <returns>Complex analytic signal</returns>
        public Tuple<float[], float[]> AnalyticSignal(float[] samples, bool norm = true)
        {
            DiscreteSignal sre = new DiscreteSignal(1, samples, allocateNew: true);
            DiscreteSignal sim = new DiscreteSignal(1, samples.Length);

            float[] re = sre.Samples;
            float[] im = sim.Samples;

            _fft32.Direct(re, im);

            for (int i = 1; i < re.Length / 2; i++)
            {
                re[i] *= 2;
                im[i] *= 2;
            }

            for (int i = re.Length / 2 + 1; i < re.Length; i++)
            {
                re[i] = 0.0f;
                im[i] = 0.0f;
            }

            _fft32.Inverse(re, im);

            if (norm)
            {
                sre.Attenuate(re.Length);
                sim.Attenuate(im.Length);
            }

            return new Tuple<float[], float[]>(re, im);
        }

        /// <summary>
        /// Direct Hilbert Transform
        /// </summary>
        /// <param name="signal">Input signal</param>
        /// <returns>Hilbert Transform</returns>
        public DiscreteSignal Direct(DiscreteSignal signal)
        {
            float[] output = new float[signal.Length];
            Direct(signal.Samples, output);
            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Direct Hilbert Transform
        /// </summary>
        /// <param name="samples">Array of samples</param>
        /// <param name="output">Hilbert Transform array</param>
        public void Direct(double[] samples, double[] output)
        {
            double[] analyticSignal = AnalyticSignal(samples).Imag;
            analyticSignal.FastCopyTo(output, analyticSignal.Length);
        }

        /// <summary>
        /// Direct Hilbert Transform
        /// </summary>
        /// <param name="samples">Array of samples</param>
        /// <param name="output">Hilbert Transform array</param>
        public void Direct(float[] samples, float[] output)
        {
            float[] analyticSignal = AnalyticSignal(samples).Item2;
            analyticSignal.FastCopyTo(output, analyticSignal.Length);
        }
    }
}
