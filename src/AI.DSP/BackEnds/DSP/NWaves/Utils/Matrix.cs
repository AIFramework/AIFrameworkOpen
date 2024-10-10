using System;

namespace AI.BackEnds.DSP.NWaves.Utils
{
    /// <summary>
    /// Class representing 2d matrix
    /// </summary>
    public class MatrixNWaves
    {
        private readonly double[][] _matrix;
        /// <summary>
        /// Число строк
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Число столбцов
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public MatrixNWaves(int rows, int columns = 0)
        {
            if (columns == 0)
            {
                columns = rows;
            }

            Guard.AgainstNonPositive(rows, "Number of rows");
            Guard.AgainstNonPositive(columns, "Number of columns");

            _matrix = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                _matrix[i] = new double[columns];
            }

            Rows = rows;
            Columns = columns;
        }

        /// <summary>
        /// Get 2d array reference
        /// </summary>
        /// <returns></returns>
        public double[][] As2dArray()
        {
            return _matrix;
        }

        /// <summary>
        /// Transposed matrix
        /// </summary>
        public MatrixNWaves T
        {
            get
            {
                MatrixNWaves transposed = new MatrixNWaves(Columns, Rows);

                for (int i = 0; i < Columns; i++)
                {
                    for (int j = 0; j < Rows; j++)
                    {
                        transposed[i][j] = _matrix[j][i];
                    }
                }

                return transposed;
            }
        }

        /// <summary>
        /// Companion matrix
        /// </summary>
        /// <param name="a">Input array</param>
        /// <returns>Companion matrix</returns>
        public static MatrixNWaves Companion(double[] a)
        {
            if (a.Length < 2)
            {
                throw new ArgumentException("The size of input array must be at least 2!");
            }

            if (Math.Abs(a[0]) < 1e-30)
            {
                throw new ArgumentException("The first coefficient must not be zero!");
            }

            int size = a.Length - 1;

            MatrixNWaves companion = new MatrixNWaves(size);

            for (int i = 0; i < size; i++)
            {
                companion[0][i] = -a[i + 1] / a[0];
            }

            for (int i = 1; i < size; i++)
            {
                companion[i][i - 1] = 1;
            }

            return companion;
        }

        /// <summary>
        /// Identity matrix
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static MatrixNWaves Eye(int size)
        {
            MatrixNWaves eye = new MatrixNWaves(size);

            for (int i = 0; i < size; i++)
            {
                eye[i][i] = 1;
            }

            return eye;
        }
        /// <summary>
        /// 
        /// </summary>
        public static MatrixNWaves operator +(MatrixNWaves m1, MatrixNWaves m2)
        {
            Guard.AgainstInequality(m1.Rows, m2.Rows, "Number of rows in first matrix", "number of rows in second matrix");
            Guard.AgainstInequality(m1.Columns, m2.Columns, "Number of columns in first matrix", "number of columns in second matrix");

            MatrixNWaves result = new MatrixNWaves(m1.Rows, m1.Columns);

            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i][j] = m1[i][j] + m2[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static MatrixNWaves operator -(MatrixNWaves m1, MatrixNWaves m2)
        {
            Guard.AgainstInequality(m1.Rows, m2.Rows, "Number of rows in first matrix", "number of rows in second matrix");
            Guard.AgainstInequality(m1.Columns, m2.Columns, "Number of columns in first matrix", "number of columns in second matrix");

            MatrixNWaves result = new MatrixNWaves(m1.Rows, m1.Columns);

            for (int i = 0; i < m1.Rows; i++)
            {
                for (int j = 0; j < m1.Columns; j++)
                {
                    result[i][j] = m1[i][j] - m2[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// Значение по индексу
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double[] this[int i] => _matrix[i];
    }
}
