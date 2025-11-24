using AI.DataStructs.Algebraic;
using System;

namespace AI.Statistics;

/// <summary>
/// Статистики зависимые только от формы функции
/// </summary>
[Serializable]
public class FormStatistcs
{
    /// <summary>
    /// Коэффициент формы (пик-фактор)
    /// </summary>
    /// <param name="vector">Вектор входных данных</param>
    public static double CrestFactor(Vector vector)
    {
        Vector vectorNorm = vector.ZNormalise();
        return vectorNorm.Max();
    }

}
