using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Матрица-вектор
    /// </summary>
    [Serializable]
    public class MatrixVectorSample
    {
        /// <summary>
        /// Матрица
        /// </summary>
        public Matrix matrix;
        /// <summary>
        /// Вектор
        /// </summary>
        public Vector vector;

        /// <summary>
        /// Матрица-вектор
        /// </summary>
        /// <param name="matr">Матрица</param>
        /// <param name="vect">Вектор</param>
        public MatrixVectorSample(Matrix matr, Vector vect)
        {
            matrix = matr.Copy();
            vector = vect.Clone();
        }
    }
}
