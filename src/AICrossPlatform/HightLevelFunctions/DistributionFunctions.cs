/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 07.02.2016
 * Time: 18:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;

namespace AI.HightLevelFunctions
{
    /// <summary>
    /// Функции распределения случайной величины
    /// </summary>
    public static class DistributionFunctions
    {
        /// <summary>
        /// Функция распределения по нормальному закону 
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        /// <param name="m">Мат. ожидание</param>
        /// <param name="std">Среднеквадратичное отклонение</param>
        public static Vector Gauss(Vector inp, double m, double std)
        {
            return 1.0 / (std * Math.Sqrt(2 * Math.PI)) * FunctionsForEachElements.Exp((inp - m).Transform(x => Math.Pow(x, 2)) / (-2 * std * std));
        }
        /// <summary>
        /// Ф-я Гаусса
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="m"></param>
        /// <param name="std"></param>
        /// <returns></returns>
        public static Matrix Gauss(Matrix inp, double m, double std)
        {
            Matrix outp = new Matrix(inp.Height, inp.Width);

            for (int i = 0; i < inp.Height; i++)
            {
                for (int j = 0; j < inp.Width; j++)
                {
                    outp[i, j] = Gauss(inp[i, j], m, std);
                }
            }

            return outp;
        }
        /// <summary>
        /// Фильтр гаусса
        /// </summary>
        /// <param name="h">Высота фильтра</param>
        /// <param name="w">Ширина</param>
        /// <param name="std">Среднеквадратичное отклонение</param>
        /// <returns>Фильтр Гаусса</returns>
        public static Matrix GaussRect(int h, int w, double std = 160)
        {
            int h05 = h / 2, w05 = w / 2;
            Matrix matrix = new Matrix(h, w);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    matrix[i, j] = Math.Sqrt(((i - h05) * (i - h05)) + ((j - w05) * (j - w05)));
                }
            }

            matrix = Gauss1(matrix, 0, std);

            return matrix;
        }
        /// <summary>
        /// Функция вероятность принадлежности
        /// </summary>
        /// <param name="Inp">Входное значение</param>
        /// <param name="m">Мат. ожидание</param>
        /// <param name="sko">Среднеквадратичное отклонение</param>
        public static double Gauss(double Inp, double m, double sko)
        {
            return 1.0 / (sko * Math.Sqrt(2 * Math.PI)) * Math.Exp((Inp - m) * (Inp - m) / (-2 * sko * sko));
        }
        /// <summary>
        /// Функция Гаусса при x=m -> G(x) = 1
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="m"></param>
        /// <param name="sko"></param>
        /// <returns></returns>
        public static Matrix Gauss1(Matrix inp, double m, double sko)
        {
            Matrix matr = new Matrix(inp.Height, inp.Width);

            for (int i = 0; i < inp.Height; i++)
            {
                for (int j = 0; j < inp.Width; j++)
                {
                    matr[i, j] = GaussNorm1(inp[i, j], m, sko);
                }
            }

            return matr;
        }
        /// <summary>
        /// Функция вероятность принадлежности при inp = m, out = 1
        /// </summary>
        /// <param name="Inp">Входное значение</param>
        /// <param name="m">Мат. ожидание</param>
        /// <param name="std">Среднеквадратичное отклонение</param>
        public static double GaussNorm1(double Inp, double m, double std)
        {
            return Math.Exp((Inp - m) * (Inp - m) / (-2 * std * std));
        }
        /// <summary>
        /// Функция вероятность принадлежности при inp = m, out = 1
        /// </summary>
        /// <param name="Inp">Входной вектор</param>
        /// <param name="m">Мат. ожидание</param>
        /// <param name="std">Среднеквадратичное отклонение</param>
        public static Vector GaussNorm1(Vector Inp, double m, double std)
        {
            Vector vect = new Vector(Inp.Count);

            for (int i = 0; i < vect.Count; i++)
            {
                vect[i] = GaussNorm1(Inp[i], m, std);
            }

            return vect;
        }
        /// <summary>
        /// Функция распределения Пуасона 
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        /// <param name="m">Мат. ожидание от 0 до +inf</param>
        public static Vector Puasson(Vector inp, double m)
        {
            Vector pow = FunctionsForEachElements.Pow(m, inp);
            return pow / FunctionsForEachElements.Factorial(inp) * Math.Exp(-m);
        }
    }
}