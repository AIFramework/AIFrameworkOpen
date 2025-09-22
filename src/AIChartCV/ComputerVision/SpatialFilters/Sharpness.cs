using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters;

/// <summary>
/// Повышение резкости
/// </summary>
[Serializable]
public class Sharpness : CustomFilter
{
    /// <summary>
    /// Повышение резкости
    /// </summary>
    public Sharpness(double sharp = 1)
    {
        filter_kernel = new Matrix(3, 3) - 1;
        filter_kernel[1, 1] = 8 + sharp;
    }
}

/// <summary>
/// Фильтр Гауссова размытия для качественного удаления шума.
/// Применяет взвешенное усреднение, давая больший вес центральным пикселям.
/// </summary>
[Serializable]
public class GaussianBlurFilter : CustomFilter
{
    /// <summary>
    /// Создает ядро для Гауссова размытия 3x3.
    /// </summary>
    public GaussianBlurFilter()
    {
        filter_kernel = new Matrix(3, 3);

        // Коэффициент нормализации (сумма всех элементов ядра равна 16)
        double normalizationFactor = 1.0 / 16.0;

        filter_kernel[0, 0] = 1 * normalizationFactor;
        filter_kernel[0, 1] = 2 * normalizationFactor;
        filter_kernel[0, 2] = 1 * normalizationFactor;

        filter_kernel[1, 0] = 2 * normalizationFactor;
        filter_kernel[1, 1] = 4 * normalizationFactor; // Центральный пиксель имеет наибольший вес
        filter_kernel[1, 2] = 2 * normalizationFactor;

        filter_kernel[2, 0] = 1 * normalizationFactor;
        filter_kernel[2, 1] = 2 * normalizationFactor;
        filter_kernel[2, 2] = 1 * normalizationFactor;
    }
}
