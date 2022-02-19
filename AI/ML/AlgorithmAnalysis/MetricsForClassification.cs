using AI.DataStructs.Algebraic;
using System;
using System.Linq;

namespace AI.ML.AlgorithmAnalysis
{
    /// <summary>
    /// Quality metrics for assessing classification
    /// </summary>
    [Serializable]
    public static class MetricsForClassification
    {
        /// <summary>
        /// Accuracy
        /// </summary>
        public static double Accuracy(int[] real, int[] outAlg)
        {
            double y = 0;
            for (int i = 0; i < real.Length; i++)
            {
                if (real[i] == outAlg[i])
                {
                    y++;
                }
            }

            return y / real.Length;
        }
        /// <summary>
        /// Algorithm precision value for each class
        /// </summary>
        public static Vector PrecisionForEachClass(int[] real, int[] outAlg)
        {
            int classes = GetClasses(real, outAlg);
            double[] tp = new double[classes], all = new double[classes], ac = new double[classes];

            for (int i = 0; i < real.Length; i++)
            {
                if (real[i] == outAlg[i])
                {
                    tp[real[i]]++;
                }

                all[real[i]]++;
            }

            for (int i = 0; i < classes; i++)
            {
                ac[i] = tp[i] / all[i];
            }

            return new Vector(ac);
        }
        /// <summary>
        /// Average precision across all classes
        /// </summary>
        public static double AveragePrecision(int[] real, int[] outAlg)
        {
            return PrecisionForEachClass(real, outAlg).Mean();
        }
        /// <summary>
        /// Confusion matrix (confusion)
        /// </summary>
        public static Matrix ConfusionMatrix(int[] real, int[] outAlg)
        {
            int classes = GetClasses(real, outAlg);
            Matrix matrix = new Matrix(classes, classes);

            for (int i = 0; i < real.Length; i++)
            {
                matrix[real[i], outAlg[i]]++;
            }

            return matrix;
        }
        /// <summary>
        /// Recall
        /// </summary>
        public static double Recall(int[] real, int[] outAlg)
        {
            Matrix matrix = ConfusionMatrix(real, outAlg);
            int classes = GetClasses(real, outAlg);
            double denom = 0;
            double nom = 0;

            for (int i = 0; i < classes; i++)
            {
                for (int j = 0; j < classes; j++)
                {
                    denom += matrix[j, i];
                }

                nom += matrix[i, i];
            }

            return nom / denom;
        }
        /// <summary>
        /// Algorithm recall value for each class
        /// </summary>
        public static Vector RecallForEachClass(int[] real, int[] outAlg)
        {
            Matrix matrix = ConfusionMatrix(real, outAlg);
            int classes = GetClasses(real, outAlg);
            Vector data = new Vector(classes);

            for (int i = 0; i < classes; i++)
            {
                double denom = 0;
                for (int j = 0; j < classes; j++)
                {
                    denom += matrix[j, i];
                }

                data[i] = matrix[i, i] / denom;
            }

            return data;
        }
        /// <summary>
        /// Average recall across all classes
        /// </summary>
        public static double AverageRecall(int[] real, int[] outAlg)
        {
            return RecallForEachClass(real, outAlg).Mean();
        }
        /// <summary>
        /// F measure formula: 2 * recall * precision / (recall + precision)
        /// </summary>
        public static double FMeasure(int[] real, int[] outAlg)
        {
            double pr = AveragePrecision(real, outAlg);
            double rec = AverageRecall(real, outAlg);

            return 2.0 * pr * rec / (pr + rec);
        }
        /// <summary>
        /// F measure formula: (1+beta^2) * recall * precision / (recall + beta^2 * precision)
        /// </summary>
        public static double FMeasure(int[] real, int[] outAlg, double beta)
        {
            double pr = AveragePrecision(real, outAlg);
            double rec = AverageRecall(real, outAlg);
            double b2 = beta * beta;

            return (1 + b2) * pr * rec / (b2 * pr + rec);
        }
        /// <summary>
        /// Complete classifier metrics report
        /// </summary>
        public static string FullReport(int[] real, int[] outAlg, double betaForFMeasure = 1, bool isForEachClass = false)
        {
            string report = $"Precision:             {GetElementReport(AveragePrecision(real, outAlg))}";
            report += $"\nAverage Recall:      {GetElementReport(AverageRecall(real, outAlg))}";
            report += $"\nFMeasure:            {GetElementReport(FMeasure(real, outAlg, betaForFMeasure))}";
            report += $"\nAccuracy:            {GetElementReport(Accuracy(real, outAlg))}";

            if (isForEachClass)
            {
                report += "\n\n\n--Precision value for each class--\n";

                Vector pr = PrecisionForEachClass(real, outAlg);

                for (int i = 0; i < pr.Count; i++)
                {
                    report += $"\nClass #{i + 1}:  {GetElementReport(pr[i])}";
                }
            }

            return report;
        }

        private static string GetElementReport(double p)
        {
            double outpP = Math.Round(p, 4);
            return $"{outpP:N4}\t{outpP * 100:N2}%";
        }

        private static int GetClasses(int[] real, int[] outAlg)
        {
            return Math.Max(real.Max(), outAlg.Max()) + 1;
        }
    }
}
