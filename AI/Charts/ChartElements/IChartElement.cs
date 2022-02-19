using AI.Charts.Data;
using AI.DataStructs.Algebraic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    internal interface IChartElement
    {
        string Name { get; }
        IData Data { get; }
        Series Series { get; }

        void SetColor(Color color);
        void LoadData(Vector x, Vector y);
        void LoadData(IData data);
        void Recalc(double min, double max);

        double GetXMin();
        double GetXMax();
        double GetYMin();
        double GetYMax();
    }
}
