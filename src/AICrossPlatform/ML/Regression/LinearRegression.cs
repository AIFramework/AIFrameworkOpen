/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 04.03.2017
 * Время: 18:00
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;

namespace AI.ML.Regression
{

    /// <summary>
    /// Модель для линейной регрессии хранит k и b   
    /// f(x) = k*x+b;
    /// </summary>
    [Serializable]
    public class LinearRegressionModel
    {
        /// <summary>
        /// Тангенс угла наклона
        /// </summary>
        public double k { get; set; }
        /// <summary>
        /// Смещение относительно (0;0)
        /// </summary>
        public double b { get; set; }
    }








    /// <summary>
    /// Линейная регрессия
    /// </summary>
    public class LinearRegression
    {
        /// <summary>
        /// Парамметры линейной регрессии
        /// </summary>
        public LinearRegressionModel Lrm { get; set; }



        /// <summary>
        /// Обучающая выборка
        /// </summary>
        /// <param name="X">Вектор X(независимая переменная)</param>
        /// <param name="Y">Вектор Y(зависимая переменная)</param>
        public LinearRegression(Vector X, Vector Y)
        {
            Lrm = new LinearRegressionModel();
            double d = Statistic.СalcVariance(X);
            Lrm.k = Statistic.Cov(X, Y) / (d == 0 ? 1e-9 : d);
            Lrm.b = Statistic.ExpectedValue(Y) - (Lrm.k * Statistic.ExpectedValue(X));
        }


        /// <summary>
        /// Вывод в строку
        /// </summary>
        /// <returns>Строка типа: f(x) = k*x+(b)</returns>
        public override string ToString()
        {
            return string.Format("f(x) ={0}*x+({1})", Lrm.k, Lrm.b);
        }

        /// <summary>
        /// Прогнозирование с помощью линейной модели
        /// </summary>
        /// <param name="x">Независимая переменная</param>
        /// <returns>Зависимая переменная</returns>
        public double Predict(double x)
        {
            return (Lrm.k * x) + Lrm.b;
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
