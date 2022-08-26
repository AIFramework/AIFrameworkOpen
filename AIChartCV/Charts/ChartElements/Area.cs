using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class Area : BaseChart
    {
        public Area(Chart chart) : base(chart)
        {
            SymbolSet();
        }

        public Area(Chart chart, string name) : base(chart, name)
        {
            SymbolSet();
        }

        private void SymbolSet()
        {
            Series.ChartType = SeriesChartType.Area;
        }
    }
}
