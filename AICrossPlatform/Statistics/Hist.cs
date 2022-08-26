using AI.DataStructs.Algebraic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AI.Statistics
{
    /// <summary>
	/// Структура гистограммы
	/// </summary>
	[Serializable]
    public class Histogramm
    {
        private Vector _x;
        private Vector _y;
        private string _name = "Гистограмма";
        private string _description = "нет";
        private string _xlable = "x";
        private string _ylable = "P(x)";

        /// <summary>
        ///  Структура гистограммы
        /// </summary>
        public Histogramm()
        {

        }

        /// <summary>
        ///  Структура гистограммы
        /// </summary>
        /// <param name="n">Число разрядов</param>
        public Histogramm(int n)
        {
            _x = new Vector(n);
            _y = new Vector(n);
        }

        /// <summary>
        /// Значения столбцов
        /// </summary>
        public Vector X
        {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// Высоты столбцов
        /// </summary>
        public Vector Y
        {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// Название гистограммы
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }


        /// <summary>
        /// Описание гистограммы
        /// </summary>
        public string Info
        {
            get => _description;
            set => _description = value;
        }


        /// <summary>
        /// Название оси "Х" гистограммы
        /// </summary>
        public string XLable
        {
            get => _xlable;
            set => _xlable = value;
        }


        /// <summary>
        /// Название оси "У" гистограммы
        /// </summary>
        public string YLables
        {
            get => _ylable;
            set => _ylable = value;
        }


        /// <summary>
        /// Сохранение гистограммы
        /// </summary>
        /// <param name="path">File path</param>
        public void Save(string path)
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
                throw new ArgumentException("Ошибка сохранения", "Сохранение");
            }

        }

        /// <summary>
        /// Сумма значений по строкам
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector IntegralValueH(Matrix matrix)
        {
            Vector vector = new Vector(matrix.Height);

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    vector[i] += matrix[i, j];
                }
            }

            return vector;
        }

        /// <summary>
        /// Средние знач. яркости
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector IntegralValueHMean(Matrix matrix)
        {
            Vector vector = new Vector(matrix.Height);

            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    vector[i] += matrix[i, j];
                }
            }

            return vector / matrix.Width;
        }


        /// <summary>
        /// Сумма значений по строкам
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector IntegralValueW(Matrix matrix)
        {
            Vector vector = new Vector(matrix.Height);

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    vector[i] += matrix[j, i];
                }
            }

            return vector;
        }

        /// <summary>
        /// Средние знач. яркости
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector IntegralValueWMean(Matrix matrix)
        {
            Vector vector = new Vector(matrix.Width);

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    vector[i] += matrix[j, i];
                }
            }

            return vector / matrix.Height;
        }

        /// <summary>
        /// Получение характеристики яркости на базе умножения(Эквивалент "И")
        /// </summary>
        /// <param name="matrix">Входная матрица</param>
        public static Matrix HarAnd(Matrix matrix)
        {
            Vector v1 = IntegralValueHMean(1 - matrix);
            Vector v2 = IntegralValueWMean(1 - matrix);
            v1 = v1.Scale();
            v2 = v2.Scale();
            Matrix matrixOut = Matrix.Mul2Vec(v1, v2);
            return 1 - matrixOut; // max;
        }

        /// <summary>
        /// Получение характеристики яркости на базе сложения
        /// </summary>
        /// <param name="matrix">Входная матрица</param>
        public static Matrix HarSumm(Matrix matrix)
        {
            Vector v1 = IntegralValueHMean(matrix);
            Vector v2 = IntegralValueWMean(matrix);
            v1 = v1.Scale();
            v2 = v2.Scale();
            Matrix matrixOut = Matrix.Sum2Vec(v1, v2);
            return matrixOut / 2; // max;
        }

        /// <summary>
        /// Получение характеристики яркости на базе вычисления модуля
        /// </summary>
        /// <param name="matrix">Входная матрица</param>
        public static Matrix HarNorm(Matrix matrix)
        {
            Vector v1 = IntegralValueHMean(matrix);
            Vector v2 = IntegralValueWMean(matrix);
            v1 = v1.Scale();
            v2 = v2.Scale();
            Matrix matrixOut = Matrix.Norm2Vec(v1, v2);
            return matrixOut / 2; // max;
        }



        /// <summary>
        /// Загрузка гистограммы
        /// </summary>
        /// <param name="path">File path</param>		
        public void Open(string path)
        {

            try
            {

                Histogramm hist = new Histogramm();
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path,
                  FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    hist = (Histogramm)binFormat.Deserialize(fStream);
                }

                _ylable = hist._ylable;
                _xlable = hist._xlable;
                _description = hist._description;
                _name = hist._name;
                _x = hist._x;
                _y = hist._y;

            }

            catch
            {
                throw new ArgumentException("Ошибка загрузки", "Загрузка");
            }

        }


    }
}
