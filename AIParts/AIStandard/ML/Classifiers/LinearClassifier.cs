using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.ML.LinearModelTools;
using System;

namespace AI.ML.Classifiers
{
    [Serializable]
    public class LinearClassifierBinarry : BaseClassifier<LinearClassifierBinarry>
    {
        public double LearningRate { get; set; } = 0.01;
        public int EpochesToPass { get; set; } = 10;
        public double L1 { get; set; } = 0;
        public double L2 { get; set; } = 0;
        public double MinimalMargin { get; set; } = 0;
        public double C { get; set; } = 1;

        private Vector w;
        private double b = 0, nW;

        public LinearClassifierBinarry(int dim)
        {
            w = new Vector(dim) + 0.1 / dim;
            nW = Math.Sqrt(dim);
        }

        public override int Classify(Vector inp)
        {
            double outp = Margin.OutputLinModel(inp, w, b);
            return outp >= 0 ? 1 : 0;
        }

        public override Vector ClassifyProbVector(Vector inp)
        {
            double outp = Margin.OutputLinModel(inp, w, b);
            outp /= nW + 1e-20;

            return ActivationFunctions.Softmax(new Vector(0.0 - outp, 0 + outp));
        }

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
