using AI.DataStructs.Algebraic;
using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class ScatterPlot : BaseChart
    {
        public ScatterPlot(Chart chart) : base(chart)
        {
            SymbolSet();
        }

        public ScatterPlot(Chart chart, string name) : base(chart, name)
        {
            SymbolSet();
        }


        public void SetMarkSize(int markSize)
        {
            Series.MarkerSize = markSize;
        }

        public void AutoSetMarkSize()
        {
            int markSize = 5;

            if (Data.Count < 10000)
            {
                markSize = 7;
                if (Data.Count < 5000)
                {
                    markSize = 14;
                }
            }

            SetMarkSize(markSize);
        }

        private void SymbolSet()
        {
            Series.ChartType = SeriesChartType.FastPoint;
            Series.MarkerStyle = MarkerStyle.Circle;
        }

        public override void Recalc(double min, double max)
        {
            Series.Points.Clear();


            Vector xN = data.GetX();
            Vector yN = data.GetY();

            Tuple<Vector, Vector> dat = ReducMethod(xN, yN);
            xN = dat.Item1;
            yN = dat.Item2;

            for (int j = 0; j < xN.Count; j++)
            {
                Series.Points.AddXY(xN[j], yN[j]);
            }
        }
    }
}
