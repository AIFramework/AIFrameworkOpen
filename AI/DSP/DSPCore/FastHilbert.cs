/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 03.06.2017
 * Время: 17:17
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using System;
using System.Numerics;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Description of Hilbert.
    /// </summary>
    public static class FastHilbert
    {
        /// <summary>
        /// Сигнал сопряженный по Гильберту
        /// </summary>
        /// <param name="st">Исходный сигнал</param>
        public static Vector ConjugateToTheHilbert(Vector st)
        {
            Vector stNew = st.CutAndZero(Functions.NextPow2(st.Count));
            ComplexVector cv = FFT.CalcFFT(stNew);

            Complex j = new Complex(0, 1);
            Complex mj = -j;

            int n1 = stNew.Count / 2, n2 = stNew.Count;


            for (int i = 0; i < n1; i++)
            {
                cv[i] = cv[i] * mj;
            }


            for (int i = n1; i < n2; i++)
            {
                cv[i] = cv[i] * j;
            }

            cv = FFT.CalcIFFT(cv).CutAndZero(st.Count);
            return cv.RealVector;
        }
        /// <summary>
        /// Аналитический сигнал
        /// </summary>
        /// <param name="st">Входной сигнал</param>
        public static ComplexVector GetAnalSig(Vector st)
        {
            ComplexVector cv = new ComplexVector(st.Count);
            Vector stH = ConjugateToTheHilbert(st);

            for (int i = 0; i < st.Count; i++)
            {
                cv[i] = new Complex(st[i], stH[i]);
            }

            return cv;
        }
        /// <summary>
        /// Огибающая
        /// </summary>
        /// <param name="st">Входной сигнал</param>
        public static Vector Envelope(Vector st)
        {
            return GetAnalSig(st).MagnitudeVector;
        }

        /// <summary>
        /// Мгновенная фаза
        /// </summary>
        /// <param name="st">Входной сигнал</param>
        public static Vector Phase(Vector st)
        {
            return GetAnalSig(st).PhaseVector;
        }

        /// <summary>
        /// Мгновенная частота
        /// </summary>
        /// <param name="st">Входной сигнал</param>
        public static Vector Frequency(Vector st)
        {
            return Functions.Diff(GetAnalSig(st).PhaseVector);
        }


        /// <summary>
        /// Выделение огибающей на базе квадратурн. сост
        /// </summary>
        public static Vector EnvelopeIQ(Vector st, double fd, double f0)
        {
            double _2pi = Math.PI * 2;
            ComplexVector complexVector = Filters.ButterworthLowCFH(st.Count, f0, (int)fd, 5); // передаточная ф-я фильтра Батерворта 5-го порядка


            Vector t = new Vector(st.Count);

            for (int i = 0; i < t.Count; i++)
            {
                t[i] = i / fd;
            }

            Vector cos = new Vector(t.Count);
            Vector sin = new Vector(t.Count);

            for (int i = 0; i < t.Count; i++)
            {
                double arg = _2pi * f0 * t[i];

                cos[i] = st[i] * Math.Cos(arg);
                sin[i] = -st[i] * Math.Sin(arg);
            }

            cos = Filters.Filter(cos, complexVector, true);
            sin = Filters.Filter(sin, complexVector, true);


            return cos * cos + sin * sin;

        }


        /// <summary>
        /// Выделение девиации частоты на базе квадратурн. сост
        /// </summary>
        public static Vector PhaseIQ(Vector st, double fd, double f0)
        {
            double _2pi = Math.PI * 2;
            ComplexVector complexVector = Filters.ButterworthLowCFH(st.Count, f0, (int)fd, 5); // передаточная ф-я фильтра Батерворта 5-го порядка


            Vector t = new Vector(st.Count);

            for (int i = 0; i < t.Count; i++)
            {
                t[i] = i / fd;
            }

            Vector cos = new Vector(t.Count);
            Vector sin = new Vector(t.Count);

            for (int i = 0; i < t.Count; i++)
            {
                double arg = _2pi * f0 * t[i];

                cos[i] = st[i] * Math.Cos(arg);
                sin[i] = st[i] * Math.Sin(arg);
            }

            cos = Filters.Filter(cos, complexVector, true);
            sin = Filters.Filter(sin, complexVector, true);

            Vector ph = (sin / cos).Transform(x => Math.Atan(x));
            ph = FunctionsForEachElements.Unwrap(ph);


            return ph;

        }
        /// <summary>
        /// Выделение девиации частоты на базе квадратурн. сост
        /// </summary>
        public static Tuple<Vector, Vector> IQ(Vector st, double fd, double f0)
        {
            double _2pi = Math.PI * 2;
            ComplexVector complexVector = Filters.ButterworthLowCFH(st.Count, f0, (int)fd, 5); // передаточная ф-я фильтра Батерворта 5-го порядка


            Vector t = new Vector(st.Count);

            for (int i = 0; i < t.Count; i++)
            {
                t[i] = i / fd;
            }

            Vector cos = new Vector(t.Count);
            Vector sin = new Vector(t.Count);

            for (int i = 0; i < t.Count; i++)
            {
                double arg = _2pi * f0 * t[i];

                cos[i] = st[i] * Math.Cos(arg);
                sin[i] = st[i] * Math.Sin(arg);
            }

            cos = Filters.Filter(cos, complexVector, true);
            sin = Filters.Filter(sin, complexVector, true);

            return new Tuple<Vector, Vector>(cos, sin);

        }
    }
}