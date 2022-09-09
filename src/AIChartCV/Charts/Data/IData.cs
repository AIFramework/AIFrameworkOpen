using AI.DataStructs.Algebraic;

namespace AI.Charts.Data
{
    internal interface IData
    {
        int Count { get; set; }
        void LoadData(Vector x, Vector y);
        int IndexValueNeighborhoodMin(double value);
        Vector GetRegionX(int a, int b);
        Vector GetRegionY(int a, int b);
        Vector GetX();
        Vector GetY();

        double MaxX();
        double MinY();
        double MinX();
        double MaxY();
    }
}
