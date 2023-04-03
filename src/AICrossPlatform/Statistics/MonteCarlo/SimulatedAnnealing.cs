using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Statistics.MonteCarlo
{
    /// <summary>
    /// Метод имитации отжига
    /// </summary>
    [Serializable]
    public class SimulatedAnnealing
    {
        Random rnd;

        /// <summary>
        /// Предыдущая ошибка
        /// </summary>
        public double LastLoss { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        public double T { get; set; } = 50;

        /// <summary>
        /// Коэффициент уменьшения температуры
        /// </summary>
        public double Kt { get; set; } = 1.7;

        /// <summary>
        /// Метод имитации отжига
        /// </summary>
        public SimulatedAnnealing(double startLoss, int seed = -1)
        {
            LastLoss = startLoss;
            rnd = seed == -1 ? new Random() : new Random(seed);
        }

        /// <summary>
        /// Принимаем ли новое решение
        /// </summary>
        public bool IsAccept(double newLoss)
        {
            double dif = LastLoss - newLoss;
            double p = Math.Exp(dif / T);
            double treshold = rnd.NextDouble();
            bool isAccept = p > treshold;

            if (isAccept) LastLoss = newLoss;

            T /= Kt;
            return isAccept;
        }
    }
}
