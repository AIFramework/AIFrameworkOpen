using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Повышение резкости
    /// </summary>
    [Serializable]
    public class Sharpness : CustomFilter
    {
        /// <summary>
        /// Повышение резкости
        /// </summary>
        public Sharpness()
        {
            filter_kernel = new Matrix(3, 3) - (1.0 / 9.0);
            filter_kernel[1, 1] = 2;
        }
    }
}
