using System;
using System.Collections.Generic;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Поиск кратчайшего пути
    /// </summary>
    [Serializable]
    public abstract class ShortestPath<T> where T : BaseEdge, new()
    {
        /// <summary>
        /// Поиск кратчайшего пути
        /// </summary>
        protected abstract void Init(GraphW<T> graph, int startVertex);


        /// <summary>
        /// Дистанция до конечной вершины
        /// </summary>
        /// <param name="endVertex"></param>
        /// <returns></returns>
        public abstract double DistanceTo(int endVertex);

        /// <summary>
        /// Отдает ребра кратчайшего пути
        /// </summary>
        /// <param name="endVertex"></param>
        /// <returns></returns>
        public abstract IEnumerable<T> GetEdgesFromPath(int endVertex);
    }
}
