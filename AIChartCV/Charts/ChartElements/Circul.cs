using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class Circul : BaseChart
    {
        public Circul(Chart chart) : base(chart)
        {
            SymbolSet();
        }

        public Circul(Chart chart, string name) : base(chart, name)
        {
            SymbolSet();
        }

        private void SymbolSet()
        {
            Series.ChartType = SeriesChartType.Pie;
        }
    }
}
