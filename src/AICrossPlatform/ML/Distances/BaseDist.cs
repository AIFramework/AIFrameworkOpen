using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Distances
{
    /// <summary>
    /// Basic distance functions
    /// </summary>
    [Serializable]
    public static class BaseDist
    {
        /// <summary>
        /// Lp family distances
        /// </summary>
        public static double LpDist(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B, int p)
        {
            if (A.Shape.Count != B.Shape.Count)
            {
                throw new InvalidOperationException("Dimensions of the given structures mismatche");
            }

            double res = 0, sempl;
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                sempl = Math.Pow(A.Data[i] - B.Data[i], p);
                res += Math.Abs(sempl);
            }

            res = Math.Pow(res, 1.0 / p);

            return res;
        }
        /// <summary>
        /// Euclidean distance
        /// </summary>
        public static double EuclideanDistance(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B)
        {
            return Math.Sqrt(SquareEucl(A, B));
        }
        /// <summary>
        /// Distance L-infinity
        /// </summary>
        public static double LinfDist(IAlgebraicStructure<double> A, IAlgebraicStructure<double> B)
        {
            return LinfDist((Vector)A.Data, (Vector)B.Data);
        }
        /// <summary>
        /// Cosine similarity
        /// </summary>
        public static double Cos(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            return ((Vector)a.Data).Cos(b.Data);
        }
        /// <summary>
        /// Cosine distance
        /// </summary>
        public static double CosDist(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double eps = 1e-8;
            double cos = ((Vector)a.Data).Cos(b.Data);
            return 1.0 - (cos < eps ? eps : cos);
        }
        /// <summary>
        /// Square of Euclidean distance
        /// </summary>
        public static double SquareEucl(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
            {
                double dif = a.Data[i] - b.Data[i];
                sum += dif * dif;
            }


            return sum;
        }
        /// <summary>
        /// Square of Euclidean distance
        /// </summary>
        public static double ManhattanDistance(IAlgebraicStructure<double> a, IAlgebraicStructure<double> b)
        {
            double sum = 0;

            for (int i = 0; i < a.Shape.Count; i++)
            {
                sum += Math.Abs(a.Data[i] - b.Data[i]);
            }

            return sum;
        }
    }
}
