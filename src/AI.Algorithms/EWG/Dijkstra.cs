using AI.Algorithms.PriorityQueues;
using System;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Алгоритм Дейкстры
    /// </summary>
    [Serializable]
    public class DijkstraSPath<T> where T : BaseEdge, new()
    {
        private readonly T[] _edges;
        private readonly double[] _distace;
        private readonly IndexPriorityQueueMin<double> minPQ;

        /// <summary>
        /// Расстояния
        /// </summary>
        public double[] Distances => _distace;

        /// <summary>
        /// Ребра кратчайшего пути
        /// </summary>
        public T[] Edges => _edges;


        /// <summary>
        /// Алгоритм Дейкстры
        /// </summary>
        public DijkstraSPath(GraphW<T> graph, int vertex_start)
        {
            _edges = new T[graph.V];
            _distace = new double[graph.V];
            minPQ = new IndexPriorityQueueMin<double>(graph.V);

            for (int i = 0; i < graph.V; i++)
                _distace[i] = double.MaxValue;

            _distace[vertex_start] = 0;

            minPQ.Insert(vertex_start, 0);

            while (!minPQ.IsEmpty())
            {
                int v = minPQ.DelMinGetIndex();

                foreach (T e in graph.AdjEW(v))
                    Upd(e);
            }
        }

        // Обновление (ослабление связи)
        private void Upd(T e)
        {
            int v_in = e.StartV, v_out = e.EndV;
            double w = _distace[v_in] + e.W;

            if (_distace[v_out] > w)
            {

                _distace[v_out] = w;
                _edges[v_out] = e;
                if (minPQ.IsContain(v_out))
                    minPQ.Update(v_out, w);
                else minPQ.Insert(v_out, w);
            }
        }
    }
}
