/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 08.04.2015
 * Time: 22:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.Clustering
{

    /// <summary>
    /// Кластеризация методом FOREL
    /// </summary>
    [Serializable]
    public class Forel : IClustering
    {
        private Vector[] _dataset, datasetNotClasteris, newDataset; // Выборка
        private readonly List<Cluster> clusters = new List<Cluster>(); // Кластеры в выборке
        private Cluster claster = new Cluster();
        private double R0 = 0, Rn = 0;
        private Vector mainCentr;


        /// <summary>
        /// Массив кластеров
        /// </summary>
        public Cluster[] Clusters => clusters.ToArray();

        public Vector[] Centroids => Clusters.Select(cl => cl.Centr).ToArray();

        /// <summary>
        /// Функция измерения расстояния
        /// </summary>
        public Func<Vector, Vector, double> DistanceFunction { get; set; } = Distances.BaseDist.EuclideanDistance;


        /// <summary>
        /// Кластеризация методом FOREL
        /// </summary>
        public Forel()
        {

        }

        /// <summary>
        /// Обучение кластеризации
        /// </summary>
        /// <param name="dataset">Векторы</param>
        /// <param name="minR">Minimum cluster radius</param>
        public void Train(Vector[] dataset, int minR = 0)
        {
            Vector oldCentr, newCentr; // Центры гиперсфер

            datasetNotClasteris = _dataset = dataset; // Загрузка выборки
            oldCentr = mainCentr = Vector.Mean(_dataset); // Получение центра
            Rn = R0 = Max(_dataset, mainCentr);// Начальный радиус гиперсферы




            // Кластеризация
            while (datasetNotClasteris.Length != 0)
            {
                Rn = 0.9 * R0; // Уменьшение радиуса гиперсферы
                newDataset = GetHypersphere(Rn, datasetNotClasteris[0], datasetNotClasteris); // обводка гиперсферой
                newCentr = Vector.Mean(newDataset);// новый центр

                //Центр кластера
                while ((oldCentr != newCentr) && (Rn >= minR))
                {
                    Rn *= 0.9; //Уменьшение радиуса гиперсферы
                    oldCentr = newCentr; // сохранение старого радиуса
                    newDataset = GetHypersphere(Rn, oldCentr, datasetNotClasteris); // обводка гиперсферой					
                    newCentr = Vector.Mean(newDataset);// новый центр
                }

                claster = new Cluster
                {
                    Centr = newCentr,// Добавление центра
                    Dataset = newDataset// выборка
                };// Новый кластер
                clusters.Add(claster);// Добавление кластера в коллекцию
                datasetNotClasteris = AWithOutB(datasetNotClasteris, newDataset); // Удаление кластеризированных данных

            }
        }


        /// <summary>
        /// Распознать вектор
        /// </summary>
        public int Classify(Vector vector)
        {
            double minDist = double.PositiveInfinity;
            int indexClass = 0;

            for (int clusterIndex = 0; clusterIndex < Clusters.Length; clusterIndex++)
            {
                double dist = DistanceFunction(vector, Clusters[clusterIndex].Centr);
                if (dist < minDist)
                {
                    minDist = dist;
                    indexClass = clusterIndex;
                }
            }

            return indexClass;
        }

        /// <summary>Классификация векторов</summary>
        /// <param name="vectors">Векторы</param>
        public int[] Classify(IEnumerable<Vector> vectors)
        {
            return vectors.Select((vector) => Classify(vector)).ToArray();
        }


        // TODO: оптимизировать
        /// <summary>
        /// Проводит гиперсферу нужного радиуса из конкретной точки и на заданном множестве
        /// </summary>
        /// <param name="R">Радиус</param>
        /// <param name="m">Центр масс</param>
        /// <param name="pointsSet">Множество точек</param>
        /// <returns></returns>
        private Vector[] GetHypersphere(double R, Vector m, Vector[] pointsSet)
        {
            List<Vector> hypersphere = new List<Vector>();

            foreach (Vector point in pointsSet)
            {
                if (DistanceFunction(m, point) <= R)
                {
                    hypersphere.Add(point); // проведение окружности
                }
            }

            return hypersphere.ToArray();
        }



        /// <summary>
        /// Максимальная дистанция
        /// </summary>
        /// <returns></returns>
        private double Max(Vector[] mass, Vector m)
        {
            double max = DistanceFunction(mass[0], m), d;
            for (int i = 1; i < mass.Length; i++)
            {
                d = DistanceFunction(mass[i], m);

                if (max < d)
                {
                    max = d;
                }
            }
            return max;
        }



        // ToDo: Оптимизировать
        /// <summary>
        /// Множество А\В
        /// </summary>
        /// <param name="A">Множество А</param>
        /// <param name="B">Множество В</param>
        /// <returns>А\B</returns>
        private Vector[] AWithOutB(Vector[] A, Vector[] B)
        {
            List<Vector> C = new List<Vector>();

            for (int i = 0; i < A.Length; i++)
            {
                bool flag = true;

                for (int j = 0; j < B.Length; j++)
                {
                    if (A[i] == B[j])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    C.Add(A[i]);
                }
            }

            return C.ToArray();
        }







    }




}
