using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.Statistics
{
    /// <summary>
    /// Работа с гистограммами изображений
    /// </summary>
    public static class ImgHist
    {
        /// <summary>
        /// Получение вектора частот
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Vector Freqs(Matrix img)
        {
            Vector fr = new Vector(256);

            for (int i = 0; i < img.Data.Length; i++)
            {
                fr[(int)img[i]]++;
            }

            return fr;
        }

        /// <summary>
        /// Получение интегральной ф-ии
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Vector GetCDF(Matrix img) 
        {
           return  Functions.Integral(Freqs(img));
        }

        // Расчет минимальной ненулевой частоты
        private static double GetMin(Vector freq) 
        {
            double min = double.MaxValue;

            foreach (var item in freq)
            {
                if (item != 0 && item < min) min = item;
            }

            return min;
        }

        /// <summary>
        /// Получение частоты из изображения
        /// </summary>
        /// <param name="value"></param>
        /// <param name="freqs"></param>
        /// <returns></returns>
        public static double FreqFromValue(double value, Vector freqs) 
        {
            return freqs[(int)value];
        }


        /// <summary>
        /// Выравнивание гистограммы
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Matrix EqualizeHist(Matrix img)
        {
            Vector freq = GetCDF(img);
            Matrix normal = new Matrix(img.Height, img.Width);
            double min = GetMin(freq);
            double denom = img.Data.Length - 1;

            for (int i = 0; i < freq.Count; i++)
            {
                if (freq[i] == 0) freq[i] = min;
            }

            for (int i = 0; i < img.Data.Length; i++)
            {
                normal[i] = 255 * (FreqFromValue(img[i], freq) - min) / denom;
            }

            return normal;
        }

        /// <summary>
        /// Выравнивание гистограммы
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Matrix EqualizeHistMiniMax(Matrix img)
        {
            Vector freq = GetCDF(img);
            Matrix normal = new Matrix(img.Height, img.Width);
            double min = GetMin(freq);

            for (int i = 0; i < freq.Count; i++)
            {
                if (freq[i] == 0) freq[i] = min;
            }

            for (int i = 0; i < img.Data.Length; i++)
            {
                normal[i] = FreqFromValue(img[i], freq);
            }

            return normal.Minimax(255);
        }
    }
}
