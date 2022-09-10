/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 08.07.2017
 * Время: 15:47
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Numerics;
using Vector = AI.DataStructs.Algebraic.Vector;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Класс для реализации цифровых фильтров
    /// </summary>
	public static class Filters
    {
        /// <summary>
        /// Реализация простого фильтра
        /// </summary>
        /// <param name="st">Вектор сигнала</param>
        /// <param name="kw">АЧХ</param>
        /// <param name="mean">Нужна ли постоянная составляющая</param>
        /// <returns>Фильтрованный сигнал</returns>
		public static Vector Filter(Vector st, Vector kw, bool mean = false)
        {
            Vector newSt = st.CutAndZero(Functions.NextPow2(st.Count));
            double meanValue = newSt.Mean();
            newSt -= meanValue; // Убираем постоянную составляющую, чтобы избежать растекания спектра от нее
            Vector newKw = kw.CutAndZero(newSt.Count / 2);
            newKw = newKw.AddSimmetr();
            ComplexVector Sw = FFT.CalcFFT(newSt);
            Sw *= newKw;
            newSt = FFT.CalcIFFT(Sw).RealVector;

            if (mean)
            {
                return newSt.CutAndZero(st.Count) + meanValue;
            }
            else
            {
                return newSt.CutAndZero(st.Count);
            }
        }
        /// <summary>
        /// Реализация простого фильтра
        /// </summary>
        /// <param name="st">Вектор сигнала</param>
        /// <param name="kw">КЧХ</param>
        /// <param name="mean">Нужна ли постоянная составляющая</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector Filter(Vector st, ComplexVector kw, bool mean = false)
        {
            Vector newSt = st.CutAndZero(Functions.NextPow2(st.Count));
            double meanValue = newSt.Mean();
            newSt -= meanValue; // Убираем постоянную составляющую, чтобы избежать растекания спектра от нее
            ComplexVector newKw = kw.CutAndZero(newSt.Count / 2);
            newKw = newKw.AddSimmetr();
            ComplexVector Sw = FFT.CalcFFT(newSt);
            Sw *= newKw;
            newSt = FFT.CalcIFFT(Sw).RealVector;
            if (mean)
            {
                return newSt.CutAndZero(st.Count) + meanValue;
            }
            else
            {
                return newSt.CutAndZero(st.Count);
            }
        }
        /// <summary>
        /// Реализация колебательного контура
        /// </summary>
        /// <param name="st">Вектор сигнала</param>
        /// <param name="Q">Добротность</param>
        /// <param name="f0">Резонансная частота</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterKontur(Vector st, double Q, double f0, int fd)
        {
            Vector newSt = st.CutAndZero(Functions.NextPow2(st.Count));
            Complex j = new Complex(0, 1);
            ComplexVector Sw = FFT.CalcFFT(st);
            ComplexVector kw = new ComplexVector(Sw.Count);
            Vector f = Signal.Frequency(kw.Count, fd);

            for (int i = 1; i < f.Count / 2; i++)
            {
                kw[i] = 1.0 / (1 + (j * Q * ((f[i] / f0) - (f0 / f[i]))));
            }

            for (int i = f.Count / 2; i < f.Count - 1; i++)
            {
                kw[i] = 1.0 / (1 + (j * Q * ((f[i] / (2 * f0)) - (2 * f0 / f[i]))));
            }

            Sw = Sw * kw;
            newSt = FFT.CalcIFFT(Sw).RealVector;
            return newSt.CutAndZero(st.Count);
        }
        /// <summary>
        /// ФНЧ (Прямоугольная АЧХ)
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterLow(Vector signal, double sr, int fd)
        {
            Vector freq = Signal.Frequency(signal.Count, fd);
            double srNew = Statistic.MaximalValue(freq) - sr;
            Vector kw = ActivationFunctions.Threshold(freq, srNew).Revers();
            return Filter(signal, kw, true);
        }
        /// <summary>
        /// ФНЧ (АЧХ повторяет АЧХ Баттерворта), задан через КЧХ
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="order">Порядок фильтра</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterLowButterworthCFH(Vector signal, double sr, int fd, int order = 3)
        {
            ComplexVector kw = ButterworthLowCFH(signal.Count, sr, fd, order);
            return Filter(signal, kw, true);
        }
        /// <summary>
        /// ФНЧ (АЧХ повторяет АЧХ Баттерворта), задан через АЧХ
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="order">Порядок фильтра</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterLowButterworthAFH(Vector signal, double sr, int fd, int order = 3)
        {
            Vector kw = ButterworthLowAFH(signal.Count, sr, fd, order);
            return Filter(signal, kw, true);
        }
        /// <summary>
        /// Полосовой фильтр (Прямоугольная АЧХ)
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr1">Нижняя частота полосы</param>
        /// <param name="sr2">Верхняя частота полосы</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterBand(Vector signal, double sr1, double sr2, int fd)
        {
            Vector freq = Signal.Frequency(signal.Count, fd);
            Vector kw = freq.Transform(x => (x >= sr1 && x <= sr2) ? 1 : 0);
            return Filter(signal, kw, sr1 == 0);
        }
        /// <summary>
        /// ФВЧ (Прямоугольная АЧХ)
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterHigh(Vector signal, double sr, int fd)
        {
            Vector freq = Signal.Frequency(signal.Count, fd);
            Vector kw = ActivationFunctions.Threshold(freq, sr);
            return Filter(signal, kw);
        }
        /// <summary>
        /// Режекторный фильтр
        /// </summary>
        /// <param name="signal">Отсчеты сигнала</param>
        /// <param name="sr1">Нижняя частота полосы</param>
        /// <param name="sr2">Верхняя частота полосы</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Фильтрованный сигнал</returns>
        public static Vector FilterRezector(Vector signal, double sr1, double sr2, int fd)
        {
            Vector freq = Signal.Frequency(signal.Count, fd);
            Vector kw = new Vector(signal.Count);

            kw += 1;

            for (int i = 0; i < signal.Count; i++)
            {
                if ((freq[i] >= sr1) && (freq[i] <= sr2))
                {
                    kw[i] = 0;
                }
            }


            return Filter(signal, kw, sr1 != 0);
        }
        /// <summary>
        /// Создание АЧХ нужного типа
        /// </summary>
        /// <param name="f">Вектор частот</param>
        /// <param name="param">параметры</param>
        /// <param name="afh">Тип АЧХ</param>
        public static Vector GetAFH(Vector f, double[] param, AFHType afh)
        {
            Vector kw = new Vector(f.Count / 2);

            if (afh == AFHType.Band)
            {

                for (int i = 0; i < kw.Count; i++)
                {
                    if ((f[i] >= param[0]) && (f[i] <= param[1]))
                    {
                        kw[i] = 1;
                    }
                }

            }

            if (afh == AFHType.High)
            {
                kw = ActivationFunctions.Threshold(f, param[0]).CutAndZero(kw.Count);
            }

            if (afh == AFHType.Low)
            {
                double srNew = f[f.Count - 1] - param[0];
                kw = ActivationFunctions.Threshold(f, srNew).Revers().CutAndZero(kw.Count); ;
            }


            if (afh == AFHType.Rezector)
            {
                kw += 1;

                for (int i = 0; i < kw.Count; i++)
                {
                    if ((f[i] >= param[0]) && (f[i] <= param[1]))
                    {
                        kw[i] = 0;
                    }
                }
            }

            return kw.AddSimmetr();
        }
        /// <summary>
        /// Создание составной АЧХ
        /// </summary>
        /// <param name="f">Вектор частот</param>
        /// <param name="param">Параметры</param>
        /// <returns>Возвращает АЧХ</returns>
        public static Vector CreatAFH(Vector f, string[] param)
        {
            Vector kw = new Vector(f.Count) + 1;
            double[] fP;

            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].Split(':')[0] == "r")
                {
                    fP = new double[2];
                    fP[0] = Convert.ToDouble(param[i].Split(':')[1]);
                    fP[1] = Convert.ToDouble(param[i].Split(':')[2]);
                    kw *= GetAFH(f, fP, AFHType.Rezector).CutAndZero(kw.Count);
                }

                if (param[i].Split(':')[0] == "l")
                {
                    fP = new double[1];
                    fP[0] = Convert.ToDouble(param[i].Split(':')[1]);
                    kw *= GetAFH(f, fP, AFHType.Low).CutAndZero(kw.Count);
                }

                if (param[i].Split(':')[0] == "h")
                {
                    fP = new double[1];
                    fP[0] = Convert.ToDouble(param[i].Split(':')[1]);
                    kw *= GetAFH(f, fP, AFHType.High).CutAndZero(kw.Count);
                }

                if (param[i].Split(':')[0] == "b")
                {
                    fP = new double[2];
                    fP[0] = Convert.ToDouble(param[i].Split(':')[1]);
                    fP[1] = Convert.ToDouble(param[i].Split(':')[2]);
                    kw *= GetAFH(f, fP, AFHType.Band).CutAndZero(kw.Count);
                }
            }

            return kw;

        }
        /// <summary>
        /// Эспоненциональное скользящее среднее
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <param name="oldPart">Коэффициент сглаживания</param>
        /// <returns></returns>
        public static Vector ExpAv(Vector inp, double oldPart = 0.99)
        {
            Vector outp = new Vector(inp.Count)
            {
                [0] = inp[0]
            };
            double newPart = 1 - oldPart;

            for (int i = 1; i < inp.Count; i++)
            {
                outp[i] = (oldPart * outp[i - 1]) + (newPart * inp[i]);
            }

            return outp;
        }
        /// <summary>
        /// Cкользящее среднее
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <param name="l">Размер окна</param>
		public static Vector MAv(Vector inp, int l = 10)
        {
            return Vector.GetWindowsWithFuncVect(Statistic.ExpectedValue, inp, l, 1);
        }
        /// <summary>
        /// Получение огибающей
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="dec">Прореживание</param>
        public static Vector GetEnvelope(Vector inp, int dec = 1)
        {
            Vector inp2 = FunctionsForEachElements.Abs(inp);
            inp2 = Filters.ExpAv(inp2, 0.9999);

            Vector outp = new Vector(inp.Count / dec);

            for (int i = 0, k = 0, max = outp.Count - dec + 1; i < max; i += dec)
            {
                outp[k++] = inp2[i];
            }


            return outp;
        }
        /// <summary>
        /// Ачх фильтра Баттерворта
        /// </summary>
        /// <param name="Count">Число отсчетов сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="order">Порядок фильтра</param>
        public static Vector ButterworthLowAFH(int Count, double sr, int fd, int order)
        {
            Vector freq = Signal.Frequency(Count, fd);
            Vector w_n = freq / sr;
            return w_n.Transform(x => 1.0 / Math.Sqrt(1.0 + Math.Pow(x, 2 * order)));
        }
        /// <summary>
        /// КЧХ фильтра Баттерворта
        /// </summary>
        /// <param name="Count">Число отсчетов сигнала</param>
        /// <param name="sr">Частота среза</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="order">Порядок фильтра</param>
        public static ComplexVector ButterworthLowCFH(int Count, double sr, int fd, int order)
        {
            Vector freq = Signal.Frequency(Count, fd);
            Complex j = new Complex(0, 1);
            ComplexVector p_n = new ComplexVector(freq / sr);
            p_n = (p_n / 2) + (j * p_n / 2);
            int sign = FunctionsForEachElements.MinusOnePow(order);

            return ComplexVector.TransformVectorX(p_n, x =>
            1.0 / (1.0 + (sign * Complex.Pow(x, 2 * order)))
            );
        }
    }

    /// <summary>
    /// Типы АЧХ
    /// </summary>
    public enum AFHType
    {
        /// <summary>
        /// ФНЧ
        /// </summary>
		Low,
        /// <summary>
        /// ФВЧ
        /// </summary>
		High,
        /// <summary>
        /// Режектор
        /// </summary>
		Rezector,
        /// <summary>
        /// Полосовой
        /// </summary>
		Band
    }
}
