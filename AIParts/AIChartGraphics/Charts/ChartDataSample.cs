using AI.DataStructs.Algebraic;
using System;
using System.Drawing;

namespace AI.Charts
{
    /// <summary>
    /// Описание одной части графика
    /// </summary>
    [Serializable]
    public class ChartDataSample
    {
        /// <summary>
        /// Описание (метаданные)
        /// </summary>
        public Description DescriptionData { get; set; }

        /// <summary>
        /// Данные по оси X
        /// </summary>
        public Vector DataX;

        /// <summary>
        /// Данные по Y
        /// </summary>
        public Vector DataY;

        /// <summary>
        /// Цвет графика
        /// </summary>
        public Color ColorChart { get; set; }

        /// <summary>
        /// Тип графика
        /// </summary>
        public ChartType ChartType { get; set; }

        /// <summary>
        /// Данные графика
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="description"></param>
        /// <param name="color"></param>
        /// <param name="chartType"></param>
        public ChartDataSample(Vector x, Vector y, Description description, Color color, ChartType chartType = ChartType.Plot)
        {
            DescriptionData = description;
            DataX = x;
            DataY = y;
            ColorChart = color;
            ChartType = chartType;
        }
    }

    /// <summary>
    /// Доступные типы графиков
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// График в виде линии 
        /// </summary>
        Plot,
        /// <summary>
        /// График в виде столбцов(гистограмма)
        /// </summary>
        Bar,
        /// <summary>
        /// Скаттерограмма, график в виде точек
        /// </summary>
        Scatter,
        /// <summary>
        /// Сплайновая кривая 
        /// </summary>
        Spline
    }
}
