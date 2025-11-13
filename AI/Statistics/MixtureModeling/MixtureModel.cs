using AI.DataStructs.Algebraic;
using AI.Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Statistics.MixtureModeling
{
    /// <summary>
    /// Модель смеси распределений
    /// </summary>
    [Serializable]
    public class MixtureModel : IDistributionWithoutParams
    {
        /// <summary>
        /// Одномерная ли смесь
        /// </summary>
        public bool IsOneD { get; private set; }

        private readonly Dictionary<string, double>[] _paramDists1D;
        private readonly Dictionary<string, Vector>[] _paramDistsND;
        private readonly Vector _w;
        private readonly IDistribution _perentDistribution;

        /// <summary>
        /// Модель смеси распределений
        /// </summary>
        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, double>> paramDists, Vector w)
        {
            IsOneD = true;
            _paramDists1D = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        /// <summary>
        /// Модель смеси распределений
        /// </summary>
        public MixtureModel(IDistribution perentDistribution, IEnumerable<Dictionary<string, Vector>> paramDists, Vector w)
        {
            IsOneD = false;
            _paramDistsND = paramDists.ToArray();
            _perentDistribution = perentDistribution;
            _w = w;
        }

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        public double CulcLogProb(double x)
        {
            return Math.Log(CulcProb(x));
        }

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        public double CulcLogProb(Vector x)
        {
            return Math.Log(CulcProb(x));
        }

        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        public double CulcProb(Vector x)
        {
            if (IsOneD)
                throw new Exception("Это одномерное распределение");

            double fx = 0;

            for (int i = 0; i < _paramDistsND.Length; i++)
                fx += _w[i] * _perentDistribution.CulcProb(x, _paramDistsND[i]);

            return fx;
        }

        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
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
