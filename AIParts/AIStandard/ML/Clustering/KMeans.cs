//MIT License

//Copyright (c) 2019 T.Yoshimura

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AI.ML.Clustering
{

    /// <summary>Алгоритм k-means</summary>
    [Serializable]
    public class KMeans : IClustering
    {
        /// <summary>
        /// Центроиды
        /// </summary>
        public Vector[] Centroids { get; private set; }
        /// <summary>
        /// Максимальное число циклов
        /// </summary>
        public int MaxCount { get; set; } = int.MaxValue;

        /// <summary>K-Mean</summary>
        /// <param name="clasterCount">Количество кластеров</param>
        public KMeans(int clasterCount)
        {
            if (clasterCount <= 1)
            {
                throw new ArgumentException(nameof(clasterCount));
            }

            GroupCount = clasterCount;
        }

        /// <summary>Количество кластеров</summary>
        public int GroupCount
        {
            get; private set;
        }

        /// <summary>Размерность данных</summary>
        public int DimentionOfData
        {
            get; private set;
        }

        /// <summary>Цендроиды кластеров</summary>
        public Vector[] Сentroids => Centroids;

        /// <summary>
        /// Distance function
        /// </summary>
        public Func<Vector, Vector, double> DistanceFunction { get; set; } = Distances.BaseDist.EuclideanDistance;

        /// <summary>
        /// Clusters array
        /// </summary>
        public Cluster[] Clusters
        {
            get
            {
                Cluster[] cls = new Cluster[Centroids.Length];

                for (int i = 0; i < Centroids.Length; i++)
                {
                    cls[i] = new Cluster
                    {
                        Centr = Centroids[i],
                        Dataset = new Vector[] { Centroids[i] }
                    };
                }

                return cls;
            }
        }

        /// <summary>Classify vector</summary>
        /// <param name="vector">Vector</param>
        public int Classify(Vector vector)
        {
            return NearestVector(vector);
        }

        /// <summary>Classify vectors</summary>
        /// <param name="vectors">Vectors</param>
        public int[] Classify(IEnumerable<Vector> vectors)
        {
            return vectors.Select((vector) => Classify(vector)).ToArray();
        }

        /// <summary>
        /// Clustering training
        /// </summary>
        /// <param name="dataset">Vectors</param>
        /// <param name="seed">Seed</param>
        public void Train(Vector[] dataset, int seed = 10)
        {
            Initialize();

            int vectorDim = dataset[0].Count;
            ValidateData(vectorDim, dataset);

            Centroids = new Vector[GroupCount];
            DimentionOfData = vectorDim;

            Random random = new Random(seed);
            int vectorCount = dataset.Length;

            //ToDo: сделать замер времени
            // K-means поиск цендроидов 
            Centroids[0] = dataset[random.Next(vectorCount)];
            for (int groupIndex = 1; groupIndex < GroupCount; groupIndex++)
            {
                double distSum = 0;
                double[] dist_list = new double[vectorCount];

                for (int vectorIndex = 0; vectorIndex < vectorCount; vectorIndex++)
                {
                    double dist_min = double.PositiveInfinity;

                    for (int cluster_index = 0; cluster_index < groupIndex; cluster_index++)
                    {
                        double dist = DistanceFunction(dataset[vectorIndex], Centroids[cluster_index]);
                        if (dist < dist_min)
                        {
                            dist_min = dist;
                        }
                    }

                    distSum += dist_list[vectorIndex] = dist_min;
                }

                double r = random.NextDouble() * distSum;

                for (int vectorIndex = 0; vectorIndex < vectorCount; vectorIndex++)
                {
                    r -= dist_list[vectorIndex];
                    if (r < 0)
                    {
                        Centroids[groupIndex] = dataset[vectorIndex];
                        break;
                    }
                    Centroids[groupIndex] = dataset[vectorCount - 1];
                }
            }

            // Cluster allocation
            VectorClass[] labeledVectors = dataset.Select((vector) => new VectorClass { Features = vector, ClassMark = NearestVector(vector) }).ToArray();
            bool isChangedLabel = true;
            int count = 0;

            // k-mean convergence loop
            while (isChangedLabel && count < MaxCount)
            {
                isChangedLabel = false;
                count++;

                for (int clusterIndex = 0; clusterIndex < Centroids.Length; clusterIndex++)
                {
                    Centroids[clusterIndex] = new Vector(DimentionOfData);
                }

                int[] labedCount = new int[Centroids.Length];

                foreach (VectorClass vector in labeledVectors)
                {
                    Centroids[vector.ClassMark] += vector.Features;
                    labedCount[vector.ClassMark]++;
                }

                for (int clusterIndex = 0; clusterIndex < Centroids.Length; clusterIndex++)
                {
                    Centroids[clusterIndex] /= labedCount[clusterIndex];
                }

                for (int vectorIndex = 0; vectorIndex < vectorCount; vectorIndex++)
                {
                    VectorClass labeled_vector = labeledVectors[vectorIndex];

                    int label_old = labeled_vector.ClassMark;
                    int labelNew = NearestVector(labeled_vector.Features);

                    isChangedLabel = (label_old != labelNew);
                    labeled_vector.ClassMark = labelNew;
                }

            }
        }

        /// <summary>Инизиализация</summary>
        public void Initialize()
        {
            Centroids = null;
        }

        /// <summary>Ближайший вектор</summary>
        protected int NearestVector(Vector vector)
        {
            double minDist = double.PositiveInfinity;
            int indexClass = 0;

            for (int clusterIndex = 0; clusterIndex < Centroids.Length; clusterIndex++)
            {
                double dist = DistanceFunction(vector, Centroids[clusterIndex]);
                if (dist < minDist)
                {
                    minDist = dist;
                    indexClass = clusterIndex;
                }
            }

            return indexClass;
        }

        /// <summary>Validate the sample</summary>
        private void ValidateData(int vectorDim, Vector[] vectors)
        {
            if (vectorDim < 1)
            {
                throw new ArgumentException(nameof(vectorDim));
            }
            if (vectors == null)
            {
                throw new ArgumentNullException(nameof(vectors));
            }

            if (vectors.Length < GroupCount)
            {
                throw new ArgumentException(nameof(vectors));
            }
            foreach (Vector vector in vectors)
            {
                if (vector.Count != vectorDim)
                {
                    throw new ArgumentException(nameof(vectors));
                }
            }

        }
    }
}