// <copyright file="LU.cs" company="Math.NET">
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

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Factorization
{
    /// <summary>
    /// <para>A class which encapsulates the functionality of an LU factorization.</para>
    /// <para>For a matrix A, the LU factorization is a pair of lower triangular matrix L and
    /// upper triangular matrix U so that A = L*U.</para>
    /// <para>In the Math.Net implementation we also store a set of pivot elements for increased
    /// numerical stability. The pivot elements encode a permutation matrix P such that P*A = L*U.</para>
    /// </summary>
    /// <remarks>
    /// The computation of the LU factorization is done at construction time.
    /// </remarks>
    /// <typeparam name="T">Supported data types are double, single, <see cref="Complex"/>, and <see cref="Complex32"/>.</typeparam>
    public abstract class LU<T> : ISolver<T>
        where T : struct, IEquatable<T>, IFormattable
    {
        private static readonly T One = BuilderInstance<T>.Matrix.One;
        private readonly Lazy<MatrixMathNet<T>> _lazyL;
        private readonly Lazy<MatrixMathNet<T>> _lazyU;
        private readonly Lazy<Permutation> _lazyP;

        protected readonly MatrixMathNet<T> Factors;
        protected readonly int[] Pivots;

        protected LU(MatrixMathNet<T> factors, int[] pivots)
        {
            Factors = factors;
            Pivots = pivots;

            _lazyL = new Lazy<MatrixMathNet<T>>(ComputeL);
            _lazyU = new Lazy<MatrixMathNet<T>>(Factors.UpperTriangle);
            _lazyP = new Lazy<Permutation>(() => Permutation.FromInversions(Pivots));
        }

        private MatrixMathNet<T> ComputeL()
        {
            MatrixMathNet<T> result = Factors.LowerTriangle();
            for (int i = 0; i < result.RowCount; i++)
            {
                result.At(i, i, One);
            }
            return result;
        }

        /// <summary>
        /// Gets the lower triangular factor.
        /// </summary>
        public MatrixMathNet<T> L => _lazyL.Value;

        /// <summary>
        /// Gets the upper triangular factor.
        /// </summary>
        public MatrixMathNet<T> U => _lazyU.Value;

        /// <summary>
        /// Gets the permutation applied to LU factorization.
        /// </summary>
        public Permutation P => _lazyP.Value;

        /// <summary>
        /// Gets the determinant of the matrix for which the LU factorization was computed.
        /// </summary>
        public abstract T Determinant { get; }

        /// <summary>
        /// Solves a system of linear equations, <b>AX = B</b>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="MatrixMathNet{T}"/>, <b>B</b>.</param>
        /// <returns>The left hand side <see cref="MatrixMathNet{T}"/>, <b>X</b>.</returns>
        public virtual MatrixMathNet<T> Solve(MatrixMathNet<T> input)
        {
            MatrixMathNet<T> x = MatrixMathNet<T>.Build.SameAs(input, input.RowCount, input.ColumnCount, fullyMutable: true);
            Solve(input, x);
            return x;
        }

        /// <summary>
        /// Solves a system of linear equations, <b>AX = B</b>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="MatrixMathNet{T}"/>, <b>B</b>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <b>X</b>.</param>
        public abstract void Solve(MatrixMathNet<T> input, MatrixMathNet<T> result);

        /// <summary>
        /// Solves a system of linear equations, <b>Ax = b</b>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <b>b</b>.</param>
        /// <returns>The left hand side <see cref="VectorMathNet{T}"/>, <b>x</b>.</returns>
        public virtual VectorMathNet<T> Solve(VectorMathNet<T> input)
        {
            VectorMathNet<T> x = VectorMathNet<T>.Build.SameAs(input, input.Count);
            Solve(input, x);
            return x;
        }

        /// <summary>
        /// Solves a system of linear equations, <b>Ax = b</b>, with A LU factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <b>b</b>.</param>
        /// <param name="result">The left hand side <see cref="MatrixMathNet{T}"/>, <b>x</b>.</param>
        public abstract void Solve(VectorMathNet<T> input, VectorMathNet<T> result);

        /// <summary>
        /// Returns the inverse of this matrix. The inverse is calculated using LU decomposition.
        /// </summary>
        /// <returns>The inverse of this matrix.</returns>
        public abstract MatrixMathNet<T> Inverse();
    }
}
