using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Matrix = AI.DataStructs.Algebraic.Matrix;

namespace AI.Charts.Control
{
    /// <summary>
    /// Тепловая карта
    /// </summary>
    [Serializable]
    public partial class HeatMapControl : UserControl
    {
        /// <summary>
        /// Тепловая карта
        /// </summary>
        public HeatMapControl()
        {
            InitializeComponent();
            bitmapHM = new Bitmap(2, 2);
        }

        private Bitmap grad, bitmapHM;
        private double min, mean, max, len600;

        private void Gradient_SizeChanged(object sender, EventArgs e)
        {

        }

        private void HeatMap_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Задает градиент тепловой карты
        /// </summary>
        private void NewGrad()
        {
            grad = new Bitmap(gradient.Width, 600);

            LinearGradientBrush linGrad = new LinearGradientBrush(
                                      new Point(0, 0),
                                      new Point(0, 600),
                                      Color.Gold,
                                      Color.Blue);

            ColorBlend colorBlend = new ColorBlend(2)
            {
                Colors = new Color[]
            {
                Color.Red,
                Color.Orange,
                Color.Gold,
                Color.GreenYellow,
                Color.Blue
            },

                Positions = new float[]
            {
                0,
                0.25f,
                0.5f,
                0.75f,
                1
            }
            };

            linGrad.InterpolationColors = colorBlend;

            linGrad.GammaCorrection = true;

            Graphics graphics = Graphics.FromImage(grad);
            graphics.FillRectangle(linGrad, 0, 0, grad.Width, grad.Height);
            gradient.Image = grad;
        }

        /// <summary>
        /// Получение цвета из значения 
        /// </summary>
        /// <param name="value">значение</param>
        /// <returns></returns>
        private Color GetColor(double value)
        {
            double position = 599 - ((value - min) / len600);
            return grad.GetPixel(5, (int)position);
        }

        private void MainPict_SizeChanged(object sender, EventArgs e)
        {
            mainPict.Image = ResizeImage(bitmapHM, mainPict.Width, mainPict.Height);
        }


        /// <summary>
        /// Удержание позиций меток
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HeatMap_SizeChanged(object sender, EventArgs e)
        {
            q25.Location = new Point(q25.Location.X, gradient.Location.Y + (int)(0.75 * gradient.Size.Height) - q25.Size.Height);
            q75.Location = new Point(q75.Location.X, gradient.Location.Y + (int)(0.25 * gradient.Size.Height) - q75.Size.Height);
            meanLabel.Location = new Point(meanLabel.Location.X, gradient.Location.Y + (int)(0.5 * gradient.Size.Height) - meanLabel.Size.Height);
        }

        private void DrawHeatMapPix(Matrix matrix)
        {
            bitmapHM = new Bitmap(matrix.Width, matrix.Height);

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    bitmapHM.SetPixel(i, j, GetColor(matrix[j, i]));
                }
            }

            mainPict.Image = ResizeImage(bitmapHM, mainPict.Width, mainPict.Height);
        }

        /// <summary>
        /// Преобразование изображения
        /// Источник: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp/24199315
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            try
            {
                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destImage = new Bitmap(width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using ImageAttributes wrapMode = new ImageAttributes();
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }

                return destImage;
            }
            catch
            {
                return new Bitmap(1, 1);
            }
        }

        /// <summary>
        /// Расчет тепловой карты для матрицы
        /// </summary>
        /// <param name="matrix">Матрица</param>
        public void CalculateHeatMap(Matrix matrix)
        {
            try
            {
                min = matrix.Min();
                max = matrix.Max();
                mean = (max + min) / 2;
                len600 = (max - min) / 599;
                NewGrad();
                minLabel.Text = Math.Round(min, 3).ToString();
                maxLabel.Text = Math.Round(max, 3).ToString();
                meanLabel.Text = Math.Round(mean, 3).ToString();
                q25.Text = Math.Round((mean + min) / 2, 3).ToString();
                q75.Text = Math.Round((max + mean) / 2, 3).ToString();

                DrawHeatMapPix(matrix);

                xValue.Text = matrix.Width + ",0";
                yValue.Text = "0," + matrix.Height + "";
                xyValue.Text = "" + matrix.Width + "," + matrix.Height + "";
            }
            catch { }
        }

        /// <summary>
        /// Расчет тепловой карты для двумерного массива
        /// </summary>
        /// <param name="data">Массив</param>
        public void CalculateHeatMap(double[,] data)
        {
            Matrix matrix = new Matrix(data);
            CalculateHeatMap(matrix);
        }
    }
}
