﻿using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI.ML.Clustering
{
    /// <summary>
    /// Самоорганизующиеся карты Кохонена
    /// </summary>
    [Serializable]
    public class KohonenNet : IClustering
    {
        /// <summary>
        /// Функция расстояния
        /// </summary>
        public Func<Vector, Vector, double> DistanceFunction { get; set; } = Distances.BaseDist.EuclideanDistance;

        /// <summary>
        /// Веса сети
        /// </summary>
        public Vector[] w;
        /// <summary>
        /// Веса смещения
        /// </summary>
        public Vector bias;
        /// <summary>
        /// Neural network setup steps
        /// </summary>
        public int Steps { get; set; } = 50;

        private readonly int _clusters;
        private readonly Random rnd;

        /// <summary>
        /// Массив кластеров
        /// </summary>
        public Cluster[] Clusters
        {
            get
            {
                Cluster[] cls = new Cluster[w.Length];

                for (int i = 0; i < w.Length; i++)
                {
                    cls[i] = new Cluster
                    {
                        Centr = w[i],
                        Dataset = new[] { w[i] }
                    };
                }

                return cls;
            }
        }

        /// <summary>
        /// Самоорганизующиеся карты Кохонена
        /// </summary>
        public KohonenNet(int clusters, int inpDim, int seed = 1)
        {
            w = new Vector[clusters];
            rnd = new Random(seed);

            for (int i = 0; i < w.Length; i++)
            {
                w[i] = Statistics.Statistic.UniformDistribution(inpDim, rnd);
            }

            _clusters = clusters;
            bias = new Vector(clusters);
        }


        /// <summary>
        /// Классификация
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public int Classify(Vector vector)
        {
            Vector outp = new Vector(w.Length);

            _ = Parallel.For(0, _clusters, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
            {
                outp[i] = AnalyticGeometryFunctions.Dot(w[i], vector) + bias[i];
            });

            return outp.MaxElementIndex();
        }

        /// <summary>
        /// Массива векторов
        /// </summary>
        public int[] Classify(IEnumerable<Vector> vectors)
        {
            return vectors.Select((vector) => Classify(vector)).ToArray();
        }

        /// <summary>
        /// Обучение и классификация
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public int ClassifyAndTrain(Vector vect)
        {
            double newP = 0.0001, old = 1.0 - newP;
            Vector k = new Vector(_clusters);

            _ = Parallel.For(0, _clusters, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, j =>
            {
                k[j] = DistanceFunction(vect, w[j]);
                w[j] = (old * w[j]) - (0.01 * newP * vect);
            });

            int ind = k.MinElementIndex();
            w[ind] += newP * vect;

            return Classify(vect);
        }

        /// <summary>
        /// Обучение сети кохонена
        /// </summary>
        /// <param name="datasetInp"></param>
        /// <param name="param"></param>
        public void Train(Vector[] datasetInp, int param)
        {

            Vector std = Statistics.Statistic.EnsembleStd(datasetInp);
            Vector mean = Vector.Mean(datasetInp);
            Vector[] dataset = new Vector[datasetInp.Length];

            for (int i = 0; i < dataset.Length; i++)
            {
                dataset[i] = (datasetInp[i] - mean) / std;
            }

            RunEpoch(dataset);

            // Расчет параметров
            for (int i = 0; i < w.Length; i++)
            {
                w[i] /= std;
                bias[i] = -AnalyticGeometryFunctions.Dot(mean, w[i]);
            }
        }

        private void RunEpoch(Vector[] dataset)
        {
            Vector k = new Vector(_clusters);
            double newP = 0.2, old = 1.0 - newP;
            int rep = dataset.Length / Steps;
            rep = rep < 1 ? 1 : rep;
            double delta = newP / Steps;



            for (int i = 0, repCount = 0; i < dataset.Length; i++)
            {
                _ = Parallel.For(0, _clusters, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, j =>
                {
                    k[j] = DistanceFunction(dataset[i], w[j]);
                    w[j] = (old * w[j]) - (0.1 * newP * dataset[i]);
                });

                int ind = k.MinElementIndex();
                w[ind] += newP * dataset[i];

                if (i % rep == 0)
                {
                    repCount++;
                    newP -= delta;
                    old = 1 - newP;
                }
            }
        }


    }
}
