using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    [Serializable]
    public class WLine : CustomFilter
    {
        public WLine()
        {
            f = new Matrix(3, 1);
            f.Data[1] = 2.0;
            f.Data[0] = -1.0;
            f.Data[2] = -1.0;
        }
    }
}
