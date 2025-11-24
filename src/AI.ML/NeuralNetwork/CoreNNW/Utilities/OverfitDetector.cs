using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    /// <summary>
    /// Детектор определяющий переобучение нейронной сети
    /// </summary>
    [Serializable]
    public static class OverfitDetector
    {
        /// <summary>
        /// Проверка на переобучение
        /// </summary>
        /// <param name="valLoss">История изменения валидационного лосса</param>
        /// <param name="trainLoss">История изменения тренировочного лосса</param>
        public static bool IsOverfit(Vector valLoss, Vector trainLoss)
        {
            int k = 3, start = 5;

            if (valLoss.Count < start)
            {
                return false;
            }

            Vector newVal = valLoss.GetInterval(valLoss.Count - k - 1, valLoss.Count - 1);
            Vector newTrain = trainLoss.GetInterval(valLoss.Count - k - 1, valLoss.Count - 1);
            double trainStep = Statistics.Statistic.MeanStep(newTrain);
            double valStep = Statistics.Statistic.MeanStep(newVal);


            return trainStep < 0 && valStep > 0;
        }
    }
}