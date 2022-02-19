using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Statistics.Distributions
{
    public interface IDistributionWithoutParams
    {
        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        double CulcProb(Vector x);

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        double CulcProb(double x);

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        double CulcLogProb(double x);

        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        double CulcLogProb(Vector x);
    }
}
