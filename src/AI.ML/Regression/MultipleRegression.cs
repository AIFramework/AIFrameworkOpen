/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 10.08.2018
 * Время: 13:35
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.MathUtils.Algebra;
using AI.Statistics;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace AI.ML.Regression
{
    /// <summary>
    /// Множественная линейная регрессия
    /// </summary>
    [Serializable]
    public class MultipleRegression : IRegression
    {
        private Vector _param, std, mean;

        [NonSerialized]
        private Vector[] _x;
        [NonSerialized]
        private double[] _y;
        [NonSerialized]
        private Matrix A;
        [NonSerialized]
        private Vector B;
        [NonSerialized]
        private int n;
        [NonSerialized]
        private int m;
        [NonSerialized]
        private readonly bool IsScale;

        /// <summary>
        /// Параметры модели
        /// </summary>
        public Vector Parammetrs
        {
            get;
            private set;
        }



        /// <summary>
        /// Множественная линейная регрессия
        /// </summary>
        /// <param name="isScale">Стоит ли применить масштабирование к данным</param>
        public MultipleRegression(bool isScale = false)
        {
            IsScale = isScale;
        }


        /// <summary>
        /// Множественная линейная регрессия
        /// </summary>
        /// <param name="path">Путь до модели</param>
        public MultipleRegression(string path)
        {
            LoadModel(path);
        }

        // Составление матрицы
        private void GenA()
        {
            A = new Matrix(m, m);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {

                    double c = 0;
                    for (int k = 0; k < n; k++)
                    {
                        c += _x[k][i] * _x[k][j];
                    }

                    A[i, j] = c;
                }
            }
        }



        // Вектор ответа
        private void GenB()
        {
            B = new Vector(m);
            for (int i = 0; i < m; i++)
            {
                double c = 0;

                for (int j = 0; j < n; j++)
                {
                    c += _x[j][i] * _y[j];
                }

                B[i] = c;
            }

        }

        // Генерация параметров системы
        private void GenParam()
        {
            _param = Gauss.SolvingEquations(A, B);
        }

        /// <summary>
        /// Прогноз
        /// </summary>
        /// <param name="vect">Вектор входных данных</param>
        /// <returns>Выход</returns>
        public double Predict(Vector vect)
        {
            if (_param == null) return 0;

            Vector inp = vect.AddOne();
            inp -= mean;
            inp /= std;
            return AnalyticGeometryFunctions.Dot(inp, _param);
        }


        /// <summary>
        /// Прогноз
        /// </summary>
        /// <param name="inp">Вектора входа</param>
        /// <returns>Вектор выхода</returns>
        public Vector Predict(Vector[] inp)
        {
            Vector outp = new Vector(inp.Length);

            for (int i = 0; i < inp.Length; i++)
                outp[i] = Predict(inp[i]);

            return outp;
        }


        /// <summary>
        /// Сохранение модели
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void SaveModel(string path)
        {
            try
            {
                BinaryFormatter binFormat = new BinaryFormatter();

                using Stream fStream = new FileStream(path,
                                           FileMode.Create, FileAccess.Write, FileShare.None);
                binFormat.Serialize(fStream, this);
            }
            catch
            {
                throw new ArgumentException("Ошибка сохранения");
            }
        }

        /// <summary>
        /// Загрузка модели
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void LoadModel(string path)
        {
            try
            {
                MultipleRegression mR;
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    mR = (MultipleRegression)binFormat.Deserialize(fStream);
                }

                std = mR.std;
                mean = mR.mean;
                _param = mR._param;
            }

            catch
            {
                throw new ArgumentException("Ошибка загрузки");
            }
        }

        /// <summary>
        /// Обучение множественной регрессии
        /// </summary>
        /// <param name="data">Признаки</param>
        /// <param name="targets">Целевые выходы</param>
        public void Train(Vector[] data, Vector targets)
        {
            DataPrepaire(data, targets);
            GenA();
            GenB();
            GenParam();
        }

        /// <summary>
        /// Обучение градиентным спуском (Эластик)
        /// </summary>
        public void TrainGrad(Vector[] data, Vector targets, double epoch, double lr, double l1, double l2)
        {
            DataPrepaire(data, targets);

            if (_param == null)
                _param = new Vector(m);

            if (_param.Count != m)
                _param = new Vector(m);

            Vector dif = new Vector(m);

            // Обучение град. спуском
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double pred = AnalyticGeometryFunctions.Dot(_x[j], _param);
                    double delta = pred - _y[j];

                    for (int k = 0; k < m; k++)
                        dif[k] += data[j][k] * delta + l1 + l2 * _param[k]; // Вычисление производной
                }

                _param -= lr * dif;
            }
        }

        /// <summary>
        /// Пред. обработка
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targets"></param>
        private void DataPrepaire(Vector[] data, Vector targets)
        {
            n = data.Length;
            m = data[0].Count + 1;

            if (IsScale)
            {
                _x = Vector.ScaleData(data);
                std = FunctionsForEachElements.Sqrt(Statistic.EnsembleDispersion(data));
                mean = Statistic.MeanVector(data);
                std = std.AddOne();
                mean = mean.Shift(1);
            }
            else
            {
                std = new Vector(m) + 1;
                mean = new Vector(m);
                _x = new Vector[n];

                for (int i = 0; i < n; i++)
                    _x[i] = data[i].Clone();
            }

            // append one to feaures
            for (int i = 0; i < n; i++)
                _x[i] = _x[i].AddOne();

            _y = targets;
        }
    }
}
