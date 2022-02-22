// <copyright file="SparseMatrix.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Storage;
using AI.BackEnds.MathLibs.MathNet.Numerics.Providers.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Double
{
    /// <summary>
    /// A Matrix with sparse storage, intended for very large matrices where most of the cells are zero.
    /// The underlying storage scheme is 3-array compressed-sparse-row (CSR) Format.
    /// <a href="http://en.wikipedia.org/wiki/Sparse_matrix#Compressed_sparse_row_.28CSR_or_CRS.29">Wikipedia - CSR</a>.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("SparseMatrix {RowCount}x{ColumnCount}-Double {NonZerosCount}-NonZero")]
    public class SparseMatrix : MatrixMathNet
    {
        private readonly SparseCompressedRowMatrixStorage<double> _storage;

        /// <summary>
        /// Gets the number of non zero elements in the matrix.
        /// </summary>
        /// <value>The number of non zero elements.</value>
        public int NonZerosCount => _storage.ValueCount;

        /// <summary>
        /// Create a new sparse matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public SparseMatrix(SparseCompressedRowMatrixStorage<double> storage)
            : base(storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Create a new square sparse matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If the order is less than one.</exception>
        public SparseMatrix(int order)
            : this(order, order)
        {
        }

        /// <summary>
        /// Create a new sparse matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public SparseMatrix(int rows, int columns)
            : this(new SparseCompressedRowMatrixStorage<double>(rows, columns))
        {
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfMatrix(MatrixMathNet<double> matrix)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfMatrix(matrix.Storage));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfArray(double[,] array)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfArray(array));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfIndexed(int rows, int columns, IEnumerable<Tuple<int, int, double>> enumerable)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfIndexedEnumerable(rows, columns, enumerable));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfIndexed(int rows, int columns, IEnumerable<(int, int, double)> enumerable)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfIndexedEnumerable(rows, columns, enumerable));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in row-major order (row by row).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public static SparseMatrix OfRowMajor(int rows, int columns, IEnumerable<double> rowMajor)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowMajorEnumerable(rows, columns, rowMajor));
        }

        /// <summary>
        /// Create a new sparse matrix with the given number of rows and columns as a copy of the given array.
        /// The array is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public static SparseMatrix OfColumnMajor(int rows, int columns, IList<double> columnMajor)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnMajorList(rows, columns, columnMajor));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumns(IEnumerable<IEnumerable<double>> data)
        {
            return OfColumnArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumns(int rows, int columns, IEnumerable<IEnumerable<double>> data)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumnArrays(params double[][] columns)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnArrays(columns));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumnArrays(IEnumerable<double[]> columns)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnArrays((columns as double[][]) ?? columns.ToArray()));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumnVectors(params VectorMathNet<double>[] columns)
        {
            VectorStorage<double>[] storage = new VectorStorage<double>[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                storage[i] = columns[i].Storage;
            }
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnVectors(storage));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfColumnVectors(IEnumerable<VectorMathNet<double>> columns)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfColumnVectors(columns.Select(c => c.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRows(IEnumerable<IEnumerable<double>> data)
        {
            return OfRowArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRows(int rows, int columns, IEnumerable<IEnumerable<double>> data)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRowArrays(params double[][] rows)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowArrays(rows));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRowArrays(IEnumerable<double[]> rows)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowArrays((rows as double[][]) ?? rows.ToArray()));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRowVectors(params VectorMathNet<double>[] rows)
        {
            VectorStorage<double>[] storage = new VectorStorage<double>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                storage[i] = rows[i].Storage;
            }
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowVectors(storage));
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfRowVectors(IEnumerable<VectorMathNet<double>> rows)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfRowVectors(rows.Select(r => r.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfDiagonalVector(VectorMathNet<double> diagonal)
        {
            SparseMatrix m = new SparseMatrix(diagonal.Count, diagonal.Count);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfDiagonalVector(int rows, int columns, VectorMathNet<double> diagonal)
        {
            SparseMatrix m = new SparseMatrix(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfDiagonalArray(double[] diagonal)
        {
            SparseMatrix m = new SparseMatrix(diagonal.Length, diagonal.Length);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static SparseMatrix OfDiagonalArray(int rows, int columns, double[] diagonal)
        {
            SparseMatrix m = new SparseMatrix(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new sparse matrix and initialize each value to the same provided value.
        /// </summary>
        public static SparseMatrix Create(int rows, int columns, double value)
        {
            if (value == 0d)
            {
                return new SparseMatrix(rows, columns);
            }

            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfValue(rows, columns, value));
        }

        /// <summary>
        /// Create a new sparse matrix and initialize each value using the provided init function.
        /// </summary>
        public static SparseMatrix Create(int rows, int columns, Func<int, int, double> init)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new diagonal sparse matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static SparseMatrix CreateDiagonal(int rows, int columns, double value)
        {
            if (value == 0d)
            {
                return new SparseMatrix(rows, columns);
            }

            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfDiagonalInit(rows, columns, i => value));
        }

        /// <summary>
        /// Create a new diagonal sparse matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static SparseMatrix CreateDiagonal(int rows, int columns, Func<int, double> init)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfDiagonalInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new square sparse identity matrix where each diagonal value is set to One.
        /// </summary>
        public static SparseMatrix CreateIdentity(int order)
        {
            return new SparseMatrix(SparseCompressedRowMatrixStorage<double>.OfDiagonalInit(order, order, i => One));
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override MatrixMathNet<double> LowerTriangle()
        {
            MatrixMathNet<double> result = Build.SameAs(this);
            LowerTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void LowerTriangle(MatrixMathNet<double> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(this, result))
            {
                MatrixMathNet<double> tmp = Build.SameAs(result);
                LowerTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                LowerTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void LowerTriangleImpl(MatrixMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < result.RowCount; row++)
            {
                int endIndex = rowPointers[row + 1];
                for (int j = rowPointers[row]; j < endIndex; j++)
                {
                    if (row >= columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override MatrixMathNet<double> UpperTriangle()
        {
            MatrixMathNet<double> result = Build.SameAs(this);
            UpperTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void UpperTriangle(MatrixMathNet<double> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(this, result))
            {
                MatrixMathNet<double> tmp = Build.SameAs(result);
                UpperTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                UpperTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void UpperTriangleImpl(MatrixMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < result.RowCount; row++)
            {
                int endIndex = rowPointers[row + 1];
                for (int j = rowPointers[row]; j < endIndex; j++)
                {
                    if (row <= columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>
        public override MatrixMathNet<double> StrictlyLowerTriangle()
        {
            MatrixMathNet<double> result = Build.SameAs(this);
            StrictlyLowerTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyLowerTriangle(MatrixMathNet<double> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(this, result))
            {
                MatrixMathNet<double> tmp = Build.SameAs(result);
                StrictlyLowerTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                StrictlyLowerTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the strictly lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void StrictlyLowerTriangleImpl(MatrixMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < result.RowCount; row++)
            {
                int endIndex = rowPointers[row + 1];
                for (int j = rowPointers[row]; j < endIndex; j++)
                {
                    if (row > columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix. The new matrix
        /// does not contain the diagonal elements of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>
        public override MatrixMathNet<double> StrictlyUpperTriangle()
        {
            MatrixMathNet<double> result = Build.SameAs(this);
            StrictlyUpperTriangleImpl(result);
            return result;
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public override void StrictlyUpperTriangle(MatrixMathNet<double> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(this, result, "result");
            }

            if (ReferenceEquals(this, result))
            {
                MatrixMathNet<double> tmp = Build.SameAs(result);
                StrictlyUpperTriangle(tmp);
                tmp.CopyTo(result);
            }
            else
            {
                result.Clear();
                StrictlyUpperTriangleImpl(result);
            }
        }

        /// <summary>
        /// Puts the strictly upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        private void StrictlyUpperTriangleImpl(MatrixMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < result.RowCount; row++)
            {
                int endIndex = rowPointers[row + 1];
                for (int j = rowPointers[row]; j < endIndex; j++)
                {
                    if (row < columnIndices[j])
                    {
                        result.At(row, columnIndices[j], values[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(MatrixMathNet<double> result)
        {
            CopyTo(result);
            DoMultiply(-1, result);
        }

        /// <summary>Calculates the induced infinity norm of this matrix.</summary>
        /// <returns>The maximum absolute row sum of the matrix.</returns>
        public override double InfinityNorm()
        {
            int[] rowPointers = _storage.RowPointers;
            double[] values = _storage.Values;
            double norm = 0d;
            for (int i = 0; i < RowCount; i++)
            {
                int startIndex = rowPointers[i];
                int endIndex = rowPointers[i + 1];

                if (startIndex == endIndex)
                {
                    // Begin and end are equal. There are no values in the row, Move to the next row
                    continue;
                }

                double s = 0d;
                for (int j = startIndex; j < endIndex; j++)
                {
                    s += Math.Abs(values[j]);
                }
                norm = Math.Max(norm, s);
            }
            return norm;
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            SparseCompressedRowMatrixStorage<double> aat = (SparseCompressedRowMatrixStorage<double>)(this * Transpose()).Storage;
            double norm = 0d;
            for (int i = 0; i < aat.RowCount; i++)
            {
                int startIndex = aat.RowPointers[i];
                int endIndex = aat.RowPointers[i + 1];

                if (startIndex == endIndex)
                {
                    // Begin and end are equal. There are no values in the row, Move to the next row
                    continue;
                }

                for (int j = startIndex; j < endIndex; j++)
                {
                    if (i == aat.ColumnIndices[j])
                    {
                        norm += Math.Abs(aat.Values[j]);
                    }
                }
            }
            return Math.Sqrt(norm);
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            if (other is SparseMatrix sparseOther && result is SparseMatrix sparseResult)
            {
                if (ReferenceEquals(this, other))
                {
                    if (!ReferenceEquals(this, result))
                    {
                        CopyTo(result);
                    }

                    LinearAlgebraControl.Provider.ScaleArray(2.0, sparseResult._storage.Values, sparseResult._storage.Values);
                    return;
                }

                SparseMatrix left;

                if (ReferenceEquals(sparseOther, sparseResult))
                {
                    left = this;
                }
                else if (ReferenceEquals(this, sparseResult))
                {
                    left = sparseOther;
                }
                else
                {
                    CopyTo(sparseResult);
                    left = sparseOther;
                }

                SparseCompressedRowMatrixStorage<double> leftStorage = left._storage;
                for (int i = 0; i < leftStorage.RowCount; i++)
                {
                    int endIndex = leftStorage.RowPointers[i + 1];
                    for (int j = leftStorage.RowPointers[i]; j < endIndex; j++)
                    {
                        int columnIndex = leftStorage.ColumnIndices[j];
                        double resVal = leftStorage.Values[j] + result.At(i, columnIndex);
                        result.At(i, columnIndex, resVal);
                    }
                }
            }
            else
            {
                base.DoAdd(other, result);
            }
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract to this matrix.</param>
        /// <param name="result">The matrix to store the result of subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            if (other is SparseMatrix sparseOther && result is SparseMatrix sparseResult)
            {
                if (ReferenceEquals(this, other))
                {
                    result.Clear();
                    return;
                }

                SparseCompressedRowMatrixStorage<double> otherStorage = sparseOther._storage;

                if (ReferenceEquals(this, sparseResult))
                {
                    for (int i = 0; i < otherStorage.RowCount; i++)
                    {
                        int endIndex = otherStorage.RowPointers[i + 1];
                        for (int j = otherStorage.RowPointers[i]; j < endIndex; j++)
                        {
                            int columnIndex = otherStorage.ColumnIndices[j];
                            double resVal = sparseResult.At(i, columnIndex) - otherStorage.Values[j];
                            result.At(i, columnIndex, resVal);
                        }
                    }
                }
                else
                {
                    if (!ReferenceEquals(sparseOther, sparseResult))
                    {
                        sparseOther.CopyTo(sparseResult);
                    }

                    sparseResult.Negate(sparseResult);

                    int[] rowPointers = _storage.RowPointers;
                    int[] columnIndices = _storage.ColumnIndices;
                    double[] values = _storage.Values;

                    for (int i = 0; i < RowCount; i++)
                    {
                        int endIndex = rowPointers[i + 1];
                        for (int j = rowPointers[i]; j < endIndex; j++)
                        {
                            int columnIndex = columnIndices[j];
                            double resVal = sparseResult.At(i, columnIndex) + values[j];
                            result.At(i, columnIndex, resVal);
                        }
                    }
                }
            }
            else
            {
                base.DoSubtract(other, result);
            }
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(double scalar, MatrixMathNet<double> result)
        {
            if (scalar == 1.0)
            {
                CopyTo(result);
                return;
            }

            if (scalar == 0.0 || NonZerosCount == 0)
            {
                result.Clear();
                return;
            }

            if (result is SparseMatrix sparseResult)
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(sparseResult);
                }

                LinearAlgebraControl.Provider.ScaleArray(scalar, sparseResult._storage.Values, sparseResult._storage.Values);
            }
            else
            {
                result.Clear();

                int[] rowPointers = _storage.RowPointers;
                int[] columnIndices = _storage.ColumnIndices;
                double[] values = _storage.Values;

                for (int row = 0; row < RowCount; row++)
                {
                    int start = rowPointers[row];
                    int end = rowPointers[row + 1];

                    if (start == end)
                    {
                        continue;
                    }

                    for (int index = start; index < end; index++)
                    {
                        int column = columnIndices[index];
                        result.At(row, column, values[index] * scalar);
                    }
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            SparseMatrix sparseOther = other as SparseMatrix;
            SparseMatrix sparseResult = result as SparseMatrix;
            if (sparseOther != null && sparseResult != null)
            {
                DoMultiplySparse(sparseOther, sparseResult);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<double> diagonalOther && sparseResult != null)
            {
                double[] diagonal = diagonalOther.Data;
                if (other.ColumnCount == other.RowCount)
                {
                    Storage.MapIndexedTo(result.Storage, (i, j, x) => x * diagonal[j], Zeros.AllowSkip, ExistingData.Clear);
                }
                else
                {
                    result.Storage.Clear();
                    Storage.MapSubMatrixIndexedTo(result.Storage, (i, j, x) => x * diagonal[j], 0, 0, RowCount, 0, 0, Math.Min(ColumnCount, other.ColumnCount), Zeros.AllowSkip, ExistingData.AssumeZeros);
                }
                return;
            }

            result.Clear();
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            if (other.Storage is DenseColumnMajorMatrixStorage<double> denseOther)
            {
                // in this case we can directly address the underlying data-array
                for (int row = 0; row < RowCount; row++)
                {
                    int startIndex = rowPointers[row];
                    int endIndex = rowPointers[row + 1];

                    if (startIndex == endIndex)
                    {
                        continue;
                    }

                    for (int column = 0; column < other.ColumnCount; column++)
                    {
                        int otherColumnStartPosition = column * other.RowCount;
                        double sum = 0d;
                        for (int index = startIndex; index < endIndex; index++)
                        {
                            sum += values[index] * denseOther.Data[otherColumnStartPosition + columnIndices[index]];
                        }

                        result.At(row, column, sum);
                    }
                }
                return;
            }

            DenseVector columnVector = new DenseVector(other.RowCount);
            for (int row = 0; row < RowCount; row++)
            {
                int startIndex = rowPointers[row];
                int endIndex = rowPointers[row + 1];

                if (startIndex == endIndex)
                {
                    continue;
                }

                for (int column = 0; column < other.ColumnCount; column++)
                {
                    // Multiply row of matrix A on column of matrix B
                    other.Column(column, columnVector);

                    double sum = 0d;
                    for (int index = startIndex; index < endIndex; index++)
                    {
                        sum += values[index] * columnVector[columnIndices[index]];
                    }

                    result.At(row, column, sum);
                }
            }
        }

        private void DoMultiplySparse(SparseMatrix other, SparseMatrix result)
        {
            result.Clear();

            double[] ax = _storage.Values;
            int[] ap = _storage.RowPointers;
            int[] ai = _storage.ColumnIndices;

            double[] bx = other._storage.Values;
            int[] bp = other._storage.RowPointers;
            int[] bi = other._storage.ColumnIndices;

            int rows = RowCount;
            int cols = other.ColumnCount;

            int[] cp = result._storage.RowPointers;

            int[] marker = new int[cols];
            for (int ib = 0; ib < cols; ib++)
            {
                marker[ib] = -1;
            }

            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                // For each row of A
                for (int j = ap[i]; j < ap[i + 1]; j++)
                {
                    // Row number to be added
                    int a = ai[j];
                    for (int k = bp[a]; k < bp[a + 1]; k++)
                    {
                        int b = bi[k];
                        if (marker[b] != i)
                        {
                            marker[b] = i;
                            count++;
                        }
                    }
                }

                // Record non-zero count.
                cp[i + 1] = count;
            }

            int[] ci = new int[count];
            double[] cx = new double[count];

            for (int ib = 0; ib < cols; ib++)
            {
                marker[ib] = -1;
            }

            count = 0;
            for (int i = 0; i < rows; i++)
            {
                int rowStart = cp[i];
                for (int j = ap[i]; j < ap[i + 1]; j++)
                {
                    int a = ai[j];
                    double aEntry = ax[j];
                    for (int k = bp[a]; k < bp[a + 1]; k++)
                    {
                        int b = bi[k];
                        double bEntry = bx[k];
                        if (marker[b] < rowStart)
                        {
                            marker[b] = count;
                            ci[marker[b]] = b;
                            cx[marker[b]] = aEntry * bEntry;
                            count++;
                        }
                        else
                        {
                            cx[marker[b]] += aEntry * bEntry;
                        }
                    }
                }
            }

            result._storage.Values = cx;
            result._storage.ColumnIndices = ci;
            result._storage.Normalize();
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(VectorMathNet<double> rightSide, VectorMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < RowCount; row++)
            {
                int startIndex = rowPointers[row];
                int endIndex = rowPointers[row + 1];

                if (startIndex == endIndex)
                {
                    continue;
                }

                double sum = 0d;
                for (int index = startIndex; index < endIndex; index++)
                {
                    sum += values[index] * rightSide[columnIndices[index]];
                }

                result[row] = sum;
            }
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            if (other is SparseMatrix otherSparse && result is SparseMatrix resultSparse)
            {
                resultSparse.Clear();

                int[] rowPointers = _storage.RowPointers;
                double[] values = _storage.Values;

                SparseCompressedRowMatrixStorage<double> otherStorage = otherSparse._storage;

                for (int j = 0; j < RowCount; j++)
                {
                    int startIndexOther = otherStorage.RowPointers[j];
                    int endIndexOther = otherStorage.RowPointers[j + 1];

                    if (startIndexOther == endIndexOther)
                    {
                        continue;
                    }

                    for (int i = 0; i < RowCount; i++)
                    {
                        int startIndexThis = rowPointers[i];
                        int endIndexThis = rowPointers[i + 1];

                        if (startIndexThis == endIndexThis)
                        {
                            continue;
                        }

                        double sum = 0d;
                        for (int index = startIndexOther; index < endIndexOther; index++)
                        {
                            int ind = _storage.FindItem(i, otherStorage.ColumnIndices[index]);
                            if (ind >= 0)
                            {
                                sum += otherStorage.Values[index] * values[ind];
                            }
                        }

                        resultSparse._storage.At(i, j, sum + result.At(i, j));
                    }
                }
            }
            else
            {
                base.DoTransposeAndMultiply(other, result);
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(VectorMathNet<double> rightSide, VectorMathNet<double> result)
        {
            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < RowCount; row++)
            {
                int startIndex = rowPointers[row];
                int endIndex = rowPointers[row + 1];

                if (startIndex == endIndex)
                {
                    continue;
                }

                double rightSideValue = rightSide[row];
                for (int index = startIndex; index < endIndex; index++)
                {
                    result[columnIndices[index]] += values[index] * rightSideValue;
                }
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            result.Clear();

            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int i = 0; i < RowCount; i++)
            {
                int endIndex = rowPointers[i + 1];
                for (int j = rowPointers[i]; j < endIndex; j++)
                {
                    double resVal = values[j] * other.At(i, columnIndices[j]);
                    if (resVal != 0d)
                    {
                        result.At(i, columnIndices[j], resVal);
                    }
                }
            }
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(MatrixMathNet<double> divisor, MatrixMathNet<double> result)
        {
            result.Clear();

            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int i = 0; i < RowCount; i++)
            {
                int endIndex = rowPointers[i + 1];
                for (int j = rowPointers[i]; j < endIndex; j++)
                {
                    if (values[j] != 0d)
                    {
                        result.At(i, columnIndices[j], values[j] / divisor.At(i, columnIndices[j]));
                    }
                }
            }
        }

        public override void KroneckerProduct(MatrixMathNet<double> other, MatrixMathNet<double> result)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.RowCount != (RowCount * other.RowCount) || result.ColumnCount != (ColumnCount * other.ColumnCount))
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(this, other, result);
            }

            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int i = 0; i < RowCount; i++)
            {
                int endIndex = rowPointers[i + 1];
                for (int j = rowPointers[i]; j < endIndex; j++)
                {
                    if (values[j] != 0d)
                    {
                        result.SetSubMatrix(i * other.RowCount, other.RowCount, columnIndices[j] * other.ColumnCount, other.ColumnCount, values[j] * other);
                    }
                }
            }
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoModulus(double divisor, MatrixMathNet<double> result)
        {
            if (result is SparseMatrix sparseResult)
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                SparseCompressedRowMatrixStorage<double> resultStorage = sparseResult._storage;
                for (int index = 0; index < resultStorage.Values.Length; index++)
                {
                    resultStorage.Values[index] = Euclid.Modulus(resultStorage.Values[index], divisor);
                }
            }
            else
            {
                base.DoModulus(divisor, result);
            }
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoRemainder(double divisor, MatrixMathNet<double> result)
        {
            if (result is SparseMatrix sparseResult)
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                SparseCompressedRowMatrixStorage<double> resultStorage = sparseResult._storage;
                for (int index = 0; index < resultStorage.Values.Length; index++)
                {
                    resultStorage.Values[index] %= divisor;
                }
            }
            else
            {
                base.DoRemainder(divisor, result);
            }
        }

        /// <summary>
        /// Evaluates whether this matrix is symmetric.
        /// </summary>
        public override bool IsSymmetric()
        {
            if (RowCount != ColumnCount)
            {
                return false;
            }

            int[] rowPointers = _storage.RowPointers;
            int[] columnIndices = _storage.ColumnIndices;
            double[] values = _storage.Values;

            for (int row = 0; row < RowCount; row++)
            {
                int start = rowPointers[row];
                int end = rowPointers[row + 1];

                if (start == end)
                {
                    continue;
                }

                for (int index = start; index < end; index++)
                {
                    int column = columnIndices[index];
                    if (!values[index].Equals(At(column, row)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator +(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator +(SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseMatrix)rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator -(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw DimensionsDontMatch<ArgumentException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator -(SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseMatrix)rightSide.Negate();
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator *(SparseMatrix leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator *(double leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseMatrix)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static SparseMatrix operator *(SparseMatrix leftSide, SparseMatrix rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide.ColumnCount != rightSide.RowCount)
            {
                throw DimensionsDontMatch<ArgumentException>(leftSide, rightSide);
            }

            return (SparseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a Vector.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(SparseMatrix leftSide, SparseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a Vector and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(SparseVector leftSide, SparseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseVector)rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseMatrix operator %(SparseMatrix leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseMatrix)leftSide.Remainder(rightSide);
        }

        public override string ToTypeString()
        {
            return FormattableString.Invariant($"SparseMatrix {RowCount}x{ColumnCount}-Double {NonZerosCount / (RowCount * (double)ColumnCount):P2} Filled");
        }
    }
}
