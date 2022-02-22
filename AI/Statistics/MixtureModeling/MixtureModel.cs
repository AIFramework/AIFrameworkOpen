using AI.DataStructs.Algebraic;
using AI.Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Statistics.MixtureModeling
{
    public class MixtureModel : IDistributionWithoutParams
    {
        public bool IsOneD { get; private set; }

        private readonly Dictionary<string, double>[] _paramDists1D;
        private readonly Dictionary<string, Vector>[] _paramDistsND;
        private readonly Vector _w;
        private readonly IDistribution _perentDistribution;

        /// <summary>
        /// Mixture modeling
        /// </summary>
        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, double>> paramDists, Vector w)
        {
            IsOneD = true;
            _paramDists1D = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        /// <summary>
        /// Mixture modeling
        /// </summary>
        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, Vector>> paramDists, Vector w)
        {
            IsOneD = false;
            _paramDistsND = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        public double CulcLogProb(double x)
        {
            return Math.Log(CulcProb(x));
        }

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        public double CulcLogProb(Vector x)
        {
            return Math.Log(CulcProb(x));
        }

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        public double CulcProb(Vector x)
        {
            if (IsOneD)
            {
                throw new Exception("Is 1D distribution");
            }

            double fx = 0;

            for (int i = 0; i < _paramDistsND.Length; i++)
            {
                fx += _w[i] * _perentDistribution.CulcProb(x, _paramDistsND[i]);
            }
            return fx;
        }

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        public double CulcProb(double x)
        {
            if (!IsOneD)
            {
                throw new Exception("Is ND distribution");
            }

            double fx = 0;

            for (int i = 0; i < _paramDists1D.Length; i++)
            {
                fx += _w[i] * _perentDistribution.CulcProb(x, _paramDists1D[i]);
            }
            return fx;
        }
    }
}
