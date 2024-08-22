using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.Algorithms.TransportTask.PlanBuilders
{

    /// <summary>
    /// Метод Фогеля для создания начального плана
    /// </summary>
    public class VogelApproximationMethod : IInitialPlanBuilder
    {
        /// <summary>
        /// Метод создания начального плана
        /// </summary>
        /// <param name="costs">Матрица стоимостей</param>
        /// <param name="supply">Вектор предложений</param>
        /// <param name="demand">Вектор потребностей</param>
        public double[,] BuildInitialPlan(double[,] costs, double[] supply, double[] demand)
        {
            int rows = supply.Length;
            int cols = demand.Length;
            double[,] allocation = new double[rows, cols];
            double[] supplyCopy = (double[])supply.Clone();
            double[] demandCopy = (double[])demand.Clone();

            while (supplyCopy.Sum() > 0 && demandCopy.Sum() > 0)
            {
                int maxPenalty = int.MinValue;
                int selectedRow = -1, selectedCol = -1;
                bool isRowSelected = true;

                // Row penalties
                for (int i = 0; i < rows; i++)
                {
                    if (supplyCopy[i] == 0) continue;
                    var sortedCosts = Enumerable.Range(0, cols)
                                                .Where(j => demandCopy[j] > 0)
                                                .Select(j => costs[i, j])
                                                .OrderBy(c => c)
                                                .ToList();

                    int penalty = sortedCosts.Count > 1 ? (int)(sortedCosts[1] - sortedCosts[0]) : (int)sortedCosts[0];
                    if (penalty > maxPenalty)
                    {
                        maxPenalty = penalty;
                        selectedRow = i;
                        isRowSelected = true;
                    }
                }

                // Column penalties
                for (int j = 0; j < cols; j++)
                {
                    if (demandCopy[j] == 0) continue;
                    var sortedCosts = Enumerable.Range(0, rows)
                                                .Where(i => supplyCopy[i] > 0)
                                                .Select(i => costs[i, j])
                                                .OrderBy(c => c)
                                                .ToList();

                    int penalty = sortedCosts.Count > 1 ? (int)(sortedCosts[1] - sortedCosts[0]) : (int)sortedCosts[0];
                    if (penalty > maxPenalty)
                    {
                        maxPenalty = penalty;
                        selectedCol = j;
                        isRowSelected = false;
                    }
                }

                if (isRowSelected)
                {
                    selectedCol = Enumerable.Range(0, cols)
                                            .Where(j => demandCopy[j] > 0)
                                            .OrderBy(j => costs[selectedRow, j])
                                            .First();
                }
                else
                {
                    selectedRow = Enumerable.Range(0, rows)
                                            .Where(i => supplyCopy[i] > 0)
                                            .OrderBy(i => costs[i, selectedCol])
                                            .First();
                }

                double allocationValue = Math.Min(supplyCopy[selectedRow], demandCopy[selectedCol]);
                allocation[selectedRow, selectedCol] = allocationValue;
                supplyCopy[selectedRow] -= allocationValue;
                demandCopy[selectedCol] -= allocationValue;
            }

            return allocation;
        }
    }
}
