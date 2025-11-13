using AI.DataStructs.Algebraic;
using System;

namespace AI.ClassicMath.MatrixUtils
{
    /// <summary>
    /// Экстрактор признаков (создает карту признаков)
    /// </summary>
    [Serializable]
    public class FeaturesExtractorMaper
    {
        private readonly Func<Matrix, Vector> _transformer;

        /// <summary>
        /// Экстрактор признаков (создает карту признаков)
        /// </summary>
        public FeaturesExtractorMaper() { }

        /// <summary>
        /// Экстрактор признаков (создает карту признаков)
        /// </summary>
        public FeaturesExtractorMaper(Func<Matrix, Vector> transformFunction)
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
        public Vector[] CreateMap(Matrix img, int sizeH = 10, int sizeW = 10)
        {
            int stepsH = (img.Height - 1) / sizeH;
            int stepsW = (img.Width - 1) / sizeW;
            Vector[] map = new Vector[stepsH * stepsW];

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
