using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.AlgorithmAnalysis
{
    [Serializable]
    public static class MetricsForRegression
    {
        private static readonly double _eps = 1e-100;

        private static double TSum(IAlgebraicStructure targ)
        {
            double ts = 0;

            for (int i = 0; i < targ.Shape.Count; i++)
            {
                ts += targ.Data[i];
            }

            return ts;
        }

        private static double AvMetric(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output, Func<IAlgebraicStructure, IAlgebraicStructure, double> metric)
        {
            int len = target.Count();
            double ret = 0;
            IAlgebraicStructure[] t = target.ToArray();
            IAlgebraicStructure[] o = output.ToArray();
            for (int i = 0; i < len; i++)
            {
                ret += metric(t[i], o[i]);
            }

            return ret / len;
        }

        public static double MAE(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            double ret = 0;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += Math.Abs(output.Data[i] - target.Data[i]);
            }

            return ret / output.Shape.Count;
        }

        public static double MAEPercent(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            double ret = 0;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += Math.Abs(output.Data[i] - target.Data[i]);
            }

            return ret / (TSum(output) + _eps);
        }

        public static double MAPE(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            double ret = 0;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += Math.Abs(output.Data[i] - target.Data[i]) / (target.Data[i] + _eps);
            }

            return ret / output.Shape.Count;
        }

        /// <summary>
        /// Средний квадрат ошибки
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double MSE(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            double ret = 0, q;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                q = output.Data[i] - target.Data[i];
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
        public static double RMSE(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            return Math.Sqrt(MSE(target, output));
        }
        /// <summary>
        /// RMLE (target[i]>-1, output[i]>-1 for all i \in [0; N-1])
        /// </summary>
        public static double RMSLE(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            Vector newTarg = new Vector(target.Shape.Count), newOut = new Vector(target.Shape.Count);

            for (int i = 0; i < target.Shape.Count; i++)
            {
                newOut[i] = Math.Log(output.Data[i] + 1);
                newTarg[i] = Math.Log(target.Data[i] + 1);
            }

            return RMSE(newTarg, newOut);
        }

        public static double RMSEPercent(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            return target.Shape.Count * Math.Sqrt(MSE(target, output)) / (TSum(target) + _eps);
        }

        /// <summary>
        /// Корреляция Пирсона в квадрате
        /// </summary>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static double R2(IAlgebraicStructure target, IAlgebraicStructure output)
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
        public static double Bias(IAlgebraicStructure target, IAlgebraicStructure output)
        {
            double ret = 0;

            for (int i = 0; i < output.Shape.Count; i++)
            {
                ret += output.Data[i] - target.Data[i];
            }

            return ret / output.Shape.Count;
        }

        public static double MAE(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, MAE);
        }

        public static double MAEPercent(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, MAEPercent);
        }

        public static double MAPE(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, MAPE);
        }

        public static double MSE(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, MSE);
        }

        public static double RMSE(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, RMSE);
        }

        public static double RMSEPercent(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, RMSEPercent);
        }

        public static double R2(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, R2);
        }

        public static double Bias(IEnumerable<IAlgebraicStructure> target, IEnumerable<IAlgebraicStructure> output)
        {
            return AvMetric(target, output, Bias);
        }
    }
}
