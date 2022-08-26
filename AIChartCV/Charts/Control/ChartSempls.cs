using AI.DataStructs;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.Charts.Control
{
    /// <summary>
    /// Данные графика
    /// </summary>
    [Serializable]
    public class ChartSample
    {
        /// <summary>
        /// Ось X
        /// </summary>
        public Vector Data { get; set; }
        /// <summary>
        /// Ось Y
        /// </summary>
        public Vector Steps { get; set; }

        /// <summary>
        /// Ускоренный поиск регионов
        /// </summary>
        public TableOfContentsOfTheSortedVector tableOfContentsOfX;


        /// <summary>
        /// Данные графика
        /// </summary>
        public ChartSample(Vector data, Vector steps)
        {

            ParallelDataChart[] parallelDatas = new ParallelDataChart[data.Count];

            for (int i = 0; i < data.Count; i++)
            {
                parallelDatas[i] = new ParallelDataChart
                {
                    x = steps[i],
                    y = data[i]
                };
            }


            List<ParallelDataChart> parallelDataCharts = new List<ParallelDataChart>();
            parallelDataCharts.AddRange(parallelDatas);

            // Сортирует по x

            Data = new Vector(data.Count);
            Steps = new Vector(data.Count);
            parallelDataCharts.Sort((a, b) => a.x.CompareTo(b.x));

            for (int i = 0; i < data.Count; i++)
            {
                Steps[i] = parallelDataCharts[i].x;
                Data[i] = parallelDataCharts[i].y;
            }

            tableOfContentsOfX = new TableOfContentsOfTheSortedVector(Steps);
        }

        /// <summary>
        /// Максимум по оси Y
        /// </summary>
        public double GetMaxData()
        {
            return Data.Max();
        }

        /// <summary>
        /// Максимум по оси X
        /// </summary>
        public double GetMaxSteps()
        {
            return Steps.Max();
        }


        /// <summary>
        /// Минимум по оси Y
        /// </summary>
        public double GetMinData()
        {
            return Data.Min();
        }

        /// <summary>
        /// Минимум по оси X
        /// </summary>
        public double GetMinSteps()
        {
            return Steps.Min();
        }


    }

    /// <summary>
    /// Коллекция
    /// </summary>
    public class ChartSempls : List<ChartSample>
    {

        /// <summary>
        /// Коллеция данных графика
        /// </summary>
        public ChartSempls()
        {

        }

        /// <summary>
        /// Добавление данных
        /// </summary>
        /// <param name="x">Вектор x</param>
        /// <param name="y">Вектор y</param>
        public void Add(Vector x, Vector y)
        {
            ChartSample chartSempl = new ChartSample(y, x);
            Add(chartSempl);

        }


        /// <summary>
        /// Максимум по оси Y
        /// </summary>
        public double GetMaxData()
        {
            double max = double.MinValue;

            for (int i = 0; i < Count; i++)
            {
                double semlMax = this[i].GetMaxData();

                if (semlMax > max && (!double.IsNaN(semlMax)))
                {
                    max = semlMax;
                }
            }

            return max;
        }

        /// <summary>
        /// Максимум по оси X
        /// </summary>
        public double GetMaxSteps()
        {
            double max = double.MinValue;

            for (int i = 0; i < Count; i++)
            {
                double semlMax = this[i].GetMaxSteps();

                if (semlMax > max && (!double.IsNaN(semlMax)))
                {
                    max = semlMax;
                }
            }

            return max;
        }


        /// <summary>
        /// Минимум по оси Y
        /// </summary>
        public double GetMinData()
        {
            double min = double.MaxValue;

            for (int i = 0; i < Count; i++)
            {
                double semlMin = this[i].GetMinData();

                if (semlMin < min && (!double.IsNaN(semlMin)))
                {
                    min = semlMin;
                }
            }

            return min;
        }

        /// <summary>
        /// Минимум по оси X
        /// </summary>
        public double GetMinSteps()
        {
            double min = double.MaxValue;

            for (int i = 0; i < Count; i++)
            {
                double semlMin = this[i].GetMinSteps();

                if (semlMin < min && (!double.IsNaN(semlMin)))
                {
                    min = semlMin;
                }
            }

            return min;
        }
    }

    internal class ParallelDataChart
    {
        public double x;
        public double y;


        // Возвращает вектора из списка точек
        public static Tuple<Vector, Vector> GetXY(List<ParallelDataChart> parallelDatas)
        {
            Vector x = new Vector(parallelDatas.Count), y = new Vector(parallelDatas.Count);

            for (int i = 0; i < x.Count; i++)
            {
                x[i] = parallelDatas[i].x;
                y[i] = parallelDatas[i].y;
            }

            return new Tuple<Vector, Vector>(x, y);
        }
    }



}
