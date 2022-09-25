using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;

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

        /// <summary>
        /// Получить собственные числа матрицы
        /// </summary>
        public static Vector GetEigenvalues(Matrix a, int iterations = 100, double difRQ = 1e-4) 
        {
            Vector eigenvalues = new Vector(a.Height);
            Matrix at = a.Copy();
            Matrix r = new Matrix();

            for (int i = 0; i < iterations; i++)
            {
                Matrix q = GetQ(at);
                r = GetR(at, q);
                at = r * q;
            }

            if (AlgorithmAnalysis.MetricsForRegression.RMSEPercent(r.Transform(Math.Abs), at.Transform(Math.Abs)) > difRQ)
                throw new Exception("Точное решение не получено, вероятно присутствуют комплекснные числа, увеличте difRQ или число итераций");

            for (int i = 0; i < a.Height; i++)
                eigenvalues[i] = at[i, i];

            return eigenvalues;
        }
    }
}
