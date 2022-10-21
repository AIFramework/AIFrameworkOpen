using AI.DataStructs.Shapes;
using AI.Extensions;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AI.DataStructs.Algebraic
{
    /// <summary>
    /// Класс представляющий матрицы и операции с ними
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}")]
    public class Matrix : IAlgebraicStructure<double> , IEquatable<Matrix>, ISavable, ITextSavable, IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Данные(компоненты) матрицы
        /// </summary>
        public double[] Data { get; set; }
        /// <summary>
        /// Тип матрицы
        /// </summary>
        public MatrixType DataType { get; set; }
        /// <summary>
        /// Высота
        /// </summary>
        public int Height => Shape[1];
        /// <summary>
        /// Ширина
        /// </summary>
        public int Width => Shape[0];
        /// <summary>
        /// Форма матрицы
        /// </summary>
        public Shape Shape { get; } = new Shape2D(3, 3);
        /// <summary>
        /// Выдает элемент по индексу
        /// </summary>
        /// <param name="i">Индекс высоты</param>
        /// <param name="j">Индекс ширины</param>
        public double this[int i, int j]
        {
            get => Get(i, j);
            set => Set(i, j, value);
        }
        /// <summary>
        /// Выдает элемент по индексу
        /// </summary>
        /// <param name="i">Индекс</param>
        public double this[int i]
        {
            get => Data[i];
            set => Data[i] = value;
        }
        /// <summary>
        /// Определитель матрицы
        /// </summary>
        /// <returns></returns>
        public double Determinant
        {
            get
            {
                if (!IsSquared)
                {
                    throw new InvalidOperationException("Матрица не является квадратной");
                }

                double result = 1.0;

                if (IsZero)
                {
                    return 0;
                }

                if (IsTriangle || IsDiagonal)
                {
                    for (int i = 0; i < Height; i++)
                    {
                        result *= this[i, i];
                    }

                    return result;
                }

                Matrix matrix = ToTriangularMatr();

                for (int i = 0; i < Height; i++)
                {
                    result *= matrix[i, i];
                }

                return result;
            }
        }
        /// <summary>
        /// Все ли элементы матрицы равны нулю
        /// </summary>
        public bool IsZero => Data.All(el => el == 0);
        /// <summary>
        /// Квадратная ли матрица
        /// </summary>
        public bool IsSquared => Height == Width;
        /// <summary>
        /// Является ли матрица диагональной
        /// </summary>
        public bool IsDiagonal
        {
            get
            {
                if (!IsSquared)
                {
                    return false;
                }

                double eU = 0, eD = 0, all = 0;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        all += Math.Abs(this[i, j]);

                        if (i == j)
                        {
                            eD += Math.Abs(this[i, j]);
                        }
                    }
                }

                for (int i = 0; i < Height; i++)
                {
                    for (int j = i; j < Width; j++)
                    {
                        eU += Math.Abs(this[i, j]);
                    }
                }

                double allND = all - eD;

                return eD > (allND * 1000);
            }
        }
        /// <summary>
        /// Является ли матрица треугольной
        /// </summary>
        public bool IsTriangle
        {
            get
            {
                if (!IsSquared)
                {
                    return false;
                }

                double eU = 0, eD = 0, all = 0;

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        all += Math.Abs(this[i, j]);

                        if (i == j)
                        {
                            eD += Math.Abs(this[i, j]);
                        }
                    }
                }

                for (int i = 0; i < Height; i++)
                {
                    for (int j = i; j < Width; j++)
                    {
                        eU += Math.Abs(this[i, j]);
                    }
                }

                double allND = all - eD, deL = all - eU, deU = eU - eD;

                return Math.Abs(deU - deL) > 0.999 * allND;
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Создает матрицу со всеми нулями размерности 3х3
        /// </summary>
        public Matrix()
        {
            DataType = MatrixType.MatStruct;
            Data = new double[Shape.Count];
        }
        /// <summary>
        /// Создает матрицу на основе двумерного массива
        /// </summary>
        public Matrix(double[,] matr)
        {
            DataType = MatrixType.MatStruct;
            Shape = new Shape2D(matr.GetLength(0), matr.GetLength(1));
            Data = new double[Shape.Count];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Data[GetIndex(i, j)] = matr[i, j];
                }
            }
        }
        /// <summary>
        /// Создает матрицу заданной формы инициализированную нулями 
        /// </summary>
        /// <param name="shape"></param>
        public Matrix(Shape shape)
        {
            if (shape.Rank > 2)
            {
                throw new ArgumentException("Rank of the given shape if greater than 2", nameof(shape));
            }

            DataType = MatrixType.MatStruct;

            switch (shape.Rank)
            {
                case 1:
                    Shape = new Shape2D(1, shape[0]);
                    break;
                case 2:
                    Shape = new Shape2D(shape[1], shape[0]);
                    break;
            }

            Data = new double[Shape.Count];
        }
        /// <summary>
        /// Создает матрицу со всеми нулями размерности MxN
        /// </summary>
        public Matrix(int height, int width) : this(new Shape2D(height, width)) { }
        #endregion

        #region Операторы 
        /// <summary>
        /// Поэлементная сумма
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix A, Matrix B)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            if (A.Shape != B.Shape)
            {
                throw new InvalidOperationException("Matrices dimensions don't match");
            }

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] + B.Data[i];
            }

            return C;
        }
        /// <summary>
        /// Поэлементная разность
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix A, Matrix B)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            if (A.Shape != B.Shape)
            {
                throw new InvalidOperationException("Matrices dimensions don't match");
            }

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] - B.Data[i];
            }

            return C;
        }
        /// <summary>
        /// Addition 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix A, double k)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] + k;
            }

            return C;
        }
        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="k"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Matrix operator +(double k, Matrix A)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] + k;
            }

            return C;
        }
        /// <summary>
        /// вычитание
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix A, double k)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] - k;
            }

            return C;
        }
        /// <summary>
        /// Вычитание
        /// </summary>
        /// <param name="k"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Matrix operator -(double k, Matrix A)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = k - A.Data[i];
            }

            return C;
        }

        /// <summary>
        /// Умножение
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix A, double k)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] * k;
            }

            return C;
        }
        /// <summary>
        /// Деление
        /// </summary>
        public static Matrix operator /(Matrix A, double k)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = A.Data[i] / k;
            }

            return C;
        }

        /// <summary>
        /// Деление
        /// </summary>
        public static Matrix operator /(double k, Matrix A)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = k / A.Data[i];
            }

            return C;
        }
        /// <summary>
        /// Умножение
        /// </summary>
        public static Matrix operator *(double k, Matrix A)
        {
            Matrix C = new Matrix(A.Height, A.Width);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = k * A.Data[i];
            }

            return C;
        }
        /// <summary>
        /// Умножение вектора на матрицу
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix A, Vector B)
        {
            return A * B.ToMatrix();
        }
        /// <summary>
        /// Умножение вектора на матрицу
        /// </summary>
        /// <param name="B"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Vector operator *(Vector B, Matrix A)
        {
            return (B.ToMatrix() * A).LikeVector();
        }

        /// <summary>
        /// Матричное умножение
        /// </summary>
        public static Matrix operator *(Matrix A, Matrix B)
        {
            Matrix C = new Matrix(A.Height, B.Width);

            int n = A.Width;

            if (!(A.Width == B.Height))
            {
                throw new InvalidOperationException("Can't multiply given matrices");
            }

            for (int i = 0; i < A.Height; i++)
            {
                for (int j = 0; j < B.Width; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        C[i, j] += A[i, k] * B[k, j];
                    }
                }
            }

            return C;
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix left, Matrix right)
        {
            bool lNull = Equals(left, null);
            bool rNull = Equals(right, null);

            if (lNull && rNull)
            {
                return true;
            }
            else if ((lNull && !rNull) || (!lNull && rNull))
            {
                return false;
            }

            return left!.Shape == right!.Shape && left.Data.ElementWiseEqual(right.Data);
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator !=(Matrix left, Matrix right)
        {
            bool lNull = Equals(left, null);
            bool rNull = Equals(right, null);

            if (lNull && rNull)
            {
                return false;
            }
            else if ((lNull && !rNull) || (!lNull && rNull))
            {
                return true;
            }

            return left!.Shape != right!.Shape || !left.Data.ElementWiseEqual(right.Data);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Замена неопределенности(nan) на среднее значение
        /// </summary>
        public Matrix NanToMean()
        {
            Matrix outp = new Matrix(Height, Width);
            double m = Mean();

            for (int i = 0; i < Data.Length; i++)
                    outp.Data[i] = double.IsNaN(Data[i]) ? m : Data[i];
              
            return outp;
        }
        /// <summary>
        ///  Замена неопределенности(nan) на заданное значение
        /// </summary>
        /// <param name="value">Число</param>
        public Matrix NanToValue(double value = 0)
        {
            Matrix outp = new Matrix(Height, Width);

            for (int i = 0; i < Data.Length; i++)
                outp.Data[i] = double.IsNaN(Data[i]) ? value : Data[i];

            return outp;
        }
        /// <summary>
        /// Получение минора
        /// </summary>
        /// <param name="h">Без какой строки</param>
        /// <param name="w">Без какого столбца</param>
        public double GetMinor(int h, int w)
        {
            Matrix result = new Matrix(Height - 1, Height - 1);

            for (int i = 0, i1 = 0; i < Height; i++)
            {
                if (i != h)
                {
                    for (int j = 0, j1 = 0; j < Height; j++)
                    {
                        if (j != w)
                        {
                            result[i1, j1] = this[i, j];
                            j1++;
                        }
                    }
                    i1++;
                }
            }

            double minor = result.Determinant;

            return minor;
        }
        /// <summary>
        /// Вычисление обратной матрицы
        /// </summary>
        public Matrix GetInvertMatrix()
        {
            if (!IsSquared)
            {
                throw new InvalidOperationException("Matrix is not squared");
            }

            if (IsZero)
            {
                throw new InvalidOperationException("Matrix is zero");
            }

            if (IsDiagonal)
            {
                Matrix output = new Matrix(Height, Height);

                for (int i = 0; i < Height; i++)
                {
                    output[i, i] = 1.0 / this[i, i];
                }

                return output;
            }
            else
            {

                Matrix output = new Matrix(Height, Height);
                double det = Determinant;

                if (!((det > 1e-104) || (det < -1e-104)))
                {
                    throw new InvalidOperationException("Determinant is close to zero");
                }


                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        output[i, j] = FunctionsForEachElements.MinusOnePow(j + i) * GetMinor(i, j) / det;
                    }
                }

                return output.Transpose();
            }
        }
        /// <summary>
        /// Минимальное значение матрицы
        /// </summary>
        /// <returns></returns>
        public double Min()
        {
            return Data.Min();
        }
        /// <summary>
        ///  Максимальное значение(Matrix)
        /// </summary>
        /// <returns></returns>
        public double Max()
        {
            return Data.Max();
        }
        /// <summary>
        /// Среднее арифметическое матрицы 
        /// </summary>
        public double Mean()
        {
            double m = 0, n = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(Data[i]))
                {
                    m += Data[i];
                    n++;
                }
            }

            return m / n;
        }
        /// <summary>
        /// Сумма 
        /// </summary>
        public double Sum()
        {
            double m = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(Data[i]))
                {
                    m += Data[i];
                }
            }

            return m;
        }
        /// <summary>
        /// Дисперсия
        /// </summary>
        public double Dispersion()
        {
            double m = 0, sq = 0, n = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(Data[i]))
                {
                    sq += Data[i] * Data[i];
                    m += Data[i];
                    n++;
                }

            }

            n = n > 0 ? n : AISettings.GlobalEps;
            m /= n;
            sq /= n;

            return sq - (m * m);
        }
        /// <summary>
        /// Среднеквадратичное отклонение
        /// </summary>
        public double Std()
        {
            return Math.Sqrt(Dispersion());
        }
        /// <summary>
        /// Адамарово произведение(поэлементное)
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix AdamarProduct(Matrix matrix)
        {
            int len = Shape.Count;
            Matrix matrixOut = new Matrix(matrix.Height, matrix.Width);

            for (int i = 0; i < len; i++)
            {
                matrixOut.Data[i] = matrix.Data[i] * Data[i];
            }

            return matrixOut;
        }

        /// <summary>
        /// Макс пулинг
        /// </summary>
        /// <param name="poolH">Шаг по высоте</param>
        /// <param name="poolW">Шаг по ширине</param>
        /// <param name="indexPool">Максимальные индексы в исходной матрице</param>
        /// <returns></returns>
        public Matrix MaxPool(int poolH, int poolW, out int[,] indexPool)
        {
            int newH = Height / poolH, newW = Width / poolW;
            Matrix outp = new Matrix(newH, newW);
            indexPool = new int[2, newH * newW];
            double max;
            int k = 0;

            for (int i = 0; i < Height - poolH; i += poolH)
            {
                int l = 0;
                for (int j = 0; j < Width - poolW; j += poolW)
                {
                    max = this[i, j];

                    for (int i2 = i + 1; i2 < i + poolH; i2++)
                    {
                        for (int j2 = j + 1; j2 < j + poolW; j2++)
                        {
                            if (this[i2, j2] > max)
                            {
                                max = this[i2, j2];
                            }
                        }
                    }

                    outp[k, l] = max;

                    l++;
                }

                k++;
            }

            return outp;
        }
        /// <summary>
        /// Умножение матрицы на вектор столбец
        /// </summary>
        /// <param name="vectCol">Вектор столбец</param>
        /// <returns></returns>
        public Vector MulMatrOnVectColumn(Vector vectCol)
        {
            Vector outp = new Vector(Height);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    outp[i] += this[i, j] * vectCol[j];
                }
            }

            return outp;
        }

        /// <summary>
        /// Минимакс нормализация
        /// </summary>
        /// <returns></returns>
        public Matrix Minimax(double maxValue = 1, double minValue = 0)
        {
            double min = Min();
            double max = Max();
            double denom = (max - min) / maxValue;
            min += minValue * denom;


            return (this - min) / denom;
        }

        /// <summary>
        ///  Представление матрицы как вектора
        /// </summary>
        public Vector LikeVector()
        {
            if (Height != 1)
            {
                throw new InvalidCastException("Cannot convert matrix to vector");
            }

            double[] vector = new double[Width];

            for (int i = 0; i < Width; i++)
            {
                vector[i] = Data[i];
            }

            return new Vector(vector);
        }
        /// <summary>
        /// Градиент свертки
        /// </summary>
        /// <param name="core"></param>
        /// <param name="delts"></param>
        /// <returns></returns>
        public Matrix GradientMatrixConvDelts(Matrix core, Matrix delts)
        {
            Matrix grad = new Matrix(core.Height, core.Width);

            for (int i = 0; i < core.Height; i++)
            {
                for (int j = 0; j < core.Width; j++)
                {
                    for (int y = 0; y < delts.Height; y++)
                    {
                        for (int x = 0; x < delts.Width; x++)
                        {
                            grad[i, j] += delts[y, x] * this[y + i, x + j];
                        }
                    }
                }
            }

            return grad / Math.Sqrt(Height * Width);
        }
        /// <summary>
        /// Выделение региона
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public Matrix Region(int x, int y, int dx, int dy)
        {
            int maxX = x + dx;
            int maxY = y + dy;
            Matrix region = new Matrix(dx, dy);

            for (int i = x; i < maxX; i++)
            {
                for (int j = y; j < maxY; j++)
                {
                    region[j - y, i - x] = this[j, i];
                }
            }

            return region;
        }
        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <returns>Возвращает транспонированную матрицу</returns>
        public Matrix Transpose()
        {
            double[,] T = new double[Width, Height];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    T[j, i] = this[i, j];
                }
            }

            return new Matrix(T);
        }
        /// <summary>
        /// Трансформирование матрицы
        /// </summary>
        /// <param name="transformFunc">Функция трансформации</param>
        /// <returns></returns>
        public Matrix Transform(Func<double, double> transformFunc)
        {
            Matrix T = new Matrix(Height, Width);
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                T.Data[i] = transformFunc(Data[i]);
            }

            return T;
        }
        /// <summary>
        /// Копирование матрицы
        /// </summary>
        /// <returns>Возвращает копию</returns>
        public Matrix Copy()
        {
            Matrix B = new Matrix(Height, Width);
            Buffer.BlockCopy(Data, 0, B.Data, 0, Data.Length * 8);
            return B;
        }
        /// <summary>
        /// Окугление значений
        /// </summary>
        /// <param name="n">До какого знака</param>
        public Matrix Round(int n)
        {
            Matrix matr = new Matrix(Height, Width);
            int count = Shape.Count;

            for (int i = 0; i < count; i++)
            {
                matr.Data[i] = Math.Round(Data[i], n);
            }

            return matr;
        }
        /// <summary>
        /// Переводит произвольную матрицу в треугольную
        /// </summary>
        /// <returns>Диагональная матрица</returns>
        public Matrix ToTriangularMatr()
        {
            Matrix matrix = Copy();
            int n = matrix.Height;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double koef = matrix[j, i] / matrix[i, i];

                    for (int k = i; k < n; k++)
                    {
                        matrix[j, k] -= matrix[i, k] * koef;
                    }
                }
            }
            return matrix.Transform(x => double.IsNaN(x) ? 0 : x);
        }
        /// <summary>
        /// Возвращает вектор с нужного среза, нужный индекс
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="dimension">Срез/размерность</param>
        /// <returns>Вектор</returns>
        public Vector GetVector(int index, int dimension)
        {
            Vector result;

            switch (dimension)
            {
                case 0:
                    result = new Vector(Height);
                    for (int i = 0; i < Height; i++)
                    {
                        result[i] = this[i, index];
                    }

                    return result;
                case 1:
                    result = new Vector(Width);
                    for (int i = 0; i < Width; i++)
                    {
                        result[i] = this[index, i];
                    }

                    return result;
            }

            throw new Exception("Индекс может быть только 1 или 0");
        }
        /// <summary>
        /// Перегруппировка матрицы (Замена индексов)
        /// </summary>
        /// <param name="i">На какой индекс заменить</param>
        /// <param name="j">Какой индекс заменить</param>
        /// <param name="dimension">Размерность среза 0 или 1</param>
        public void Swap(int i, int j, int dimension)
        {
            if (i != j)
            {
                double c;
                switch (dimension)
                {
                    case 0:
                        for (int k = 0; k < Height; k++)
                        {
                            c = this[k, i];
                            this[k, i] = this[k, j];
                            this[k, j] = c;
                        }
                        break;
                    case 1:
                        for (int k = 0; k < Width; k++)
                        {
                            c = this[i, k];
                            this[i, k] = this[j, k];
                            this[j, k] = c;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Статические методы
        /// <summary>
        /// Замена неопределенности средним значением
        /// </summary>
        /// <param name="matrices"></param>
        public static Matrix[] NanToMeanOfFeatures(Matrix[] matrices)
        {
            Matrix[] outp = new Matrix[matrices.Length];

            for (int i = 0; i < matrices.Length; i++)
            {
                outp[i] = matrices[i].NanToValue();
            }

            Matrix mean = MeanMatrix(outp);

            for (int k = 0; k < matrices.Length; k++)
            {
                for (int i = 0; i < outp[0].Height; i++)
                {
                    for (int j = 0; j < outp[0].Width; j++)
                    {
                        outp[k][i, j] = double.IsNaN(matrices[k][i, j]) ? mean[i, j] : outp[k][i, j];
                    }
                }
            }

            return outp;
        }
        /// <summary>
        /// Умножение вектора-столбца на вектор строку, возвращается матрица результата
        /// </summary>
        /// <param name="ABinaryBip">Бинарный вектор</param>
        /// <param name="B">строка</param>
        /// <returns></returns>
        public static Matrix Mul2VecFast(Vector ABinaryBip, Vector B)
        {
            Matrix matr = new Matrix(ABinaryBip.Count, B.Count);

            for (int i = 0; i < ABinaryBip.Count; i++)
            {

                if (ABinaryBip[i] < 0)
                {
                    for (int j = 0; j < B.Count; j++)
                    {
                        matr[i, j] = -B[j];
                    }
                }
                else
                {
                    for (int j = 0; j < B.Count; j++)
                    {
                        matr[i, j] = B[j];
                    }
                }

            }

            return matr;
        }
        /// <summary>
        /// Умножение вектора-столбца на вектор строку, возвращается матрица результата
        /// </summary>
        /// <param name="A">столбец</param>
        /// <param name="B">строка</param>
        /// <returns></returns>
        public static Matrix Mul2Vec(Vector A, Vector B)
        {
            Matrix matr = new Matrix(A.Count, B.Count);

            for (int i = 0; i < A.Count; i++)
            {
                for (int j = 0; j < B.Count; j++)
                {
                    matr[i, j] = B[j] * A[i];
                }
            }

            return matr;
        }
        /// <summary>
        /// Сложение вектора-столбца на вектор строку по следующему правилу "matr[i, j] = B[j] + A[i];" возвращается матрица результата
        /// </summary>
        /// <param name="A">столбец</param>
        /// <param name="B">строка</param>
        public static Matrix Sum2Vec(Vector A, Vector B)
        {
            Matrix matr = new Matrix(A.Count, B.Count);

            for (int i = 0; i < A.Count; i++)
            {
                for (int j = 0; j < B.Count; j++)
                {
                    matr[i, j] = B[j] + A[i];
                }
            }

            return matr;
        }
        /// <summary>
        /// Вычисление нормы по след. правилу  matr[i, j] = Math.Sqrt(B[j]*B[j]+ A[i]*A[i]);, возвращается матрица результата
        /// </summary>
        /// <param name="A">столбец</param>
        /// <param name="B">строка</param>
        /// <returns></returns>
        public static Matrix Norm2Vec(Vector A, Vector B)
        {
            Matrix matr = new Matrix(A.Count, B.Count);

            for (int i = 0; i < A.Count; i++)
            {
                for (int j = 0; j < B.Count; j++)
                {
                    matr[i, j] = Math.Sqrt((B[j] * B[j]) + (A[i] * A[i]));
                }
            }

            return matr;
        }
        /// <summary>
        /// Возведение матрицы в степень 
        /// путем матричного умножения на саму себя
        /// </summary>
        /// <param name="A">Входная матрица</param>
        /// <param name="exponent">Степень</param>
        public static Matrix Pow(Matrix A, int exponent)
        {
            Matrix B = A.Copy();

            for (int i = 1; i < exponent; i++)
            {
                B *= A;
            }

            return B;
        }
        /// <summary>
        /// Разложение матрицы на столбцы
        /// </summary>
        /// <param name="matr">Матрица</param>
        /// <returns>Массив векторов</returns>
        public static Vector[] GetColumns(Matrix matr)
        {
            Vector[] columns = new Vector[matr.Width];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new Vector(matr.Height);
                for (int j = 0; j < matr.Height; j++)
                {
                    columns[i][j] = matr[j, i];
                }
            }

            return columns;
        }

        /// <summary>
        /// Разложение матрицы на строки
        /// </summary>
        /// <param name="matr">Матрица</param>
        /// <returns>Массив векторов</returns>
        public static Vector[] GetRows(Matrix matr)
        {
            Vector[] rows = new Vector[matr.Height];

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new Vector(matr.Width);
                for (int j = 0; j < matr.Width; j++)
                {
                    rows[i][j] = matr[i, j];
                }
            }

            return rows;
        }
        /// <summary>
        /// Альтернативная матрица
        /// </summary>
        /// <param name="functions">Функции</param>
        /// <param name="values">Значения</param>
        /// <returns>Возвращает альтернативную матрицу</returns>
        public static Matrix AlternativMatrix(Func<double, double>[] functions, Vector values)
        {
            Matrix matr = new Matrix(values.Count, functions.Length);

            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < functions.Length; j++)
                {
                    matr[i, j] = functions[j](values[i]);
                }
            }

            return matr;
        }
        /// <summary>
        /// Ортогональная матрица
        /// </summary>
        /// <param name="functions">Порождающая функция</param>
        /// <param name="values">Значения</param>
        /// <param name="count">Число выходов</param>
        /// <returns>Возвращает ортогональную матрицу</returns>
        public static Matrix OrtogonalMatrix(Func<int, double, double> functions, Vector values, int count)
        {
            Matrix matr = new Matrix(values.Count, count);

            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    matr[i, j] = functions(j, values[i]);
                }
            }

            return matr;
        }
        /// <summary>
        /// Метод создает матрицу с коэффициентами попарной корреляции векторов
        /// </summary>
        /// <param name="vectors">Вектора</param>
        /// <returns>Корреляционная матрица</returns>
        public static Matrix GetCorrelationMatrixNorm(Vector[] vectors)
        {
            Matrix corelationMatrix = new Matrix(vectors.Length, vectors.Length);
            for (int i = 0; i < vectors.Length; i++)
                for (int j = i; j < vectors.Length; j++)
                    if (i == j) corelationMatrix[i, j] = 1;
                    else
                    {
                        corelationMatrix[i, j] = Statistic.CorrelationCoefficient(vectors[i], vectors[j]);
                        corelationMatrix[j, i] = corelationMatrix[i, j];
                    }

            return corelationMatrix;
        }
        /// <summary>
        /// Метод создает матрицу с коэффициентами попарной ковариции векторов
        /// </summary>
        /// <param name="vectors">Вектора</param>
        /// <returns>Ковариационнай матрица</returns>
        public static Matrix GetCovMatrix(Vector[] vectors)
        {
            Matrix covMatrix = new Matrix(vectors.Length, vectors.Length);
            for (int i = 0; i < vectors.Length; i++)
                for (int j = i; j < vectors.Length; j++)
                {
                    covMatrix[i, j] = Statistic.Cov(vectors[i], vectors[j]);
                    covMatrix[j, i] = covMatrix[i, j];
                }

            return covMatrix;
        }

        /// <summary>
        /// Метод создает матрицу с коэффициентами попарной ковариции векторов
        /// </summary>
        /// <param name="matrix">Матрица</param>
        /// <returns>Ковариационнай матрица</returns>
        public static Matrix GetCovMatrixFromColumns(Matrix matrix)
        {
            Vector[] vectors = GetColumns(matrix);
            return GetCovMatrix(vectors);
        }
        /// <summary>
        /// Матрица средних 
        /// </summary>
        public static Matrix MeanMatrix(Matrix[] matrices)
        {
            if (matrices == null)
            {
                throw new ArgumentNullException(nameof(matrices));
            }

            if (matrices.Length == 0)
            {
                throw new ArgumentException("Given array is empty", nameof(matrices));
            }

            Matrix m = new Matrix(matrices[0].Height, matrices[0].Width);

            for (int i = 0; i < matrices.Length; i++)
            {
                m += matrices[i];
            }

            return m / matrices.Length;
        }
        /// <summary>
        /// Матрица дисперсий 
        /// </summary>
        public static Matrix DispersionMatrix(Matrix[] matrices)
        {
            if (matrices == null)
            {
                throw new ArgumentNullException(nameof(matrices));
            }

            if (matrices.Length == 0)
            {
                throw new ArgumentException("Given array is empty", nameof(matrices));
            }

            Matrix m = new Matrix(matrices[0].Height, matrices[0].Width), sq, matrixM = MeanMatrix(matrices);

            for (int i = 0; i < matrices.Length; i++)
            {
                sq = matrices[i] - matrixM;
                m += sq * sq;
            }

            return m / (matrices.Length - 1);
        }
        /// <summary>
        /// Матрица среднеквадратичных отклонений
        /// </summary>
        /// <param name="matrices">Массив матриц</param>
        public static Matrix StdMatrix(Matrix[] matrices)
        {
            return DispersionMatrix(matrices).Transform(Math.Sqrt);
        }
        #endregion

        #region Статические методы инициализации
        /// <summary>
        /// Инициализация матрицы с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Matrix Parse(string str)
        {
            return Parse(str, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация матрицы с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Matrix Parse(string str, NumberFormatInfo provider)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            string trimmed = str.Trim();

            string[] rows = trimmed.Split('\n');

            Vector[] vects = new Vector[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                vects[i] = Vector.Parse(rows[i].Trim('\r'), provider);
            }

            return FromVectorsAsRows(vects);
        }
        /// <summary>
        /// Инициализация матрицы с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out Matrix result)
        {
            return TryParse(str, out result, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация матрицы с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out Matrix result, NumberFormatInfo provider)
        {
            if (str == null)
            {
                result = null;
                return false;
            }

            if (provider == null)
            {
                result = null;
                return false;
            }

            string trimmed = str.Trim();
            string[] rows = trimmed.Split('\n');
            Vector[] vects = new Vector[rows.Length];
            int width = -1;

            for (int i = 0; i < rows.Length; i++)
            {
                if (!Vector.TryParse(rows[i].Trim('\r'), out Vector res, provider))
                {
                    result = null;
                    return false;
                }

                if (width == -1)
                    width = res.Count;
                
                if (res.Count != width)
                {
                    result = null;
                    return false;
                }

                vects[i] = res;
            }

            result = FromVectorsAsRows(vects);
            return true;
        }
        /// <summary>
        /// Инициализация матрицы с помощью векторов-строк
        /// </summary>
        /// <param name="rows">Строки</param>
        /// <returns></returns>
        public static Matrix FromVectorsAsRows(IEnumerable<Vector> rows)
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            Vector[] vectors = rows.ToArray();
            int width = vectors[0].Count;
            Matrix result = new Matrix(vectors.Length, width);

            for (int i = 0; i < vectors.Length; i++)
            {
                if (vectors[i].Count != width)
                    throw new ArgumentException($"Число элементов входного вектора ({i}) не равно ширине матрицы", nameof(vectors));
                
                for (int j = 0; j < width; j++)
                    result[i, j] = vectors[i][j];
            }

            return result;
        }
        /// <summary>
        /// Инициализация матрицы с помощью векторов-столбцов
        /// </summary>
        /// <param name="colums">Столбцы матрицы</param>
        public static Matrix FromVectorsAsColumns(IEnumerable<Vector> colums)
        {
            if (colums == null)
                throw new ArgumentNullException(nameof(colums));

            Vector[] vectors = colums.ToArray();
            int height = vectors[0].Count;

            Matrix result = new Matrix(height, vectors.Length);

            for (int i = 0; i < vectors.Length; i++)
            {
                if (vectors[i].Count != height)
                    throw new ArgumentException($"Число элементов входного вектора ({i}) не равно высоте матрицы", nameof(vectors));
              
                for (int j = 0; j < height; j++)
                    result[j, i] = vectors[i][j];
            }

            return result;
        }
        /// <summary>
        /// Инициализация матрицы с помощью двухмерного массива строк
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Matrix FromStrings(string[,] arr)
        {
            return FromStrings(arr, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация матрицы с помощью двухмерного массива строк
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Matrix FromStrings(string[,] arr, NumberFormatInfo provider)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Matrix result = new Matrix(arr.GetLength(0), arr.GetLength(1));

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    result[i, j] = double.Parse(arr[i, j], provider);
                }
            }

            return result;
        }
        #endregion

        #region Технические методы
        /// <summary>
        /// Преобразование матрицы в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(AISettings.GetProvider());
        }

        /// <summary>
        /// Преобразование матрицы в строку
        /// </summary>
        public string ToString(NumberFormatInfo provider)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Height; i++)
            {
                sb.Append("[");

                for (int j = 0; j < Width; j++)
                {
                    sb.Append(this[i, j].ToString(provider));
                    sb.Append(" ");
                }

                sb.Length--;
                sb.AppendLine("]");
            }

            sb.Length -= Environment.NewLine.Length;
            return sb.ToString();
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Matrix matrix)
            {
                return matrix == this;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Проверка равенства
        /// </summary>
        public bool Equals(Matrix other)
        {
            return this == other;
        }
        /// <summary>
        /// Получение хэша
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ((Vector)Data).GetHashCode();
                hash = (hash * 13) + Height;
                hash = (hash * 13) + Width;
                return hash;
            }
        }
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Сохранение матрицы в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранение матрицы в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Сохранение матрицы в файл в текстовом формате
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void SaveAsText(string path)
        {
            File.WriteAllText(path, ToString());
        }
        /// <summary>
        /// Представление массивом байт
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.Matrix).Write((byte)DataType).Write(Height).Write(Width).Write(Data).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузка матрицы
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static Matrix Load(string path)
        {
            return BinarySerializer.Load<Matrix>(path);
        }
        /// <summary>
        /// Загрузка матрицы
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static Matrix Load(Stream stream)
        {
            return BinarySerializer.Load<Matrix>(stream);
        }
        /// <summary>
        /// Загрузка матрицы
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static Matrix LoadAsText(string path)
        {
            return Parse(File.ReadAllText(path));
        }
        /// <summary>
        /// Загрузка матрицы
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Matrix FromBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return FromDataStream(InMemoryDataStream.FromByteArray(data));
        }
        /// <summary>
        /// Загрузка матрицы
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public static Matrix FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            dataStream.SkipIfEqual(KeyWords.Matrix);
            MatrixType type = (MatrixType)dataStream.ReadByte();
            dataStream.ReadInt(out int height).ReadInt(out int width).ReadDoubles(out double[] mData);
            Matrix result = new Matrix(height, width)
            {
                DataType = type,
                Data = mData
            };
            return result;
        }
        #endregion

        #endregion

        #region Приватные методы
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetIndex(int i, int j)
        {
            return (Width * i) + j;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Get(int i, int j)
        {
            return Data[GetIndex(i, j)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int i, int j, double value)
        {
            Data[GetIndex(i, j)] = value;
        }
        #endregion
    }

    /// <summary>
    /// Тип матрицы
    /// </summary>
    public enum MatrixType : byte
    {
        /// <summary>
        /// Изображение
        /// </summary>
        Image,
        /// <summary>
        /// Математическая структура
        /// </summary>
        MatStruct
    }
}