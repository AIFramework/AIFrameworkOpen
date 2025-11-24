/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 04.03.2017
 * Время: 23:30
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Threading;


namespace AI.ML
{
    /// <summary>
    /// Класс для хранения обучающей выборки для градиентного спуска
    /// </summary>
    [Serializable]
    public class GradientDecentDataset
    {
        private List<Vector> _X = new List<Vector>();
        private List<Vector> _Y = new List<Vector>();


        /// <summary>
        /// Набор векторов "Х"
        /// </summary>
        public List<Vector> X
        {
            get => _X;
            set => _X = value;
        }

        /// <summary>
        /// Набор векторов "Y"
        /// </summary>
        public List<Vector> Y
        {
            get => _Y;
            set => _Y = value;
        }


        /// <summary>
        /// Создает экземпляр GradientDecentDataset
        /// </summary>
        public GradientDecentDataset()
        {

        }



        /// <summary>
        /// Создает экземпляр GradientDecentDataset
        /// </summary>
        /// <param name="xVector">Набор векторов "Х"</param>
        /// <param name="yVector">Набор векторов "Y"</param>
        public GradientDecentDataset(List<Vector> xVector, List<Vector> yVector)
        {
            _X = xVector;
            _Y = yVector;
        }


        /// <summary>
        /// Добавляет данные в обучающую выборку
        /// </summary>
        /// <param name="x">Вектор х</param>
        /// <param name="y">Вектор у</param>
        public void Add(Vector x, Vector y)
        {
            _X.Add(x);
            _Y.Add(y);
        }

        /// <summary>
        /// Добавляет данные в обучающую выборку
        /// </summary>
        /// <param name="x">переменная х</param>
        /// <param name="y">переменная у</param>
        public void Add(double x, double y)
        {
            _X.Add(new Vector(x));
            _Y.Add(new Vector(y));
        }

        /// <summary>
        /// Добавляет данные в обучающую выборку
        /// </summary>
        /// <param name="x">Вектор х</param>
        /// <param name="y">Переменная у</param>
        public void Add(Vector x, double y)
        {
            _X.Add(x);
            _Y.Add(new Vector(y));
        }


        /// <summary>
        /// Очистка данных
        /// </summary>
        public void Clear()
        {
            _X.Clear();
            _Y.Clear();
        }

    }



    /// <summary>
    /// Градиентный спуск
    /// </summary>
    public class GradientDecent
    {
        private Thread th;

        /// <summary>
        /// Шаг для вычисления частных производных и градиента
        /// (чем он меньше, тем точнее вычисление частных производных)
        /// по умолчанию step 1e-7
        /// </summary>
        public double Step { get; set; } = 1e-5;
        /// <summary>
        /// Обучающая выборка
        /// </summary>
        public GradientDecentDataset GdDataset { get; set; }
        /// <summary>
        /// Вектор оптимизируемых парамметров
        /// </summary>
        public Vector Parammetrs { get; set; }
        /// <summary>
        /// Целевая функция типа:
        ///	double SF(Vector_парамметры, ListVector_обучающая_выборка_Х, ListVector_обучающая_выборка_Y)
        /// </summary>
        public Func<Vector, List<Vector>, List<Vector>, double> Function { get; set; }
        /// <summary>
        /// Норма обучения, по умолчанию 0.002
        /// </summary>
        public double Norm { get; set; } = 0.002;
        /// <summary>
        /// Кол-во иттераций, по умолчанию 30
        /// </summary>
        public int Itterations { get; set; }


        /// <summary>
        /// Создание объекта Градиентный спуск
        /// </summary>
        /// <param name="param">Вектор оптимизируемых парамметров</param>
        /// <param name="function">Целевая функция типа:
        ///	double SF(Vector_парамметры, ListVector_обучающая_выборка_Х, ListVector_обучающая_выборка_Y)</param>
        /// <param name="gdd">Обучающая выборка</param>
        public GradientDecent(Vector param, Func<Vector, List<Vector>, List<Vector>, double> function, GradientDecentDataset gdd)
        {
            Itterations = 30;
            GdDataset = gdd;
            Function = function;
            Parammetrs = param;
        }





        /// <summary>
        /// Одна иттерация спуска
        /// </summary>
        private void DecentIter()
        {
            Vector pdip = PartialDerivatives(Parammetrs, GdDataset.X, GdDataset.Y, Function);
            Parammetrs -= pdip * Norm;
        }


        /// <summary>
        /// Синхронное выполнение градиентного спуска
        /// </summary>
        public void Decent()
        {
            for (int i = 0; i < Itterations; i++)
            {
                DecentIter();
            }
        }


        /// <summary>
        /// Асинхронное выполнение градиентного спуска
        /// </summary>
        public void AsyncDecent()
        {
            th = new Thread(Decent);
            th.Start();
        }



        /// <summary>
        /// Частные производные
        /// </summary>
        /// <param name="param">Начальный вектор параметров(точка)</param>
        /// <param name="function">Целевая функция(принимает вектор параметров, выдает результирующее значение)</param>
        ///  <param name="inp"> Вектора входа</param>
        ///   <param name="ideal"> Целевые выходы</param>
        /// <returns>Возвращает вектор частных производных</returns>
        public Vector PartialDerivatives(Vector param, List<Vector> inp, List<Vector> ideal, Func<Vector, List<Vector>, List<Vector>, double> function)
        {
            Vector partialDerivatives = new Vector(param.Count);
            double out1 = function(param, inp, ideal);
            Vector newParam = param.Clone();

            for (int i = 0; i < param.Count; i++)
            {
                newParam[i] += Step;
                double out2 = function(newParam, inp, ideal);
                partialDerivatives[i] = (out2 - out1) / Step;
                newParam[i] -= Step;
            }

            return partialDerivatives;
        }


    }
}
