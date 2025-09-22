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
        public static unsafe Matrix SpatialFilter(Matrix img, Matrix filter)
        {
            int imgHeight = img.Height;
            int imgWidth = img.Width;
            int filterHeight = filter.Height;
            int filterWidth = filter.Width;

            int halfFilterHeight = filterHeight / 2;
            int halfFilterWidth = filterWidth / 2;

            Matrix result = new Matrix(imgHeight, imgWidth);

            // Копируем данные в массивы
            double[] filterData = new double[filterHeight * filterWidth];
            double[] imgData = new double[imgHeight * imgWidth];
            double[] resultData = new double[imgHeight * imgWidth];

            for (int i = 0; i < filterHeight; i++)
                for (int j = 0; j < filterWidth; j++)
                    filterData[i * filterWidth + j] = filter[i, j];

            for (int i = 0; i < imgHeight; i++)
                for (int j = 0; j < imgWidth; j++)
                    imgData[i * imgWidth + j] = img[i, j];

            // Используем параллелизм БЕЗ unsafe
            Parallel.For(0, imgHeight, y =>
            {
                for (int x = 0; x < imgWidth; x++)
                {
                    double sum = 0;

                    for (int fy = 0; fy < filterHeight; fy++)
                    {
                        int imgY = y - halfFilterHeight + fy;

                        if (imgY >= 0 && imgY < imgHeight)
                        {
                            for (int fx = 0; fx < filterWidth; fx++)
                            {
                                int imgX = x - halfFilterWidth + fx;

                                if (imgX >= 0 && imgX < imgWidth)
                                {
                                    sum += filterData[fy * filterWidth + fx] *
                                           imgData[imgY * imgWidth + imgX];
                                }
                            }
                        }
                    }

                    resultData[y * imgWidth + x] = sum;
                }
            });

            // Копируем результат обратно
            for (int i = 0; i < imgHeight; i++)
                for (int j = 0; j < imgWidth; j++)
                    result[i, j] = resultData[i * imgWidth + j];

            return result;
        }




        /// <summary>
        /// Median grayscale filter
        /// </summary>
        /// <param name="img">Image matrix</param>
        /// <param name="filter">Filter matrix</param>
        public static Matrix MedianFilterMask(Matrix img, Matrix filter)
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
        /// Median grayscale filter
        /// </summary>
        /// <param name="img">Image matrix</param>
        /// <param name="filter">Filter matrix</param>
        public static Matrix MedianFilter(Matrix img, int h = 3, int w =3)
        {
            int H = img.Height - h + 1, W = img.Width - w;
            Matrix newMatr = new Matrix(H, W);

            _ = Parallel.For(0, H, i =>
            {
                for (int j = 0; j < W; j++)
                {
                    newMatr[i, j] = FilterMedian(img, w, h, j, i);
                }
            });

            return newMatr;

        }

        private static double FilterMedian(Matrix img, int w, int h, int dx, int dy)
        {

            List<double> ld = new List<double>();

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    ld.Add(img[dy + i, dx + j]);

            ld.Sort();

            return ld[ld.Count / 2];
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
