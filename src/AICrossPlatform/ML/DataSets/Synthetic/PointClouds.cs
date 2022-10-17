using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;

namespace AI.ML.DataSets.Synthetic
{
    /// <summary>
    /// Синтетические облака точек (данных)
    /// </summary>
    [Serializable]
    public class PointClouds
    {
        private readonly List<Vector> pointsList = new List<Vector>();
        /// <summary>
        /// Размерность
        /// </summary>
        public int Dimention { get; set; }

        private readonly Random random = new Random(10);

        /// <summary>
        /// Синтетические облака точек (данных)
        /// </summary>
        /// <param name="dim">Размерность</param>
        public PointClouds(int dim = 2)
        {
            Dimention = dim;
        }

        /// <summary>
        /// Добавить облако точек (данных)
        /// </summary>
        public void AddCloud(int count, Vector centr, CloudType type = CloudType.Hypersphere, Vector std = null)
        {
            Vector[] cloud = new Vector[count];

            if (std == null)
            {
                std = new Vector(Dimention) + 1;
            }

            switch (type)
            {
                case CloudType.Hypersphere:
                    for (int i = 0; i < count; i++)
                    {
                        cloud[i] = (std * Statistics.Statistic.RandNorm(Dimention, random) / 3.0) + centr;
                    }

                    break;

                case CloudType.Hypercube:
                    for (int i = 0; i < count; i++)
                    {
                        cloud[i] = (std * 2 * Statistics.Statistic.UniformDistribution(Dimention, random)) - 1.0 + centr;
                    }

                    break;
            }

            pointsList.AddRange(cloud);
        }

        /// <summary>
        /// Вывести как массив данных
        /// </summary>
        public Vector[] GetArray()
        {
            Vector[] data = pointsList.ToArray();
            data.Shuffle();
            return data;
        }

        /// <summary>
        /// Тип облака точек
        /// </summary>
        public enum CloudType
        {
            /// <summary>
            /// Гиперсфера
            /// </summary>
            Hypersphere,
            /// <summary>
            /// Гиперкуб
            /// </summary>
            Hypercube
        }
    }
}
