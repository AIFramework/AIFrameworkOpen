using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Distances
{
    /// <summary>
    /// Probability (entropy) distances
    /// </summary>
    [Serializable]
    public static class ProbabilityDistances
    {
        /// <summary>
        /// Kullback–Leibler divergence
        /// </summary>
        public static double DKL(Vector v1, Vector v2)
        {
            return Functions.Summ(
                Vector.Crosser(v1, v2,
                (x, y) => x * Math.Log((x + AISettings.GlobalEps) / (y + AISettings.GlobalEps))
                )
                ) / v2.Count;
        }

        /// <summary>
        /// Symmetrical Kullback - Leibler divergence
        /// </summary>
        public static double DKLSymmetrical(Vector v1, Vector v2)
        {
            return Functions.Summ(
                Vector.Crosser(v1, v2,
                (x, y) => x * Math.Log((x + AISettings.GlobalEps) / (y + AISettings.GlobalEps)) + y * Math.Log((y + AISettings.GlobalEps) / (x + AISettings.GlobalEps))
                )
                ) / (2 * v2.Count);
        }
    }
}
