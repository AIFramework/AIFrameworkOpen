using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Общая стратегия фильтрации
    /// </summary>
    [Serializable]
    public enum FilteringMethod
    {
        /// <summary>
        /// Стратегия фильтрации динамически определяется библиотекой NWaves.
        /// </summary>
        Auto,

        /// <summary>
        /// Фильтрация во временной области на основе разностных уравнений
        /// </summary>
        DifferenceEquation,

        /// <summary>
        /// Фильтрация в частотной области на основе алгоритма OLA
        /// </summary>
        OverlapAdd,

        /// <summary>
        /// Фильтрация в частотной области на основе алгоритма OLS
        /// </summary>
        OverlapSave,

        /// <summary>
        /// Кастомная фильтрация
        /// </summary>
        Custom
    }
}
