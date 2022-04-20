using System;
using System.Collections.Generic;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Коллекция(датасет) матрица-вектор
    /// </summary>
    [Serializable]
    public class MatrixVector : List<MatrixVectorSample>
    {
        private readonly Random random = new Random();
        /// <summary>
        /// random instance
        /// </summary>
        /// <returns></returns>
        public MatrixVectorSample MatrixVectorSempleRand()
        {
            return this[random.Next(0, Count)];
        }
    }
}
