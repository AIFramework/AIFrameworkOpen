using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.ImgTransforms
{
    /// <summary>
    /// Создает гистограмму направленных градиентов
    /// </summary>
    public class HOG
    {
        private SobelTransform _sobelTransformer;
        private int _bins;
        private double _k = 0;

        public HOG(int bins = 8)
        {
            _sobelTransformer = new SobelTransform();
            _bins = bins;
            _k = _bins / Math.PI;
        }

        public HOG(Matrix maskY, int bins = 8) 
        {
            _sobelTransformer = new SobelTransform(maskY);
            _bins = bins;
            _k = _bins / Math.PI-0.001;
        }

        /// <summary>
        /// Вычисление гистограммы направленных градиентов
        /// </summary>
        /// <param name="img"></param>
        /// <param name="normalyze"></param>
        /// <returns></returns>
        public Vector CalcHist(Matrix img, bool normalyze = false) 
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

            return hist;
        }
    }
}
