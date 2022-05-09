using AI.ComputerVision.SpatialFilters;
using AI.DataStructs.Algebraic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.ImgTransforms
{
    /// <summary>
    /// Преобразование Собеля
    /// </summary>
    public class SobelTransform
    {
        private readonly CustomFilter _filterX, _filterY;

        /// <summary>
        /// Преобразование Собеля
        /// </summary>
        public SobelTransform() 
        {
            Matrix maskY = new Matrix(3, 3);
            maskY[0, 0] = -1;
            maskY[0, 1] = -2;
            maskY[0, 2] = -1;

            maskY[2, 0] = 1;
            maskY[2, 1] = 2;
            maskY[2, 2] = 1;

            _filterY = new CustomFilter(maskY);
            _filterX = new CustomFilter(maskY.Transpose());
        }

        /// <summary>
        /// Преобразование Собеля (Кастомная маска)
        /// </summary>
        public SobelTransform(Matrix maskY)
        {
            _filterY = new CustomFilter(maskY);
            _filterX = new CustomFilter(maskY.Transpose());
        }

        /// <summary>
        /// Запуск расчета
        /// </summary>
        /// <param name="img">Входное изображение</param>
        public SobelData Transform(Matrix img) 
        {
            return new SobelData(_filterX.Filtration(img),
                _filterY.Filtration(img));
        }
    }

}
