using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;

namespace AI.ML
{
    /// <summary>
    /// Корреляционная матрица
    /// </summary>
    [Serializable]
    public class CrossCorrMatrix
    {
        /// <summary>
        /// Рассчет корреляционной матрицы
        /// </summary>
        public static Matrix CalcMatrix(Vector[] x, Vector[] y)
        {
            Matrix matrix = new Matrix(x.Length, y.Length);

            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < y.Length; j++)
                {
                    matrix[i, j] = Statistic.CorrelationCoefficient(x[i], y[j]);
                }
            }

            return matrix;       }
    }
}
