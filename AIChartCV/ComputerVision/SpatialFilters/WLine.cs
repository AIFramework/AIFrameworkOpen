using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Детектор горизонтальных линий
    /// </summary>
    [Serializable]
    public class WLine : CustomFilter
    {
        /// <summary>
        /// Детектор горизонтальных линий
        /// </summary>
        public WLine()
        {
            filter_kernel = new Matrix(3, 1);
            filter_kernel.Data[1] = 2.0;
            filter_kernel.Data[0] = -1.0;
            filter_kernel.Data[2] = -1.0;
        }
    }
}
