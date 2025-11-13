using AI.DataStructs.Algebraic;
using System;

namespace AI.ClassicMath.MatrixUtils
{
    /// <summary>
    /// Формирователь карт
    /// </summary>
    [Serializable]
    public class Maper
    {
        private readonly Func<Matrix, double> _transformer;

        /// <summary>
        /// Формирователь карт
        /// </summary>
        public Maper() { }
        /// <summary>
        /// Формирователь карт
        /// </summary>
        /// <param name="transformFunction">Функция преобразования региона(кропа), в число</param>
        public Maper(Func<Matrix, double> transformFunction)
        {
            _transformer = transformFunction;
        }

        /// <summary>
        /// Формирование карты
        /// </summary>
        /// <param name="img"></param>
        /// <param name="sizeH"></param>
        /// <param name="sizeW"></param>
        /// <returns></returns>
        public Matrix CreateMap(Matrix img, int sizeH = 10, int sizeW = 10)
        {
            int stepsH = (img.Height - 1) / sizeH;
            int stepsW = (img.Width - 1) / sizeW;
            Matrix map = new Matrix(stepsH, stepsW);

            for (int i = 0; i < stepsH; i++)
            {
                for (int j = 0; j < stepsW; j++)
                {
                    map[i, j] = _transformer(img.Region(j * sizeW, i * sizeH, sizeW, sizeH));
                }
            }

            return map;
        }

        /// <summary>
        /// Создание одномерного массива меток
        /// </summary>
        public double[] CreateMap1D(Matrix img, int sizeH = 10, int sizeW = 10)
        {
            int stepsH = (img.Height - 1) / sizeH;
            int stepsW = (img.Width - 1) / sizeW;
            double[] map = new double[stepsH * stepsW];

            for (int i = 0, k = 0; i < stepsH; i++)
            {
                for (int j = 0; j < stepsW; j++)
                {
                    map[k++] = _transformer(img.Region(j * sizeW, i * sizeH, sizeW, sizeH));
                }
            }

            return map;
        }
    }
}
