using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.ML.LinearModelTools;
using System;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Машина опорных векторов (для 2 классов)
    /// </summary>
    [Serializable]
    public class SVMBinary : BaseClassifier<SVMBinary>
    {
        /// <summary>
        /// Скорость обучения
        /// </summary>
        public double LearningRate { get; set; } = 0.01;
        /// <summary>
        /// Число эпох
        /// </summary>
        public int EpochesToPass { get; set; } = 10;
        /// <summary>
        /// Регуляризация L1
        /// </summary>
        public double L1 { get; set; } = 0;
        /// <summary>
        /// Регуляризация L2
        /// </summary>
        public double L2 { get; set; } = 0;
        /// <summary>
        /// Минимальный отступ
        /// </summary>
        public double MinimalMargin { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public double C { get; set; } = 1;
        /// <summary>
        /// Число опорных векторов
        /// </summary>
        public int NumSupportVectors { get; set; } = 2;

        private Vector w;
        private double b = 0, nW;

        /// <summary>
        /// Машина опорных векторов (для 2 классов)
        /// </summary>
        public SVMBinary(int dim)
        {
            w = new Vector(dim) + (0.1 / dim);
            nW = Math.Sqrt(dim);
        }

        /// <summary>
        /// Классификация
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public override int Classify(Vector inp)
        {
            double outp = Margin.OutputLinModel(inp, w, b);
            return outp >= 0 ? 1 : 0;
        }

        /// <summary>
        /// Классификация (вектор вероятностей)
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public override Vector ClassifyProbVector(Vector inp)
        {
            double outp = Margin.OutputLinModel(inp, w, b);
            outp /= nW + 1e-20;

            return ActivationFunctions.Softmax(new Vector(0.0 - outp, 0 + outp));
        }

        /// <summary>
        /// Обучение
        /// </summary>
        /// <param name="features"></param>
        /// <param name="classes"></param>
        public override void Train(Vector[] features, int[] classes)
        {
            GradientMargin<Vector, double> data;

            for (int i = 0; i < EpochesToPass; i++)
            {
                Tuple<Vector[], int[]> sv = GetSupportVectors(features, classes); // Получение опорных векторов

                data = Margin.GetAverageMarginWithGradient(sv.Item2, sv.Item1, w, b, C, L1, L2, MinimalMargin); // Расчет градиента по опрным векторам

                w -= LearningRate * data.GradientW;
                b -= LearningRate * data.GradientBias;
            }

            nW = AnalyticGeometryFunctions.NormVect(w);
        }

        private Tuple<Vector[], int[]> GetSupportVectors(Vector[] features, int[] classes)
        {
            DataSets.VectorClass[] data = Margin.GetSupportVector(classes, features, w, b, NumSupportVectors);
            Vector[] feats = new Vector[data.Length];
            int[] marks = new int[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                feats[i] = data[i].Features;
                marks[i] = data[i].ClassMark;
            }

            return new Tuple<Vector[], int[]>(feats, marks);
        }


    }
}
