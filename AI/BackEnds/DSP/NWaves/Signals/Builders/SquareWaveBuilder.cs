﻿using System;
using System.Collections.Generic;
using AI.BackEnds.DSP.NWaves.Utils;

namespace AI.BackEnds.DSP.NWaves.Signals.Builders
{
    /// <summary>
    /// Class for the generator of triangle waves
    /// </summary>
    public class SquareWaveBuilder : SignalBuilder
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
        /// Frequency of the triangle wave
        /// </summary>
        private double _frequency;

        /// <summary>
        /// Constructor
        /// </summary>
        public SquareWaveBuilder()
        {
            ParameterSetters = new Dictionary<string, Action<double>>
            {
                { "low, lo, min",    param => _low = param },
                { "high, hi, max",   param => _high = param },
                { "frequency, freq", param => { _frequency = param; _cycles = SamplingRate / _frequency; }}
            };
        }

        /// <summary>
        /// Method generates square wave
        /// </summary>
        /// <returns></returns>
        public override float NextSample()
        {
            var x = _n % _cycles;
            var sample = x < _cycles / 2 ? _high : _low;
            _n++;
            return (float)sample;
        }

        public override void Reset()
        {
            _n = 0;
        }

        public override SignalBuilder SampledAt(int samplingRate)
        {
            _cycles = samplingRate / _frequency;
            return base.SampledAt(samplingRate);
        }

        protected override DiscreteSignal Generate()
        {
            Guard.AgainstNonPositive(_frequency, "Frequency");
            Guard.AgainstInvalidRange(_low, _high, "Upper amplitude", "Lower amplitude");
            return base.Generate();
        }

        private int _n;

        private double _cycles;
    }
}
