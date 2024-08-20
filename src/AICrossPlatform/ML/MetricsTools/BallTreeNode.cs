using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.MetricsTools
{

    /// <summary>
    /// Узел Ball-дерева, представляющий подмножество точек и их меток
    /// </summary>
    [Serializable]
    public class BallTreeNode
    {
        /// <summary>
        /// Список точек, содержащихся в данном узле
        /// </summary>
        public List<Vector> Points { get; }

        /// <summary>
        /// Список меток, соответствующих точкам
        /// </summary>
        public List<int> Labels { get; }

        /// <summary>
        /// Радиус гиперсферы, охватывающей все точки в данном узле
        /// </summary>
        public double Radius { get; private set; }

        /// <summary>
        /// Центр гиперсферы, охватывающей все точки в данном узле
        /// </summary>
        public Vector Center { get; private set; }

        /// <summary>
        /// Левый дочерний узел.
        /// </summary>
        public BallTreeNode Left { get; private set; }

        /// <summary>
        /// Правый дочерний узел.
        /// </summary>
        public BallTreeNode Right { get; private set; }

        /// <summary>
        /// Функция для вычисления расстояния между двумя векторами
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр узла Ball-дерева с заданными точками, метками и функцией расстояния.
        /// </summary>
        /// <param name="points">Точки, содержащиеся в узле</param>
        /// <param name="labels">Метки, соответствующие точкам</param>
        /// <param name="dist">Функция для вычисления расстояния между двумя векторами</param>
        public BallTreeNode(List<Vector> points, List<int> labels, Func<Vector, Vector, double> dist)
        {
            Points = points;
            Labels = labels;
            Dist = dist;
            Radius = 0;
            Center = null;
            Left = null;
            Right = null;
        }

        /// <summary>
        /// Рекурсивно строит Ball-дерево из текущих точек и меток
        /// </summary>
        public void BuildTree()
        {
            if (Points.Count == 1)
            {
                Center = Points[0];
                return;
            }

            // Вычисление центра (центроида) текущих точек
            Center = new Vector(Points[0].Select((_, i) => Points.Select(p => p[i]).Average()).ToArray());

            // Вычисление расстояний от центра до каждой точки
            List<double> distances = Points.Select(point => Dist(Center, point)).ToList();
            int medianIndex = distances.IndexOf(distances.OrderBy(d => d).ElementAt(distances.Count / 2));

            List<Vector> leftPoints = new List<Vector>();
            List<Vector> rightPoints = new List<Vector>();
            List<int> leftLabels = new List<int>();
            List<int> rightLabels = new List<int>();

            // Разделение точек на левую и правую подгруппы
            for (int i = 0; i < distances.Count; i++)
            {
                if (distances[i] <= distances[medianIndex])
                {
                    leftPoints.Add(Points[i]);
                    leftLabels.Add(Labels[i]);
                }
                else
                {
                    rightPoints.Add(Points[i]);
                    rightLabels.Add(Labels[i]);
                }
            }

            if (leftPoints.Count > 0)
            {
                Left = new BallTreeNode(leftPoints, leftLabels, Dist);
                Left.BuildTree();
            }

            if (rightPoints.Count > 0)
            {
                Right = new BallTreeNode(rightPoints, rightLabels, Dist);
                Right.BuildTree();
            }

            Radius = distances.Max();
        }

        /// <summary>
        /// Ищет k ближайших соседей к заданной точке в дереве
        /// </summary>
        /// <param name="point">Точка, для которой нужно найти ближайших соседей</param>
        /// <param name="k">Количество ближайших соседей, которых нужно найти</param>
        /// <returns>Список ближайших соседей в формате "метка класса и близость"</returns>
        public List<(int Label, double Distance)> KNearestNeighbors(Vector point, int k)
        {
            var bestNeighbors = new SortedList<double, int>(new DuplicateKeyComparer<double>());
            SearchKNearestNeighbors(point, k, bestNeighbors);
            return bestNeighbors.Select(pair => (pair.Value, pair.Key)).ToList();
        }

        /// <summary>
        /// Рекурсивно ищет ближайших соседей, обновляя список лучших найденных соседей.
        /// </summary>
        /// <param name="point">Точка, для которой нужно найти ближайших соседей</param>
        /// <param name="k">Количество ближайших соседей, которых нужно найти</param>
        /// <param name="bestNeighbors">Список лучших найденных соседей</param>
        private void SearchKNearestNeighbors(Vector point, int k, SortedList<double, int> bestNeighbors)
        {
            if (Points.Count == 1)
            {
                double distance = Dist(point, Points[0]);
                if (bestNeighbors.Count < k || distance < bestNeighbors.Keys[bestNeighbors.Count - 1])
                {
                    bestNeighbors.Add(distance, Labels[0]);

                    if (bestNeighbors.Count > k)
                        bestNeighbors.RemoveAt(bestNeighbors.Count - 1);

                }
                return;
            }

            double distToCenter = Dist(point, Center);

            if (distToCenter <= Radius || bestNeighbors.Count < k || distToCenter - Radius <= bestNeighbors.Keys[bestNeighbors.Count - 1])
            {
                Left?.SearchKNearestNeighbors(point, k, bestNeighbors);
                Right?.SearchKNearestNeighbors(point, k, bestNeighbors);
            }
        }
    }

    /// <summary>
    /// Компаратор для SortedList, позволяющий использовать одинаковые ключи
    /// </summary>
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        /// <summary>
        /// Метод для работы с дубликатами
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            // Если ключи равны, возвращаем 1, чтобы реализовать добавление одинаковых ключей
            if (result == 0) return 1;
            else return result;
        }
    }

}
