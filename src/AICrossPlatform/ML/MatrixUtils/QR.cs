using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.MatrixUtils
{
    /// <summary>
    /// QR разложение
    /// </summary>
    [Serializable]
    public static class QR
    {
        /// <summary>
        /// Получение матрицы Q
        /// </summary>
        public static Matrix GetQ(Matrix a)
        {
            return GramSchmidtProcedure.GetNormalBasis(a);
        }

        /// <summary>
        /// Получение верхне треугольной матрицы
        /// </summary>
        /// <param name="a"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Matrix GetR(Matrix a, Matrix q)
        {
            return q.Transpose() * a;
        }
    }
}
