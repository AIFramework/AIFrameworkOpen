// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------

using AI.DataStructs.Algebraic;
using System.Collections.Generic;

namespace AI.ExplainitALL.Metrics
{
    /// <summary>
    /// Интерфейс для определения матрицы схожестей
    /// </summary>
    public interface ISimMatrix 
    {
        /// <summary>
        /// Рассчет матрицы
        /// </summary>
        /// <param name="ans"></param>
        /// <param name="support"></param>
        Matrix GenerateMatrix(IEnumerable<string> ans, IEnumerable<string> support);
    }
}
