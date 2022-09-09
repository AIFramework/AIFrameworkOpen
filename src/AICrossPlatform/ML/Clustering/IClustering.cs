using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.ML.Clustering
{
    /// <summary>
    /// Interface for metric clustering algorithms
    /// </summary>
    public interface IClustering
    {
        /// <summary>
        /// Classify vector
        /// </summary>
        int Classify(Vector vector);

        /// <summary>
        /// Classify vectors
        /// </summary>
        int[] Classify(IEnumerable<Vector> vectors);

        /// <summary>
        /// Clustering training
        /// </summary>
        /// <param name="dataset">Vectors</param>
        /// <param name="param">Free parametr</param>
        void Train(Vector[] dataset, int param);

        /// <summary>
        /// Distance function
        /// </summary>
        Func<Vector, Vector, double> DistanceFunction { get; set; }

        /// <summary>
        /// Clusters array
        /// </summary>
        Cluster[] Clusters { get; }
    }
}
