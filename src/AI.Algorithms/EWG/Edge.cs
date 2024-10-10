using System;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Ребро
    /// </summary>
    [Serializable]
    public class Edge : BaseEdge
    {
        /// <summary>
        /// Ребро
        /// </summary>
        public Edge(int startV, int endV, double w) : base(startV, endV, w) { }

        /// <summary>
        /// Ребро
        /// </summary>
        public Edge() : base() { }


    }
}
