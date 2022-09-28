using AI.Algebra;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.MatrixUtils
{
    /// <summary>
    /// Алгоритм вычисления собственных чисел и векторов
    /// </summary>
    [Serializable]
    public class EigenValuesVectors
    {
        /// <summary>
        /// Собственные числа
        /// </summary>
        public Vector Eigenvalues { get; set; }

        /// <summary>
        /// Ошибка
        /// </summary>
        public double Eps { get; set; }

        /// <summary>
        /// Сошелся ли алгоритм
        /// </summary>
        public bool IsConvergence { get; set; }

        /// <summary>
        /// Собственные векторы
        /// </summary>
        public Vector[] Eigenvectors 
        {
            get 
            {
                if (!_isCalcVectors) 
                    _eigenvectors = GetEigenvectors(_matrix, Eigenvalues);
                return _eigenvectors;
            } 
        }


        private Vector[] _eigenvectors;
        private bool _isCalcVectors = false;
        private Matrix _matrix;

        /// <summary>
        /// Алгоритм вычисления собственных чисел и векторов
        /// </summary>
        public EigenValuesVectors(Matrix matrix, int iterations = 60, double eps = 1e-2) 
        {
            _matrix = matrix;
            GetEigenvalues(iterations, eps);
        }



        // Получить собственные числа матрицы
        private void GetEigenvalues(int iterations, double eps)
        {
            Eigenvalues = new Vector(_matrix.Height);
            Matrix at = _matrix.Copy();
            Matrix r = new Matrix();

            for (int i = 0; i < iterations; i++)
            {
                Matrix q = QR.GetQ(at);
                r = QR.GetR(at, q);
                at = r * q;
            }

            Eps = AlgorithmAnalysis.MetricsForRegression.RMSEPercent(r.Transform(Math.Abs), at.Transform(Math.Abs)); // Ошибка
            IsConvergence = Eps <= eps; // Сошелся ли алгоритм

            for (int i = 0; i < at.Height; i++)
                Eigenvalues[i] = at[i, i];
        }



        // Получить собственные векторы
        private Vector[] GetEigenvectors(Matrix a, Vector eigenvalues)
        {
            Vector[] eigenvectors = new Vector[eigenvalues.Count];
            var parOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };

            Parallel.For(0, eigenvalues.Count, parOptions, i =>
            {
                eigenvectors[i] = GetEigenvector(a, eigenvalues[i]);
            }
            );

            _isCalcVectors = true;

            return eigenvectors;
        }

        // Получение собственного вектора
        private static Vector GetEigenvector(Matrix a, double eigenvalue)
        {
            Matrix coef = a.Copy();
            Vector b = new Vector(a.Width) + 1e-5;

            for (int i = 0; i < a.Width; i++)
                coef[i, i] -= eigenvalue;


            var vect = Gauss.SolvingEquations(coef, b);
            vect /= vect.NormL2();

            return vect;
        }

        /// <summary>
        /// Получить собственные векторы
        /// </summary>
        /// <param name="a"></param>
        /// <param name="eigenvalues"></param>
        /// <returns></returns>
        public static Vector[] GetEigenvectorsStatic(Matrix a, Vector eigenvalues)
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

    }
}
