using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.ML.DataSets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AI.ML.LinearModelTools
{

    /// <summary>
    /// Работа с отступами
    /// </summary>
    [Serializable]
    public class Margin
    {
        /// <summary>
        /// Вычислить отступ
        /// </summary>
        /// <param name="ideal"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static double GetMargin(float ideal, float outp)
        {
            double t = Normal(ideal);
            return t * outp;
        }

        /// <summary>
        /// Вычислить отступ
        /// </summary>
        public static double GetMargin(double ideal, double outp)
        {
            double t = Normal(ideal);
            return t * outp;
        }

        /// <summary>
        /// Вычислить отступ
        /// </summary>
        public static double GetMargin(double ideal, Vector inp, Vector w, double b)
        {
            double outp = OutputLinModel(inp, w, b);
            return GetMargin(ideal, outp);
        }

        /// <summary>
        /// Выход линейной модели
        /// </summary>
        public static double OutputLinModel(Vector inp, Vector w, double b)
        {
            return AnalyticGeometryFunctions.Dot(inp, w) + b;
        }

        /// <summary>
        /// Получить опорные векторы
        /// </summary>
        /// <param name="ideal"></param>
        /// <param name="inp"></param>
        /// <param name="w"></param>
        /// <param name="b"></param>
        /// <param name="supportVectorsCount"></param>
        /// <returns></returns>
        public static VectorClass[] GetSupportVector(Vector ideal, Vector[] inp, Vector w, double b, int supportVectorsCount = 1)
        {
            int count = Math.Min(supportVectorsCount, inp.Length);
            VectorClass[] vectorClasses = new VectorClass[inp.Length];
            VectorClass[] data = new VectorClass[count];

            Parallel.For(0, inp.Length, i =>
            {
                VectorClass vectorClass = new VectorClass()
                {
                    R = GetMargin(ideal[i], inp[i], w, b),
                    Features = inp[i],
                    ClassMark = (int)ideal[i]
                };

                vectorClasses[i] = vectorClass;
            });

            System.Collections.Generic.List<VectorClass> vectorClassesList = vectorClasses.ToList();
            vectorClassesList.Sort((x, y) => x.R.CompareTo(y.R));

            for (int i = 0; i < count; i++)
            {
                data[i] = vectorClassesList[i];
            }

            return data;
        }

        //public static GradientMargin<Vector, double> GetMarginClassifierWithGradient(double ideal, Vector inp, Vector param)
        //{
        //    double tNorm = Normal(ideal);
        //    NNValue x, y, w, t;

        //    GraphCPU g = new GraphCPU();
        //    g.IsBackward = true; // Создавать функции для расчета градиента

        //    x = new NNValue(inp);
        //    w = new NNValue(param);
        //    t = new NNValue(tNorm);

        //    y = g.ScalarProduct(x, w);
        //    var margin = g.AdamarMul(y, t);

        //    var M = margin[0];
        //    var dif1 = M < 0 ? -1 : 0;

        //    margin.DifData[0] = dif1;
        //    g.Backward(); //Расчет производных

        //    return new GradientMargin<Vector, double>()
        //    { GradientW = w.DifData, Margin = M };
        //}
        
        /// <summary>
        /// Отступ с градиентами
        /// </summary>
        /// <param name="ideal"></param>
        /// <param name="inp"></param>
        /// <param name="param"></param>
        /// <param name="bias"></param>
        /// <param name="C"></param>
        /// <param name="minMargin"></param>
        /// <returns></returns>
        public static GradientMargin<Vector, double> GetMarginWithGradient(double ideal, Vector inp, Vector param, double bias, double C = 1, double minMargin = 0)
        {
            double tNorm = Normal(ideal);
            double margin = tNorm * OutputLinModel(inp, param, bias);
            double dif1 = margin - minMargin <= 0 ? -C : 0;
            Vector gradW = dif1 * tNorm * inp;
            double gradB = dif1 * tNorm;

            return new GradientMargin<Vector, double>()
            { GradientW = gradW, Margin = margin, GradientBias = gradB };
        }

        /// <summary>
        /// Средний отступ с градиентами (по всей выборке)
        /// </summary>
        public static GradientMargin<Vector, double> GetAverageMarginWithGradient(Vector ideal, Vector[] inp, Vector param, double bias, double C = 1, double l1 = 0.0, double l2 = 0.0, double minMargin = 0)
        {
            double margin = 0, gradB = 0;
            int n = inp.Length;
            Vector gradW = new Vector(inp[0].Count);

            for (int i = 0; i < n; i++)
            {
                GradientMargin<Vector, double> gData = GetMarginWithGradient(ideal[i], inp[i], param, bias, C, minMargin);
                margin += gData.Margin;
                gradW += gData.GradientW;
                gradB += gData.GradientBias;
            }

            gradB /= n;
            gradW /= n;
            gradB += (l2 * bias) + (l1 * Math.Sign(bias));
            gradW += (l2 * param) + (l1 * param.Transform(x => Math.Sign(x)));

            return new GradientMargin<Vector, double>()
            { GradientW = gradW, Margin = margin, GradientBias = gradB };
        }

        private static double Normal(double outp)
        {
            return outp > 0.5 ? 1 : -1;
        }
    }

    /// <summary>
    /// Отступ с графдиентами
    /// </summary>
    /// <typeparam name="T1">Тип градиента</typeparam>
    /// <typeparam name="T2">Тип отступа</typeparam>
    public class GradientMargin<T1, T2>
    {
        /// <summary>
        /// Градиенты вектора признаков
        /// </summary>
        public T1 GradientW { get; set; }
        /// <summary>
        /// Градиенты веса смещения
        /// </summary>
        public T2 GradientBias { get; set; }
        /// <summary>
        /// Отступ
        /// </summary>
        public T2 Margin { get; set; }
    }

    /// <summary>
    /// Отступ и вектор признаков
    /// </summary>
    public class MarginVector
    {
        /// <summary>
        /// Отступ
        /// </summary>
        public double Margin { get; set; }
        /// <summary>
        /// Вектор признаков
        /// </summary>
        public Vector Features { get; set; }
    }
}
