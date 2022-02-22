// <copyright file="Vector.Arithmetic.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2016 Math.NET
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

namespace AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra
{
    public abstract partial class VectorMathNet<T>
    {
        /// <summary>
        /// The zero value for type T.
        /// </summary>
        public static readonly T Zero = BuilderInstance<T>.Vector.Zero;

        /// <summary>
        /// The value of 1.0 for type T.
        /// </summary>
        public static readonly T One = BuilderInstance<T>.Vector.One;

        /// <summary>
        /// Negates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected abstract void DoNegate(VectorMathNet<T> result);

        /// <summary>
        /// Complex conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected abstract void DoConjugate(VectorMathNet<T> result);

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected abstract void DoAdd(T scalar, VectorMathNet<T> result);

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected abstract void DoAdd(VectorMathNet<T> other, VectorMathNet<T> result);

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected abstract void DoSubtract(T scalar, VectorMathNet<T> result);

        /// <summary>
        /// Subtracts each element of the vector from a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected void DoSubtractFrom(T scalar, VectorMathNet<T> result)
        {
            DoNegate(result);
            result.DoAdd(scalar, result);
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected abstract void DoSubtract(VectorMathNet<T> other, VectorMathNet<T> result);

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="result">The vector to store the result of the multiplication.</param>
        protected abstract void DoMultiply(T scalar, VectorMathNet<T> result);

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        protected abstract T DoDotProduct(VectorMathNet<T> other);

        /// <summary>
        /// Computes the dot product between the conjugate of this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of conj(a[i])*b[i] for all i.</returns>
        protected abstract T DoConjugateDotProduct(VectorMathNet<T> other);

        /// <summary>
        /// Computes the outer product M[i,j] = u[i]*v[j] of this and another vector and stores the result in the result matrix.
        /// </summary>
        /// <param name="other">The other vector</param>
        /// <param name="result">The matrix to store the result of the product.</param>
        protected void DoOuterProduct(VectorMathNet<T> other, MatrixMathNet<T> result)
        {
            VectorMathNet<T> work = Build.Dense(Count);
            for (int i = 0; i < other.Count; i++)
            {
                DoMultiply(other.At(i), work);
                result.SetColumn(i, work);
            }
        }

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected abstract void DoDivide(T divisor, VectorMathNet<T> result);

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected abstract void DoDivideByThis(T dividend, VectorMathNet<T> result);

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoModulus(T divisor, VectorMathNet<T> result);

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoModulusByThis(T dividend, VectorMathNet<T> result);

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoRemainder(T divisor, VectorMathNet<T> result);

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected abstract void DoRemainderByThis(T dividend, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected abstract void DoPointwiseMultiply(VectorMathNet<T> other, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the division.</param>
        protected abstract void DoPointwiseDivide(VectorMathNet<T> divisor, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise raise this vector to an exponent and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected abstract void DoPointwisePower(T exponent, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise raise this vector to an exponent vector and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent vector to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected abstract void DoPointwisePower(VectorMathNet<T> exponent, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected abstract void DoPointwiseModulus(VectorMathNet<T> divisor, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected abstract void DoPointwiseRemainder(VectorMathNet<T> divisor, VectorMathNet<T> result);

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected abstract void DoPointwiseExp(VectorMathNet<T> result);

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected abstract void DoPointwiseLog(VectorMathNet<T> result);

        protected abstract void DoPointwiseAbs(VectorMathNet<T> result);
        protected abstract void DoPointwiseAcos(VectorMathNet<T> result);
        protected abstract void DoPointwiseAsin(VectorMathNet<T> result);
        protected abstract void DoPointwiseAtan(VectorMathNet<T> result);
        protected abstract void DoPointwiseCeiling(VectorMathNet<T> result);
        protected abstract void DoPointwiseCos(VectorMathNet<T> result);
        protected abstract void DoPointwiseCosh(VectorMathNet<T> result);
        protected abstract void DoPointwiseFloor(VectorMathNet<T> result);
        protected abstract void DoPointwiseLog10(VectorMathNet<T> result);
        protected abstract void DoPointwiseRound(VectorMathNet<T> result);
        protected abstract void DoPointwiseSign(VectorMathNet<T> result);
        protected abstract void DoPointwiseSin(VectorMathNet<T> result);
        protected abstract void DoPointwiseSinh(VectorMathNet<T> result);
        protected abstract void DoPointwiseSqrt(VectorMathNet<T> result);
        protected abstract void DoPointwiseTan(VectorMathNet<T> result);
        protected abstract void DoPointwiseTanh(VectorMathNet<T> result);
        protected abstract void DoPointwiseAtan2(VectorMathNet<T> other, VectorMathNet<T> result);
        protected abstract void DoPointwiseAtan2(T scalar, VectorMathNet<T> result);
        protected abstract void DoPointwiseMinimum(T scalar, VectorMathNet<T> result);
        protected abstract void DoPointwiseMinimum(VectorMathNet<T> other, VectorMathNet<T> result);
        protected abstract void DoPointwiseMaximum(T scalar, VectorMathNet<T> result);
        protected abstract void DoPointwiseMaximum(VectorMathNet<T> other, VectorMathNet<T> result);
        protected abstract void DoPointwiseAbsoluteMinimum(T scalar, VectorMathNet<T> result);
        protected abstract void DoPointwiseAbsoluteMinimum(VectorMathNet<T> other, VectorMathNet<T> result);
        protected abstract void DoPointwiseAbsoluteMaximum(T scalar, VectorMathNet<T> result);
        protected abstract void DoPointwiseAbsoluteMaximum(VectorMathNet<T> other, VectorMathNet<T> result);

        /// <summary>
        /// Adds a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <returns>A copy of the vector with the scalar added.</returns>
        public VectorMathNet<T> Add(T scalar)
        {
            if (scalar.Equals(Zero))
            {
                return Clone();
            }

            VectorMathNet<T> result = Build.SameAs(this);
            DoAdd(scalar, result);
            return result;
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Add(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            if (scalar.Equals(Zero))
            {
                CopyTo(result);
                return;
            }

            DoAdd(scalar, result);
        }

        /// <summary>
        /// Adds another vector to this vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <returns>A new vector containing the sum of both vectors.</returns>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public VectorMathNet<T> Add(VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            VectorMathNet<T> result = Build.SameAs(this, other);
            DoAdd(other, result);
            return result;
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Add(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoAdd(other, result);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <returns>A new vector containing the subtraction of this vector and the scalar.</returns>
        public VectorMathNet<T> Subtract(T scalar)
        {
            if (scalar.Equals(Zero))
            {
                return Clone();
            }

            VectorMathNet<T> result = Build.SameAs(this);
            DoSubtract(scalar, result);
            return result;
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Subtract(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            if (scalar.Equals(Zero))
            {
                CopyTo(result);
                return;
            }

            DoSubtract(scalar, result);
        }

        /// <summary>
        /// Subtracts each element of the vector from a scalar.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <returns>A new vector containing the subtraction of the scalar and this vector.</returns>
        public VectorMathNet<T> SubtractFrom(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoSubtractFrom(scalar, result);
            return result;
        }

        /// <summary>
        /// Subtracts each element of the vector from a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract from.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void SubtractFrom(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoSubtractFrom(scalar, result);
        }

        /// <summary>
        /// Returns a negated vector.
        /// </summary>
        /// <returns>The negated vector.</returns>
        /// <remarks>Added as an alternative to the unary negation operator.</remarks>
        public VectorMathNet<T> Negate()
        {
            VectorMathNet<T> retrunVector = Build.SameAs(this);
            DoNegate(retrunVector);
            return retrunVector;
        }

        /// <summary>
        /// Negates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        public void Negate(VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoNegate(result);
        }

        /// <summary>
        /// Subtracts another vector from this vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <returns>A new vector containing the subtraction of the two vectors.</returns>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public VectorMathNet<T> Subtract(VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            VectorMathNet<T> result = Build.SameAs(this, other);
            DoSubtract(other, result);
            return result;
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Subtract(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoSubtract(other, result);
        }

        /// <summary>
        /// Return vector with complex conjugate values of the source vector
        /// </summary>
        /// <returns>Conjugated vector</returns>
        public VectorMathNet<T> Conjugate()
        {
            VectorMathNet<T> retrunVector = Build.SameAs(this);
            DoConjugate(retrunVector);
            return retrunVector;
        }

        /// <summary>
        /// Complex conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        public void Conjugate(VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoConjugate(result);
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <returns>A new vector that is the multiplication of the vector and the scalar.</returns>
        public VectorMathNet<T> Multiply(T scalar)
        {
            if (scalar.Equals(One))
            {
                return Clone();
            }

            if (scalar.Equals(Zero))
            {
                return Build.SameAs(this);
            }

            VectorMathNet<T> result = Build.SameAs(this);
            DoMultiply(scalar, result);
            return result;
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="result">The vector to store the result of the multiplication.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Multiply(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            if (scalar.Equals(One))
            {
                CopyTo(result);
                return;
            }

            if (scalar.Equals(Zero))
            {
                result.Clear();
                return;
            }

            DoMultiply(scalar, result);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        /// <exception cref="ArgumentException">If <paramref name="other"/> is not of the same size.</exception>
        /// <seealso cref="ConjugateDotProduct"/>
        public T DotProduct(VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            return DoDotProduct(other);
        }

        /// <summary>
        /// Computes the dot product between the conjugate of this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of conj(a[i])*b[i] for all i.</returns>
        /// <exception cref="ArgumentException">If <paramref name="other"/> is not of the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="other"/> is <see langword="null"/>.</exception>
        /// <seealso cref="DotProduct"/>
        public T ConjugateDotProduct(VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            return DoConjugateDotProduct(other);
        }

        /// <summary>
        /// Divides each element of the vector by a scalar.
        /// </summary>
        /// <param name="scalar">The scalar to divide with.</param>
        /// <returns>A new vector that is the division of the vector and the scalar.</returns>
        public VectorMathNet<T> Divide(T scalar)
        {
            if (scalar.Equals(One))
            {
                return Clone();
            }

            VectorMathNet<T> result = Build.SameAs(this);
            DoDivide(scalar, result);
            return result;
        }

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide with.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void Divide(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            if (scalar.Equals(One))
            {
                CopyTo(result);
                return;
            }

            DoDivide(scalar, result);
        }

        /// <summary>
        /// Divides a scalar by each element of the vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide.</param>
        /// <returns>A new vector that is the division of the vector and the scalar.</returns>
        public VectorMathNet<T> DivideByThis(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoDivideByThis(scalar, result);
            return result;
        }

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to divide.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void DivideByThis(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoDivideByThis(scalar, result);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <returns>A vector containing the result.</returns>
        public VectorMathNet<T> Modulus(T divisor)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoModulus(divisor, result);
            return result;
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void Modulus(T divisor, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoModulus(divisor, result);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <returns>A vector containing the result.</returns>
        public VectorMathNet<T> ModulusByThis(T dividend)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoModulusByThis(dividend, result);
            return result;
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void ModulusByThis(T dividend, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoModulusByThis(dividend, result);
        }

        /// <summary>
        /// Computes the remainder (vector % divisor), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <returns>A vector containing the result.</returns>
        public VectorMathNet<T> Remainder(T divisor)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoRemainder(divisor, result);
            return result;
        }

        /// <summary>
        /// Computes the remainder (vector % divisor), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void Remainder(T divisor, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoRemainder(divisor, result);
        }

        /// <summary>
        /// Computes the remainder (dividend % vector), where the result has the sign of the dividend,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <returns>A vector containing the result.</returns>
        public VectorMathNet<T> RemainderByThis(T dividend)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoRemainderByThis(dividend, result);
            return result;
        }

        /// <summary>
        /// Computes the remainder (dividend % vector), where the result has the sign of the dividend,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        public void RemainderByThis(T dividend, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoRemainderByThis(dividend, result);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <returns>A new vector which is the pointwise multiplication of the two vectors.</returns>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        public VectorMathNet<T> PointwiseMultiply(VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            VectorMathNet<T> result = Build.SameAs(this, other);
            DoPointwiseMultiply(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMultiply(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseMultiply(other, result);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <returns>A new vector which is the pointwise division of the two vectors.</returns>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        public VectorMathNet<T> PointwiseDivide(VectorMathNet<T> divisor)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            VectorMathNet<T> result = Build.SameAs(this, divisor);
            DoPointwiseDivide(divisor, result);
            return result;
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseDivide(VectorMathNet<T> divisor, VectorMathNet<T> result)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseDivide(divisor, result);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        public VectorMathNet<T> PointwisePower(T exponent)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwisePower(exponent, result);
            return result;
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        /// <param name="result">The matrix to store the result into.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwisePower(T exponent, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwisePower(exponent, result);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        public VectorMathNet<T> PointwisePower(VectorMathNet<T> exponent)
        {
            if (Count != exponent.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(exponent));
            }

            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwisePower(exponent, result);
            return result;
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        /// <param name="result">The vector to store the result into.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwisePower(VectorMathNet<T> exponent, VectorMathNet<T> result)
        {
            if (Count != exponent.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(exponent));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwisePower(exponent, result);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this vector with another vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        public VectorMathNet<T> PointwiseModulus(VectorMathNet<T> divisor)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            VectorMathNet<T> result = Build.SameAs(this, divisor);
            DoPointwiseModulus(divisor, result);
            return result;
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The vector to store the result of the pointwise modulus.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseModulus(VectorMathNet<T> divisor, VectorMathNet<T> result)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseModulus(divisor, result);
        }
        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this vector with another vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        public VectorMathNet<T> PointwiseRemainder(VectorMathNet<T> divisor)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            VectorMathNet<T> result = Build.SameAs(this, divisor);
            DoPointwiseRemainder(divisor, result);
            return result;
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The vector to store the result of the pointwise remainder.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="divisor"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseRemainder(VectorMathNet<T> divisor, VectorMathNet<T> result)
        {
            if (Count != divisor.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(divisor));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseRemainder(divisor, result);
        }

        /// <summary>
        /// Helper function to apply a unary function to a vector. The function
        /// f modifies the vector given to it in place.  Before its
        /// called, a copy of the 'this' vector with the same dimension is
        /// first created, then passed to f.  The copy is returned as the result
        /// </summary>
        /// <param name="f">Function which takes a vector, modifies it in place and returns void</param>
        /// <returns>New instance of vector which is the result</returns>
        protected VectorMathNet<T> PointwiseUnary(Action<VectorMathNet<T>> f)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            f(result);
            return result;
        }

        /// <summary>
        /// Helper function to apply a unary function which modifies a vector
        /// in place.
        /// </summary>
        /// <param name="f">Function which takes a vector, modifies it in place and returns void</param>
        /// <param name="result">The vector where the result is to be stored</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        protected void PointwiseUnary(Action<VectorMathNet<T>> f, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            f(result);
        }

        /// <summary>
        /// Helper function to apply a binary function which takes a scalar and
        /// a vector and modifies the latter in place. A copy of the "this"
        /// vector is therefore first made and then passed to f together with
        /// the scalar argument.  The copy is then returned as the result
        /// </summary>
        /// <param name="f">Function which takes a scalar and a vector, modifies the vector in place and returns void</param>
        /// <param name="other">The scalar to be passed to the function</param>
        /// <returns>The resulting vector</returns>
        protected VectorMathNet<T> PointwiseBinary(Action<T, VectorMathNet<T>> f, T other)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            f(other, result);
            return result;
        }

        /// <summary>
        /// Helper function to apply a binary function which takes a scalar and
        /// a vector, modifies the latter in place and returns void.
        /// </summary>
        /// <param name="f">Function which takes a scalar and a vector, modifies the vector in place and returns void</param>
        /// <param name="x">The scalar to be passed to the function</param>
        /// <param name="result">The vector where the result will be placed</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        protected void PointwiseBinary(Action<T, VectorMathNet<T>> f, T x, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }
            f(x, result);
        }

        /// <summary>
        /// Helper function to apply a binary function which takes two vectors
        /// and modifies the latter in place.  A copy of the "this" vector is
        /// first made and then passed to f together with the other vector. The
        /// copy is then returned as the result
        /// </summary>
        /// <param name="f">Function which takes two vectors, modifies the second in place and returns void</param>
        /// <param name="other">The other vector to be passed to the function as argument. It is not modified</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        protected VectorMathNet<T> PointwiseBinary(Action<VectorMathNet<T>, VectorMathNet<T>> f, VectorMathNet<T> other)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            VectorMathNet<T> result = Build.SameAs(this, other);
            f(other, result);
            return result;
        }

        /// <summary>
        /// Helper function to apply a binary function which takes two vectors
        /// and modifies the second one in place
        /// </summary>
        /// <param name="f">Function which takes two vectors, modifies the second in place and returns void</param>
        /// <param name="other">The other vector to be passed to the function as argument. It is not modified</param>
        /// <param name="result">The resulting vector</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="other"/> are not the same size.</exception>
        protected void PointwiseBinary(Action<VectorMathNet<T>, VectorMathNet<T>> f, VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != other.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(other));
            }

            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }
            f(other, result);
        }

        /// <summary>
        /// Pointwise applies the exponent function to each value.
        /// </summary>
        public VectorMathNet<T> PointwiseExp()
        {
            return PointwiseUnary(DoPointwiseExp);
        }

        /// <summary>
        /// Pointwise applies the exponent function to each value.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseExp(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseExp, result);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value.
        /// </summary>
        public VectorMathNet<T> PointwiseLog()
        {
            return PointwiseUnary(DoPointwiseLog);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseLog(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseLog, result);
        }

        /// <summary>
        /// Pointwise applies the abs function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseAbs()
        {
            return PointwiseUnary(DoPointwiseAbs);
        }

        /// <summary>
        /// Pointwise applies the abs function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseAbs(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseAbs, result);
        }

        /// <summary>
        /// Pointwise applies the acos function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseAcos()
        {
            return PointwiseUnary(DoPointwiseAcos);
        }

        /// <summary>
        /// Pointwise applies the acos function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseAcos(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseAcos, result);
        }

        /// <summary>
        /// Pointwise applies the asin function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseAsin()
        {
            return PointwiseUnary(DoPointwiseAsin);
        }

        /// <summary>
        /// Pointwise applies the asin function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseAsin(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseAsin, result);
        }

        /// <summary>
        /// Pointwise applies the atan function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseAtan()
        {
            return PointwiseUnary(DoPointwiseAtan);
        }

        /// <summary>
        /// Pointwise applies the atan function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseAtan(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseAtan, result);
        }

        /// <summary>
        /// Pointwise applies the atan2 function to each value of the current
        /// vector and a given other vector being the 'x' of atan2 and the
        /// 'this' vector being the 'y'
        /// </summary>
        /// <param name="other"></param>
        public VectorMathNet<T> PointwiseAtan2(VectorMathNet<T> other)
        {
            return PointwiseBinary(DoPointwiseAtan2, other);
        }

        /// <summary>
        /// Pointwise applies the atan2 function to each value of the current
        /// vector and a given other vector being the 'x' of atan2 and the
        /// 'this' vector being the 'y'
        /// </summary>
        /// <param name="other"></param>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseAtan2(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            PointwiseBinary(DoPointwiseAtan2, other, result);
        }

        /// <summary>
        /// Pointwise applies the ceiling function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseCeiling()
        {
            return PointwiseUnary(DoPointwiseCeiling);
        }

        /// <summary>
        /// Pointwise applies the ceiling function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseCeiling(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseCeiling, result);
        }

        /// <summary>
        /// Pointwise applies the cos function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseCos()
        {
            return PointwiseUnary(DoPointwiseCos);
        }

        /// <summary>
        /// Pointwise applies the cos function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseCos(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseCos, result);
        }

        /// <summary>
        /// Pointwise applies the cosh function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseCosh()
        {
            return PointwiseUnary(DoPointwiseCosh);
        }

        /// <summary>
        /// Pointwise applies the cosh function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseCosh(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseCosh, result);
        }

        /// <summary>
        /// Pointwise applies the floor function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseFloor()
        {
            return PointwiseUnary(DoPointwiseFloor);
        }

        /// <summary>
        /// Pointwise applies the floor function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseFloor(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseFloor, result);
        }

        /// <summary>
        /// Pointwise applies the log10 function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseLog10()
        {
            return PointwiseUnary(DoPointwiseLog10);
        }

        /// <summary>
        /// Pointwise applies the log10 function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseLog10(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseLog10, result);
        }

        /// <summary>
        /// Pointwise applies the round function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseRound()
        {
            return PointwiseUnary(DoPointwiseRound);
        }

        /// <summary>
        /// Pointwise applies the round function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseRound(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseRound, result);
        }

        /// <summary>
        /// Pointwise applies the sign function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseSign()
        {
            return PointwiseUnary(DoPointwiseSign);
        }

        /// <summary>
        /// Pointwise applies the sign function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseSign(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseSign, result);
        }

        /// <summary>
        /// Pointwise applies the sin function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseSin()
        {
            return PointwiseUnary(DoPointwiseSin);
        }

        /// <summary>
        /// Pointwise applies the sin function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseSin(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseSin, result);
        }

        /// <summary>
        /// Pointwise applies the sinh function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseSinh()
        {
            return PointwiseUnary(DoPointwiseSinh);
        }

        /// <summary>
        /// Pointwise applies the sinh function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseSinh(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseSinh, result);
        }

        /// <summary>
        /// Pointwise applies the sqrt function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseSqrt()
        {
            return PointwiseUnary(DoPointwiseSqrt);
        }

        /// <summary>
        /// Pointwise applies the sqrt function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseSqrt(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseSqrt, result);
        }

        /// <summary>
        /// Pointwise applies the tan function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseTan()
        {
            return PointwiseUnary(DoPointwiseTan);
        }

        /// <summary>
        /// Pointwise applies the tan function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseTan(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseTan, result);
        }

        /// <summary>
        /// Pointwise applies the tanh function to each value
        /// </summary>
        public VectorMathNet<T> PointwiseTanh()
        {
            return PointwiseUnary(DoPointwiseTanh);
        }

        /// <summary>
        /// Pointwise applies the tanh function to each value
        /// </summary>
        /// <param name="result">The vector to store the result</param>
        public void PointwiseTanh(VectorMathNet<T> result)
        {
            PointwiseUnary(DoPointwiseTanh, result);
        }

        /// <summary>
        /// Computes the outer product M[i,j] = u[i]*v[j] of this and another vector.
        /// </summary>
        /// <param name="other">The other vector</param>
        public MatrixMathNet<T> OuterProduct(VectorMathNet<T> other)
        {
            MatrixMathNet<T> matrix = MatrixMathNet<T>.Build.SameAs(this, Count, other.Count);
            DoOuterProduct(other, matrix);
            return matrix;
        }

        /// <summary>
        /// Computes the outer product M[i,j] = u[i]*v[j] of this and another vector and stores the result in the result matrix.
        /// </summary>
        /// <param name="other">The other vector</param>
        /// <param name="result">The matrix to store the result of the product.</param>
        public void OuterProduct(VectorMathNet<T> other, MatrixMathNet<T> result)
        {
            if (Count != result.RowCount || other.Count != result.ColumnCount)
            {
                throw new ArgumentException("Matrix dimensions must agree.", nameof(result));
            }

            DoOuterProduct(other, result);
        }

        public static MatrixMathNet<T> OuterProduct(VectorMathNet<T> u, VectorMathNet<T> v)
        {
            return u.OuterProduct(v);
        }

        /// <summary>
        /// Pointwise applies the minimum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        public VectorMathNet<T> PointwiseMinimum(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseMinimum(scalar, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the minimum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMinimum(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseMinimum(scalar, result);
        }

        /// <summary>
        /// Pointwise applies the maximum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        public VectorMathNet<T> PointwiseMaximum(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseMaximum(scalar, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the maximum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMaximum(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseMaximum(scalar, result);
        }

        /// <summary>
        /// Pointwise applies the absolute minimum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        public VectorMathNet<T> PointwiseAbsoluteMinimum(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseAbsoluteMinimum(scalar, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the absolute minimum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseAbsoluteMinimum(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseAbsoluteMinimum(scalar, result);
        }

        /// <summary>
        /// Pointwise applies the absolute maximum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        public VectorMathNet<T> PointwiseAbsoluteMaximum(T scalar)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseAbsoluteMaximum(scalar, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the absolute maximum with a scalar to each value.
        /// </summary>
        /// <param name="scalar">The scalar value to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseAbsoluteMaximum(T scalar, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseAbsoluteMaximum(scalar, result);
        }

        /// <summary>
        /// Pointwise applies the minimum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        public VectorMathNet<T> PointwiseMinimum(VectorMathNet<T> other)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseMinimum(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the minimum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMinimum(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseMinimum(other, result);
        }

        /// <summary>
        /// Pointwise applies the maximum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        public VectorMathNet<T> PointwiseMaximum(VectorMathNet<T> other)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseMaximum(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the maximum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseMaximum(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseMaximum(other, result);
        }

        /// <summary>
        /// Pointwise applies the absolute minimum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        public VectorMathNet<T> PointwiseAbsoluteMinimum(VectorMathNet<T> other)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseAbsoluteMinimum(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the absolute minimum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseAbsoluteMinimum(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseAbsoluteMinimum(other, result);
        }

        /// <summary>
        /// Pointwise applies the absolute maximum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        public VectorMathNet<T> PointwiseAbsoluteMaximum(VectorMathNet<T> other)
        {
            VectorMathNet<T> result = Build.SameAs(this);
            DoPointwiseAbsoluteMaximum(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise applies the absolute maximum with the values of another vector to each value.
        /// </summary>
        /// <param name="other">The vector with the values to compare to.</param>
        /// <param name="result">The vector to store the result.</param>
        /// <exception cref="ArgumentException">If this vector and <paramref name="result"/> are not the same size.</exception>
        public void PointwiseAbsoluteMaximum(VectorMathNet<T> other, VectorMathNet<T> result)
        {
            if (Count != result.Count)
            {
                throw new ArgumentException("All vectors must have the same dimensionality.", nameof(result));
            }

            DoPointwiseAbsoluteMaximum(other, result);
        }

        /// <summary>
        /// Calculates the L1 norm of the vector, also known as Manhattan norm.
        /// </summary>
        /// <returns>The sum of the absolute values.</returns>
        public abstract double L1Norm();

        /// <summary>
        /// Calculates the L2 norm of the vector, also known as Euclidean norm.
        /// </summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public abstract double L2Norm();

        /// <summary>
        /// Calculates the infinity norm of the vector.
        /// </summary>
        /// <returns>The maximum absolute value.</returns>
        public abstract double InfinityNorm();

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <returns><c>Scalar ret = (sum(abs(this[i])^p))^(1/p)</c></returns>
        public abstract double Norm(double p);

        /// <summary>
        /// Normalizes this vector to a unit vector with respect to the p-norm.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <returns>This vector normalized to a unit vector with respect to the p-norm.</returns>
        public abstract VectorMathNet<T> Normalize(double p);

        /// <summary>
        /// Returns the value of the absolute minimum element.
        /// </summary>
        /// <returns>The value of the absolute minimum element.</returns>
        public abstract T AbsoluteMinimum();

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public abstract int AbsoluteMinimumIndex();

        /// <summary>
        /// Returns the value of the absolute maximum element.
        /// </summary>
        /// <returns>The value of the absolute maximum element.</returns>
        public abstract T AbsoluteMaximum();

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public abstract int AbsoluteMaximumIndex();

        /// <summary>
        /// Returns the value of maximum element.
        /// </summary>
        /// <returns>The value of maximum element.</returns>
        public T Maximum()
        {
            return At(MaximumIndex());
        }

        /// <summary>
        /// Returns the index of the maximum element.
        /// </summary>
        /// <returns>The index of maximum element.</returns>
        public abstract int MaximumIndex();

        /// <summary>
        /// Returns the value of the minimum element.
        /// </summary>
        /// <returns>The value of the minimum element.</returns>
        public T Minimum()
        {
            return At(MinimumIndex());
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>
        public abstract int MinimumIndex();

        /// <summary>
        /// Computes the sum of the vector's elements.
        /// </summary>
        /// <returns>The sum of the vector's elements.</returns>
        public abstract T Sum();

        /// <summary>
        /// Computes the sum of the absolute value of the vector's elements.
        /// </summary>
        /// <returns>The sum of the absolute value of the vector's elements.</returns>
        public double SumMagnitudes()
        {
            return L1Norm();
        }
    }
}
