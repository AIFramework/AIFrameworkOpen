using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Взвешенный граф
    /// </summary>
    [Serializable]
    public class GraphW<T> where T : BaseEdge, new()
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

        private readonly ConcurrentBag<T>[] _adjacencyArray;

        /// <summary>
        /// Создание графа
        /// </summary>
        /// <param name="numV"></param>
        public GraphW(int numV)
        {
            V = numV;
            _adjacencyArray = new ConcurrentBag<T>[numV];

            for (int i = 0; i < V; i++)
            {
                _adjacencyArray[i] = new ConcurrentBag<T>();
            }

            E = 0;
            Arcs = 0;
        }


        /// <summary>
        /// Добавить ребро между 2мя вершинами
        /// </summary>
        public virtual void AddEdgeW(T edge)
        {

            int v = edge.Either(), w = edge.Other(v);
            _adjacencyArray[v].Add(edge);
            _adjacencyArray[w].Add(edge);

            E++;
            Arcs += 2;
        }

        /// <summary>
        /// Добавить ребро между 2мя вершинами
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="w"></param>
        public virtual void AddEdge(int i, int j, double w = 1)
        {
            T e = new T() { StartV = i, EndV = j, W = w };
            AddEdgeW(e);
        }




        /// <summary>
        /// Добавить дугу между 2мя вершинами
        /// </summary>
        public virtual void AddArceW(T edge)
        {
            int v = edge.Either();
            _adjacencyArray[v].Add(edge);

            E++;
            Arcs++;
        }

        /// <summary>
        /// Добавить дугу между 2мя вершинами
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="w"></param>
        public virtual void AddArce(int i, int j, double w = 1)
        {
            T e = new T() { StartV = i, EndV = j, W = w };
            AddArceW(e);
        }



        /// <summary>
        /// Смежные вершины
        /// </summary>
        /// <param name="i">Вершина</param>
        /// <returns></returns>
        public virtual int[] Adj(int i)
        {
            var eArr = AdjEW(i);
            int[] ints = new int[eArr.Length];

            for (int j = 0; j < eArr.Length; j++)
                ints[i] = eArr[j].Other(i);

            return ints.ToArray();
        }

        /// <summary>
        /// Смежные вершины
        /// </summary>
        /// <param name="i">Вершина</param>
        /// <returns></returns>
        public virtual T[] AdjEW(int i) { return _adjacencyArray[i].ToArray(); }

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
