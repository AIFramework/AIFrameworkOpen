/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 31.01.2016
 * Time: 11:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Numerics;
using Vector = AI.DataStructs.Algebraic.Vector;

namespace AI
{
    /// <summary>
    /// Класс реализует авто- и взаимо- кореляционные функции
    /// Для действительных и комплексных векторов
    /// </summary>
    [Serializable]
    public static class Correlation
    {
        #region Взаимокорелляция
        /// <summary>
        /// Cross-correlation of two real vectors
        /// </summary>
        /// <param name="A">Первый вектор</param>
        /// <param name="B">Второй вектор</param>
        public static Vector CrossCorrelation(Vector A, Vector B)
        {

            Vector ht = B - B.Mean();
            Vector signal = A - A.Mean();
            int nMax = signal.Count + ht.Count - 1;
            Vector st = Convolution.StWithHt(signal, ht.Count);
            Vector outp = new Vector(nMax);


            for (int i = 0; i < nMax; i++)
            {
                for (int j = 0; j < ht.Count; j++)
                {
                    outp[i] += st[i + j] * ht[j];
                }
            }


            double d1 = 0, d2 = 0;

            for (int i = 0; i < ht.Count; i++)
            {
                d1 += ht[i] * ht[i];
            }

            for (int i = 0; i < signal.Count; i++)
            {
                d2 += signal[i] * signal[i];
            }

            return outp / Math.Sqrt(d1 * d2);
        }


        /// <summary>
        /// Cross-correlation of two complex vectors
        /// </summary>
        /// <param name="A">Первый вектор</param>
        /// <param name="B">Второй вектор</param>
        public static ComplexVector CrossCorrelation(ComplexVector A, ComplexVector B)
        {
            ComplexVector ht = B - B.Mean();
            ComplexVector signal = A - A.Mean();
            int nMax = signal.Count + ht.Count - 1;
            ComplexVector st = Convolution.StWithHt(signal, ht.Count);
            ComplexVector outp = new ComplexVector(nMax);


            for (int i = 0; i < nMax; i++)
            {
                for (int j = 0; j < ht.Count; j++)
                {
                    outp[i] += st[i + j] * ht[j];
                }
            }


            Complex d1 = 0, d2 = 0;

            for (int i = 0; i < ht.Count; i++)
            {
                d1 += ht[i] * ht[i];
            }

            for (int i = 0; i < signal.Count; i++)
            {
                d2 += signal[i] * signal[i];
            }

            return outp / Complex.Sqrt(d1 * d2);
        }

        #endregion

        #region Авто-корреляция
        /// <summary>
        /// Автокорелляция действительного векторов
        /// </summary>
        /// <param name="A">Вектор</param>
        /// <returns>Возвращает осчеты АКФ</returns>
        public static Vector AutoCorrelation(Vector A)
        {
            return CrossCorrelation(A, A);
        }



        /// <summary>
        /// Автокорелляция комплексного векторов
        /// </summary>
        /// <param name="A">Вектор</param>
        /// <returns>Возвращает осчеты АКФ</returns>	
        public static ComplexVector AutoCorrelation(ComplexVector A)
        {
            return CrossCorrelation(A, A);
        }


        #endregion
    }
}
