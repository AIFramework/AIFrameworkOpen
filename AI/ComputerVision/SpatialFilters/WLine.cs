using AI.DataStructs.Algebraic;

namespace AI.ComputerVision.SpatialFilters
{
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
