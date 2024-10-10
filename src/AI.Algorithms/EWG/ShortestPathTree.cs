using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Дерево кратчайших путей
    /// </summary>
    [Serializable]
    public class ShortestPathTree<T> where T : BaseEdge, new()
    {

        private readonly T[] _eds;
        private readonly double[] _dists;


        /// <summary>
        /// Дерево кратчайших путей
        /// </summary>
        /// <param name="eds"></param>
        /// <param name="dists"></param>
        public ShortestPathTree(IEnumerable<T> eds, IEnumerable<double> dists)
        {
            _dists = dists.ToArray();
            _eds = eds.ToArray();
        }

        /// <summary>
        /// Выдает расстояние до вершины
        /// </summary>
        /// <param name="endV"></param>
        /// <returns></returns>
        public double DistanceTo(int endV)
        {
            return _dists[endV];
        }

        /// <summary>
        /// Выдает путь до вершины
        /// </summary>
        /// <param name="endV"></param>
        /// <returns></returns>
        public T[] GetPath(int endV)
        {
            Stack<T> path = new Stack<T>();

            for (T e = _eds[endV]; e != null; e = _eds[e.StartV])
                path.Push(e);

            return path.ToArray();
        }
    }
}
