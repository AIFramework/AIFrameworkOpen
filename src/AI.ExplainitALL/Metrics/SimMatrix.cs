// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------

using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ExplainitALL.Metrics
{
    /// <summary>
    /// Класс для определения матрицы схожестей
    /// </summary>
    [Serializable]
    public abstract class SimMatrix<T>
    {
        /// <summary>
        /// Определение схожести
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns>
        public abstract double Sim(T text1, T text2);

        /// <summary>
        /// Пред. обработка данных (текстов)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract T Transform(string text);

        /// <summary>
        /// Рассчет матрицы
        /// </summary>
        /// <param name="ans"></param>
        /// <param name="support"></param>
        /// <returns></returns>
        public Matrix GenerateMatrix(IEnumerable<string> ans, IEnumerable<string> support)
        {
            T[] ansT = new T[ans.Count()];
            T[] supportT = new T[support.Count()];
            Matrix simMatr = new Matrix(ansT.Length, supportT.Length);

            int k = 0;
            foreach (string s in support)
                supportT[k++] = Transform(s);

            k = 0;
            foreach (string s in ans)
                ansT[k++] = Transform(s);

            for (int i = 0; i < ansT.Length; i++)
                for (int j = 0; j < supportT.Length; j++)
                    simMatr[i, j] = Sim(ansT[i], supportT[j]);

            return simMatr;
        }
    }
}
