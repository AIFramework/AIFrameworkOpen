using AI.DataStructs.Algebraic;
using System.Drawing;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Интерфейс пространственного фильтра оттенков серого
    /// </summary>
    public interface ISpatialFilterGray
    {
        /// <summary>
        /// Фильтрация
        /// </summary>
        Matrix Filtration(Matrix input);
        /// <summary>
        /// Фильтрация
        /// </summary>
        Bitmap Filtration(Bitmap input);
        /// <summary>
        /// Фильтрация
        /// </summary>
        Bitmap Filtration(string path);
    }
}
