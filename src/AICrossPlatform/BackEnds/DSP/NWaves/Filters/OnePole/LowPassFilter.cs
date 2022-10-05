using System;

namespace AI.BackEnds.DSP.NWaves.Filters.OnePole
{
    /// <summary>
    /// Class for one-pole low-pass filter
    /// </summary>
    [Serializable]

    public class LowPassFilter : OnePoleFilter
    {
        /// <summary>
        /// Frequency
        /// </summary>
        public double Freq { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="freq"></param>
        public LowPassFilter(double freq)
        {
            SetCoefficients(freq);
        }

        /// <summary>
        /// Set filter coefficients
        /// </summary>
        /// <param name="freq"></param>
        private void SetCoefficients(double freq)
        {
            Freq = freq;

            _a[0] = 1;
            _a[1] = (float)-Math.Exp(-2 * Math.PI * freq);

            _b[0] = 1 + _a[1];
        }

        /// <summary>
        /// Change filter parameters (preserving its state)
        /// </summary>
        /// <param name="freq"></param>
        public void Change(double freq)
        {
            SetCoefficients(freq);
        }
    }
}
