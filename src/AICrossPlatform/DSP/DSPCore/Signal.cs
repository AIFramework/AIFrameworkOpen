/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 05.06.2017
 * Время: 11:12
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Description of Signal.
    /// </summary>
    public static class Signal
    {

        #region Синус
        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        /// <param name="fi">Начальная фаза</param>
        public static Vector Sin(Vector t, double A, double f, double fi)
        {
            return A * FunctionsForEachElements.Sin((t * 2 * Math.PI * f) + fi);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        /// <param name="fi">Начальная фаза</param>
        public static Vector Sin(Vector t, double A, Vector f, double fi)
        {
            return A * FunctionsForEachElements.Sin((t * 2 * Math.PI * f) + fi);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        /// <param name="fi">Начальная фаза</param>
        public static Vector Sin(Vector t, double A, double f, Vector fi)
        {
            return A * FunctionsForEachElements.Sin((t * 2 * Math.PI * f) + fi);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        /// <param name="fi">Начальная фаза</param>
        public static Vector Sin(Vector t, Vector A, double f, double fi)
        {
            return A * FunctionsForEachElements.Sin((t * 2 * Math.PI * f) + fi);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        public static Vector Sin(Vector t, double A, double f)
        {
            return A * FunctionsForEachElements.Sin(t * 2 * Math.PI * f);
        }

        /// <summary>
        /// Массив частот
        /// </summary>
        /// <param name="Count">Кол-во значений</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <returns>Вектор частот</returns>
        public static Vector Frequency(int Count, double fd)
        {
            double dt = 1.0 / fd, df = 1 / (Count * dt);
            return FunctionsForEachElements.GenerateTheSequence(0, df, Count * df).CutAndZero(Count);
        }


        /// <summary>
        /// Центрированный массив частот 
        /// </summary>
        /// <param name="Count">Кол-во значений</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <returns>Вектор частот</returns>
        public static Vector FrequencyCentr(int Count, double fd)
        {
            double dt = 1.0 / fd, df = 1 / (Count * dt);
            return FunctionsForEachElements.GenerateTheSequence(-Count / 2.0 * df, df, Count / 2.0 * df).CutAndZero(Count);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        public static Vector Sin(Vector t, double A, Vector f)
        {
            return A * FunctionsForEachElements.Sin(t * 2 * Math.PI * f);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">частота</param>
        public static Vector Sin(Vector t, Vector A, double f)
        {
            return A * FunctionsForEachElements.Sin(t * 2 * Math.PI * f);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="f">частота</param>
        public static Vector Sin(Vector t, double f)
        {
            return FunctionsForEachElements.Sin(t * 2 * Math.PI * f);
        }

        /// <summary>
        /// Синусоидальные колебания
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="f">частота</param>
        public static Vector Sin(Vector t, Vector f)
        {
            return FunctionsForEachElements.Sin(t * 2 * Math.PI * f);
        }
        #endregion


        #region Прямоугольный

        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">Частота</param>
        /// <param name="fi">Фаза</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double A, double f, double fi)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f, fi), 0.1);
        }

        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Вектор амплитуд</param>
        /// <param name="f">Частота</param>
        /// <param name="fi">Фаза</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, Vector A, double f, double fi)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f, fi), 0.1);
        }

        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">Вектор частот</param>
        /// <param name="fi">Фаза</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double A, Vector f, double fi)
        {
            return A * ActivationFunctions.Threshold(Sin(t, A, 1, fi), 0.1);
        }

        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">Частота</param>
        /// <param name="fi">Вектор фаз</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double A, double f, Vector fi)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f, fi), 0.1);
        }


        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">Частота</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double A, double f)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f), 0.1);
        }


        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f">Вектор частот</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double A, Vector f)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f), 0.1);
        }


        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="A">Вектор амплитуда</param>
        /// <param name="f">Вектор частот</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, Vector A, double f)
        {
            return A * ActivationFunctions.Threshold(Sin(t, 1, f), 0.1);
        }


        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="f">Частота</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, double f)
        {
            return ActivationFunctions.Threshold(Sin(t, f), 0.1);
        }

        /// <summary>
        /// Прямоугольный сигнал
        /// </summary>
        /// <param name="t">Вектор отсчетов времени</param>
        /// <param name="f">Вектор частот</param>
        /// <returns>Отсчеты сигнала</returns>
        public static Vector Rect(Vector t, Vector f)
        {
            return ActivationFunctions.Threshold(Sin(t, f), 0.1);
        }
        #endregion

        #region Радиоимпульс

        /// <summary>
        /// Амплитудно-модулированые колебания (прямоугольное модулирующее колебание)
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f1">Carrier frequency</param>
        /// <param name="fi1">Фаза модулирующего сигала</param>
        /// <param name="f2">Частота модулятора</param>
        /// <param name="fi2">Фаза модулируемого сигала</param>
        /// <param name="k">Modulation rate</param>
        /// <returns>Вектор отсчетов</returns>
        public static Vector AmkRect(Vector t, double A, double f1, double fi1, double f2, double fi2, double k)
        {
            Vector modul = Rect(t, A, f2, fi2) + k;
            return Sin(t, modul, f1, fi1);
        }

        /// <summary>
        /// Амплитудно-модулированые колебания (прямоугольное модулирующее колебание)
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f1">Carrier frequency</param>
        /// <param name="f2">Частота модулятора</param>
        /// <param name="fi2">Фаза модулируемого сигала</param>
        /// <param name="k">Modulation rate</param>
        /// <returns>Вектор отсчетов</returns>
        public static Vector AmkRect(Vector t, double A, double f1, double f2, double fi2, double k)
        {
            Vector modul = Rect(t, A, f2, fi2) + k;
            return Sin(t, modul, f1);
        }

        /// <summary>
        /// Амплитудно-модулированые колебания (прямоугольное модулирующее колебание)
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f1">Carrier frequency</param>
        /// <param name="f2">Частота модулятора</param>
        /// <param name="k">Modulation rate</param>
        /// <returns>Вектор отсчетов</returns>
        public static Vector AmkRect(Vector t, double A, double f1, double f2, double k)
        {
            Vector modul = Rect(t, A, f2) + k;
            return Sin(t, modul, f1);
        }

        /// <summary>
        /// Амплитудно-модулированые колебания (прямоугольное модулирующее колебание)
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="f1">Carrier frequency</param>
        /// <param name="f2">Частота модулятора</param>
        /// <param name="k">Modulation rate</param>
        /// <returns>Вектор отсчетов</returns>
        public static Vector AmkRectK(Vector t, double f1, double f2, double k)
        {
            Vector modul = Rect(t, f2) + k;
            return Sin(t, modul, f1);
        }

        /// <summary>
        /// Амплитудно-модулированые колебания (прямоугольное модулирующее колебание)
        /// </summary>
        /// <param name="t">Вектор времени</param>
        /// <param name="A">Амплитуда</param>
        /// <param name="f1">Carrier frequency</param>
        /// <param name="f2">Частота модулятора</param>
        /// <returns>Вектор отсчетов</returns>
        public static Vector AmkRectA(Vector t, double A, double f1, double f2)
        {
            Vector modul = Rect(t, A, f2);
            return Sin(t, modul, f1);
        }

        /// <summary>
        /// Амплитудно-модулированые колебания
        /// </summary>
        /// <param name="t"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Vector AmkRect(Vector t, double f1, double f2)
        {
            Vector modul = Rect(t, f2);
            return Sin(t, modul, f1);
        }
        #endregion

        #region Затухающие колебания
        /// <summary>
        /// Затухающие колебания
        /// </summary>
        /// <param name="t">Время симуляции</param>
        /// <param name="f">частота</param>
        /// <param name="kDamp">Коэффициент затухания</param>
        /// <param name="A">Амплитуда(начальная)</param>
        /// <param name="fi">Фаза</param>
        public static Vector DampedOscillations(Vector t, double f = 1, double kDamp = -0.01, double A = 1, double fi = 0)
        {
            return FunctionsForEachElements.Exp(t * kDamp) * Sin(t, A, f, fi);
        }
        #endregion

        #region Параметры сигналов

        /// <summary>
        /// Энергия выделяемая на едичном резисторе за все время
        /// </summary>
        /// <param name="signal">Сигнал отсчеты</param>
        /// <param name="fd">Частота дискретизация</param>
        /// <returns></returns>
        public static double Energe(Vector signal, double fd)
        {
            double energe = Functions.Summ(signal.Transform(x => Math.Pow(x, 2)));
            return energe / fd;
        }







        /// <summary>
        /// Пачка ЛЧМ
        /// </summary>
        /// <param name="f"></param>
        /// <param name="f0"></param>
        /// <param name="fd"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Vector LFMRectNP(double f, int f0, int fd, double time)
        {
            Vector[] v = new Vector[(int)((f * time) + 0.99)];
            Vector v1 = OneLFM(f, f0, fd);

            for (int i = 0; i < v.Length; i++)
            {
                v[i] = v1;
            }

            return Vector.Concat(v);
        }

        private static Vector OneLFM(double f, int f0, int fd)
        {
            double dt = 1.0 / fd, time = 1.0 / f;
            Vector t = FunctionsForEachElements.GenerateTheSequence(0, dt, time / 2);
            Vector outp = new Vector(t.Count);
            double arg;

            for (int i = 0; i < t.Count; i++)
            {

                arg = 2 * Math.PI * ((f0 * t[i]) + (f0 * f * t[i] * t[i]));
                outp[i] = Math.Cos(arg);
            }

            return outp;
        }


        /// <summary>
        /// ЛЧМ
        /// </summary>
        /// <param name="df"></param>
        /// <param name="f0"></param>
        /// <param name="fd"></param>
        /// <param name="time"></param>
        public static Vector LFM(double df, double f0, double fd, double time)
        {
            double dt = 1.0 / fd;
            Vector t = FunctionsForEachElements.GenerateTheSequence(0, dt, time);
            Vector outp = new Vector(t.Count);
            double arg;

            for (int i = 0; i < t.Count; i++)
            {

                arg = 2 * Math.PI * ((f0 * t[i]) + (df / time * t[i] * t[i]));
                outp[i] = Math.Cos(arg);
            }

            return outp;
        }
        /// <summary>
        /// Передискретизация сигнала
        /// (повышение частоты дискретизации в целое число раз)
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        /// <param name="fd">Старая частота дискретизации</param>
        /// <param name="newfd">Новая частота дикретизации</param>
        /// <returns>Вектор тойже длительности, что и входной,
        /// но с более высокой частотой дискретизации</returns>
        public static Vector Perediscr(Vector inp, int fd, int newfd)
        {
            int k = newfd / fd;
            ComplexVector inputSpectr = FFT.CalcFFT(inp);
            int len = inp.Count * (k - 1), lenFull = inp.Count * k;
            int i = 0;
            ComplexVector cV = new ComplexVector(lenFull);
            int end = inp.Count / 2;

            for (; i < end; i++)
            {
                cV[i] = inputSpectr[i];
            }

            end = lenFull - (inp.Count / 2);

            for (; i < end; i++)
            {
                cV[i] = new System.Numerics.Complex(0, 0);
            }

            for (int j = inp.Count / 2; i < lenFull; i++)
            {
                cV[i] = inputSpectr[j];
            }


            return FFT.CalcIFFT(cV).RealVector;
        }


        /// <summary>
        /// Норма сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="fd"></param>
        /// <returns></returns>
        public static double Norm(Vector signal, double fd)
        {
            double norm = Energe(signal, fd);
            return Math.Sqrt(norm);
        }
        #endregion



    }



}
