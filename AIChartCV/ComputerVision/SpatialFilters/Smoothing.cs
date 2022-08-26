using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    [Serializable]
    public class Smoothing : CustomFilter
    {
        public Smoothing()
        {
            f = new Matrix(3, 3) + (1.0 / 9.0);
        }
    }
}
