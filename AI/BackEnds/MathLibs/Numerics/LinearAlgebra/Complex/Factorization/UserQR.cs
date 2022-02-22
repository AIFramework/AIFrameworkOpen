// <copyright file="UserQR.cs" company="Math.NET">
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

using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Factorization;
using AI.BackEnds.MathLibs.MathNet.Numerics.Threading;
using System;
using System.Linq;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Complex.Factorization
{
    using Complex = System.Numerics.Complex;

    /// <summary>
    /// <para>A class which encapsulates the functionality of the QR decomposition.</para>
    /// <para>Any real square matrix A may be decomposed as A = QR where Q is an orthogonal matrix
    /// (its columns are orthogonal unit vectors meaning QTQ = I) and R is an upper triangular matrix
    /// (also called right triangular matrix).</para>
    /// </summary>
    /// <remarks>
    /// The computation of the QR decomposition is done at construction time by Householder transformation.
    /// </remarks>
    internal sealed class UserQR : QR
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserQR"/> class. This object will compute the
        /// QR factorization when the constructor is called and cache it's factorization.
        /// </summary>
        /// <param name="matrix">The matrix to factor.</param>
        /// <param name="method">The QR factorization method to use.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
        public static UserQR Create(MatrixMathNet<Complex> matrix, QRMethod method = QRMethod.Full)
        {
            if (matrix.RowCount < matrix.ColumnCount)
            {
                throw MatrixMathNet.DimensionsDontMatch<ArgumentException>(matrix);
            }

            MatrixMathNet<Complex> q;
            MatrixMathNet<Complex> r;

            int minmn = Math.Min(matrix.RowCount, matrix.ColumnCount);
            Complex[][] u = new Complex[minmn][];

            if (method == QRMethod.Full)
            {
                r = matrix.Clone();
                q = MatrixMathNet<Complex>.Build.SameAs(matrix, matrix.RowCount, matrix.RowCount, fullyMutable: true);

                for (int i = 0; i < matrix.RowCount; i++)
                {
                    q.At(i, i, 1.0f);
                }

                for (int i = 0; i < minmn; i++)
                {
                    u[i] = GenerateColumn(r, i, i);
                    ComputeQR(u[i], r, i, matrix.RowCount, i + 1, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
                }

                for (int i = minmn - 1; i >= 0; i--)
                {
                    ComputeQR(u[i], q, i, matrix.RowCount, i, matrix.RowCount, Control.MaxDegreeOfParallelism);
                }
            }
            else
            {
                q = matrix.Clone();

                for (int i = 0; i < minmn; i++)
                {
                    u[i] = GenerateColumn(q, i, i);
                    ComputeQR(u[i], q, i, matrix.RowCount, i + 1, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
                }

                r = q.SubMatrix(0, matrix.ColumnCount, 0, matrix.ColumnCount);
                q.Clear();

                for (int i = 0; i < matrix.ColumnCount; i++)
                {
                    q.At(i, i, 1.0f);
                }

                for (int i = minmn - 1; i >= 0; i--)
                {
                    ComputeQR(u[i], q, i, matrix.RowCount, i, matrix.ColumnCount, Control.MaxDegreeOfParallelism);
                }
            }

            return new UserQR(q, r, method);
        }

        private UserQR(MatrixMathNet<Complex> q, MatrixMathNet<Complex> rFull, QRMethod method)
            : base(q, rFull, method)
        {
        }

        /// <summary>
        /// Generate column from initial matrix to work array
        /// </summary>
        /// <param name="a">Initial matrix</param>
        /// <param name="row">The first row</param>
        /// <param name="column">Column index</param>
        /// <returns>Generated vector</returns>
        private static Complex[] GenerateColumn(MatrixMathNet<Complex> a, int row, int column)
        {
            int ru = a.RowCount - row;
            Complex[] u = new Complex[ru];

            for (int i = row; i < a.RowCount; i++)
            {
                u[i - row] = a.At(i, column);
                a.At(i, column, 0.0);
            }

            Complex norm = u.Aggregate(Complex.Zero, (current, t) => current + (t.Magnitude * t.Magnitude));
            norm = norm.SquareRoot();

            if (row == a.RowCount - 1 || norm.Magnitude == 0)
            {
                a.At(row, column, -u[0]);
                u[0] = Constants.Sqrt2;
                return u;
            }

            if (u[0].Magnitude != 0.0)
            {
                norm = norm.Magnitude * (u[0] / u[0].Magnitude);
            }

            a.At(row, column, -norm);

            for (int i = 0; i < ru; i++)
            {
                u[i] /= norm;
            }

            u[0] += 1.0;

            Complex s = (1.0 / u[0]).SquareRoot();
            for (int i = 0; i < ru; i++)
            {
                u[i] = u[i].Conjugate() * s;
            }

            return u;
        }

        /// <summary>
        /// Perform calculation of Q or R
        /// </summary>
        /// <param name="u">Work array</param>
        /// <param name="a">Q or R matrices</param>
        /// <param name="rowStart">The first row</param>
        /// <param name="rowDim">The last row</param>
        /// <param name="columnStart">The first column</param>
        /// <param name="columnDim">The last column</param>
        /// <param name="availableCores">Number of available CPUs</param>
        private static void ComputeQR(Complex[] u, MatrixMathNet<Complex> a, int rowStart, int rowDim, int columnStart, int columnDim, int availableCores)
        {
            if (rowDim < rowStart || columnDim < columnStart)
            {
                return;
            }

            int tmpColCount = columnDim - columnStart;

            if ((availableCores > 1) && (tmpColCount > 200))
            {
                int tmpSplit = columnStart + (tmpColCount / 2);
                int tmpCores = availableCores / 2;

                CommonParallel.Invoke(
                    () => ComputeQR(u, a, rowStart, rowDim, columnStart, tmpSplit, tmpCores),
                    () => ComputeQR(u, a, rowStart, rowDim, tmpSplit, columnDim, tmpCores));
            }
            else
            {
                for (int j = columnStart; j < columnDim; j++)
                {
                    Complex scale = Complex.Zero;
                    for (int i = rowStart; i < rowDim; i++)
                    {
                        scale += u[i - rowStart] * a.At(i, j);
                    }

                    for (int i = rowStart; i < rowDim; i++)
                    {
                        a.At(i, j, a.At(i, j) - (u[i - rowStart].Conjugate() * scale));
                    }
                }
            }
        }

        /// <summary>
        /// Solves a system of linear equations, <b>AX = B</b>, with A QR factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="MatrixMathNet{T}"/>, <b>B</b>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <b>X</b>.</param>
        public override void Solve(MatrixMathNet<Complex> input, MatrixMathNet<Complex> result)
        {
            // The solution X should have the same number of columns as B
            if (input.ColumnCount != result.ColumnCount)
            {
                throw new ArgumentException("Matrix column dimensions must agree.");
            }

            // The dimension compatibility conditions for X = A\B require the two matrices A and B to have the same number of rows
            if (FullR.RowCount != input.RowCount)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }

            // The solution X row dimension is equal to the column dimension of A
            if (FullR.ColumnCount != result.RowCount)
            {
                throw new ArgumentException("Matrix column dimensions must agree.");
            }

            MatrixMathNet<Complex> inputCopy = input.Clone();

            // Compute Y = transpose(Q)*B
            Complex[] column = new Complex[FullR.RowCount];
            for (int j = 0; j < input.ColumnCount; j++)
            {
                for (int k = 0; k < FullR.RowCount; k++)
                {
                    column[k] = inputCopy.At(k, j);
                }

                for (int i = 0; i < FullR.RowCount; i++)
                {
                    Complex s = Complex.Zero;
                    for (int k = 0; k < FullR.RowCount; k++)
                    {
                        s += Q.At(k, i).Conjugate() * column[k];
                    }

                    inputCopy.At(i, j, s);
                }
            }

            // Solve R*X = Y;
            for (int k = FullR.ColumnCount - 1; k >= 0; k--)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    inputCopy.At(k, j, inputCopy.At(k, j) / FullR.At(k, k));
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < input.ColumnCount; j++)
                    {
                        inputCopy.At(i, j, inputCopy.At(i, j) - (inputCopy.At(k, j) * FullR.At(i, k)));
                    }
                }
            }

            for (int i = 0; i < FullR.ColumnCount; i++)
            {
                for (int j = 0; j < inputCopy.ColumnCount; j++)
                {
                    result.At(i, j, inputCopy.At(i, j));
                }
            }
        }

        /// <summary>
        /// Solves a system of linear equations, <b>Ax = b</b>, with A QR factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <b>b</b>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <b>x</b>.</param>
        public override void Solve(VectorMathNet<Complex> input, VectorMathNet<Complex> result)
        {
            // Ax=b where A is an m x n matrix
            // Check that b is a column vector with m entries
            if (FullR.RowCount != input.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.");
            }

            // Check that x is a column vector with n entries
            if (FullR.ColumnCount != result.Count)
            {
                throw MatrixMathNet.DimensionsDontMatch<ArgumentException>(FullR, result);
            }

            VectorMathNet<Complex> inputCopy = input.Clone();

            // Compute Y = transpose(Q)*B
            Complex[] column = new Complex[FullR.RowCount];
            for (int k = 0; k < FullR.RowCount; k++)
            {
                column[k] = inputCopy[k];
            }

            for (int i = 0; i < FullR.RowCount; i++)
            {
                Complex s = Complex.Zero;
                for (int k = 0; k < FullR.RowCount; k++)
                {
                    s += Q.At(k, i).Conjugate() * column[k];
                }

                inputCopy[i] = s;
            }

            // Solve R*X = Y;
            for (int k = FullR.ColumnCount - 1; k >= 0; k--)
            {
                inputCopy[k] /= FullR.At(k, k);
                for (int i = 0; i < k; i++)
                {
                    inputCopy[i] -= inputCopy[k] * FullR.At(i, k);
                }
            }

            for (int i = 0; i < FullR.ColumnCount; i++)
            {
                result[i] = inputCopy[i];
            }
        }
    }
}
