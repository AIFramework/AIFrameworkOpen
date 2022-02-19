﻿using System;
using System.Collections.Generic;
using AI.BackEnds.DSP.NWaves.Utils;

namespace AI.BackEnds.DSP.NWaves.Signals.Builders
{
    /// <summary>
    /// Class for the generator of periodic pulse waves
    /// </summary>
    public class PulseWaveBuilder : SignalBuilder
    {
        /// <summary>
        /// Lower amplitude level
        /// </summary>
        private double _low;

        /// <summary>
        /// Upper amplitude level
        /// </summary>
        private double _high;

        /// <summary>
        /// Pulse duration
        /// </summary>
        private double _pulse;

        /// <summary>
        /// Period of pulse wave
        /// </summary>
        private double _period;

        /// <summary>
        /// Constructor
        /// </summary>
        public PulseWaveBuilder()
        {
            ParameterSetters = new Dictionary<string, Action<double>>
            {
                { "low, lo, min",  param => _low = param },
                { "high, hi, max", param => _high = param },
                { "pulse, width",  param => _pulse = param },
                { "period, t",     param => _period = param }
            };

            _low = -1.0;
            _high = 1.0;
            _pulse = 0.0;
            _period = 0.0;
        }

        /// <summary>
        /// Method generates simple sequence of rectangular pulses.
        /// </summary>
        /// <returns></returns>
        public override float NextSample()
        {
            var sample = _n <= (int)(_pulse * SamplingRate) ? _high : _low;

            if (++_n == (int)(_period * SamplingRate))
            {
                _n = 0;
            }

            return (float)sample;
        }

        public override void Reset()
        {
            _n = 0;
        }

        protected override DiscreteSignal Generate()
        {
            Guard.AgainstNonPositive(_period, "Period");
            Guard.AgainstNonPositive(_pulse, "Pulse duration");
            Guard.AgainstInvalidRange(_pulse, _period, "Pulse duration", "Period");
            return base.Generate();
        }

        private int _n;
    }
}
