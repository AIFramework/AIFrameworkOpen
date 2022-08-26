using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;

namespace AI.DSP.Analyse
{
    /// <summary>
    /// Тренд сигнала
    /// </summary>
    [Serializable]
    public class Trend
    {
        /// <summary>
        /// Коэффициент наклона
        /// </summary>
        public double K { get; set; }
        /// <summary>
        /// Смещение
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Обучающая выборка
        /// </summary>
        /// <param name="X">Вектор X(независимая переменная)</param>
        /// <param name="Y">Вектор Y(зависимая переменная)</param>
        public Trend(Vector X, Vector Y)
        {
            double d = Statistic.СalcVariance(X);
            K = Statistic.Cov(X, Y) / (d == 0 ? 1e-9 : d);
            B = Statistic.ExpectedValue(Y) - (K * Statistic.ExpectedValue(X));
        }

        /// <summary>
        /// Вывод в строку
        /// </summary>
        /// <returns>Строка типа: f(x) = k*x+(b)</returns>
        public override string ToString()
        {
            return string.Format("f(x) ={0:N3}*x+({1:N3})", K, B);
        }

        /// <summary>
        /// Прогнозирование с помощью линейной модели
        /// </summary>
        /// <param name="x">Независимая переменная</param>
        /// <returns>Зависимая переменная</returns>
        public double Predict(double x)
        {
            return (K * x) + B;
        }

        /// <summary>
        /// Прогнозирование с помощью линейной модели
        /// </summary>
        /// <param name="X">Вектор независимых переменных</param>
        /// <returns>Вектор зависимых переменных</returns>
        public Vector Predict(Vector X)
        {
            Vector outp = new Vector(X.Count);

            for (int i = 0; i < X.Count; i++)
            {
                outp[i] = Predict(X[i]);
            }

            return outp;
        }
    }
}
