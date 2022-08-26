using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations
{
    /// <summary>
    ///  Class providing modulation methods:
    /// 
    ///     - ring
    ///     - amplitude
    ///     - frequency
    ///     - phase
    /// 
    /// </summary>
    [Serializable]
    /// 
    public class Modulator
    {
        /// <summary>
        /// Ring modulation (RM)
        /// </summary>
        /// <param name="carrier">Carrier signal</param>
        /// <param name="modulator">Modulator signal</param>
        /// <returns>RM signal</returns>
        public DiscreteSignal Ring(DiscreteSignal carrier,
                                   DiscreteSignal modulator)
        {
            if (carrier.SamplingRate != modulator.SamplingRate)
            {
                throw new ArgumentException("Sampling rates must be the same!");
            }

            return new DiscreteSignal(carrier.SamplingRate,
                                      carrier.Samples.Zip(modulator.Samples, (c, m) => c * m));
        }

        /// <summary>
        /// Amplitude modulation (AM)
        /// </summary>
        /// <param name="carrier">Carrier signal</param>
        /// <param name="modulatorFrequency">Modulator frequency</param>
        /// <param name="modulationIndex">Modulation index (depth)</param>
        /// <returns>AM signal</returns>
        public DiscreteSignal Amplitude(DiscreteSignal carrier,
                                        float modulatorFrequency = 20/*Hz*/,
                                        float modulationIndex = 0.5f)
        {
            int fs = carrier.SamplingRate;
            float mf = modulatorFrequency;          // just short aliases //
            float mi = modulationIndex;

            System.Collections.Generic.IEnumerable<double> output = Enumerable.Range(0, carrier.Length)
                                   .Select(i => carrier[i] * (1 + (mi * Math.Cos(2 * Math.PI * mf / fs * i))));

            return new DiscreteSignal(fs, output.ToFloats());
        }

        /// <summary>
        /// Frequency modulation (FM)
        /// </summary>
        /// <param name="baseband">Baseband signal</param>
        /// <param name="carrierAmplitude">Carrier amplitude</param>
        /// <param name="carrierFrequency">Carrier frequency</param>
        /// <param name="deviation">Frequency deviation</param>
        /// <returns>RM signal</returns>
        public DiscreteSignal Frequency(DiscreteSignal baseband,
                                        float carrierAmplitude,
                                        float carrierFrequency,
                                        float deviation = 0.1f/*Hz*/)
        {
            int fs = baseband.SamplingRate;
            float ca = carrierAmplitude;          // just short aliases //
            float cf = carrierFrequency;

            double integral = 0.0;
            System.Collections.Generic.IEnumerable<double> output = Enumerable.Range(0, baseband.Length)
                                   .Select(i => ca * Math.Cos((2 * Math.PI * cf / fs * i) +
                                                 (2 * Math.PI * deviation * (integral += baseband[i]))));

            return new DiscreteSignal(fs, output.ToFloats());
        }

        /// <summary>
        /// Sinusoidal frequency modulation (FM)
        /// </summary>
        /// <param name="carrierFrequency">Carrier signal frequency</param>
        /// <param name="carrierAmplitude">Carrier signal amplitude</param>
        /// <param name="modulatorFrequency">Modulator frequency</param>
        /// <param name="modulationIndex">Modulation index (depth)</param>
        /// <param name="length">Length of FM signal</param>
        /// <param name="samplingRate">Sampling rate</param>
        /// <returns>Sinusoidal FM signal</returns>
        public DiscreteSignal FrequencySinusoidal(
                                        float carrierFrequency,
                                        float carrierAmplitude,
                                        float modulatorFrequency,
                                        float modulationIndex,
                                        int length,
                                        int samplingRate = 1)
        {
            int fs = samplingRate;
            float ca = carrierAmplitude;
            float cf = carrierFrequency;          // just short aliases //
            float mf = modulatorFrequency;
            float mi = modulationIndex;

            System.Collections.Generic.IEnumerable<double> output = Enumerable.Range(0, length)
                                   .Select(i => ca * Math.Cos((2 * Math.PI * cf / fs * i) +
                                                (mi * Math.Sin(2 * Math.PI * mf / fs * i))));

            return new DiscreteSignal(samplingRate, output.ToFloats());
        }

        /// <summary>
        /// Linear frequency modulation (FM)
        /// </summary>
        /// <param name="carrierFrequency">Carrier signal frequency</param>
        /// <param name="carrierAmplitude">Carrier signal amplitude</param>
        /// <param name="modulationIndex">Modulation index (depth)</param>
        /// <param name="length">Length of FM signal</param>
        /// <param name="samplingRate">Sampling rate</param>
        /// <returns>Sinusoidal FM signal</returns>
        public DiscreteSignal FrequencyLinear(
                                        float carrierFrequency,
                                        float carrierAmplitude,
                                        float modulationIndex,
                                        int length,
                                        int samplingRate = 1)
        {
            int fs = samplingRate;
            float ca = carrierAmplitude;          // just short aliases //
            float cf = carrierFrequency;
            float mi = modulationIndex;

            System.Collections.Generic.IEnumerable<double> output = Enumerable.Range(0, length)
                                   .Select(i => ca * Math.Cos(2 * Math.PI * ((cf / fs) + (mi * i)) * i / fs));

            return new DiscreteSignal(fs, output.ToFloats());
        }

        /// <summary>
        /// Phase modulation (PM)
        /// </summary>
        /// <param name="baseband">Baseband signal</param>
        /// <param name="carrierAmplitude">Carrier amplitude</param>
        /// <param name="carrierFrequency">Carrier frequency</param>
        /// <param name="deviation">Frequency deviation</param>
        /// <returns>RM signal</returns>
        public DiscreteSignal Phase(DiscreteSignal baseband,
                                    float carrierAmplitude,
                                    float carrierFrequency,
                                    float deviation = 0.8f)
        {
            int fs = baseband.SamplingRate;
            float ca = carrierAmplitude;          // just short aliases //
            float cf = carrierFrequency;

            System.Collections.Generic.IEnumerable<double> output = Enumerable.Range(0, baseband.Length)
                                   .Select(i => ca * Math.Cos((2 * Math.PI * cf / fs * i) + (deviation * baseband[i])));

            return new DiscreteSignal(fs, output.ToFloats());
        }

        /// <summary>
        /// Simple amplitude demodulation based on Hilbert transform
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal DemodulateAmplitude(DiscreteSignal signal)
        {
            HilbertTransform ht = new HilbertTransform(signal.Length, false);
            float[] mag = ht.AnalyticSignal(signal.Samples).Magnitude();

            return new DiscreteSignal(signal.SamplingRate, mag) - 1.0f;
        }

        /// <summary>
        /// Simple frequency demodulation based on Hilbert transform
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal DemodulateFrequency(DiscreteSignal signal)
        {
            float[] diff = new float[signal.Length];

            MathUtils.Diff(signal.Samples, diff);

            HilbertTransform ht = new HilbertTransform(signal.Length, false);
            float[] mag = ht.AnalyticSignal(diff).Magnitude();

            return new DiscreteSignal(signal.SamplingRate, mag) - 1.0f;
        }
    }
}
