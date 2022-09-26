using AI.Algebra;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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


        /// <summary>
        /// Получить собственные векторы
        /// </summary>
        /// <param name="a"></param>
        /// <param name="eigenvalues"></param>
        /// <returns></returns>
        public static Vector[] GetEigenvectors(Matrix a, Vector eigenvalues) 
        {
            Vector[] eigenvectors = new Vector[eigenvalues.Count];
            var parOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.For(0, eigenvalues.Count, parOptions, i =>
                {
                    eigenvectors[i] = GetEigenvector(a, eigenvalues[i]);
                }
            );

            return eigenvectors;
        }

        // Получение собственного вектора
        private static Vector GetEigenvector(Matrix a, double eigenvalue) 
        {
            Matrix coef = a.Copy();
            Vector b = new Vector(a.Width)+1e-5;

            for (int i = 0; i < a.Width; i++)
                coef[i, i] -= eigenvalue;


            var vect = Gauss.SolvingEquations(coef, b);
            vect /= vect.NormL2();

           return vect;
        }
    }
}
