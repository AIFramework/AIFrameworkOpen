using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Algorithms.TransportTask
{
    /// <summary>
    /// Базовый класс для решения транспортной задачи
    /// </summary>
    [Serializable]
    public class BaseTransportTask
    {
        /// <summary>
        /// Матрица стоимостей
        /// </summary>
        public double[,] Costs { get; protected set; }

        /// <summary>
        /// Объемы у поставщиков
        /// </summary>
        public double[] Supply { get; protected set; }

        /// <summary>
        /// Потребности потребителей
        /// </summary>
        public double[] Demand { get; protected set; }

        /// <summary>
        /// Матрица распределения ресурсов
        /// </summary>
        public double[,] Allocation { get; protected set; }

        /// <summary>
        /// Число строк в таблице
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// Число строк в таблице
        /// </summary>
        public int Cols { get; protected set; }


        /// <summary>
        /// Проверка решения на оптимальность
        /// </summary>
        /// <param name="potentials">Потенциальное решение</param>
        /// <param name="minDeltaI"></param>
        /// <param name="minDeltaJ"></param>
        /// <returns></returns>
        public bool IsOptimal(out double[,] potentials, out int minDeltaI, out int minDeltaJ)
        {
            potentials = new double[2, Math.Max(Rows, Cols)];
            minDeltaI = -1;
            minDeltaJ = -1;

            bool[] uFound = new bool[Rows];
            bool[] vFound = new bool[Cols];

            uFound[0] = true;

            for (int k = 0; k < Rows + Cols; k++)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        if (Allocation[i, j] > 0)
                        {
                            if (uFound[i])
                            {
                                potentials[1, j] = Costs[i, j] - potentials[0, i];
                                vFound[j] = true;
                            }
                            else if (vFound[j])
                            {
                                potentials[0, i] = Costs[i, j] - potentials[1, j];
                                uFound[i] = true;
                            }
                        }
                    }
                }
            }

            double minDelta = 0;
            bool isOptimal = true;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (Allocation[i, j] == 0)
                    {
                        double delta = Costs[i, j] - (potentials[0, i] + potentials[1, j]);
                        if (delta < minDelta)
                        {
                            minDelta = delta;
                            minDeltaI = i;
                            minDeltaJ = j;
                            isOptimal = false;
                        }
                    }
                }
            }

            return isOptimal;
        }

        /// <summary>
        /// Получение полной стоимости
        /// </summary>
        /// <returns></returns>
        public double GetTotalCost() 
        {
            double totalCost = 0;
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    totalCost += Allocation[i, j] * Costs[i, j];
            
            return totalCost;
        }

        /// <summary>
        /// Получение средней стоимости
        /// </summary>
        /// <returns></returns>
        public double GetMeanCost() 
        {
            double totalCost = 0;
            int n = 0;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    double costEl = Allocation[i, j] * Costs[i, j];
                    totalCost += costEl;
                    if (costEl > 0) n++;
                }
            }

            return totalCost/n;
        }

        /// <summary>
        /// Отображение решения
        /// </summary>
        public void PrintSolution()
        {
            Console.WriteLine(ToString());
        }

        /// <summary>
        /// Перевод в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Оптимизированное распределение ресурсов:\n");
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                    stringBuilder.Append($"{Allocation[i, j]:0.00}\t");

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine($"\n\nОбщая стоимость решения: {GetTotalCost():0.00}");

            return stringBuilder.ToString();
        }
    }
}
