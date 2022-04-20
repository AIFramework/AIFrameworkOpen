using AI.DataStructs.Algebraic;

namespace AI.ComputerVision.SpatialFilters
{
    public class HLine : CustomFilter
    {
        public HLine()
        {
            f = new Matrix(1, 3);
            f.Data[1] = 2.0;
            f.Data[0] = -1.0;
            f.Data[2] = -1.0;
        }
    }
}
