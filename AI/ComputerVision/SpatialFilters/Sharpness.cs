using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.SpatialFilters
{
    [Serializable]
    public class Sharpness : CustomFilter
    {
        public Sharpness()
        {
            f = new Matrix(3, 3) - (1.0 / 9.0);
            f[1, 1] = 2;
        }
    }
}
