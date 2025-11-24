using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.ML.LinearModelTools;
using System;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Линейный 2х классовый классификатор
    /// </summary>
    [Serializable]
    public class LinearClassifierBinarry : BaseClassifier<LinearClassifierBinarry>
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
        /// L1 регуляризация
        /// </summary>
        public double L1 { get; set; } = 0;
        /// <summary>
        /// L2 регуляризация
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

        private Vector w;
        private double b = 0, nW;

        /// <summary>
        /// Линейный 2х классовый классификатор
        /// </summary>
        public LinearClassifierBinarry(int dim)
        {
            w = new Vector(dim) + (0.1 / dim);
            nW = Math.Sqrt(dim);
        }
        /// <summary>
        /// Классификация объектов
        /// </summary>
        public override int Classify(Vector inp)
        {
            double outp = Margin.OutputLinModel(inp, w, b);
            return outp >= 0 ? 1 : 0;
        }
        /// <summary>
        /// Классификация объектов (вектор вероятностей)
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
            Vector T = classes;
            GradientMargin<Vector, double> data;

            for (int i = 0; i < EpochesToPass; i++)
            {
                data = Margin.GetAverageMarginWithGradient(T, features, w, b, C, L1, L2, MinimalMargin);
                w -= LearningRate * data.GradientW;
                b -= LearningRate * data.GradientBias;
            }

            nW = AnalyticGeometryFunctions.NormVect(w);
        }
    }
}
