using AI.DataStructs.Algebraic;
using System;

namespace AI.Statistics
{
    /// <summary>
    /// Статистики зависимые только от формы функции
    /// </summary>
    [Serializable]
    public class FormStatistcs
    {
        /// <summary>
        /// Коэффициент формы (пик-фактор)
        /// </summary>
        /// <param name="vector">Input data vector</param>
        public static double CrestFactor(Vector vector)
        {
            Vector vectorNorm = vector.Normalise();
            return vectorNorm.Max();
        }

    }
}
