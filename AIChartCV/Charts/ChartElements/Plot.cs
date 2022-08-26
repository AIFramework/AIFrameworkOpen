using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class Plot : BaseChart
    {
        private bool isSpline;
        public bool IsSpline
        {
            get => isSpline;
            set
            {
                SetSplineMode();
                isSpline = value;
            }
        }

        public Plot(Chart chart) : base(chart) { }

        public Plot(Chart chart, string name) : base(chart, name) { }

        private void SetSplineMode()
        {
            if (isSpline)
            {
                Series.ChartType = SeriesChartType.Spline;
            }
            else
            {
                Series.ChartType = SeriesChartType.FastLine;
            }
        }
    }
}
