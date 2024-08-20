using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI.ComputerVision
{
    /// <summary>
    /// Filters for images
    /// </summary>
    public static class ImgFilters
    {

        /// <summary>
        /// Spatial grayscale filter
        /// </summary>
        /// <param name="img">Image matrix</param>
        /// <param name="filter">Filter matrix</param>
        public static Matrix SpatialFilter(Matrix img, Matrix filter)
        {
            int h = img.Height, w = img.Width;
            _ = filter.Height * filter.Width;
            Matrix Ret16Gray = new Matrix(img.Height, img.Width);

            int sX = 1 - filter.Width;
            int y = 1 - filter.Height;

            for (int y1 = 0; y1 < h; y1++, y++)
            {
                int x = sX;
                for (int x1 = 0; x1 < w; x1++, x++)
                {
                    for (int dy = 0; dy < filter.Height; dy++)
                    {
                        int y2 = y + dy;
                        for (int dx = 0; dx < filter.Width; dx++)
                        {
                            int ox = x + dx;
                            if (y2 >= 0 && y2 < h && ox >= 0 && ox < w)
                            {
                                Ret16Gray[y1, x1] += filter[dy, dx] * img[y2, ox];
                            }
                        }
                    }

                }
            }

            return Ret16Gray;

        }




        /// <summary>
        /// Median grayscale filter
        /// </summary>
        /// <param name="img">Image matrix</param>
        /// <param name="filter">Filter matrix</param>
        public static Matrix MedianFilter(Matrix img, Matrix filter)
        {
            int H = img.Height - filter.Height + 1, W = img.Width - filter.Width + 1;
            Matrix newMatr = new Matrix(H, W);

            _ = Parallel.For(0, H, i =>
            {
                for (int j = 0; j < W; j++)
                {
                    newMatr[i, j] = FilterMedian(img, filter, j, i);
                }
            });

            return newMatr;

        }



        /// <summary>
        /// std grayscale filter
        /// </summary>
        /// <param name="img">Image matrix</param>
        /// <param name="filter">Filter matrix</param>
        public static Matrix StdFilter(Matrix img, Matrix filter)
        {
            int H = img.Height - filter.Height + 1, W = img.Width - filter.Width + 1;
            Matrix newMatr = new Matrix(H, W);

            _ = Parallel.For(0, H, i =>
            {
                for (int j = 0; j < W; j++)
                {
                    newMatr[i, j] = FilterSCO(img, filter, j, i);
                }
            });

            return newMatr;

        }



        /// <summary>
        /// Фильтрация функцией
        /// </summary>
        public static Matrix FunctionFilter(Matrix img, Func<Vector, double> func_filter, int w = 3, int h = 3)
        {
            int H = img.Height - h + 1, W = img.Width - w + 1;
            Matrix newMatr = new Matrix(H, W);

            _ = Parallel.For(0, H, i =>
            {
                for (int j = 0; j < W; j++)
                    newMatr[i, j] = FilterF(img, w, h, j, i, func_filter);
            });

            return newMatr;

        }


        // Элемент медианного фильтра
        private static double FilterMedian(Matrix img, Matrix filter, int dx, int dy)
        {

            List<double> ld = new List<double>();

            for (int i = 0; i < filter.Height; i++)
            {
                for (int j = 0; j < filter.Width; j++)
                {
                    ld.Add(img[dy + i, dx + j] * filter[i, j]);
                }
            }

            ld.Sort();

            return ld[ld.Count / 2];
        }


        // Элемент СКО фильтра
        private static double FilterSCO(Matrix img, Matrix filter, int dx, int dy)
        {
            Vector vect = new Vector(filter.Width * filter.Height);

            for (int i = 0, k = 0; i < filter.Height; i++)
                for (int j = 0; j < filter.Width; j++)
                    vect[k++] = img[dy + i, dx + j] * filter[i, j];

            return Statistic.CalcStd(vect);
        }

        // Элемент фильтра-функции
        private static double FilterF(Matrix img, int w, int h, int dx, int dy, Func<Vector, double> func)
        {
            Vector vect = new Vector(w * h);

            for (int i = 0, k = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    vect[k++] = img[dy + i, dx + j];

            return func(vect);
        }

    }
}
