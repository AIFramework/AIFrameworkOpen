using AI.Charts.ChartElements;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.DSPCore;
using AI.Statistics;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AI.Charts.Forms
{
    /// <summary>
    /// Форма для отображения графиков
    /// </summary>
    [Serializable]
    public partial class FormChart : Form
    {
        /// <summary>
        /// Форма для отображения графиков
        /// </summary>
        public FormChart()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Имя графика
        /// </summary>
        public string ChartName
        {
            get => Text;

            set => Text = chartVisual1.ChartName = value;
        }

        /// <summary>
        /// Имя оси X
        /// </summary>
        public string LabelX
        {
            get => chartVisual1.LabelX;
            set => chartVisual1.LabelX = value;
        }

        /// <summary>
        /// Имя оси Y
        /// </summary>
        public string LabelY
        {
            get => chartVisual1.LabelY;
            set => chartVisual1.LabelY = value;
        }


        /// <summary>
        /// Визуализация графиков
        /// </summary>
        /// <param name="chartDatas">Данные графиков</param>
        public void VisualData(ChartData chartDatas)
        {
            Text = chartDatas.ChartName;
            chartVisual1.LabelX = chartDatas[0].DescriptionData.X;
            chartVisual1.LabelY = chartDatas[0].DescriptionData.Y;
            chartVisual1.ChartName = chartDatas.ChartName;


            for (int i = 0; i < chartDatas.Count; i++)
            {
                switch (chartDatas[i].ChartType)
                {
                    case ChartType.Plot:
                        AddPlot(chartDatas[i].DataX, chartDatas[i].DataY, chartDatas[i].DescriptionData.Name, chartDatas[i].ColorChart, 2);
                        break;
                    case ChartType.Bar:
                        AddBar(chartDatas[i].DataX, chartDatas[i].DataY, chartDatas[i].DescriptionData.Name, chartDatas[i].ColorChart);
                        break;
                    case ChartType.Spline:
                        AddPlot(chartDatas[i].DataX, chartDatas[i].DataY, chartDatas[i].DescriptionData.Name, chartDatas[i].ColorChart, 2, true);
                        break;
                    case ChartType.Scatter:
                        AddScatter(chartDatas[i].DataX, chartDatas[i].DataY, chartDatas[i].DescriptionData.Name, chartDatas[i].ColorChart);
                        break;
                }
            }
        }


        #region Plot

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void AddPlot(Vector x, Vector y, string name, Color color, int width = 2, bool isSpline = false)
        {
            chartVisual1.AddPlot(x, y, name, color, width, isSpline);
        }

        // ToDo: порешать
        internal void AddChartElement(IChartElement element)
        {
            if (element is Plot)
            {
                Plot plot = element as Plot;
                AddPlot(
                    plot.Data.GetX(),
                    plot.Data.GetY(),
                    plot.Name,
                    plot.Series.Color,
                    plot.Series.BorderWidth,
                    plot.IsSpline
                    );
            }

            if (element is Bar)
            {
                Bar bar = element as Bar;
                AddBar(
                    bar.Data.GetX(),
                    bar.Data.GetY(),
                    bar.Name,
                    bar.Series.Color
                    );
            }

            if (element is ScatterPlot)
            {
                ScatterPlot scatter = element as ScatterPlot;
                AddScatter(
                    scatter.Data.GetX(),
                    scatter.Data.GetY(),
                    scatter.Name,
                    scatter.Series.Color
                    );
            }

            if (element is RadialPlot)
            {
                RadialPlot radial = element as RadialPlot;
                AddRadialPlot(
                    radial.Data.GetX(),
                    radial.Data.GetY(),
                    radial.Name,
                    radial.Series.Color
                    );
            }

        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void AddPlot(Vector y, string name, Color color, int width = 2, bool isSpline = false)
        {
            chartVisual1.AddPlot(Vector.SeqBeginsWithZero(1, y.Count), y, name, color, width, isSpline);
        }



        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void AddPlotBlack(Vector x, Vector y, string name = "", int width = 2, bool isSpline = false)
        {
            AddPlot(x, y, name, Color.Black, width, isSpline);
        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotBlack(Vector x, Vector y, string name = "", int width = 2, bool isSpline = false)
        {
            Text = name;
            chartVisual1.PlotBlack(x, y, name, width, isSpline);
        }



        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotComplex(Vector x, ComplexVector y, string name = "", int width = 2, bool isSpline = false)
        {
            Text = name;
            chartVisual1.PlotComplex(x, y, name, width, isSpline);
        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void AddPlotBlack(Vector y, string name = "", int width = 2, bool isSpline = false)
        {
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddPlot(x, y, name, Color.Black, width, isSpline);
        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotBlack(Vector y, string name = "", int width = 1, bool isSpline = false)
        {
            Text = name;
            chartVisual1.PlotBlack(y, name, width, isSpline);
        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotComplex(ComplexVector y, string name = "", int width = 2, bool isSpline = false)
        {
            Text = name;
            chartVisual1.PlotComplex(y, name, width, isSpline);
        }

        /// <summary>
		/// Создание графика с данными
		/// </summary>
		public void AddRadialPlot(Vector x, Vector y, string name, Color color, int width = 2)
        {
            chartVisual1.AddRadialPlot(x, y, name, color, width);
        }


        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void AddRadialDegPlot(Vector x, Vector y, string name, Color color, int width = 2)
        {
            AddRadialPlot(x / 180.0 * Math.PI, y, name, color, width);
        }



        /// <summary>
        /// Радиальный график
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        /// <param name="width"></param>
        public void RadPlotBlueDeg(Vector y, string name = "", int width = 1)
        {
            Text = name;
            chartVisual1.RadPlotBlueDeg(y, name, width);
        }


        #endregion

        #region Bar
        /// <summary>
		/// Создание гистограммы с данными
		/// </summary>
		public void AddBar(Vector x, Vector y, string name, Color color)
        {
            chartVisual1.AddBar(x, y, name, color);
        }

        /// <summary>
        /// Добавляет область
        /// </summary>
        /// <param name="x">Ось X</param>
        /// <param name="y">Ось Y</param>
        /// <param name="name">Имя</param>
        /// <param name="color">Цвет</param>
        public void AddArea(Vector x, Vector y, string name, Color color)
        {
            chartVisual1.AddArea(x, y, name, color);
        }

        /// <summary>
        /// Создание гистограммы с данными
        /// </summary>
        public void AddBarBlack(Vector x, Vector y, string name = "")
        {
            AddBar(x, y, name, Color.Black);
        }


        /// <summary>
        /// Создание гистограммы с данными
        /// </summary>
        public void AddBarBlack(Vector y, string name = "")
        {
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddBar(x, y, name, Color.Black);
        }

        /// <summary>
        /// Создание гистограммы с данными
        /// </summary>
        public void BarBlack(Vector x, Vector y, string name = "")
        {
            Text = name;
            chartVisual1.BarBlack(x, y, name);
        }


        /// <summary>
        /// Создание гистограммы с данными
        /// </summary>
        public void BarBlack(Vector y, string name = "")
        {
            Text = name;
            chartVisual1.BarBlack(y, name);
        }


        #endregion

        #region Scatter
        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void AddScatter(Vector x, Vector y, string name, Color color)
        {
            chartVisual1.AddScatter(x, y, name, color);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public void AddScatterBlack(Vector y, string name = "")
        {
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddScatter(x, y, name, Color.Black);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void AddScatterBlack(Vector x, Vector y, string name = "")
        {
            AddScatter(x, y, name, Color.Black);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public void ScatterBlack(Vector y, string name = "")
        {
            Text = name;
            chartVisual1.ScatterBlack(y, name);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void ScatterBlack(Vector x, Vector y, string name = "")
        {
            Text = name;
            chartVisual1.ScatterBlack(x, y, name);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public void ScatterComplex(ComplexVector y, string name = "")
        {
            Text = name;
            chartVisual1.ScatterComplex(y, name);
        }



        /// <summary>
        /// Создание скаттерограммы отражающей комплексную плоскость
        /// </summary>
        /// <param name="y">Комплексный вектор</param>
        /// <param name="name">Имя</param>
        /// <param name="xScale">Единица измерения шкалы x</param>
        /// <param name="yScale">Единица измерения шкалы y</param>
        public void ScatterComplexPlane(ComplexVector y, string name = "", string xScale = "", string yScale = "")
        {
            Text = name;
            chartVisual1.ScatterComplexPlane(y, name, xScale, yScale);
        }

        /// <summary>
        /// Очищение графика
        /// </summary>
        public void Clear()
        {
            chartVisual1.Clear();
        }

        #endregion


        #region Преобразования

        /// <summary>
        /// Adding an amplitude spectrum (Hamming window)
        /// </summary>
        /// <param name="x">Time vector</param>
        /// <param name="y">Amplitudes vector</param>
        /// <param name="color">Color</param>
        /// <param name="name">Chart name</param>
        public void AddSpectrum(Vector x, Vector y, Color color, string name)
        {
            double dt = Statistic.MeanStep2(x);
            Vector magn = FFT.CalcFFT(y * WindowForFFT.HammingWindow(y.Count)).MagnitudeVector;
            Vector f = Signal.Frequency(magn.Count, 1.0 / dt);
            magn = magn.CutAndZero(magn.Count / 2);
            magn /= magn.Count;

            AddPlot(f.CutAndZero(f.Count / 2), magn, name, color);
        }

        /// <summary>
        /// Adding an amplitude spectrum (Hamming window)
        /// </summary>
        internal void AddSpectrum(IChartElement element)
        {
            AddSpectrum(element.Data.GetX(), element.Data.GetY(), element.Series.Color, element.Name);
        }


        /// <summary>
        /// Производная
        /// </summary>
        /// <param name="x">Вектор времени</param>
        /// <param name="y">Вектор амплитуд</param>
        /// <param name="color">Цвет</param>
        /// <param name="name">Имя графика</param>
        /// <param name="w">Толщина линии</param>
        public void AddDiff(Vector x, Vector y, Color color, string name, int w)
        {
            double dt = Statistic.MeanStep2(x);

            AddPlot(x, Functions.Diff(y, 1.0 / dt), name, color, w);
        }

        /// <summary>
        /// Интеграл
        /// </summary>
        /// <param name="x">Вектор времени</param>
        /// <param name="y">Вектор амплитуд</param>
        /// <param name="color">Цвет</param>
        /// <param name="name">Имя графика</param>
        /// /// <param name="w">Толщина линии</param>
        public void AddIntegr(Vector x, Vector y, Color color, string name, int w)
        {
            double dt = Statistic.MeanStep(x);

            AddPlot(x, Functions.Integral(y, 1.0 / dt), name, color, w);
        }

        internal void AddIntegr(IChartElement element)
        {
            AddIntegr(element.Data.GetX(), element.Data.GetY(), element.Series.Color, element.Name, 2);
        }

        internal void AddDiff(IChartElement element)
        {
            AddDiff(element.Data.GetX(), element.Data.GetY(), element.Series.Color, element.Name, 2);
        }

        internal void AddHistoramm(IChartElement element)
        {
            AddHistoramm(element.Data.GetY(), element.Series.Color, element.Name);
        }


        /// <summary>
        /// Гистограммы
        /// </summary>
        /// <param name="y">Вектор значений</param>
        /// <param name="color">Цвет</param>
        /// <param name="name">Имя</param>
        public void AddHistoramm(Vector y, Color color, string name)
        {
            Statistic statistic = new Statistic(y);
            Histogramm histogramm = statistic.Histogramm((int)(2 * Math.Log(y.Count, 2)));
            AddArea(histogramm.X, histogramm.Y, name, color);
        }



        #endregion

    }
}
