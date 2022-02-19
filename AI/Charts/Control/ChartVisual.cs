using AI.Charts.ChartElements;
using AI.Charts.Data;
using AI.Charts.Forms;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AI.Charts.Control
{
    /// <summary>
    /// Визуально представление данных (Графики)
    /// </summary>
    public partial class ChartVisual : UserControl
    {
        #region Свойства

        /// <summary>
        /// Можно ли перемещать график
        /// </summary>
        public bool IsMoove { get; set; } = true;

        /// <summary>
        /// Можно ли масштабировать
        /// </summary>
        public bool IsScale { get; set; } = true;


        /// <summary>
        /// Выводить ли значения x,y
        /// </summary>
        public bool IsShowXY { get; set; } = true;


        /// <summary>
        /// Использовать ли контекстное меню
        /// </summary>
        public bool IsContextMenu
        {
            get => chart1.ContextMenuStrip == contextMenu;

            set
            {
                if (value)
                {
                    chart1.ContextMenuStrip = contextMenu;
                }
                else
                {
                    chart1.ContextMenuStrip = null;
                }
            }
        }

        /// <summary>
        /// Имя графика
        /// </summary>
        public string ChartName
        {
            get => chart1.Titles[0].Text;
            set => chart1.Titles[0].Text = value;
        }

        /// <summary>
        /// Имя оси X
        /// </summary>
        public string LabelX
        {
            get => chart1.ChartAreas[0].AxisX.Title;
            set => chart1.ChartAreas[0].AxisX.Title = value;
        }

        /// <summary>
        /// Имя оси Y
        /// </summary>
        public string LabelY
        {
            get => chart1.ChartAreas[0].AxisY.Title;
            set => chart1.ChartAreas[0].AxisY.Title = value;
        }


        /// <summary>
        /// График в логарифмическом масштабе
        /// </summary>
        public bool IsLogScale
        {
            get => chart1.ChartAreas[0].AxisY.IsLogarithmic;
            set => chart1.ChartAreas[0].AxisY.IsLogarithmic = value;
        }


        private readonly List<IChartElement> chartElements = new List<IChartElement>();

        #endregion



        /// <summary>
        /// Графики
        /// </summary>
        public ChartVisual()
        {
            InitializeComponent();
            chart1.MouseWheel += Chart1_MouseWheel;
            Clear();
        }





        /// <summary>
        /// Визуализация графиков
        /// </summary>
        /// <param name="chartDatas">Данные графиков</param>
        public void VisualData(ChartData chartDatas)
        {

            LabelX = chartDatas[0].DescriptionData.X;
            LabelY = chartDatas[0].DescriptionData.Y;
            ChartName = chartDatas.ChartName;


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
        public void AddPlot(Vector x, Vector y, string name, Color? color = null, int width = 2, bool isSpline = false)
        {
            Plot plot = new Plot(chart1, name) { IsSpline = isSpline };
            plot.LoadData(x, y);
            if(color != null)
            plot.SetColor(color.Value);
            plot.SetWidth(width);
            chartElements.Add(plot);
            AutoScale();
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
            Clear();
            AddPlot(x, y, name, Color.Black, width, isSpline);
        }



        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotComplex(Vector x, ComplexVector y, string name = "", int width = 2, bool isSpline = false)
        {
            Clear();
            AddPlot(x, y.RealVector, name + " [Real]", Color.Blue, width, isSpline);
            AddPlot(x, y.ImaginaryVector, name + " [Imaginary]", Color.Red, width, isSpline);
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
            Clear();
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            chart1.Titles[0].Text = string.Empty;
            chart1.ChartAreas[0].AxisX.Title = string.Empty;
            chart1.ChartAreas[0].AxisY.Title = string.Empty;
            AddPlot(x, y, name, Color.Black, width, isSpline);
        }

        /// <summary>
        /// Создание графика с данными
        /// </summary>
        public void PlotComplex(ComplexVector y, string name = "", int width = 2, bool isSpline = false)
        {
            Clear();
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddPlot(x, y.RealVector, name + " [Real]", Color.Blue, width, isSpline);
            AddPlot(x, y.ImaginaryVector, name + " [Imaginary]", Color.Red, width, isSpline);
        }

        /// <summary>
		/// Создание графика с данными
		/// </summary>
		public void AddRadialPlot(Vector x, Vector y, string name, Color color, int width = 2)
        {
            RadialPlot radialPlot = new RadialPlot(chart1, name);
            radialPlot.LoadData(x, y);
            radialPlot.SetColor(color);
            radialPlot.SetWidth(width);
            chartElements.Add(radialPlot);
            AutoScale();
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
            Clear();
            double end = y.Count;
            double step = 360.0 / end;
            Vector x = Vector.SeqBeginsWithZero(step, end);
            x = x.CutAndZero(y.Count);

            AddRadialDegPlot(x, y, name, Color.Blue, width);
        }


        #endregion


        public void AddCircul(Vector x, Vector y, string name)
        {
            Circul circul = new Circul(chart1, name);
            circul.LoadData(x, y);
            chartElements.Add(circul);
            AutoScale();

        }


        public void AddArea(Vector x, Vector y, string name, Color color)
        {
            Area area = new Area(chart1, name);
            area.LoadData(x, y);
            area.SetColor(color);
            chartElements.Add(area);
            AutoScale();
        }

        #region Bar
        /// <summary>
		/// Создание гистограммы с данными
		/// </summary>
		public void AddBar(Vector x, Vector y, string name, Color color)
        {
            Bar bar = new Bar(chart1, name);
            bar.LoadData(x, y);
            bar.SetColor(color);
            chartElements.Add(bar);
            AutoScale();

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
            Clear();
            AddBar(x, y, name, Color.Black);
        }


        /// <summary>
        /// Создание гистограммы с данными
        /// </summary>
        public void BarBlack(Vector y, string name = "")
        {
            Clear();
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddBar(x, y, name, Color.Black);
        }


        #endregion

        #region Scatter
        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void AddScatter(Vector x, Vector y, string name, Color color)
        {
            ScatterPlot scatter = new ScatterPlot(chart1, name);
            scatter.LoadData(x, y);
            scatter.SetColor(color);
            scatter.AutoSetMarkSize();
            chartElements.Add(scatter);
            AutoScale();
        }


        internal void AddElement(IChartElement element)
        {
            chartElements.Add(element);
            AutoScale();
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void AddScatterMark3(Vector x, Vector y, string name, Color color)
        {
            ScatterPlot scatter = new ScatterPlot(chart1, name);
            scatter.LoadData(x, y);
            scatter.SetColor(color);
            scatter.SetMarkSize(3);
            chartElements.Add(scatter);
            AutoScale();
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        public void AddScatterMark6(Vector x, Vector y, string name, Color color)
        {
            ScatterPlot scatter = new ScatterPlot(chart1, name);
            scatter.LoadData(x, y);
            scatter.SetColor(color);
            scatter.SetMarkSize(6);
            chartElements.Add(scatter);
            AutoScale();
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
        /// <param name="y"></param>
        /// <param name="name"></param>
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
            Clear();
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddScatter(x, y, name, Color.Black);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public void ScatterBlack(Vector x, Vector y, string name = "")
        {
            Clear();
            AddScatter(x, y, name, Color.Black);
        }

        /// <summary>
        /// Создание скаттерограммы с данными
        /// </summary>
        /// <param name="y"></param>
        /// <param name="name"></param>
        public void ScatterComplex(ComplexVector y, string name = "")
        {
            Clear();
            Vector x = Vector.SeqBeginsWithZero(1, y.Count);
            AddScatter(x, y.RealVector, name + " [Real]", Color.Blue);
            AddScatter(x, y.ImaginaryVector, name + " [Imaginary]", Color.Red);
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
            Clear();
            ChartName = name;

            LabelX = (xScale != "") ? "Real [" + xScale + "]" : "Real";
            LabelY = (yScale != "") ? "Imaginary [" + yScale + "]" : "Imaginary";

            AddScatter(y.RealVector, y.ImaginaryVector, name, Color.Green);
        }
        /// <summary>
        /// Создание скаттерограммы отражающей комплексную плоскость
        /// </summary>
        /// <param name="y">Комплексный вектор</param>
        /// <param name="name">Имя</param>
        /// <param name="xScale">Единица измерения шкалы x</param>
        /// <param name="yScale">Единица измерения шкалы y</param>
        public void ScatterComplexPlaneWithRing1(ComplexVector y, string name = "", string xScale = "", string yScale = "")
        {
            Clear();
            ChartName = name;

            LabelX = (xScale != "") ? "Real [" + xScale + "]" : "Real";
            LabelY = (yScale != "") ? "Imaginary [" + yScale + "]" : "Imaginary";

            Vector x = Vector.Seq(-1, 0.001, 1);
            x = x.InterpolayrZero(2);
            Vector y1 = new Vector(x.Count);

            for (int i = 0; i < x.Count; i += 2)
            {
                y1[i] = Math.Sqrt(1 - x[i] * x[i]);
                y1[i + 1] = -y1[i];
            }

            AddScatterMark3(x, y1, "", Color.Black);
            AddScatterMark6(y.RealVector, y.ImaginaryVector, string.Empty, Color.Green);
        }
        #endregion


        /// <summary>
        ///  Отрисовка графика
        /// </summary>
        public Bitmap ChartImg()
        {
            Bitmap bmp = new Bitmap(Width, Height);

            DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

            return bmp;
        }


        /// <summary>
        /// Очистка графика
        /// </summary>
        public void Clear()
        {
            foreach (IChartElement item in chartElements)
            {
                chart1.Series.Remove(item.Series);
            }

            chartElements.Clear();
        }

        #region Контекстное меню

        // Выбор фона/стиля
        private void выборФонаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                chart1.BackImage = ofd.FileName;

                Color meanColor = DataMethods.GetColorForStyle(ofd.FileName);
                Color inversColor = Color.FromArgb(255 - meanColor.R, 255 - meanColor.G, 255 - meanColor.B);

                // Задний фон и сетка
                chart1.ChartAreas[0].BackColor = meanColor;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = inversColor;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = inversColor;
                // Оси(цифры)
                chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = inversColor;
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = inversColor;
                chart1.ChartAreas[0].AxisX.LineColor = inversColor;
                chart1.ChartAreas[0].AxisY.LineColor = inversColor;
                // Названия
                chart1.ChartAreas[0].AxisX.TitleForeColor = inversColor;
                chart1.ChartAreas[0].AxisY.TitleForeColor = inversColor;
                chart1.Titles[0].ForeColor = inversColor;

            }
        }

        // Сохранение в буфер обмена
        private void отправитьИзображениеВБуферОбменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetImage(ChartImg());
            MessageBox.Show("Изображение в буффере обмена!", "Информация");
        }

        // Сохранение в файл
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "(.jpg)|*.jpg"
            };

            if (DialogResult.OK == sfd.ShowDialog())
            {
                ChartImg().Save(sfd.FileName);
            }
        }

        // Масштабирование
        private void масштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoScale();
        }

        //Вывод в новом окне
        private void NewWindowOutp_Click(object sender, EventArgs e)
        {
            FormChart fChart = new FormChart
            {
                ChartName = ChartName,
                LabelX = LabelX,
                LabelY = LabelY
            };

            foreach (IChartElement item in chartElements)
            {
                fChart.AddChartElement(item);
            }

            fChart.Show();
        }

        //Спектр
        private void СпектрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormChart fChart = new FormChart
            {
                ChartName = "Amplitude spectrum (Hamming window)",
                LabelX = (LabelX == "X-axis" || LabelX.Contains("s")) ? "Hz" : "1/" + LabelX,
                LabelY = LabelY
            };

            foreach (IChartElement item in chartElements)
            {
                fChart.AddSpectrum(item);
            }

            fChart.Show();
        }

        //Гистограмма
        private void ГистограммаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormChart fChart = new FormChart
            {
                ChartName = "Гистограмма",
                LabelX = (LabelY == "Ось Y") ? "Значения функции" : LabelY,
                LabelY = "Вероятность попадания в интервал p(x)"
            };

            foreach (IChartElement item in chartElements)
            {
                fChart.AddHistoramm(item);
            }

            fChart.Show();
        }

        //Производная
        private void Diff_Click(object sender, EventArgs e)
        {

            FormChart fChart = new FormChart
            {
                ChartName = ChartName,
                LabelX = LabelX,
                LabelY = LabelY.Contains("[Производная]") ? LabelY : LabelY + " [Производная]"
            };

            foreach (IChartElement item in chartElements)
            {
                fChart.AddDiff(item);
            }

            fChart.Show();
        }

        //Интеграл
        private void Integ_Click(object sender, EventArgs e)
        {
            FormChart fChart = new FormChart
            {
                ChartName = ChartName,
                LabelX = LabelX,
                LabelY = LabelY.Contains("[Интеграл]") ? LabelY : LabelY + " [Интеграл]"
            };

            foreach (IChartElement item in chartElements)
            {
                fChart.AddIntegr(item);
            }

            fChart.Show();
        }

        #endregion


        #region Масштабирование

        private bool startM = true, endM;
        private int xMouseE, xMouseB, yMouseE, yMouseB, x, y, wM, hM;
        private double xC, yC;
        private Point mouseOld = Point.Empty;
        private int dxInt = 0;

        private void LabelXY_Click(object sender, EventArgs e)
        {

        }





        //Прокрутка мыши
        private void Chart1_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0)
            {
                MouseScale(0.01, e.X, e.Y);
            }
            else
            {
                MouseScale(-0.01, e.X, e.Y);
            }
        }


        //Движение мыши
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                if (chart1.Series.Count > 0)
                {
                    // Зажата левая кнопка мыши
                    if (e.Button == MouseButtons.Left)
                    {
                        // Перетаскивание графика
                        if (ModifierKeys == Keys.Control && IsMoove)
                        {
                            double
                               xV = GetValueX(e.X),
                               yV = GetValueY(e.Y),
                               maxX = MaxX(),
                               maxY = MaxY(),
                               minX = MinX(),
                               minY = MinY(),
                               xVOld = GetValueX(mouseOld.X),
                               yVOld = GetValueY(mouseOld.Y),
                               dX = xV - xVOld,
                               dY = yV - yVOld;

                            if (IsLogScale)
                            {
                                dY = (Math.Pow(10, yV) - Math.Pow(10, yVOld)) / 10;
                            }

                            double
                            newMaxX = maxX - dX,
                            newMinX = minX - dX,
                            newMaxY = maxY - dY,
                            newMinY = minY - dY;

                            if (IsLogScale)
                            {
                                newMaxY = newMaxY > 0 ? newMaxY : 1e-200;
                                newMinY = newMinY > 0 ? newMinY : 1e-200;
                            }

                            if (((newMaxX - newMinX) > 0) && ((newMaxY - newMinY) > 0))
                            {
                                SetScale(newMinX, newMaxX, newMinY, newMaxY);
                                dxInt += e.X - mouseOld.X;

                                if (Math.Abs(dxInt) > chart1.Width / 4)
                                {
                                    Rec(); // Перерасчет масштаба
                                    dxInt = 0;
                                }

                            }
                        }
                        // ---------------------------------------------------------------------------------------------------------------//
                        // Выделение зоны интереса
                        else
                        {

                            if (startM)
                            {
                                startM = false;
                                endM = true;

                                xMouseB = e.X;
                                yMouseB = e.Y;
                            }

                            xMouseE = e.X;
                            yMouseE = e.Y;

                            Refresh();
                        }
                    }



                    mouseOld.X = e.X;
                    mouseOld.Y = e.Y;


                    if (chart1.Series[0].Points.Count > 0)
                    {
                        xC = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                        yC = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

                        if (IsLogScale)
                        {
                            yC = Math.Pow(10, yC);
                        }

                        if (IsShowXY)
                        {
                            labelXY.Text = "X: " + Math.Round(xC, 6) + "  Y:" + Math.Round(yC, 6);
                        }
                        else
                        {
                            labelXY.Text = "";
                        }

                    }
                }

            }

            catch { }


        }

        private void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            if (endM)
            {
                ScaleNonRepaint();
                startM = true;
                xMouseB = 0;
                xMouseE = 0;
                yMouseB = 0;
                yMouseE = 0;
                Refresh();
                endM = false;
            }

        }

        // Отрисовка фигур на контроле
        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            if (IsScale)
            {
                x = xMouseB > xMouseE ? xMouseE : xMouseB;
                y = yMouseB > yMouseE ? yMouseE : yMouseB;

                wM = Math.Abs(xMouseB - xMouseE);
                hM = Math.Abs(yMouseB - yMouseE);

                e.Graphics.DrawRectangle(new Pen(Color.Red, 1), x, y, wM, hM);
            }
        }


        // Масштабирование прямоугольник
        private void ScaleNonRepaint()
        {
            if (IsScale)
            {
                x = (x < 0) ? x = 0 : x; // проверка x, нижний предел
                y = (y < 0) ? y = 0 : y; // проверка y, нижний предел


                try
                {

                    double
                        xb = GetValueX(x),
                        xe = GetValueX(x + wM),
                        ye = GetValueY(y),
                        yb = GetValueY(y + hM);

                    // для логарифмических графиков
                    if (IsLogScale)
                    {
                        ye = Math.Pow(10, ye);
                        yb = Math.Pow(10, yb);
                    }

                    Vector vstep = chartElements[0].Data.GetX();
                    double step = vstep[1] - vstep[0];

                    if (Math.Abs(xe - xb) > step && ye - yb > 0)
                    {
                        SetScale(xb, xe, yb, ye);
                        Rec();
                    }


                }
                catch { }
            }
        }

        private int stepsMouse = 0;

        // Прокрутка мыши
        private void MouseScale(double scale, int xC, int yC)
        {
            try
            {

                if (IsScale)
                {
                    double
                       xV = GetValueX(xC),
                       yV = GetValueY(yC);

                    double lenX = 0;
                    double lenY = 0;
                    lenX = MaxX() - MinX();
                    lenY = MaxY() - MinY();
                    double
                        newMaxX = MaxX() + scale * lenX / 2,
                        newMinX = MinX() - scale * lenX / 2,
                        newMaxY,
                        newMinY;



                    if (IsLogScale)
                    {
                        newMinY = MinY() - scale * lenY / 10;
                        newMaxY = MaxY() + scale * lenY / 10;
                        newMinY = newMinY > 0 ? newMinY : 1e-200;

                    }
                    else
                    {
                        newMinY = MinY() - scale * lenY;
                        newMaxY = MaxY() + scale * lenY;
                    }

                    Vector vstep = chartElements[0].Data.GetX();
                    double step = vstep[1] - vstep[0];

                    if (Math.Abs(newMaxX - newMinX) > step && ((newMaxY - newMinY) > 0))
                    {
                        // Масштабирование по оси X
                        if (ModifierKeys == Keys.Shift)
                        {
                            SetScaleX(newMinX, newMaxX);
                            if (stepsMouse % 4 == 0)
                            {
                                Rec();
                            }

                            stepsMouse++;
                        }
                        // Масштабирование по оси Y
                        else if (ModifierKeys == Keys.Control)
                        {
                            SetScaleY(newMinY, newMaxY);
                        }

                        // Масштабирование по оси X и Y
                        else
                        {
                            SetScale(newMinX, newMaxX, newMinY, newMaxY);
                            if (stepsMouse % 2 == 0)
                            {
                                Rec();
                            }

                            stepsMouse++;
                        }


                    }
                }
            }
            catch { }

        }

        /// <summary>
        /// Масштабирование по умолчанию
        /// </summary>
        public void AutoScale()
        {
            ScaleData scale = chartElements.GetEnumerator().GetScaleData();
            double xMin = scale.MinX, xMax = scale.MaxX, yMin = scale.MinY, yMax = scale.MaxY, yMin2, yMax2;

            if (IsLogScale)
            {
                if (yMin == 0)
                {
                    throw new Exception("При использовании логарифмического масштаба, значение 0 не допустимо");
                }

                if (yMin < 0)
                {
                    throw new Exception("При использовании логарифмического масштаба, значения ниже нуля не допустимы");
                }
            }
            double dY = Math.Abs(yMax - yMin);
            yMin2 = yMin - 0.2 * dY;
            yMax2 = yMax + dY * 0.2;


            if (IsLogScale)
            {
                yMax2 = (yMax2 > 0) ? yMax : 1e-200;
                yMin2 = (yMin2 > 0) ? yMin2 : 1e-200;
            }

            if (yMin2 == yMax2)
            {
                yMax2 = 1;
            }

            SetScale(xMin, xMax, yMin2, yMax2);
            SetFormat();
            Rec();
        }





        private void Rec()
        {
            double min = MinX(), max = MaxX();

            foreach (IChartElement item in chartElements)
            {
                item.Recalc(min, max);
            }
        }


        // Установка масштаба
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetScale(double xMin, double xMax, double yMin, double yMax)
        {
            SetScaleX(xMin, xMax);
            SetScaleY(yMin, yMax);
            SetFormat();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetScaleX(double xMin, double xMax)
        {
            chart1.ChartAreas[0].AxisX.Maximum = xMax;
            chart1.ChartAreas[0].AxisX.Minimum = xMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetScaleY(double yMin, double yMax)
        {
            chart1.ChartAreas[0].AxisY.Maximum = yMax;
            chart1.ChartAreas[0].AxisY.Minimum = yMin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetFormat()
        {
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0.000000}";
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{0.000000}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetValueX(int xPosition)
        {
            return chart1.ChartAreas[0].AxisX.PixelPositionToValue(xPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double GetValueY(int yPosition)
        {
            return chart1.ChartAreas[0].AxisY.PixelPositionToValue(yPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double MaxX()
        {
            return chart1.ChartAreas[0].AxisX.Maximum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double MinX()
        {
            return chart1.ChartAreas[0].AxisX.Minimum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double MaxY()
        {
            return chart1.ChartAreas[0].AxisY.Maximum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double MinY()
        {
            return chart1.ChartAreas[0].AxisY.Minimum;
        }

        #endregion





    }
}
