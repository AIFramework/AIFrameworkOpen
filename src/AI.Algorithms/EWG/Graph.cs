using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Невзвешенный граф
    /// </summary>
    [Serializable]
    public class Graph
    {
        /// <summary>
        /// Число вершин
        /// </summary>
        public int V { get; protected set; }
        /// <summary>
        /// Число ребер
        /// </summary>
        public int E { get; protected set; }
        /// <summary>
        /// Число дуг
        /// </summary>
        public int Arcs { get; protected set; }

        private readonly ConcurrentBag<int>[] _adjacencyArray;

        /// <summary>
        /// Создание графа
        /// </summary>
        /// <param name="numV"></param>
        public Graph(int numV)
        {
            V = numV;
            _adjacencyArray = new ConcurrentBag<int>[numV];

            for (int i = 0; i < V; i++)
            {
                _adjacencyArray[i] = new ConcurrentBag<int>();
            }

            E = 0;
            Arcs = 0;
        }

        /// <summary>
        /// Добавить ребро между 2мя вершинами
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public virtual void AddEdge(int i, int j)
        {
            if (!(_adjacencyArray[i].Contains(j) && _adjacencyArray[j].Contains(i)))
            {
                _adjacencyArray[i].Add(j);
                _adjacencyArray[j].Add(i);
                E++;
                Arcs += 2;
            }
        }

        /// <summary>
        /// Добавить дугу между 2мя вершинами
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public virtual void AddArc(int i, int j)
        {
            if (!_adjacencyArray[i].Contains(j))
            {
                _adjacencyArray[i].Add(j);
                Arcs++;
            }
        }

        /// <summary>
        /// Смежные вершины
        /// </summary>
        /// <param name="i">Вершина</param>
        /// <returns></returns>
        public virtual int[] Adj(int i) { return _adjacencyArray[i].ToArray(); }

        /// <summary>
        /// Степень связи
        /// </summary>
        /// <param name="g"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int Degree(Graph g, int i)
        {
            return g.Adj(i).Length;
        }

        /// <summary>
        /// Максимальная связь в графе
        /// </summary>
        public static int MaxDegree(Graph g)
        {
            int max = 0;
            for (int i = 0; i < g.V; i++)
            {
                int d = Degree(g, i);
                if (d > max) max = d;
            }

            return max;
        }

        /// <summary>
        /// Средняя связь
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static double AverageDegree(Graph g)
        {
            return 2.0 * g.E / g.V;
        }

        /// <summary>
        /// Число петель в графе
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static int NumSelfLoops(Graph g)
        {
            int count = 0;
            for (int i = 0; i < g.V; i++)
                foreach (var item in g.Adj(i))
                    if (i == item) count++;

            return count / 2;
        }

        /// <summary>
        /// Меняет направления в графе на противоположные
        /// </summary>
        public Graph Reverse()
        {
            Graph graph = new Graph(V);
            for (int i = 0; i < graph.V; i++)
            {
                int[] arcs = graph.Adj(i);
                for (int j = 0; j < arcs.Length; j++)
                    graph.AddArc(j, i);
            }

            return graph;

        }

        /// <summary>
        /// Строковое представление графа
        /// </summary>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < V; i++)
            {
                foreach (var item in Adj(i))
                {
                    _ = stringBuilder.Append($"{i}->{item}\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
