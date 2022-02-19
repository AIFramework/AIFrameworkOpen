using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Statistics.Distributions
{
    /// <summary>
    /// Basic interface for distribution of functions
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcProb(Vector x, Dictionary<string, Vector> param_dist);

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcProb(double x, Dictionary<string, double> param_dist);

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcLogProb(double x, Dictionary<string, double> param_dist);

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters</param>
        double CulcLogProb(Vector x, Dictionary<string, Vector> param_dist);
    }
}
