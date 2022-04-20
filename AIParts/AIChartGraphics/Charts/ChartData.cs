using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AI.Charts
{
    /// <summary>
    /// Данные графика
    /// </summary>
    [Serializable]
    public class ChartData : List<ChartDataSample>
    {
        /// <summary>
        /// Имя графика
        /// </summary>
        public string ChartName { get; set; }

        /// <summary>
        /// Добавление графика
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="description"></param>
        /// <param name="color"></param>
        /// <param name="chartType"></param>
        public void SempleADD(Vector x, Vector y, Description description, Color color, ChartType chartType = ChartType.Plot)
        {
            Add(new ChartDataSample(x, y, description, color, chartType));
        }
    }
}
