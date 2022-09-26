using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.Extensions;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vector = AI.DataStructs.Algebraic.Vector;

namespace AI.DataStructs.WithComplexElements
{
    /// <summary>
    /// Матрица с комплексными числами
    /// </summary>
    [Serializable]
    public class ComplexMatrix : IComplexStructure, ISavable, IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Matrix data
        /// </summary>
        public Complex[] Data { get; }
        /// <summary>
        /// Matrix height
        /// </summary>
        public int Height => Shape[1];
        /// <summary>
        /// Matrix width
        /// </summary>
        public int Width => Shape[0];
        /// <summary>
        /// Matrix shape
        /// </summary>
        public Shape Shape { get; } = new Shape2D(3, 3);
        /// <summary>
        /// Get element by indexes
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public Complex this[int i, int j]
        {
            get => Get(i, j);
            set => Set(i, j, value);
        }
        /// <summary>
        /// Real parts of all matrix components represented as algebraic matrix
        /// </summary>
        public Matrix RealMatrix
        {
            get
            {
                Matrix ret = new Matrix(Height, Width);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        ret[i, j] = this[i, j].Real;
                    }
                }

                return ret;
            }
        }
        /// <summary>
        /// Imaginary parts of all matrix components represented as algebraic matrix
        /// </summary>
        public Matrix ImaginaryMatrix
        {
            get
            {
                Matrix ret = new Matrix(Height, Width);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        ret[i, j] = this[i, j].Imaginary;
                    }
                }

                return ret;
            }
        }
        /// <summary>
        /// Magnitude parts of all matrix components represented as algebraic matrix
        /// </summary>
        public Matrix MagnitudeMatrix
        {
            get
            {
                Matrix ret = new Matrix(Height, Width);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        ret[i, j] = this[i, j].Magnitude;
                    }
                }

                return ret;
            }
        }
        /// <summary>
        /// Phase parts of all matrix components represented as algebraic matrix
        /// </summary>
        public Matrix PhaseMatrix
        {
            get
            {
                Matrix ret = new Matrix(Height, Width);

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        ret[i, j] = this[i, j].Phase;
                    }
                }

                return ret;
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Creates matrix of 3x3 size
        /// </summary>
        public ComplexMatrix()
        {
            Data = new Complex[Shape.Count];
        }
        /// <summary>
        /// Creates matrix of the given size
        /// </summary>
        /// <param name="height">Matrix Высота</param>
        /// <param name="width">Matrix Ширина</param>
        public ComplexMatrix(int height, int width)
        {
            Shape = new Shape2D(height, width);
            Data = new Complex[Shape.Count];
        }
        /// <summary>
        /// Creates matrix with real and imaginary element's parts represented as algebraic matrices
        /// </summary>
        /// <param name="real">Real element's part</param>
        /// <param name="imaginary">Imaginary element's part</param>
        public ComplexMatrix(Matrix real, Matrix imaginary)
        {
            if (real == null)
            {
                throw new ArgumentNullException(nameof(real));
            }

            if (imaginary == null)
            {
                throw new ArgumentNullException(nameof(imaginary));
            }

            if (real.Shape != imaginary.Shape)
            {
                throw new InvalidOperationException("Matrices dimensions don't match");
            }

            Shape = new Shape2D(real.Height, real.Width);
            Data = new Complex[Shape.Count];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = new Complex(real[i, j], imaginary[i, j]);
                }
            }
        }
        /// <summary>
        /// Creates matrix with real element's parts represented as algebraic matrix and zero imaginary parts
        /// </summary>
        /// <param name="real">Реальная часть</param>
        public ComplexMatrix(Matrix real)
        {
            if (real == null)
            {
                throw new ArgumentNullException(nameof(real));
            }

            Shape = new Shape2D(real.Height, real.Width);
            Data = new Complex[Shape.Count];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = new Complex(real[i, j], 0);
                }
            }
        }
        #endregion

        #region Операторы
        /// <summary>
        /// Перемножение матриц
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static ComplexMatrix operator *(ComplexMatrix left, ComplexMatrix right)
        {
            ComplexMatrix ret = new ComplexMatrix(left.Height, right.Width);

            if (left.Width != right.Height)
            {
                throw new InvalidOperationException("Can't multiply given matrices");
            }

            for (int i = 0; i < left.Height; i++)
            {
                for (int j = 0; j < right.Width; j++)
                {
                    for (int k = 0; k < left.Width; k++)
                    {
                        ret[i, j] += left[i, k] * right[k, j];
                    }
                }
            }

            return ret;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Поэлементное преобразование матриц
        /// </summary>
        /// <param name="func">Функция преобразования</param>
        /// <returns></returns>
        public ComplexMatrix Transform(Func<Complex, Complex> func)
        {
            ComplexMatrix matrix = new ComplexMatrix(Height, Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    matrix[i, j] = func(this[i, j]);
                }
            }

            return matrix;
        }
        /// <summary>
        /// Матрица изменяет саму себя
        /// </summary>
        /// <param name="func">Функция преобразования</param>
        public void TransformSelf(Func<Complex, Complex> func)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    this[i, j] = func(this[i, j]);
                }
            }
        }
        /// <summary>
        /// Двумерное преобразование Фурье
        /// </summary>
        /// <param name="input">Вход</param>
        public static ComplexMatrix MatrixFFT(Matrix input)
        {
            ComplexMatrix matrix;
            Vector[] vs = Matrix.GetColumns(input);
            ComplexVector[] complexVector = new ComplexVector[vs.Length];
            FFT furie = new FFT(vs[0].Count);

            for (int i = 0; i < vs.Length; i++)
            {
                complexVector[i] = furie.CalcFFT(vs[i]);
            }

            matrix = new ComplexMatrix(complexVector.Length, complexVector[0].Count);

            for (int i = 0; i < vs.Length; i++)
            {
                for (int j = 0; j < vs[0].Count; j++)
                {
                    matrix[i, j] = complexVector[i][j];
                }
            }

            complexVector = new ComplexVector[matrix.Width];

            for (int i = 0; i < matrix.Width; i++)
            {
                complexVector[i] = new ComplexVector(matrix.Height);

                for (int j = 0; j < matrix.Height; j++)
                {
                    complexVector[i][j] = matrix[j, i];
                }
            }

            for (int i = 0; i < complexVector.Length; i++)
            {
                complexVector[i] = FFT.CalcFFT(complexVector[i]);
            }

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    matrix[j, i] = complexVector[i][j];
                }
            }

            return matrix;
        }
        /// <summary>
        /// Обратное двумерное преобразование Фурье
        /// </summary>
        /// <param name="input">Входная матрица</param>
        /// <returns></returns>
        public static ComplexMatrix MatrixIFFT(ComplexMatrix input)
        {
            ComplexMatrix matrix;
            ComplexVector[] vs = GetColumns(input.ConjugateMatr());
            ComplexVector[] complexVector = new ComplexVector[vs.Length];

            for (int i = 0; i < vs.Length; i++)
            {
                complexVector[i] = FFT.CalcFFT(vs[i]);
            }

            matrix = new ComplexMatrix(complexVector.Length, complexVector[0].Count);

            for (int i = 0; i < vs.Length; i++)
            {
                for (int j = 0; j < vs[0].Count; j++)
                {
                    matrix[i, j] = complexVector[i][j];
                }
            }

            complexVector = new ComplexVector[matrix.Width];

            for (int i = 0; i < matrix.Width; i++)
            {
                complexVector[i] = new ComplexVector(matrix.Height);

                for (int j = 0; j < matrix.Height; j++)
                {
                    complexVector[i][j] = matrix[j, i];
                }
            }

            for (int i = 0; i < complexVector.Length; i++)
            {
                complexVector[i] = FFT.CalcFFT(complexVector[i]);
            }

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    matrix[j, i] = complexVector[i][j];
                }
            }

            return matrix;
        }
        /// <summary>
        /// Двумерное преобразование Фурье
        /// </summary>
        /// <param name="input">Вход</param>
        public static ComplexMatrix MatrixFFT(ComplexMatrix input)
        {
            ComplexMatrix matrix;
            ComplexVector[] vs = ComplexMatrix.GetColumns(input);
            ComplexVector[] complexVector = new ComplexVector[vs.Length];

            for (int i = 0; i < vs.Length; i++)
            {
                complexVector[i] = FFT.CalcFFT(vs[i]);
            }

            matrix = new ComplexMatrix(complexVector.Length, complexVector[0].Count);

            for (int i = 0; i < vs.Length; i++)
            {
                for (int j = 0; j < vs[0].Count; j++)
                {
                    matrix[i, j] = complexVector[i][j];
                }
            }

            complexVector = new ComplexVector[matrix.Width];

            for (int i = 0; i < matrix.Width; i++)
            {
                complexVector[i] = new ComplexVector(matrix.Height);

                for (int j = 0; j < matrix.Height; j++)
                {
                    complexVector[i][j] = matrix[j, i];
                }
            }

            for (int i = 0; i < complexVector.Length; i++)
            {
                complexVector[i] = FFT.CalcFFT(complexVector[i]);
            }

            for (int i = 0; i < matrix.Width; i++)
            {
                for (int j = 0; j < matrix.Height; j++)
                {
                    matrix[j, i] = complexVector[i][j];
                }
            }

            return matrix;
        }
        /// <summary>
        /// Разложение матрицы на столбцы
        /// </summary>
        /// <param name="matr">Матрица</param>
        /// <returns>Массив векторов</returns>
        public static ComplexVector[] GetColumns(ComplexMatrix matr)
        {
            ComplexVector[] columns = new ComplexVector[matr.Width];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new ComplexVector(matr.Height);
                for (int j = 0; j < matr.Height; j++)
                {
                    columns[i][j] = matr[j, i];
                }
            }

            return columns;
        }
        /// <summary>
        /// Сопряженная матрица
        /// </summary>
        /// <returns></returns>
        public ComplexMatrix ConjugateMatr()
        {
            ComplexMatrix cm = new ComplexMatrix(Height, Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    cm[i, j] = Complex.Conjugate(this[i, j]);
                }
            }

            return cm;
        }
        /// <summary>
        /// Адамарово произведение матриц (поэлементное)
        /// </summary>
        /// <param name="complexMatrix">Матрица на которую происходит умножение</param>
        public ComplexMatrix AdamarProduct(ComplexMatrix complexMatrix)
        {
            ComplexMatrix cm = new ComplexMatrix(Height, Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    cm[i, j] = this[i, j] * complexMatrix[i, j];
                }
            }

            return cm;
        }
        /// <summary>
        /// Адамарово произведение матриц (поэлементное)
        /// </summary>
        /// <param name="matrix">Матрица на которую происходит умножение</param>
        public ComplexMatrix AdamarProduct(Matrix matrix)
        {
            ComplexMatrix cm = new ComplexMatrix(Height, Width);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    cm[i, j] = this[i, j] * matrix[i, j];
                }
            }

            return cm;
        }
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Saves matrix to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Saves matrix to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Представить в виде массива байт
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.ComplexMatrix).Write(RealMatrix).Write(ImaginaryMatrix).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Loads matrix from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ComplexMatrix Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File was not found", path);
            }

            return BinarySerializer.Load<ComplexMatrix>(path);
        }
        /// <summary>
        /// Loads matrix from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ComplexMatrix Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return BinarySerializer.Load<ComplexMatrix>(stream);
        }
        /// <summary>
        /// Initializes matrix from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ComplexMatrix FromBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return FromDataStream(new InMemoryDataStream(data));
        }
        /// <summary>
        /// Initilizes matrix from data stream
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public static ComplexMatrix FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            dataStream.SkipIfEqual(KeyWords.ComplexMatrix).ReadMatrix(out Matrix real).ReadMatrix(out Matrix imaginary);

            return new ComplexMatrix(real, imaginary);
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
        private Complex Get(int i, int j)
        {
            return Data[GetIndex(i, j)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(int i, int j, Complex value)
        {
            Data[GetIndex(i, j)] = value;
        }
        #endregion
    }
}