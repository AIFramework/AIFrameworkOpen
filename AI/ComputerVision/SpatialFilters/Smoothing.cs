using AI.DataStructs.Algebraic;

namespace AI.ComputerVision.SpatialFilters
{
    public class Smoothing : CustomFilter
    {
        public Smoothing()
        {
            f = new Matrix(3, 3) + 1.0 / 9.0;
        }
    }
}
