using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Distances
{
    /// <summary>
    /// Вероятностные (энтропийные) расстояния
    /// </summary>
    [Serializable]
    public static class ProbabilityDistances
    {
        /// <summary>
        /// Дивергенция Кульбака Лэйблера
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
        /// Дивергенция Кульбака Лэйблера (симетричная)
        /// </summary>
        public static double DKLSymmetrical(Vector v1, Vector v2)
        {
            double eps = AISettings.GlobalEps;

            return Functions.Summ(
                Vector.Crosser(v1, v2,
                (x, y) => (x * Math.Log((x + eps) / (y + eps))) + (y * Math.Log((y + eps) / (x + eps)))
                )
                ) / (2 * v2.Count);
        }
    }
}
