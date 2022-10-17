using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.ML.Clustering
{
    /// <summary>
    /// Интерфейс для метрического алгоритма кластеризации
    /// </summary>
    public interface IClustering
    {
        /// <summary>
        /// Распознать вектор
        /// </summary>
        int Classify(Vector vector);

        /// <summary>
        /// Распознать векторs
        /// </summary>
        int[] Classify(IEnumerable<Vector> vectors);

        /// <summary>
        /// Обучение кластеризации
        /// </summary>
        /// <param name="dataset">Векторы</param>
        /// <param name="param">Свободный параметр</param>
        void Train(Vector[] dataset, int param);

        /// <summary>
        /// Функция измерения расстояния
        /// </summary>
        Func<Vector, Vector, double> DistanceFunction { get; set; }

        /// <summary>
        /// Массив кластеров
        /// </summary>
        Cluster[] Clusters { get; }
    }
}
