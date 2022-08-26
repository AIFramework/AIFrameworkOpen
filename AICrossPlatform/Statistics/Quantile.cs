using AI.DataStructs.Algebraic;
using System;

namespace AI.Statistics
{
    /// <summary>
    /// Quantiles
    /// </summary>
    [Serializable]
    public class Quantile
    {
        private readonly int _max;

        /// <summary>
        /// Sorted vector
        /// </summary>
        public Vector SortVec { get; private set; }

        /// <summary>
        /// Quantile
        /// </summary>
        /// <param name="structureDouble">Data</param>
        public Quantile(IAlgebraicStructure structureDouble)
        {
            SortVec = structureDouble.Data;
            SortVec.Sort();
            _max = SortVec.Count - 1;
        }

        /// <summary>
        /// Calculating a given quantile (0-1)
        /// </summary>
        /// <param name="q">Quantile</param>
        public double GetQuantile(double q)
        {
            int index = (int)(q * _max);

            if (index > _max)
            {
                throw new InvalidOperationException("Quantile is set incorrectly");
            }

            return SortVec[index];
        }
    }
}