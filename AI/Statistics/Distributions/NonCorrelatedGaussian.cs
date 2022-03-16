using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.Statistics.Distributions
{
    [Serializable]
    public class NonCorrelatedGaussian : IDistribution
    {
        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters, key params: "std", "mean"</param>
        public double CulcLogProb(double x, Dictionary<string, double> param_dist)
        {
            double std = param_dist["std"] == 0 ? AISettings.GlobalEps : param_dist["std"];
            double mean = param_dist["mean"];
            return CulcLogProb(x, mean, std);
        }


        /// <summary>
        /// Log probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters, key params: "std", "mean"</param>
        public double CulcLogProb(Vector x, Dictionary<string, Vector> param_dist)
        {
            Vector std = param_dist["std"].Transform(r => r == 0 ? AISettings.GlobalEps : r);
            Vector mean = param_dist["mean"];

            double p_log = 0;

            for (int i = 0; i < std.Count; i++)
            {
                p_log += CulcLogProb(x[i], mean[i], std[i]);
            }

            return p_log;
        }

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters, key params: "std", "mean"</param>
        public double CulcProb(Vector x, Dictionary<string, Vector> param_dist)
        {
            double log = CulcLogProb(x, param_dist);
            return Math.Exp(log);
        }

        /// <summary>
        /// Probability calculation
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="param_dist">Distribution parameters, key params: "std", "mean"</param>
        public double CulcProb(double x, Dictionary<string, double> param_dist)
        {
            double std = param_dist["std"] == 0 ? AISettings.GlobalEps : param_dist["std"];
            double mean = param_dist["mean"];
            return CulcProb(x, mean, std);
        }

        private double CulcProb(double x, double mean, double std)
        {
            double const_ = 1.0 / (Math.Sqrt(Math.PI * 2) * std);
            double exp = Math.Exp(-0.5 * Math.Pow((x - mean) / (std), 2));
            return const_ * exp;
        }

        private double CulcLogProb(double x, double mean, double std)
        {
            double log_const = Math.Log(1.0 / (Math.Sqrt(Math.PI * 2) * std + AISettings.GlobalEps));
            double log_exp = -0.5 * Math.Pow((x - mean) / (std), 2);
            return log_const + log_exp;
        }
    }

}
