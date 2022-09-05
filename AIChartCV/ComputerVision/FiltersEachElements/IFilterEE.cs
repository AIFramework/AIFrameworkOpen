using AI.DataStructs.Algebraic;
using System.Drawing;

namespace AI.ComputerVision.FiltersEachElements
{
    /// <summary>
    /// Интерфейс поэлементного фильтра
    /// </summary>
    public interface IFilterEE
    {
        /// <summary>
        /// Фильтрация
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
