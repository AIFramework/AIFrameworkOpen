using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.AlgorithmAnalysis
{

    /// <summary>
    /// Метрики оценки регрессионных моделей
    /// </summary>
    [Serializable]
    public static class MetricsForRegression
    {
        private static readonly double _eps = 1e-100;

        // Сумма целевых значений
        private static double TSum(IAlgebraicStructure<double> targ)
        {
            double ts = 0;
            double[] data = targ.Data;

            for (int i = 0; i < targ.Shape.Count; i++)
                ts += data[i];

            return ts;
        }

        /// <summary>
        /// Усредненная метрика
        /// </summary>
        private static double AvMetric(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output, Func<IAlgebraicStructure<double>, IAlgebraicStructure<double>, double> metric)
        {
            int len = target.Count();
            double ret = 0;
            IAlgebraicStructure<double>[] t = target.ToArray();
            IAlgebraicStructure<double>[] o = output.ToArray();
            for (int i = 0; i < len; i++)
            {
                ret += metric(t[i], o[i]);
            }

            return ret / len;
        }

        /// <summary>
        /// Cредняя абсолютная ошибка
        /// </summary>
        public static double MAE(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double ret = 0;
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += Math.Abs(dataO[i] - dataT[i]);
            }

            return ret / output.Shape.Count;
        }

        /// <summary>
        /// Cредняя абсолютная ошибка (В процентах)
        /// </summary>
        public static double MAEPercent(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double ret = 0;
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += Math.Abs(dataO[i] - dataT[i]);
            }

            return ret / (TSum(output) + _eps);
        }

        /// <summary>
        /// Средняя абсолютная процентная ошибка
        /// </summary>
        public static double MAPE(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double ret = 0;
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < output.Shape.Count; i++)
                ret += Math.Abs(dataO[i] - dataT[i]) / Math.Abs(dataT[i] + _eps);


            return ret / output.Shape.Count;
        }

        /// <summary>
        /// Средний квадрат ошибки
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double MSE(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double ret = 0, q;
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                q = dataO[i] - dataT[i];
                ret += q * q;
            }

            return ret / output.Shape.Count;
        }

        /// <summary>
        /// Корень из среднего квадрата ошибки
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double RMSE(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            return Math.Sqrt(MSE(target, output));
        }
        /// <summary>
        /// RMLE (target[i]>-1, output[i]>-1 for all i \in [0; N-1])
        /// </summary>
        public static double RMSLE(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            Vector newTarg = new Vector(target.Shape.Count), newOut = new Vector(target.Shape.Count);
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < target.Shape.Count; i++)
            {
                newOut[i] = Math.Log(dataO[i] + 1);
                newTarg[i] = Math.Log(dataT[i] + 1);
            }

            return RMSE(newTarg, newOut);
        }

        /// <summary>
        /// Корень из среднего квадрата ошибки в процентах
        /// </summary>
        public static double RMSEPercent(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            return target.Shape.Count * Math.Sqrt(MSE(target, output)) / (TSum(target) + _eps);
        }

        /// <summary>
        /// Корреляция Пирсона в квадрате
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double R2(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double cor = Statistics.Statistic.CorrelationCoefficient(target, output);
            return cor * cor;
        }

        /// <summary>
        /// Среднее смещение относительно 0
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double Bias(IAlgebraicStructure<double> target, IAlgebraicStructure<double> output)
        {
            double ret = 0;
            double[] dataO = output.Data;
            double[] dataT = target.Data;

            for (int i = 0; i < output.Shape.Count; i++)
                ret += dataO[i] - dataT[i];


            return ret / output.Shape.Count;
        }

        /// <summary>
        /// Cредняя абсолютная ошибка
        /// </summary>
        public static double MAE(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, MAE);
        }
        /// <summary>
        /// Cредняя абсолютная ошибка (В процентах)
        /// </summary>
        public static double MAEPercent(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, MAEPercent);
        }
        /// <summary>
        /// Средняя абсолютная процентная ошибка
        /// </summary>
        public static double MAPE(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, MAPE);
        }
        /// <summary>
        /// Средний квадрат ошибки
        /// </summary>
        public static double MSE(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, MSE);
        }
        /// <summary>
        /// Корень из среднего квадрата ошибки
        /// </summary>
        public static double RMSE(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, RMSE);
        }
        /// <summary>
        /// Корень из среднего квадрата ошибки в процентах
        /// </summary>
        public static double RMSEPercent(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, RMSEPercent);
        }
        /// <summary>
        /// Корреляция Пирсона в квадрате
        /// </summary>
        public static double R2(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, R2);
        }
        /// <summary>
        /// Среднее смещение относительно 0
        /// </summary>
        public static double Bias(IEnumerable<IAlgebraicStructure<double>> target, IEnumerable<IAlgebraicStructure<double>> output)
        {
            return AvMetric(target, output, Bias);
        }
    }
}
