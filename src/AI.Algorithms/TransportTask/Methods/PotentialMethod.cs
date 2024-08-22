using AI.Algorithms.TransportTask.PlanBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.Algorithms.TransportTask.Methods
{
    /// <summary>
    /// Метод потенциалов
    /// </summary>
    [Serializable]
    public class PotentialMethod : BaseTransportTask
    {
        
        /// <summary>
        /// Метод построения начального опорного плана
        /// </summary>
        private readonly IInitialPlanBuilder _initialPlanBuilder;

        /// <summary>
        /// Метод потенциалов
        /// </summary>
        public PotentialMethod(double[,] costs, double[] supply, double[] demand, IInitialPlanBuilder initialPlanBuilder)
        {
            Costs = costs;
            Supply = supply;
            Demand = demand;
            Rows = supply.Length;
            Cols = demand.Length;
            _initialPlanBuilder = initialPlanBuilder;
            Allocation = new double[Rows, Cols];
        }

        /// <summary>
        /// Решение задачи методом потенциалов
        /// </summary>
        public void Solve()
        {
            Allocation = _initialPlanBuilder.BuildInitialPlan(Costs, Supply, Demand);

            while (true)
            {
                if (IsOptimal(out double[,] potentials, out int minDeltaI, out int minDeltaJ)) break;
                if(!ImproveSolution(minDeltaI, minDeltaJ)) break;
            }
        }

        /// <summary>
        /// Метод улучшения решения
        /// </summary>
        /// <param name="minDeltaI"></param>
        /// <param name="minDeltaJ"></param>
        private bool ImproveSolution(int minDeltaI, int minDeltaJ)
        {
            List<Tuple<int, int>> loop = FindLoop(minDeltaI, minDeltaJ);

            var negativeCycleValues = loop.Where((cell, index) => index % 2 == 1)
                                          .Select(cell => Allocation[cell.Item1, cell.Item2])
                                          .ToList();

            // Если решение не может быть улучшено
            if (negativeCycleValues.Count == 0)
                return false;

            double theta = negativeCycleValues.Min();

            for (int i = 0; i < loop.Count; i++)
            {
                var cell = loop[i];
                Allocation[cell.Item1, cell.Item2] += (i % 2 == 0 ? 1 : -1) * theta;
            }

            return true;
        }


        // Поиск цикла
        private List<Tuple<int, int>> FindLoop(int startI, int startJ)
        {
            var loop = new List<Tuple<int, int>> { Tuple.Create(startI, startJ) };
            var visited = new HashSet<Tuple<int, int>> { Tuple.Create(startI, startJ) };

            FindLoopRecursively(startI, startJ, loop, true, visited);
           
            return loop;
        }

        // Рекурсивный алгоритм поиска циклов
        private bool FindLoopRecursively(int currentI, int currentJ, List<Tuple<int, int>> loop, bool rowOrCol, HashSet<Tuple<int, int>> visited)
        {
            if (loop.Count > 3 && loop.First().Equals(loop.Last()))
            {
                return true;  // Цикл замкнулся
            }

            if (rowOrCol)
            {
                // Перемещение по строке
                for (int j = 0; j < Cols; j++)
                {
                    var nextCell = Tuple.Create(currentI, j);
                    if (j != currentJ && Allocation[currentI, j] > 0 && !visited.Contains(nextCell))
                    {
                        loop.Add(nextCell);
                        visited.Add(nextCell);

                        if (FindLoopRecursively(currentI, j, loop, !rowOrCol, visited))
                        {
                            return true;
                        }

                        loop.RemoveAt(loop.Count - 1);
                        visited.Remove(nextCell);
                    }
                }
            }
            else
            {
                // Перемещение по столбцу
                for (int i = 0; i < Rows; i++)
                {
                    var nextCell = Tuple.Create(i, currentJ);
                    if (i != currentI && Allocation[i, currentJ] > 0 && !visited.Contains(nextCell))
                    {
                        loop.Add(nextCell);
                        visited.Add(nextCell);

                        if (FindLoopRecursively(i, currentJ, loop, !rowOrCol, visited))
                        {
                            return true;
                        }

                        loop.RemoveAt(loop.Count - 1);
                        visited.Remove(nextCell);
                    }
                }
            }

            return false;
        }


    }
}
