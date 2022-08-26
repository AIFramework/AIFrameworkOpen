
using System;
using System.Collections.Generic;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Датасет с картинками матрица-нормер класса
    /// </summary>
    [Serializable]
    public class MatrixInt : List<MatrixIntSample>
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MatrixIntSample GetRandMatrixIntSemple()
        {
            return this[_random.Next(Count)];
        }
    }
}
