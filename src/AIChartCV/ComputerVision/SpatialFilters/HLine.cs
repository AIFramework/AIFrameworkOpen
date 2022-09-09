using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Детектор вертикальных линий
    /// </summary>
    [Serializable]
    public class HLine : CustomFilter
    {
        /// <summary>
        /// Детектор вертикальных линий
        /// </summary>
        public HLine()
        {
            filter_kernel = new Matrix(1, 3);
            filter_kernel.Data[1] = 2.0;
            filter_kernel.Data[0] = -1.0;
            filter_kernel.Data[2] = -1.0;
        }
    }
}
