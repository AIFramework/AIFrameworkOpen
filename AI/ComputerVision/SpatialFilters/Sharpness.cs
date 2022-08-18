using AI.DataStructs.Algebraic;

namespace AI.ComputerVision.SpatialFilters
{
    public class Sharpness : CustomFilter
    {
        public Sharpness()
        {
            f = new Matrix(3, 3) - (1.0 / 9.0);
            f[1, 1] = 2;
        }
    }
}
