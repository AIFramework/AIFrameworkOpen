using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Простое размытие
    /// </summary>
    [Serializable]
    public class Smoothing : CustomFilter
    {
        /// <summary>
        /// Простое размытие
        /// </summary>
        public Smoothing()
        {
            filter_kernel = new Matrix(3, 3) + (1.0 / 9.0);
        }
    }
}
