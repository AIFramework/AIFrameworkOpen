/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 12.01.2019
 * Время: 2:31
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.ComputerVision;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.Data
{
    /// <summary>
    /// Description of DataMethods.
    /// </summary>
    public static class DataMethods
    {


        /// <summary>
        /// Прореживание данных, чтобы на графике не было большого числа точек
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Tuple<Vector, Vector> ReducDataPlot(Vector x, Vector y)
        {
            int n;

            if (y.Count > 200000)
            {
                n = 25000;
            }
            else if (y.Count > 60000)
            {
                n = 15000;
            }
            else
            {
                n = 5500;
            }

            int step = y.Count / n;

            if (step > 1)
            {
                int r = 10000;
                r = (y.Count > r) ? r : y.Count;
                double k = (double)r / y.Count; // Коэффициент приведения размерности


                double freq = Statistics.Statistic.MeanFreq(y.CutAndZero(r), r);
                bool isKotel = 2.1 * freq < k * n; // Проверка выполнения теоремы Котельникова


                if (/*x.Count > 25000 && */!isKotel)
                {
                    return ComplexReduc(x, y, step);
                }
                else
                {
                    return SimpleReduc(x, y, step);
                }
            }
            else
            {
                return new Tuple<Vector, Vector>(x, y);
            }

        }

        /// <summary>
        /// Прореживание данных, чтобы на графике не было большого числа точек
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Tuple<Vector, Vector> ReducDataRadialPlot(Vector x, Vector y)
        {
            int n = 700;
            int step = y.Count / n;
            List<double> x1 = new List<double>();
            List<double> y1 = new List<double>();

            if (step > 1)
            {
                for (int i = 0; i < y.Count; i += step)
                {
                    if (i < x.Count)
                    {
                        x1.Add(x[i]);
                        y1.Add(y[i]);
                    }
                }

                return new Tuple<Vector, Vector>(Vector.FromList(x1), Vector.FromList(y1));
            }
            else
            {
                return new Tuple<Vector, Vector>(x, y);
            }

        }

        /// <summary>
        /// Прореживание данных, чтобы на графике не было большого числа точек
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Tuple<Vector, Vector> ReducData40000(Vector x, Vector y)
        {
            int n = 40000;
            int step = y.Count / n;
            List<double> x1 = new List<double>();
            List<double> y1 = new List<double>();

            if (step > 1)
            {
                for (int i = 0, r = step / 2; i < y.Count - r; i += step)
                {
                    x1.Add(x[i]);
                    y1.Add(y[i]);
                }

                return new Tuple<Vector, Vector>(Vector.FromList(x1), Vector.FromList(y1));
            }
            else
            {
                return new Tuple<Vector, Vector>(x, y);
            }

        }



        /// <summary>
        /// Получение изображения графика 
        /// </summary>
        /// <param name="chart">График</param>
        public static Bitmap ImageFromChart(Chart chart)
        {
            Bitmap bmp = new Bitmap(chart.Width, chart.Height);

            chart.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

            return bmp;
        }

        /// <summary>
        /// Получение среднего цвета из картинки
        /// </summary>
        /// <param name="path">Путь до картинки</param>
        public static Color GetColorForStyle(string path)
        {
            Bitmap bmp = new Bitmap(path);
            bmp = new Bitmap(bmp, 20, 20);
            Tensor ten = ImageMatrixConverter.BmpToTensor(bmp);

            double r = 0, g = 0, b = 0;

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    r += ten[i, j, 0];
                    g += ten[i, j, 1];
                    b += ten[i, j, 2];
                }
            }

            return Color.FromArgb((int)(r / 400.0), (int)(g / 400.0), (int)(b / 400.0));

        }


        private static Tuple<Vector, Vector> ComplexReduc(Vector x, Vector y, int pool)
        {



            int newDim = x.Count / pool;
            int points = 256; // Разбика экрана
            int pool2 = y.Count / points;



            Vector xNew = new Vector(newDim), yNew = new Vector(newDim);
            float[] absY = new float[y.Count];

            for (int i = 0; i < y.Count; i += pool2)
            {
                double mY = 0, n = 0;

                for (int j = 0; j < pool2; j++)
                {
                    int k = i + j;
                    if (k < y.Count)
                    {
                        if (!double.IsNaN(y[k]))
                        {
                            mY += y[k];
                            n++;
                        }
                    }
                }

                mY /= n;

                for (int j = 0; j < pool2; j++)
                {
                    int k = i + j;
                    if (k < y.Count)
                    {
                        absY[k] = (float)Math.Abs(y[k] - mY);
                    }
                }
            }





            for (int w = 0, w2 = 0; w2 < newDim; w += pool, w2++)
            {
                int i = w;
                for (int dx = 0; dx < pool; dx++)
                {
                    int x1 = w + dx;

                    if (absY[i] < absY[x1])
                    {
                        i = x1;
                    }
                }

                yNew[w2] = y[i];
                xNew[w2] = x[w];
            }

            return new Tuple<Vector, Vector>(xNew, yNew);
        }


        private static Tuple<Vector, Vector> SimpleReduc(Vector x, Vector y, int pool)
        {
            List<double> x1 = new List<double>();
            List<double> y1 = new List<double>();


            for (int i = 0, r = pool / 2; i < y.Count - r; i += pool)
            {
                x1.Add(x[i]);
                y1.Add(y[i]);
            }

            return new Tuple<Vector, Vector>(Vector.FromList(x1), Vector.FromList(y1));
        }
    }
}
