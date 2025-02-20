using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.Clustering
{
    /// <summary>
    /// Дерево поиска ближайшего соседа BallTree
    /// </summary>
    public class BallTree
    {
        private readonly BallTreeNode _root;

        /// <summary>
        /// Дерево поиска ближайшего соседа BallTree
        /// </summary>
        public BallTree(Vector[] dataset)
        {
            _root = BuildTree(dataset, 0);
        }

        /// <summary>
        /// Рекурсивная функция для построения дерева BallTree
        /// </summary>
        private BallTreeNode BuildTree(Vector[] dataset, int depth)
        {
            if (dataset.Length == 1)
            {
                return new BallTreeNode(dataset[0]);
            }

            // Выбираем медиану для разделения на две группы
            int dimension = dataset[0].Count;
            int axis = depth % dimension;  // Чередуем оси
            var sortedDataset = dataset.OrderBy(v => v[axis]).ToArray();
            int medianIndex = sortedDataset.Length / 2;

            // Разделяем данные на две части
            Vector[] left = sortedDataset.Take(medianIndex).ToArray();
            Vector[] right = sortedDataset.Skip(medianIndex).ToArray();

            // Создаем узел дерева
            var node = new BallTreeNode
            {
                Point = sortedDataset[medianIndex],
                Left = BuildTree(left, depth + 1),
                Right = BuildTree(right, depth + 1)
            };

            return node;
        }

        /// <summary>
        /// Метод для поиска ближайшего соседа с использованием BallTree
        /// </summary>
        public Vector NearestNeighbor(Vector point)
        {
            return FindNearestNeighbor(_root, point);
        }

        /// <summary>
        /// Рекурсивный поиск ближайшего соседа в дереве
        /// </summary>
        private Vector FindNearestNeighbor(BallTreeNode node, Vector point)
        {
            if (node == null) return null;

            double distToNode = Distances.BaseDist.EuclideanDistance(point, node.Point);

            // Проверка в левой и правой поддеревьях
            Vector bestMatch = node.Point;
            double bestDist = distToNode;

            // Определим, в какое поддерево идти дальше
            BallTreeNode nextNode = (point[node.Depth % point.Count] < node.Point[node.Depth % point.Count]) ? node.Left : node.Right;
            BallTreeNode otherNode = (nextNode == node.Left) ? node.Right : node.Left;

            // Рекурсивно ищем ближайшего соседа
            Vector temp = FindNearestNeighbor(nextNode, point);
            if (temp != null)
            {
                double tempDist = Distances.BaseDist.EuclideanDistance(point, temp);
                if (tempDist < bestDist)
                {
                    bestMatch = temp;
                    bestDist = tempDist;
                }
            }

            // Проверяем, не можем ли мы улучшить результат, проверив другое поддерево
            double radius = Math.Abs(point[node.Depth % point.Count] - node.Point[node.Depth % point.Count]);
            if (radius < bestDist)
            {
                temp = FindNearestNeighbor(otherNode, point);
                if (temp != null)
                {
                    double tempDist = Distances.BaseDist.EuclideanDistance(point, temp);
                    if (tempDist < bestDist)
                    {
                        bestMatch = temp;
                        bestDist = tempDist;
                    }
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Метод для поиска минимального расстояния до нескольких центроидов
        /// </summary>
        public double NearestDistanceToCentroids(Vector point, Vector[] centroids)
        {
            // Для каждого центроида ищем расстояние до точки
            double minDist = double.PositiveInfinity;

            foreach (var centroid in centroids)
            {
                double dist = Distances.BaseDist.EuclideanDistance(point, centroid);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }

            return minDist;
        }
    }

    /// <summary>
    /// Ветвь дерева гиперсфер
    /// </summary>
    [Serializable]
    public class BallTreeNode
    {
        /// <summary>
        /// Точка (центр гиперсферы)
        /// </summary>
        public Vector Point { get; set; }

        /// <summary>
        /// Левое поддерево
        /// </summary>
        public BallTreeNode Left { get; set; }

        /// <summary>
        /// Правое поддерево
        /// </summary>
        public BallTreeNode Right { get; set; }

        /// <summary>
        /// Глубина в дереве (используется для выбора оси)
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Ветвь дерева гиперсфер
        /// Конструктор, инициализирующий точку
        /// </summary>
        /// <param name="point"></param>
        public BallTreeNode(Vector point)
        {
            Point = point;
        }

        /// <summary>
        /// Ветвь дерева гиперсфер
        /// </summary>
        public BallTreeNode() { }
    }
}
