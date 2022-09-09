using System;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class ScaleData
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
    }
}
