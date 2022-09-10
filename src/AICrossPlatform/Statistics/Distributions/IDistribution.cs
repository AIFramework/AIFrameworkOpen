using AI.DataStructs.Algebraic;
using System.Collections.Generic;

namespace AI.Statistics.Distributions
{
    /// <summary>
    /// Basic interface for distribution of functions
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcProb(Vector x, Dictionary<string, Vector> param_dist);

        /// <summary>
        /// Рассчет функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcProb(double x, Dictionary<string, double> param_dist);

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcLogProb(double x, Dictionary<string, double> param_dist);

        /// <summary>
        /// Рассчет логарифма функции распределения
        /// </summary>
        /// <param name="x">Вход</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcLogProb(Vector x, Dictionary<string, Vector> param_dist);
    }
}
