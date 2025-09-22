using AI.DataStructs.Algebraic;
using System;
using System.Diagnostics; // Для Stopwatch, если захотите измерить производительность

namespace AI.ComputerVision.Statistics
{
    /// <summary>
    /// Предоставляет статические методы для анализа и эквализации (выравнивания) гистограмм изображений.
    /// Реализация оптимизирована для производительности.
    /// </summary>
    public static class ImageHistogram
    {
        private const int IntensityLevels = 256;

        /// <summary>
        /// Выполняет эквализацию (выравнивание) гистограммы изображения.
        /// Этот процесс увеличивает глобальный контраст изображения, что полезно для изображений,
        /// которые выглядят "блеклыми" или имеют слишком темные/светлые участки.
        /// </summary>
        /// <param name="image">Исходное изображение в виде матрицы. Ожидаются значения в диапазоне [0, 255].</param>
        /// <returns>Новая матрица с выровненной гистограммой.</returns>
        /// <exception cref="ArgumentNullException">Если входное изображение равно null.</exception>
        public static Matrix Equalize(Matrix image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image), "Исходное изображение не может быть null.");

            int pixelCount = image.Data.Length;
            if (pixelCount == 0)
                return new Matrix(image.Height, image.Width); 

           
            var histogram = new long[IntensityLevels];
            foreach (double pixel in image.Data)
            {
                // На случай выхода за пределы диапазона [0, 255]
                int intensity = (int)Math.Max(0, Math.Min(IntensityLevels - 1, pixel));
                histogram[intensity]++;
            }

            // Функция распределения (CDF)
            var cdf = new long[IntensityLevels];
            cdf[0] = histogram[0];
            for (int i = 1; i < IntensityLevels; i++)
            {
                cdf[i] = cdf[i - 1] + histogram[i];
            }

            // Минимальное ненулевое значение CDF
            // Это значение нужно для корректного масштабирования в диапазон [0, 255]
            long cdfMin = 0;
            for (int i = 0; i < IntensityLevels; i++)
            {
                if (cdf[i] > 0)
                {
                    cdfMin = cdf[i];
                    break;
                }
            }

            if (pixelCount == cdfMin)
                return image.Copy(); 

            // Таблица преобразования
            var lut = new double[IntensityLevels];
            double denominator = pixelCount - cdfMin;

            for (int i = 0; i < IntensityLevels; i++)
                lut[i] = Math.Round(255.0 * (cdf[i] - cdfMin) / denominator);
            

            // Эквализация
            var equalizedImage = new Matrix(image.Height, image.Width);
            for (int i = 0; i < pixelCount; i++)
            {
                int intensity = (int)Math.Max(0, Math.Min(IntensityLevels - 1, image.Data[i]));
                equalizedImage.Data[i] = lut[intensity];
            }

            return equalizedImage;
        }

        /// <summary>
        /// Вычисляет гистограмму распределения яркостей для изображения.
        /// </summary>
        /// <param name="image">Исходное изображение.</param>
        /// <returns>Вектор размерностью 256, где индекс - уровень яркости, а значение - количество пикселей.</returns>
        public static Vector GetHistogram(Matrix image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var histogramVector = new Vector(IntensityLevels);
            foreach (double pixel in image.Data)
            {
                int intensity = (int)Math.Max(0, Math.Min(IntensityLevels - 1, pixel));
                histogramVector[intensity]++;
            }
            return histogramVector;
        }

    }
}