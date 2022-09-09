using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.ImgTransforms
{
    /// <summary>
    /// Создает гистограмму направленных градиентов
    /// </summary>
    [Serializable]
    public class HOG
    {
        private readonly SobelTransform _sobelTransformer;
        private readonly int _bins;
        private readonly double _k = 0;

        /// <summary>
        /// Гистограмма направленных градиентов
        /// </summary>
        public HOG(int bins = 8)
        {
            _sobelTransformer = new SobelTransform();
            _bins = bins;
            _k = _bins / Math.PI;
        }

        /// <summary>
        /// Гистограмма направленных градиентов
        /// </summary>
        public HOG(Matrix maskY, int bins = 8)
        {
            _sobelTransformer = new SobelTransform(maskY);
            _bins = bins;
            _k = (_bins / Math.PI) - 0.001;
        }

        /// <summary>
        /// Вычисление гистограммы направленных градиентов
        /// </summary>
        /// <param name="img"></param>
        /// <param name="normalyze"></param>
        /// <param name="centrNorm">Нормализация центрального пика, замена значения на среднее</param>
        /// <returns></returns>
        public Vector CalcHist(Matrix img, bool normalyze = false, bool centrNorm = true)
        {
            double[] ph = _sobelTransformer.Transform(img).PhGrad.Data;
            Vector hist = new Vector(_bins);
            int sum = 0;

            for (int i = 0; i < ph.Length; i++)
            {
                int index = (int)(_k * ph[i]);
                hist[index]++;
                sum++;
            }

            if (normalyze)
            {
                hist /= sum;
            }

            if (centrNorm)
            {
                int ids = (_bins / 2) - 1;
                hist[ids] = 0;
                hist[ids] = hist.Mean();
            }

            return hist;
        }
    }
}
