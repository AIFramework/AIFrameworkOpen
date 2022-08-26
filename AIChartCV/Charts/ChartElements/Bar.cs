using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class Bar : BaseChart
    {
        public Bar(Chart chart) : base(chart)
        {
            SymbolSet();
        }

        public Bar(Chart chart, string name) : base(chart, name)
        {
            SymbolSet();
        }

        private void SymbolSet()
        {
            Series.ChartType = SeriesChartType.Column;
        }
    }
}
