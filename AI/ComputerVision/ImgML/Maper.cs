using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.ImgML
{
    /// <summary>
    /// Формирователь карт
    /// </summary>
    public class Maper
    {
        Func<Matrix, double> _transformer;

        public Maper() { }
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
                    map[i, j] = _transformer(img.Region(j * sizeW, i *sizeH, sizeW, sizeH));
                }
            }

            return map;
        }
    }
}
