using AI.Algorithms.TransportTask.Methods;
using AI.Algorithms.TransportTask.PlanBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] costs = {
                { 8.5, 6.0, 10.1, 9.3 },
                { 9.2, 12.4, 13.8, 7.6 },
                { 14.5, 9.1, 16.3, 5.2 },
                { 14.5, 9.1, 16.3, 11.2 }
            };

            double[] supply = { 12, 1000, 100, 2 };
            double[] demand = { 101, 1, 1012, 0 };

            IInitialPlanBuilder initialPlanBuilder = new VogelApproximationMethod();
            PotentialMethod potentialMethod = new PotentialMethod(costs, supply, demand, initialPlanBuilder);
            
            potentialMethod.Solve();
            potentialMethod.PrintSolution();
        }
    }
}
