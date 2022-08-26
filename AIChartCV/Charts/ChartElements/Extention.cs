using System;
using System.Collections.Generic;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal static class Extention
    {
        public static ScaleData GetScaleData(this IEnumerator<IChartElement> chartElements)
        {
            ScaleData scaleData = new ScaleData();

            while (chartElements.MoveNext())
            {
                IChartElement chartElement = chartElements.Current;

                if (scaleData.MinX > chartElement.GetXMin())
                {
                    scaleData.MinX = chartElement.GetXMin();
                }

                if (scaleData.MinY > chartElement.GetYMin())
                {
                    scaleData.MinY = chartElement.GetYMin();
                }

                if (scaleData.MaxY < chartElement.GetYMax())
                {
                    scaleData.MaxY = chartElement.GetYMax();
                }

                if (scaleData.MaxX < chartElement.GetXMax())
                {
                    scaleData.MaxX = chartElement.GetXMax();
                }
            }

            return scaleData;
        }
    }
}
