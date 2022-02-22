// <copyright file="UserLU.cs" company="Math.NET">
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

using System;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Complex.Factorization
{
    using Complex = System.Numerics.Complex;

    /// <summary>
    /// <para>A class which encapsulates the functionality of an LU factorization.</para>
    /// <para>For a matrix A, the LU factorization is a pair of lower triangular matrix L and
    /// upper triangular matrix U so that A = L*U.</para>
    /// </summary>
    /// <remarks>
    /// The computation of the LU factorization is done at construction time.
    /// </remarks>
    internal sealed class UserLU : LU
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserLU"/> class. This object will compute the
        /// LU factorization when the constructor is called and cache it's factorization.
        /// </summary>
        /// <param name="matrix">The matrix to factor.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="matrix"/> is not a square matrix.</exception>
        public static UserLU Create(MatrixMathNet<Complex> matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (matrix.RowCount != matrix.ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.");
            }

            // Create an array for the pivot indices.
            int order = matrix.RowCount;
            MatrixMathNet<Complex> factors = matrix.Clone();
            int[] pivots = new int[order];

            // Initialize the pivot matrix to the identity permutation.
            for (int i = 0; i < order; i++)
            {
                pivots[i] = i;
            }

            Complex[] vectorLUcolj = new Complex[order];
            for (int j = 0; j < order; j++)
            {
                // Make a copy of the j-th column to localize references.
                for (int i = 0; i < order; i++)
                {
                    vectorLUcolj[i] = factors.At(i, j);
                }

                // Apply previous transformations.
                for (int i = 0; i < order; i++)
                {
                    int kmax = Math.Min(i, j);
                    Complex s = Complex.Zero;
                    for (int k = 0; k < kmax; k++)
                    {
                        s += factors.At(i, k) * vectorLUcolj[k];
                    }

                    vectorLUcolj[i] -= s;
                    factors.At(i, j, vectorLUcolj[i]);
                }

                // Find pivot and exchange if necessary.
                int p = j;
                for (int i = j + 1; i < order; i++)
                {
                    if (vectorLUcolj[i].Magnitude > vectorLUcolj[p].Magnitude)
                    {
                        p = i;
                    }
                }

                if (p != j)
                {
                    for (int k = 0; k < order; k++)
                    {
                        Complex temp = factors.At(p, k);
                        factors.At(p, k, factors.At(j, k));
                        factors.At(j, k, temp);
                    }

                    pivots[j] = p;
                }

                // Compute multipliers.
                if (j < order & factors.At(j, j) != 0.0)
                {
                    for (int i = j + 1; i < order; i++)
                    {
                        factors.At(i, j, (factors.At(i, j) / factors.At(j, j)));
                    }
                }
            }

            return new UserLU(factors, pivots);
        }

        private UserLU(MatrixMathNet<Complex> factors, int[] pivots)
            : base(factors, pivots)
        {
        }

        /// <summary>
        /// Solves a system of linear equations, <c>AX = B</c>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="MatrixMathNet{T}"/>, <c>B</c>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <c>X</c>.</param>
        public override void Solve(MatrixMathNet<Complex> input, MatrixMathNet<Complex> result)
        {
            // Check for proper arguments.
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            // Check for proper dimensions.
            if (result.RowCount != input.RowCount)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }

            if (result.ColumnCount != input.ColumnCount)
            {
                throw new ArgumentException("Matrix column dimensions must agree.");
            }

            if (input.RowCount != Factors.RowCount)
            {
                throw MatrixMathNet.DimensionsDontMatch<ArgumentException>(input, Factors);
            }

            // Copy the contents of input to result.
            input.CopyTo(result);
            for (int i = 0; i < Pivots.Length; i++)
            {
                if (Pivots[i] == i)
                {
                    continue;
                }

                int p = Pivots[i];
                for (int j = 0; j < result.ColumnCount; j++)
                {
                    Complex temp = result.At(p, j);
                    result.At(p, j, result.At(i, j));
                    result.At(i, j, temp);
                }
            }

            int order = Factors.RowCount;

            // Solve L*Y = P*B
            for (int k = 0; k < order; k++)
            {
                for (int i = k + 1; i < order; i++)
                {
                    for (int j = 0; j < result.ColumnCount; j++)
                    {
                        Complex temp = result.At(k, j) * Factors.At(i, k);
                        result.At(i, j, result.At(i, j) - temp);
                    }
                }
            }

            // Solve U*X = Y;
            for (int k = order - 1; k >= 0; k--)
            {
                for (int j = 0; j < result.ColumnCount; j++)
                {
                    result.At(k, j, (result.At(k, j) / Factors.At(k, k)));
                }

                for (int i = 0; i < k; i++)
                {
                    for (int j = 0; j < result.ColumnCount; j++)
                    {
                        Complex temp = result.At(k, j) * Factors.At(i, k);
                        result.At(i, j, result.At(i, j) - temp);
                    }
                }
            }
        }

        /// <summary>
        /// Solves a system of linear equations, <c>Ax = b</c>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <c>b</c>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <c>x</c>.</param>
        public override void Solve(VectorMathNet<Complex> input, VectorMathNet<Complex> result)
        {
            // Check for proper arguments.
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            // Check for proper dimensions.
            if (input.Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.");
            }

            if (input.Count != Factors.RowCount)
            {
                throw MatrixMathNet.DimensionsDontMatch<ArgumentException>(input, Factors);
            }

            // Copy the contents of input to result.
            input.CopyTo(result);
            for (int i = 0; i < Pivots.Length; i++)
            {
                if (Pivots[i] == i)
                {
                    continue;
                }

                int p = Pivots[i];
                Complex temp = result[p];
                result[p] = result[i];
                result[i] = temp;
            }

            int order = Factors.RowCount;

            // Solve L*Y = P*B
            for (int k = 0; k < order; k++)
            {
                for (int i = k + 1; i < order; i++)
                {
                    result[i] -= result[k] * Factors.At(i, k);
                }
            }

            // Solve U*X = Y;
            for (int k = order - 1; k >= 0; k--)
            {
                result[k] /= Factors.At(k, k);
                for (int i = 0; i < k; i++)
                {
                    result[i] -= result[k] * Factors.At(i, k);
                }
            }
        }

        /// <summary>
        /// Returns the inverse of this matrix. The inverse is calculated using LU decomposition.
        /// </summary>
        /// <returns>The inverse of this matrix.</returns>
        public override MatrixMathNet<Complex> Inverse()
        {
            int order = Factors.RowCount;
            MatrixMathNet<Complex> inverse = MatrixMathNet<Complex>.Build.SameAs(Factors, order, order);
            for (int i = 0; i < order; i++)
            {
                inverse.At(i, i, 1.0);
            }

            return Solve(inverse);
        }
    }
}
