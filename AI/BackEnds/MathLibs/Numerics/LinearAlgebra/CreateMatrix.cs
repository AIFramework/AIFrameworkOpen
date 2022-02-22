// <copyright file="CreateMatrix.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
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

using AI.BackEnds.MathLibs.MathNet.Numerics.Distributions;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Storage;
using System;
using System.Collections.Generic;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra
{
    public static class CreateMatrix
    {
        /// <summary>
        /// Create a new matrix straight from an initialized matrix storage instance.
        /// If you have an instance of a discrete storage type instead, use their direct methods instead.
        /// </summary>
        public static MatrixMathNet<T> WithStorage<T>(MatrixStorage<T> storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.OfStorage(storage);
        }

        /// <summary>
        /// Create a new matrix with the same kind of the provided example.
        /// </summary>
        public static MatrixMathNet<T> SameAs<T, TU>(MatrixMathNet<TU> example, int rows, int columns, bool fullyMutable = false)
            where T : struct, IEquatable<T>, IFormattable
            where TU : struct, IEquatable<TU>, IFormattable
        {
            return MatrixMathNet<T>.Build.SameAs(example, rows, columns, fullyMutable);
        }

        /// <summary>
        /// Create a new matrix with the same kind and dimensions of the provided example.
        /// </summary>
        public static MatrixMathNet<T> SameAs<T, TU>(MatrixMathNet<TU> example)
            where T : struct, IEquatable<T>, IFormattable
            where TU : struct, IEquatable<TU>, IFormattable
        {
            return MatrixMathNet<T>.Build.SameAs(example);
        }

        /// <summary>
        /// Create a new matrix with the same kind of the provided example.
        /// </summary>
        public static MatrixMathNet<T> SameAs<T>(VectorMathNet<T> example, int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SameAs(example, rows, columns);
        }

        /// <summary>
        /// Create a new matrix with a type that can represent and is closest to both provided samples.
        /// </summary>
        public static MatrixMathNet<T> SameAs<T>(MatrixMathNet<T> example, MatrixMathNet<T> otherExample, int rows, int columns, bool fullyMutable = false)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SameAs(example, otherExample, rows, columns, fullyMutable);
        }

        /// <summary>
        /// Create a new matrix with a type that can represent and is closest to both provided samples and the dimensions of example.
        /// </summary>
        public static MatrixMathNet<T> SameAs<T>(MatrixMathNet<T> example, MatrixMathNet<T> otherExample)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SameAs(example, otherExample);
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the provided random distribution.
        /// </summary>
        public static MatrixMathNet<T> Random<T>(int rows, int columns, IContinuousDistribution distribution)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Random(rows, columns, distribution);
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the standard distribution with a system random source.
        /// </summary>
        public static MatrixMathNet<T> Random<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Random(rows, columns);
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the standard distribution with a system random source.
        /// </summary>
        public static MatrixMathNet<T> Random<T>(int rows, int columns, int seed)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Random(rows, columns, seed);
        }

        /// <summary>
        /// Create a new positive definite dense matrix where each value is the product
        /// of two samples from the provided random distribution.
        /// </summary>
        public static MatrixMathNet<T> RandomPositiveDefinite<T>(int order, IContinuousDistribution distribution)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.RandomPositiveDefinite(order, distribution);
        }

        /// <summary>
        /// Create a new positive definite dense matrix where each value is the product
        /// of two samples from the standard distribution.
        /// </summary>
        public static MatrixMathNet<T> RandomPositiveDefinite<T>(int order)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.RandomPositiveDefinite(order);
        }

        /// <summary>
        /// Create a new positive definite dense matrix where each value is the product
        /// of two samples from the provided random distribution.
        /// </summary>
        public static MatrixMathNet<T> RandomPositiveDefinite<T>(int order, int seed)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.RandomPositiveDefinite(order, seed);
        }

        /// <summary>
        /// Create a new dense matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public static MatrixMathNet<T> Dense<T>(DenseColumnMajorMatrixStorage<T> storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Dense(storage);
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        public static MatrixMathNet<T> Dense<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Dense(rows, columns);
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public static MatrixMathNet<T> Dense<T>(int rows, int columns, T[] storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Dense(rows, columns, storage);
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> Dense<T>(int rows, int columns, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Dense(rows, columns, value);
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value using the provided init function.
        /// </summary>
        public static MatrixMathNet<T> Dense<T>(int rows, int columns, Func<int, int, T> init)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Dense(rows, columns, init);
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> DenseDiagonal<T>(int rows, int columns, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseDiagonal(rows, columns, value);
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> DenseDiagonal<T>(int order, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseDiagonal(order, value);
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static MatrixMathNet<T> DenseDiagonal<T>(int rows, int columns, Func<int, T> init)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseDiagonal(rows, columns, init);
        }

        /// <summary>
        /// Create a new diagonal dense identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> DenseIdentity<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseIdentity(rows, columns);
        }

        /// <summary>
        /// Create a new diagonal dense identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> DenseIdentity<T>(int order)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseIdentity(order);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfMatrix<T>(MatrixMathNet<T> matrix)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfMatrix(matrix);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfArray<T>(T[,] array)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfArray(array);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfIndexed<T>(int rows, int columns, IEnumerable<Tuple<int, int, T>> enumerable)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfIndexed(rows, columns, enumerable);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfIndexed<T>(int rows, int columns, IEnumerable<(int, int, T)> enumerable)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfIndexed(rows, columns, enumerable);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumnMajor<T>(int rows, int columns, IEnumerable<T> columnMajor)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumnMajor(rows, columns, columnMajor);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumns<T>(IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumns(data);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumns<T>(int rows, int columns, IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumns(rows, columns, data);
        }

        /// <summary>
        /// Create a new dense matrix of T as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumnArrays<T>(params T[][] columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumnArrays(columns);
        }

        /// <summary>
        /// Create a new dense matrix of T as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumnArrays<T>(IEnumerable<T[]> columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumnArrays(columns);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumnVectors<T>(params VectorMathNet<T>[] columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumnVectors(columns);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfColumnVectors<T>(IEnumerable<VectorMathNet<T>> columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfColumnVectors(columns);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRows<T>(IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRows(data);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRows<T>(int rows, int columns, IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRows(rows, columns, data);
        }

        /// <summary>
        /// Create a new dense matrix of T as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRowArrays<T>(params T[][] rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRowArrays(rows);
        }

        /// <summary>
        /// Create a new dense matrix of T as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRowArrays<T>(IEnumerable<T[]> rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRowArrays(rows);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRowVectors<T>(params VectorMathNet<T>[] rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRowVectors(rows);
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfRowVectors<T>(IEnumerable<VectorMathNet<T>> rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfRowVectors(rows);
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfDiagonalVector<T>(VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfDiagonalVector(diagonal);
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfDiagonalVector<T>(int rows, int columns, VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfDiagonalVector(rows, columns, diagonal);
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfDiagonalArray<T>(T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfDiagonalArray(diagonal);
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DenseOfDiagonalArray<T>(int rows, int columns, T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfDiagonalArray(rows, columns, diagonal);
        }

        /// <summary>
        /// Create a new dense matrix from a 2D array of existing matrices.
        /// The matrices in the array are not required to be dense already.
        /// If the matrices do not align properly, they are placed on the top left
        /// corner of their cell with the remaining fields left zero.
        /// </summary>
        public static MatrixMathNet<T> DenseOfMatrixArray<T>(MatrixMathNet<T>[,] matrices)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DenseOfMatrixArray(matrices);
        }

        /// <summary>
        /// Create a new sparse matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public static MatrixMathNet<T> Sparse<T>(SparseCompressedRowMatrixStorage<T> storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Sparse(storage);
        }

        /// <summary>
        /// Create a sparse matrix of T with the given number of rows and columns.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        public static MatrixMathNet<T> Sparse<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Sparse(rows, columns);
        }

        /// <summary>
        /// Create a new sparse matrix and initialize each value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> Sparse<T>(int rows, int columns, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Sparse(rows, columns, value);
        }

        /// <summary>
        /// Create a new sparse matrix and initialize each value using the provided init function.
        /// </summary>
        public static MatrixMathNet<T> Sparse<T>(int rows, int columns, Func<int, int, T> init)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Sparse(rows, columns, init);
        }

        /// <summary>
        /// Create a new diagonal sparse matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> SparseDiagonal<T>(int rows, int columns, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseDiagonal(rows, columns, value);
        }

        /// <summary>
        /// Create a new diagonal sparse matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> SparseDiagonal<T>(int order, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseDiagonal(order, value);
        }

        /// <summary>
        /// Create a new diagonal sparse matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static MatrixMathNet<T> SparseDiagonal<T>(int rows, int columns, Func<int, T> init)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseDiagonal(rows, columns, init);
        }

        /// <summary>
        /// Create a new diagonal dense identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> SparseIdentity<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseIdentity(rows, columns);
        }

        /// <summary>
        /// Create a new diagonal dense identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> SparseIdentity<T>(int order)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseIdentity(order);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfMatrix<T>(MatrixMathNet<T> matrix)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfMatrix(matrix);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfArray<T>(T[,] array)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfArray(array);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfIndexed<T>(int rows, int columns, IEnumerable<Tuple<int, int, T>> enumerable)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfIndexed(rows, columns, enumerable);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfIndexed<T>(int rows, int columns, IEnumerable<(int, int, T)> enumerable)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfIndexed(rows, columns, enumerable);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in row-major order (row by row).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public static MatrixMathNet<T> SparseOfRowMajor<T>(int rows, int columns, IEnumerable<T> rowMajor)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRowMajor(rows, columns, rowMajor);
        }

        /// <summary>
        /// Create a new sparse matrix with the given number of rows and columns as a copy of the given array.
        /// The array is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public static MatrixMathNet<T> SparseOfColumnMajor<T>(int rows, int columns, IList<T> columnMajor)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumnMajor(rows, columns, columnMajor);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumns<T>(IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumns(data);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumns<T>(int rows, int columns, IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumns(rows, columns, data);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumnArrays<T>(params T[][] columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumnArrays(columns);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumnArrays<T>(IEnumerable<T[]> columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumnArrays(columns);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumnVectors<T>(params VectorMathNet<T>[] columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumnVectors(columns);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfColumnVectors<T>(IEnumerable<VectorMathNet<T>> columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfColumnVectors(columns);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRows<T>(IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRows(data);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRows<T>(int rows, int columns, IEnumerable<IEnumerable<T>> data)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRows(rows, columns, data);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRowArrays<T>(params T[][] rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRowArrays(rows);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRowArrays<T>(IEnumerable<T[]> rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRowArrays(rows);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRowVectors<T>(params VectorMathNet<T>[] rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRowVectors(rows);
        }

        /// <summary>
        /// Create a new sparse matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfRowVectors<T>(IEnumerable<VectorMathNet<T>> rows)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfRowVectors(rows);
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfDiagonalVector<T>(VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfDiagonalVector(diagonal);
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfDiagonalVector<T>(int rows, int columns, VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfDiagonalVector(rows, columns, diagonal);
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfDiagonalArray<T>(T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfDiagonalArray(diagonal);
        }

        /// <summary>
        /// Create a new sparse matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> SparseOfDiagonalArray<T>(int rows, int columns, T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfDiagonalArray(rows, columns, diagonal);
        }

        /// <summary>
        /// Create a new sparse matrix from a 2D array of existing matrices.
        /// The matrices in the array are not required to be sparse already.
        /// If the matrices do not align properly, they are placed on the top left
        /// corner of their cell with the remaining fields left zero.
        /// </summary>
        public static MatrixMathNet<T> SparseOfMatrixArray<T>(MatrixMathNet<T>[,] matrices)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseOfMatrixArray(matrices);
        }

        /// <summary>
        /// Create a new sparse matrix from a coordinate format.
        /// This new matrix will be independent from the given arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="valueCount">The number of stored values including explicit zeros.</param>
        /// <param name="rowIndices">The row index array of the coordinate format.</param>
        /// <param name="columnIndices">The column index array of the coordinate format.</param>
        /// <param name="values">The data array of the coordinate format.</param>
        /// <returns>The sparse matrix from the coordinate format.</returns>
        /// <remarks>Duplicate entries will be summed together and explicit zeros will be not intentionally removed.</remarks>
        public static MatrixMathNet<T> SparseFromCoordinateFormat<T>(int rows, int columns, int valueCount, int[] rowIndices, int[] columnIndices, T[] values)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseFromCoordinateFormat(rows, columns, valueCount, rowIndices, columnIndices, values);
        }

        /// <summary>
        /// Create a new sparse matrix from a compressed sparse row format.
        /// This new matrix will be independent from the given arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="valueCount">The number of stored values including explicit zeros.</param>
        /// <param name="rowPointers">The row pointer array of the compressed sparse row format.</param>
        /// <param name="columnIndices">The column index array of the compressed sparse row format.</param>
        /// <param name="values">The data array of the compressed sparse row format.</param>
        /// <returns>The sparse matrix from the compressed sparse row format.</returns>
        /// <remarks>Duplicate entries will be summed together and explicit zeros will be not intentionally removed.</remarks>
        public static MatrixMathNet<T> SparseFromCompressedSparseRowFormat<T>(int rows, int columns, int valueCount, int[] rowPointers, int[] columnIndices, T[] values)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseFromCompressedSparseRowFormat(rows, columns, valueCount, rowPointers, columnIndices, values);
        }

        /// <summary>
        /// Create a new sparse matrix from a compressed sparse column format.
        /// This new matrix will be independent from the given arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="columns">The number of columns.</param>
        /// <param name="valueCount">The number of stored values including explicit zeros.</param>
        /// <param name="rowIndices">The row index array of the compressed sparse column format.</param>
        /// <param name="columnPointers">The column pointer array of the compressed sparse column format.</param>
        /// <param name="values">The data array of the compressed sparse column format.</param>
        /// <returns>The sparse matrix from the compressed sparse column format.</returns>
        /// <remarks>Duplicate entries will be summed together and explicit zeros will be not intentionally removed.</remarks>
        public static MatrixMathNet<T> SparseFromCompressedSparseColumnFormat<T>(int rows, int columns, int valueCount, int[] rowIndices, int[] columnPointers, T[] values)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.SparseFromCompressedSparseColumnFormat(rows, columns, valueCount, rowIndices, columnPointers, values);
        }

        /// <summary>
        /// Create a new diagonal matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(DiagonalMatrixStorage<T> storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(storage);
        }

        /// <summary>
        /// Create a new diagonal matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(rows, columns);
        }

        /// <summary>
        /// Create a new diagonal matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to represent the diagonal values and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(int rows, int columns, T[] storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(rows, columns, storage);
        }

        /// <summary>
        /// Create a new square diagonal matrix directly binding to a raw array.
        /// The array is assumed to represent the diagonal values and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(T[] storage)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(storage);
        }

        /// <summary>
        /// Create a new diagonal matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(int rows, int columns, T value)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(rows, columns, value);
        }

        /// <summary>
        /// Create a new diagonal matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static MatrixMathNet<T> Diagonal<T>(int rows, int columns, Func<int, T> init)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.Diagonal(rows, columns, init);
        }

        /// <summary>
        /// Create a new diagonal identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> DiagonalIdentity<T>(int rows, int columns)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalIdentity(rows, columns);
        }

        /// <summary>
        /// Create a new diagonal identity matrix with a one-diagonal.
        /// </summary>
        public static MatrixMathNet<T> DiagonalIdentity<T>(int order)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalIdentity(order);
        }


        /// <summary>
        /// Create a new diagonal matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DiagonalOfDiagonalVector<T>(VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalOfDiagonalVector(diagonal);
        }

        /// <summary>
        /// Create a new diagonal matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DiagonalOfDiagonalVector<T>(int rows, int columns, VectorMathNet<T> diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalOfDiagonalVector(rows, columns, diagonal);
        }

        /// <summary>
        /// Create a new diagonal matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DiagonalOfDiagonalArray<T>(T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalOfDiagonalArray(diagonal);
        }

        /// <summary>
        /// Create a new diagonal matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static MatrixMathNet<T> DiagonalOfDiagonalArray<T>(int rows, int columns, T[] diagonal)
            where T : struct, IEquatable<T>, IFormattable
        {
            return MatrixMathNet<T>.Build.DiagonalOfDiagonalArray(rows, columns, diagonal);
        }
    }
}
