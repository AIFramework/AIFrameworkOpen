// <copyright file="Matrix.cs" company="Math.NET">
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

using System;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Complex32.Factorization;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Factorization;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Storage;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Complex32
{
    using Numerics;

    /// <summary>
    /// <c>Complex32</c> version of the <see cref="MatrixMathNet{T}"/> class.
    /// </summary>
    [Serializable]
    public abstract class MatrixMathNet : MatrixMathNet<Complex32>
    {
        /// <summary>
        /// Initializes a new instance of the Matrix class.
        /// </summary>
        protected MatrixMathNet(MatrixStorage<Complex32> storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Set all values whose absolute value is smaller than the threshold to zero.
        /// </summary>
        public override void CoerceZero(double threshold)
        {
            MapInplace(x => x.Magnitude < threshold ? Complex32.Zero : x, Zeros.AllowSkip);
        }

        /// <summary>
        /// Returns the conjugate transpose of this matrix.
        /// </summary>
        /// <returns>The conjugate transpose of this matrix.</returns>
        public sealed override MatrixMathNet<Complex32> ConjugateTranspose()
        {
            var ret = Transpose();
            ret.MapInplace(c => c.Conjugate(), Zeros.AllowSkip);
            return ret;
        }

        /// <summary>
        /// Puts the conjugate transpose of this matrix into the result matrix.
        /// </summary>
        public sealed override void ConjugateTranspose(MatrixMathNet<Complex32> result)
        {
            Transpose(result);
            result.MapInplace(c => c.Conjugate(), Zeros.AllowSkip);
        }

        /// <summary>
        /// Complex conjugates each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the conjugation.</param>
        protected override void DoConjugate(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Conjugate, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Negate, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Add a scalar to each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        protected override void DoAdd(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            Map(x => x + scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            Map2(Complex32.Add, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            Map(x => x - scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract to this matrix.</param>
        /// <param name="result">The matrix to store the result of subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            Map2(Complex32.Subtract, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            Map(x => x*scalar, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(VectorMathNet<Complex32> rightSide, VectorMathNet<Complex32> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                var s = Complex32.Zero;
                for (var j = 0; j < ColumnCount; j++)
                {
                    s += At(i, j)*rightSide[j];
                }
                result[i] = s;
            }
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(Complex32 divisor, MatrixMathNet<Complex32> result)
        {
            Map(x => x/divisor, result, divisor.IsZero() ? Zeros.Include : Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides a scalar by each element of the matrix and stores the result in the result matrix.
        /// </summary>
        /// <param name="dividend">The scalar to divide by each element of the matrix.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivideByThis(Complex32 dividend, MatrixMathNet<Complex32> result)
        {
            Map(x => dividend/x, result, Zeros.Include);
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j != other.ColumnCount; j++)
                {
                    var s = Complex32.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            for (var j = 0; j < other.RowCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var s = Complex32.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(j, k);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with the conjugate transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeAndMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            for (var j = 0; j < other.RowCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var s = Complex32.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(j, k).Conjugate();
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            for (var j = 0; j < other.ColumnCount; j++)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    var s = Complex32.Zero;
                    for (var k = 0; k < RowCount; k++)
                    {
                        s += At(k, i)*other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeThisAndMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            for (var j = 0; j < other.ColumnCount; j++)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    var s = Complex32.Zero;
                    for (var k = 0; k < RowCount; k++)
                    {
                        s += At(k, i).Conjugate()*other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(VectorMathNet<Complex32> rightSide, VectorMathNet<Complex32> result)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = Complex32.Zero;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j)*rightSide[i];
                }
                result[j] = s;
            }
        }

        /// <summary>
        /// Multiplies the conjugate transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeThisAndMultiply(VectorMathNet<Complex32> rightSide, VectorMathNet<Complex32> result)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = Complex32.Zero;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j).Conjugate()*rightSide[i];
                }
                result[j] = s;
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            Map2(Complex32.Multiply, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(MatrixMathNet<Complex32> divisor, MatrixMathNet<Complex32> result)
        {
            Map2(Complex32.Divide, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The matrix to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Complex32 exponent, MatrixMathNet<Complex32> result)
        {
            Map(x => x.Power(exponent), result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(MatrixMathNet<Complex32> exponent, MatrixMathNet<Complex32> result)
        {
            Map2(Complex32.Pow, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise denominator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseModulus(MatrixMathNet<Complex32> divisor, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise denominator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseRemainder(MatrixMathNet<Complex32> divisor, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected sealed override void DoModulus(Complex32 divisor, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoModulusByThis(Complex32 dividend, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected sealed override void DoRemainder(Complex32 divisor, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoRemainderByThis(Complex32 dividend, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseExp(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Exp, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseLog(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Log, result, Zeros.Include);
        }

        protected override void DoPointwiseAbs(MatrixMathNet<Complex32> result)
        {
            Map(x => (Complex32)Complex32.Abs(x), result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAcos(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Acos, result, Zeros.Include);
        }
        protected override void DoPointwiseAsin(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Asin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Atan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan2(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCeiling(MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCos(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Cos, result, Zeros.Include);
        }
        protected override void DoPointwiseCosh(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Cosh, result, Zeros.Include);
        }
        protected override void DoPointwiseFloor(MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseLog10(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Log10, result, Zeros.Include);
        }
        protected override void DoPointwiseRound(MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSign(MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSin(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Sin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSinh(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Sinh, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSqrt(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Sqrt, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTan(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Tan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTanh(MatrixMathNet<Complex32> result)
        {
            Map(Complex32.Tanh, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Computes the Moore-Penrose Pseudo-Inverse of this matrix.
        /// </summary>
        public override MatrixMathNet<Complex32> PseudoInverse()
        {
            var svd = Svd(true);
            var w = svd.W;
            var s = svd.S;
            float tolerance = (float)(Math.Max(RowCount, ColumnCount) * svd.L2Norm * Precision.SinglePrecision);

            for (int i = 0; i < s.Count; i++)
            {
                s[i] = s[i].Magnitude < tolerance ? 0 : 1/s[i];
            }

            w.SetDiagonal(s);
            return (svd.U * (w * svd.VT)).ConjugateTranspose();
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override Complex32 Trace()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException("Matrix must be square.");
            }

            var sum = Complex32.Zero;
            for (var i = 0; i < RowCount; i++)
            {
                sum += At(i, i);
            }

            return sum;
        }

        protected override void DoPointwiseMinimum(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            float absolute = scalar.Magnitude;
            Map(x => Math.Min(absolute, x.Magnitude), result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(Complex32 scalar, MatrixMathNet<Complex32> result)
        {
            float absolute = scalar.Magnitude;
            Map(x => Math.Max(absolute, x.Magnitude), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            Map2((x, y) => Math.Min(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(MatrixMathNet<Complex32> other, MatrixMathNet<Complex32> result)
        {
            Map2((x, y) => Math.Max(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
        }

        /// <summary>Calculates the induced L1 norm of this matrix.</summary>
        /// <returns>The maximum absolute column sum of the matrix.</returns>
        public override double L1Norm()
        {
            var norm = 0d;
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = 0d;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j).Magnitude;
                }
                norm = Math.Max(norm, s);
            }
            return norm;
        }

        /// <summary>Calculates the induced infinity norm of this matrix.</summary>
        /// <returns>The maximum absolute row sum of the matrix.</returns>
        public override double InfinityNorm()
        {
            var norm = 0d;
            for (var i = 0; i < RowCount; i++)
            {
                var s = 0d;
                for (var j = 0; j < ColumnCount; j++)
                {
                    s += At(i, j).Magnitude;
                }
                norm = Math.Max(norm, s);
            }
            return norm;
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            var transpose = ConjugateTranspose();
            var aat = this*transpose;
            var norm = 0d;
            for (var i = 0; i < RowCount; i++)
            {
                norm += aat.At(i, i).Magnitude;
            }
            return Math.Sqrt(norm);
        }

        /// <summary>
        /// Calculates the p-norms of all row vectors.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public override VectorMathNet<double> RowNorms(double norm)
        {
            if (norm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(norm), "Value must be positive.");
            }

            var ret = new double[RowCount];
            if (norm == 2.0)
            {
                Storage.FoldByRowUnchecked(ret, (s, x) => s + x.MagnitudeSquared, (x, c) => Math.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByRowUnchecked(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            }
            else if (double.IsPositiveInfinity(norm))
            {
                Storage.FoldByRowUnchecked(ret, (s, x) => Math.Max(s, x.Magnitude), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else
            {
                double invnorm = 1.0/norm;
                Storage.FoldByRowUnchecked(ret, (s, x) => s + Math.Pow(x.Magnitude, norm), (x, c) => Math.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }
            return VectorMathNet<double>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the p-norms of all column vectors.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public override VectorMathNet<double> ColumnNorms(double norm)
        {
            if (norm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(norm), "Value must be positive.");
            }

            var ret = new double[ColumnCount];
            if (norm == 2.0)
            {
                Storage.FoldByColumnUnchecked(ret, (s, x) => s + x.MagnitudeSquared, (x, c) => Math.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByColumnUnchecked(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            }
            else if (double.IsPositiveInfinity(norm))
            {
                Storage.FoldByColumnUnchecked(ret, (s, x) => Math.Max(s, x.Magnitude), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else
            {
                double invnorm = 1.0/norm;
                Storage.FoldByColumnUnchecked(ret, (s, x) => s + Math.Pow(x.Magnitude, norm), (x, c) => Math.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }
            return VectorMathNet<double>.Build.Dense(ret);
        }

        /// <summary>
        /// Normalizes all row vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override MatrixMathNet<Complex32> NormalizeRows(double norm)
        {
            var norminv = ((DenseVectorStorage<double>)RowNorms(norm).Storage).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0d ? 1d : 1d/norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => ((float)norminv[i])*x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Normalizes all column vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override MatrixMathNet<Complex32> NormalizeColumns(double norm)
        {
            var norminv = ((DenseVectorStorage<double>)ColumnNorms(norm).Storage).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0d ? 1d : 1d/norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => ((float)norminv[j])*x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Calculates the value sum of each row vector.
        /// </summary>
        public override VectorMathNet<Complex32> RowSums()
        {
            var ret = new Complex32[RowCount];
            Storage.FoldByRowUnchecked(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return VectorMathNet<Complex32>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each row vector.
        /// </summary>
        public override VectorMathNet<Complex32> RowAbsoluteSums()
        {
            var ret = new Complex32[RowCount];
            Storage.FoldByRowUnchecked(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            return VectorMathNet<Complex32>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the value sum of each column vector.
        /// </summary>
        public override VectorMathNet<Complex32> ColumnSums()
        {
            var ret = new Complex32[ColumnCount];
            Storage.FoldByColumnUnchecked(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return VectorMathNet<Complex32>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each column vector.
        /// </summary>
        public override VectorMathNet<Complex32> ColumnAbsoluteSums()
        {
            var ret = new Complex32[ColumnCount];
            Storage.FoldByColumnUnchecked(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            return VectorMathNet<Complex32>.Build.Dense(ret);
        }

        /// <summary>
        /// Evaluates whether this matrix is Hermitian (conjugate symmetric).
        /// </summary>
        public override bool IsHermitian()
        {
            if (RowCount != ColumnCount)
            {
                return false;
            }

            for (var k = 0; k < RowCount; k++)
            {
                if (!At(k, k).IsReal())
                {
                    return false;
                }
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = row + 1; column < ColumnCount; column++)
                {
                    if (!At(row, column).Equals(At(column, row).Conjugate()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override Cholesky<Complex32> Cholesky()
        {
            return UserCholesky.Create(this);
        }

        public override LU<Complex32> LU()
        {
            return UserLU.Create(this);
        }

        public override QR<Complex32> QR(QRMethod method = QRMethod.Thin)
        {
            return UserQR.Create(this, method);
        }

        public override GramSchmidt<Complex32> GramSchmidt()
        {
            return UserGramSchmidt.Create(this);
        }

        public override Svd<Complex32> Svd(bool computeVectors = true)
        {
            return UserSvd.Create(this, computeVectors);
        }

        public override Evd<Complex32> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return UserEvd.Create(this, symmetricity);
        }
    }
}
