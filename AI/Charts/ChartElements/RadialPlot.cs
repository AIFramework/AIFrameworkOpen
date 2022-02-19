using AI.Charts.Data;
using AI.DataStructs.Algebraic;
using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class RadialPlot : BaseChart
    {
        public RadialPlot(Chart chart) : base(chart)
        {
            SymbolSet();
        }

        public RadialPlot(Chart chart, string name) : base(chart, name)
        {
            SymbolSet();
        }


        public override Tuple<Vector, Vector> ReducMethod(Vector xN, Vector yN)
        {
            return DataMethods.ReducDataRadialPlot(xN, yN);
        }

        private void SymbolSet()
        {
            Series.ChartType = SeriesChartType.Polar;
        }
    }
}
