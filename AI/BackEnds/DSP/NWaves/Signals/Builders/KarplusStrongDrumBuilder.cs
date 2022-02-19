﻿namespace AI.BackEnds.DSP.NWaves.Signals.Builders
{
    public class KarplusStrongDrumBuilder : KarplusStrongBuilder
    {
        private double _probability = 0.5;

        public KarplusStrongDrumBuilder() : base(null)
        {
            Init();
        }

        public KarplusStrongDrumBuilder(float[] samples) : base(samples)
        {
            Init();
        }

        private void Init()
        {
            ParameterSetters.Add("probability, prob", param => _probability = param);
        }

        public override float NextSample()
        {
            var idx = ((int)_n) % _samples.Length;

            if (_rand.NextDouble() < 1 / _stretchFactor)
            {
                if (_rand.NextDouble() < _probability)
                {
                    _samples[idx] = 0.5f * (_samples[idx] + _prev) * _feedback;
                }
                else
                {
                    _samples[idx] = -0.5f * (_samples[idx] + _prev) * _feedback;
                }
            }

            _prev = _samples[idx];
            _n++;

            return _prev;
        }
    }
}
