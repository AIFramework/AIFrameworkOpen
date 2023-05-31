using AI.DataStructs.Algebraic;
using AI.ML.AlgorithmAnalysis;
using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    /// <summary>
    /// Тестирование нейронной сети
    /// </summary>
    [Serializable]
    public class Tester
    {
        /// <summary>
        /// Тестирование
        /// </summary>
        /// <param name="g">Граф вычислений</param>
        /// <param name="net">Нейронная сеть</param>
        /// <param name="dataset">Набор данных для теста</param>
        /// <param name="tests">Тест, который следует выполнить</param>
        /// <returns></returns>
        public static double Test(INNWGraph g, INetwork net, IEnumerable<DataSequence> dataset, Metrics tests)
        {
            if ((int)tests > 3)
            {
                return RegressionTester.Test(g, net, dataset, tests);
            }
            else
            {
                return ClassifierTester.Test(g, net, dataset, tests);
            }
        }
    }

    [Serializable]
    internal class ClassifierTester
    {
        public static double Test(INNWGraph g, INetwork net, IEnumerable<DataSequence> dataset, Metrics classifTests)
        {
            List<int> y = new List<int>();
            List<int> t = new List<int>();

            foreach (DataSequence item in dataset)
            {
                net.ResetState(); // Перезапуск сети

                foreach (DataStep step in item.Steps)
                {
                    NNValue predict = net.Forward(step.Input, g);
                    y.Add(NNValueConverter.NNValueToClass(predict));
                    t.Add(NNValueConverter.NNValueToClass(step.TargetOutput));
                }
            }

            switch (classifTests)
            {
                case Metrics.Accuracy:
                    return MetricsForClassification.Accuracy(t.ToArray(), y.ToArray());
                case Metrics.Precision:
                    return MetricsForClassification.AveragePrecision(t.ToArray(), y.ToArray());
                case Metrics.Recall:
                    return MetricsForClassification.AverageRecall(t.ToArray(), y.ToArray());
                case Metrics.F1:
                    return MetricsForClassification.FMeasure(t.ToArray(), y.ToArray());
            }

            return 0;
        }
    }

    [Serializable]
    internal class RegressionTester
    {
        public static double Test(INNWGraph g, INetwork net, IEnumerable<DataSequence> dataset, Metrics regressionTests)
        {

            Vector y = new Vector();
            Vector t = new Vector();

            foreach (DataSequence item in dataset)
            {

                net.ResetState(); // Перезапуск сети
                foreach (DataStep step in item.Steps)
                {
                    NNValue predict = net.Forward(step.Input, g);
                    for (int i = 0; i < step.TargetOutput.Shape.Count; i++)
                    {
                        y.Add(predict[i]);
                        t.Add(step.TargetOutput[i]);
                    }
                }
            }

            switch (regressionTests)
            {
                case Metrics.MAE:
                    return MetricsForRegression.MAE(t, y);
                case Metrics.MAPE:
                    return MetricsForRegression.MAPE(t, y);
                case Metrics.MSE:
                    return MetricsForRegression.MSE(t, y);
                case Metrics.RMSE:
                    return MetricsForRegression.RMSE(t, y);
                case Metrics.R2:
                    return MetricsForRegression.R2(t, y);
                case Metrics.RMSLE:
                    return MetricsForRegression.RMSLE(t, y);

            }

            return 0;
        }
    }
}
