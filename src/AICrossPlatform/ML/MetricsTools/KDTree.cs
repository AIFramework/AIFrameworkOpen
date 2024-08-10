using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.MetricsTools
{

    /// <summary>
    /// kd-дерево, метод ускорения ближ. соседа, эффективен на малых размерностях
    /// </summary>
    [Serializable]
    public class KDTree
    {
        /// <summary>
        /// Функция расстояния
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }

        /// <summary>
        /// Корень
        /// </summary>
        public KDNode Root { get; private set; }

        /// <summary>
        /// kd-дерево, метод ускорения ближ. соседа, эффективен на малых размерностях
        /// </summary>
        /// <param name="points"></param>
        /// <param name="labels"></param>
        public KDTree(List<IEnumerable<double>> points, List<int> labels)
        {
            var arrPoints = new List<double[]>(points.Count);
            foreach (var point in points) arrPoints.Add(point.ToArray());

            var pointLabelPairs = new List<(double[], int)>();
            for (int i = 0; i < arrPoints.Count; i++)
                pointLabelPairs.Add((arrPoints[i], labels[i]));
            
            Root = BuildTree(pointLabelPairs, 0);
        }

        // Алгоритм построения дерева
        private KDNode BuildTree(List<(double[] Point, int Label)> points, int depth)
        {
            if (points.Count == 0)
                return null;

            int k = points[0].Point.Length;
            int axis = depth % k;

            points.Sort((a, b) => a.Point[axis].CompareTo(b.Point[axis]));
            int median = points.Count / 2;

            return new KDNode(
                points[median].Point,
                points[median].Label)
            {
                Left = BuildTree(points.GetRange(0, median), depth + 1),
                Right = BuildTree(points.GetRange(median + 1, points.Count - (median + 1)), depth + 1)
            };
        }

        /// <summary>
        /// Поиск ближайшего соседа в kd дереве
        /// </summary>
        /// <param name="point"></param>
        /// <param name="depth"></param>
        /// <param name="best"></param>
        /// <returns></returns>
        public KDNode NearestNeighbor(double[] point, int depth = 0, KDNode best = null)
        {
            if (Root == null)
            {
                return best;
            }

            int k = point.Length;
            int axis = depth % k;

            KDNode nextBest = best;
            KDNode nextBranch = null;

            if (best == null || Dist(point, Root.DataVector) < Dist(point, best.DataVector))
            {
                nextBest = Root;
            }

            if (point[axis] < Root.DataVector[axis])
            {
                nextBranch = Root.Left;
                nextBest = NearestNeighborRecursive(point, Root.Left, depth + 1, nextBest);
            }
            else
            {
                nextBranch = Root.Right;
                nextBest = NearestNeighborRecursive(point, Root.Right, depth + 1, nextBest);
            }

            if (nextBranch != null && Math.Abs(point[axis] - Root.DataVector[axis]) < Dist(point, nextBest.DataVector))
            {
                nextBest = NearestNeighborRecursive(point, nextBranch, depth + 1, nextBest);
            }

            return nextBest;
        }

        /// <summary>
        /// Рекурсивный алгоритм поиска ближайшего соседа
        /// </summary>
        /// <param name="point"></param>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        /// <param name="best"></param>
        /// <returns></returns>
        private KDNode NearestNeighborRecursive(double[] point, KDNode node, int depth, KDNode best)
        {
            if (node == null)
            {
                return best;
            }

            int k = point.Length;
            int axis = depth % k;

            KDNode nextBest = best;
            KDNode nextBranch = null;

            if (best == null || Dist(point, node.DataVector) < Dist(point, best.DataVector))
            {
                nextBest = node;
            }

            if (point[axis] < node.DataVector[axis])
            {
                nextBranch = node.Left;
                nextBest = NearestNeighborRecursive(point, node.Left, depth + 1, nextBest);
            }
            else
            {
                nextBranch = node.Right;
                nextBest = NearestNeighborRecursive(point, node.Right, depth + 1, nextBest);
            }

            if (nextBranch != null && Math.Abs(point[axis] - node.DataVector[axis]) < Dist(point, nextBest.DataVector))
            {
                nextBest = NearestNeighborRecursive(point, nextBranch, depth + 1, nextBest);
            }

            return nextBest;
        }

    }

}
