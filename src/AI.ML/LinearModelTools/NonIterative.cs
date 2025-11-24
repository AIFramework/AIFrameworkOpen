using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.LinearModelTools
{
    /// <summary>
    /// Безытеративное обучение (Двуклассовый)
    /// </summary>
    [Serializable]
    public class NonIterativeTwoClass
    {
        /// <summary>
        /// Вектор весов
        /// </summary>
        public Vector W { get; set; }

        /// <summary>
        /// Вес смещения
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Обучение классификатора
        /// </summary>
        /// <param name="vectorsCL1">Объекты класса 1</param>
        /// <param name="vectorsCL2">Объекты класса 2</param>
        public void Train(IEnumerable<Vector> vectorsCL1, IEnumerable<Vector> vectorsCL2)
        {
            Vector[] vectorsArr1 = vectorsCL1.ToArray();
            Vector[] vectorsArr2 = vectorsCL2.ToArray();

            Vector mean1 = Vector.Mean(vectorsArr1);
            Vector mean2 = Vector.Mean(vectorsArr2);

            W = mean2 - mean1;

            Vector v1Proj = ProjW(vectorsCL1);
            Vector v2Proj = ProjW(vectorsCL2);
            B = -QSolve(v1Proj, v2Proj);
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="vect">Вектор</param>
        public bool Forward(Vector vect)
        {
            var dot = AnalyticGeometryFunctions.Dot(W, vect) + B;
            return dot > 0;
        }

        // Решения квадратного уравнения для поиска значения B
        private double QSolve(Vector v1, Vector v2)
        {
            double pv1 = v1.Mean(), pv2 = v2.Mean();
            double ps1 = v1.Std(), ps2 = v2.Std();
            double pls1 = Math.Log(ps1), pls2 = Math.Log(ps2);

            ps1 *= ps1;
            ps2 *= ps2;

            double a = 2 * (ps1 - ps2);
            double b = 4 * (ps2 * pv1 - ps1 * pv2);
            double c = 2 * ps1 * pv2 * pv2 - 2 * ps2 * pv1 * pv1 - ps2 * ps1 * (pls1 - pls2);

            // Calculating the discriminant (d) and the roots (M1 and M2)
            double d = Math.Sqrt(b * b - 4 * a * c);
            double m1 = (-b + d) / (2 * a);
            double m2 = (-b - d) / (2 * a);

            double m = (m1 < pv2 && m1 > pv1) ? m1 : m2;

            return m;
        }


        // Проекция на вектор весов
        private Vector ProjW(IEnumerable<Vector> vectors)
        {
            Vector[] vectorsArr = vectors.ToArray();
            Vector vectProj = new Vector(vectorsArr[0].Count);

            for (int i = 0; i < vectorsArr.Length; i++)
                for (int j = 0; j < vectorsArr[0].Count; j++)
                    vectProj[i] += vectorsArr[i][j] * W[j];

            return vectProj;
        }


    }
}
