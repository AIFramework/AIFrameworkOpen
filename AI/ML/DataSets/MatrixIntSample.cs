using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Элемент датасета (матрица - номер класса)
    /// </summary>
    [Serializable]
    public class MatrixIntSample
    {
        /// <summary>
        /// Матрица
        /// </summary>
        public Matrix Matrix { get; set; }

        /// <summary>
        /// Номер класса
        /// </summary>
        public int ClassNum { get; set; }

        /// <summary>
        /// Элемент датасета (матрица - номер класса)
        /// </summary>
        public MatrixIntSample() { }

        /// <summary>
        /// Элемент датасета (матрица - номер класса)
        /// </summary>
        /// <param name="matr">Матрица</param>
        /// <param name="numClass">Номер класса</param>
        public MatrixIntSample(Matrix matr, int numClass)
        {
            Matrix = matr;
            ClassNum = numClass;
        }


    }
}
