﻿using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;

namespace AI.BackEnds.DSP.NWaves.Signals.Builders
{
    /// <summary>
    /// Perlin noise (improved, 1D simplex noise)
    /// 
    /// Implementation of 1D Perlin Noise ported from Stefan Gustavson's code:
    /// 
    ///    https://github.com/stegu/perlin-noise/blob/master/src/noise1234.c
    ///
    /// </summary>
    public class PerlinNoiseBuilder : SignalBuilder
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
        /// Scale
        /// </summary>
        private double _scale;

        /// <summary>
        /// Table of permutations
        /// </summary>
        private readonly byte[] _permutation = new byte[512];

        /// <summary>
        /// Constructor
        /// </summary>
        public PerlinNoiseBuilder()
        {
            ParameterSetters = new Dictionary<string, Action<double>>
            {
                { "low, lo, min",  param => _low = param },
                { "high, hi, max", param => _high = param },
                { "scale, octave", param => _scale = param }
            };

            _low = -1.0;
            _high = 1.0;
            _scale = 0.02;

            _rand.NextBytes(_permutation);
        }

        /// <summary>
        /// 1D simplex noise
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double GenerateSample(double x)
        {
            int i1 = (int)x < x ? (int)x : (int)x - 1;
            int i2 = (i1 + 1) & 0xff;
            double f1 = x - i1;
            double f2 = f1 - 1.0;

            i1 &= 0xff;

            return 0.188 * Lerp(Fade(f1), Gradient(_permutation[i1], f1),
                                          Gradient(_permutation[i2], f2));
        }

        /// <summary>
        /// Gradient
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Gradient(int hash, double x)
        {
            int h = hash & 15;
            double g = 1.0 + (h & 7);
            return ((h & 8) == 0) ? g * x : -g * x;
        }

        /// <summary>
        /// Improved interpolator
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// Linear interpolator
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        /// <summary>
        /// Method for generating Perlin noise
        /// </summary>
        /// <returns></returns>
        public override float NextSample()
        {
            double sample = GenerateSample(_n * _scale) * (_high - _low) / 2 + (_high + _low) / 2;
            _n++;
            return (float)sample;
        }

        public override void Reset()
        {
            _n = 0;
            _rand.NextBytes(_permutation);
        }

        protected override DiscreteSignal Generate()
        {
            Guard.AgainstInvalidRange(_low, _high, "Upper amplitude", "Lower amplitude");
            return base.Generate();
        }

        private int _n;

        private readonly Random _rand = new Random();
    }
}
