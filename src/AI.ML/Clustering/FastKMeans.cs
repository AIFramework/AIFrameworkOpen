using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AI.ML.Clustering
{
    /// <summary>
    /// Быстрый алгоритм K-средних с использованием BallTree (https://en.wikipedia.org/wiki/Ball_tree) 
    /// и инициализацией KMeans++(https://en.wikipedia.org/wiki/K-means%2B%2B)
    /// </summary>
    [Serializable]
    public class FastKMeans : IClustering
    {
        /// <summary>
        /// Центроиды
        /// </summary>
        public Vector[] Centroids { get; protected set; }
        /// <summary>
        /// Центроиды
        /// </summary>    
        public int GroupCount { get; private set; }

        /// <summary>
        /// Максимальное число кластеров
        /// </summary>
        public int MaxCount { get; set; } = int.MaxValue;
        public Func<Vector, Vector, double> DistanceFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Cluster[] Clusters => throw new NotImplementedException();

        /// <summary>
        /// Быстрый алгоритм K-средних
        /// </summary>
        public FastKMeans(int groupCount)
        {
            if (groupCount <= 1)
                throw new ArgumentException(nameof(groupCount));

            GroupCount = groupCount;
        }

        /// <summary>
        /// Обучение кластеризации с использованием KMeans++ и BallTree.
        /// </summary>
        /// <param name="dataset">Векторы</param>
        /// <param name="seed">Seed</param>
        public void Train(Vector[] dataset, int seed = 10)
        {
            Initialize();

            int vectorDim = dataset[0].Count;
            ValidateData(vectorDim, dataset);

            Centroids = new Vector[GroupCount];
            Random random = new Random(seed);

            var ballTree = new BallTree(dataset);

            // Инициализация центроидов с использованием KMeans++
            Centroids[0] = dataset[random.Next(dataset.Length)];
            for (int groupIndex = 1; groupIndex < GroupCount; groupIndex++)
            {
                double[] distList = new double[dataset.Length];
                double distSum = 0;

                for (int vectorIndex = 0; vectorIndex < dataset.Length; vectorIndex++)
                {
                    //BallTree для нахождения минимального расстояния до всех центроидов
                    distList[vectorIndex] = ballTree.NearestDistanceToCentroids(dataset[vectorIndex], Centroids.Take(groupIndex).ToArray());
                    distSum += distList[vectorIndex];
                }

                // Новый центроид с вероятностью, пропорциональной расстоянию
                double r = random.NextDouble() * distSum;
                for (int vectorIndex = 0; vectorIndex < dataset.Length; vectorIndex++)
                {
                    r -= distList[vectorIndex];
                    if (r < 0)
                    {
                        Centroids[groupIndex] = dataset[vectorIndex];
                        break;
                    }
                }
            }


            // Основной цикл кластеризации
            bool isChangedLabel = true;
            int count = 0;

            while (isChangedLabel && count < MaxCount)
            {
                isChangedLabel = false;
                count++;

                // Разметка всех векторов по кластерам с использованием BallTree
                var labeledVectors = dataset.Select((vector) => new VectorClass { Features = vector, ClassMark = NearestVector(vector, ballTree) }).ToArray();

                // Обновление центроидов
                Vector[] newCentroids = new Vector[GroupCount];
                int[] labeledCount = new int[GroupCount];

                foreach (var vector in labeledVectors)
                {
                    newCentroids[vector.ClassMark] += vector.Features;
                    labeledCount[vector.ClassMark]++;
                }

                for (int i = 0; i < GroupCount; i++)
                {
                    if (labeledCount[i] > 0)
                    {
                        newCentroids[i] /= labeledCount[i];
                    }
                    else
                    {
                        newCentroids[i] = Centroids[i]; // если в кластер не попали, сохраняем старый центроид
                    }
                }

                // Проверка на изменение меток
                for (int vectorIndex = 0; vectorIndex < dataset.Length; vectorIndex++)
                {
                    int oldLabel = labeledVectors[vectorIndex].ClassMark;
                    int newLabel = NearestVector(labeledVectors[vectorIndex].Features, ballTree);

                    if (oldLabel != newLabel)
                    {
                        isChangedLabel = true;
                    }

                    labeledVectors[vectorIndex].ClassMark = newLabel;
                }

                Centroids = newCentroids;
            }
        }

        /// <summary>Классификация вектора</summary>
        /// <param name="vector">Вектор</param>
        public int Classify(Vector vector)
        {
            return NearestVector(vector);
        }

        /// <summary>Классификация векторов</summary>
        /// <param name="vectors">Векторы</param>
        public int[] Classify(IEnumerable<Vector> vectors)
        {
            return vectors.Select((vector) => Classify(vector)).ToArray();
        }

        // ToDo: Переписать под WTA
        /// <summary>
        /// Доучивание
        /// </summary>
        /// <param name="vect">Вектор</param>
        /// <param name="lr">Скорость обучения</param>
        public void OnlineTuning(Vector vect, double lr = 0.01)
        {
            int index = Classify(vect);
            Centroids[index] += lr * vect;
        }



        /// <summary>
        /// Поиск ближайшего кластера для вектора с использованием BallTree.
        /// </summary>
        /// <param name="vector">Вектор</param>
        /// <param name="ballTree">Объект BallTree</param>
        public int NearestVector(Vector vector, BallTree ballTree)
        {
            // BallTree для быстрого поиска ближайшего центроида
            var nearestCentroid = ballTree.NearestNeighbor(vector);
            double minDist = double.PositiveInfinity;
            int indexClass = 0;

            for (int clusterIndex = 0; clusterIndex < Centroids.Length; clusterIndex++)
            {
                double dist = Distances.BaseDist.EuclideanDistance(vector, Centroids[clusterIndex]);
                if (dist < minDist)
                {
                    minDist = dist;
                    indexClass = clusterIndex;
                }
            }

            return indexClass;
        }

        /// <summary>
        /// Инициализация.
        /// </summary>
        public void Initialize()
        {
            Centroids = null;
        }

        /// <summary>
        /// Проверка на валидность данных.
        /// </summary>
        private void ValidateData(int vectorDim, Vector[] vectors)
        {
            if (vectorDim < 1)
                throw new ArgumentException(nameof(vectorDim));

            if (vectors == null)
                throw new ArgumentNullException(nameof(vectors));

            if (vectors.Length < GroupCount)
                throw new ArgumentException(nameof(vectors));

            foreach (Vector vector in vectors)
                if (vector.Count != vectorDim)
                    throw new ArgumentException(nameof(vectors));
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
    }
}
