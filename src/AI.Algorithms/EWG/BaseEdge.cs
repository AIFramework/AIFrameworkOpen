using System;

namespace AI.Algorithms.EWG
{
    /// <summary>
    /// Абстрактный класс ребра
    /// </summary>
    [Serializable]
    public class BaseEdge : IComparable<BaseEdge>
    {

        /// <summary>
        /// Начальная вершина
        /// </summary>
        public int StartV { get; set; }

        /// <summary>
        /// Конечная вершина
        /// </summary>
        public int EndV { get; set; }

        /// <summary>
        /// Вес ребра
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// Ребро
        /// </summary>
        public BaseEdge(int startV, int endV, double w) { StartV = startV; EndV = endV; W = w; }
        public BaseEdge() { }

        /// <summary>
        /// Вернуть начальную вершину
        /// </summary>
        public virtual int Either()
        {
            return StartV;
        }

        /// <summary>
        /// Вернуть другую вершину
        /// </summary>
        public virtual int Other(int vertex)
        {
            if (vertex == StartV) return EndV;
            else return StartV;
        }

        /// <summary>
        /// Сравнивает ребра по весу
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(BaseEdge other)
        {
            if (W > other.W) return 1;
            else if (W < other.W) return -1;
            else return 0;
        }
    }
}
