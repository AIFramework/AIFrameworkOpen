using AI.BackEnds.DSP.NWaves.Operations;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.DataStructs.Shapes;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AI.DataStructs.Algebraic
{
    /// <summary>
    /// Class that implements vector and operations on it
    /// </summary>
    [Serializable]
    public class Vector : List<double>, IAlgebraicStructure, IEquatable<Vector>, IEquatable<List<double>>, ISavable, ITextSavable, IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Vector data as an array
        /// </summary>
        double[] IAlgebraicStructure.Data => ToArray();
        /// <summary>
        /// Vector shape
        /// </summary>
        public Shape Shape => new Shape1D(Count);
        #endregion

        #region Конструкторы
        /// <summary>
        /// Creates a vector of capacity 3
        /// </summary>
        public Vector() : base(3) { AddRange(new double[3]); }
        /// <summary>
        /// Creates a vector of custom capacity
        /// </summary>
        /// <param name="n"></param>
        public Vector(int n) : base(n) { AddRange(new double[n]); }
        /// <summary>
        /// Creates a vector of dimension 1 with the given value
        /// </summary>
        /// <param name="value"></param>
        public Vector(double value)
        {
            Add(value);
        }
        /// <summary>
        /// Creates a vector from a double array
        /// </summary>
        /// <param name="vector"></param>
        public Vector(params double[] vector)
        {
            AddRange(vector);
        }
        /// <summary>
        /// Creates a vector from the IEnumerable interface of double
        /// </summary>
        /// <param name="data"></param>
        public Vector(IEnumerable<double> data)
        {
            AddRange(data);
        }
        /// <summary>
        /// Creates a vector from the IEnumerable interface of float
        /// </summary>
        /// <param name="data"></param>
        public Vector(IEnumerable<float> data)
        {
            double[] d = new double[data.Count()];

            int c = 0;

            foreach (float item in data)
            {
                d[c++] = item;
            }

            AddRange(d);
        }
        #endregion

        #region Операторы
        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector operator +(Vector A, Vector B)
        {
            int n1 = A.Count;
            int n2 = B.Count;

            if (n1 != n2)
            {
                throw new InvalidOperationException("Lengths of input vectors mismatche");
            }

            Vector C = new Vector(n1);

            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] + B[i];
            }

            return C;
        }
        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator +(Vector A, double k)
        {
            int n1 = A.Count;
            Vector C = new Vector(n1);

            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] + k;
            }

            return C;
        }
        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator +(double k, Vector A)
        {
            int n1 = A.Count;
            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] + k;
            }

            return C;
        }
        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator -(double k, Vector A)
        {
            int n1 = A.Count;
            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = k - A[i];
            }

            return C;
        }
        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator -(Vector A, double k)
        {
            int n1 = A.Count;
            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] - k;
            }

            return C;
        }
        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector operator -(Vector A, Vector B)
        {
            int n1 = A.Count;
            int n2 = B.Count;

            if (n1 != n2)
            {
                throw new InvalidOperationException("Lengths of input vectors mismatche");
            }

            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] - B[i];
            }

            return C;
        }
        /// <summary>
        /// Negation
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Vector operator -(Vector A)
        {
            return 0.0 - A;
        }
        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector operator *(Vector A, Vector B)
        {
            int n1 = A.Count;
            int n2 = B.Count;

            if (n1 != n2)
            {
                throw new InvalidOperationException("Lengths of input vectors mismatche");
            }

            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] * B[i];
            }

            return C;
        }
        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator *(double k, Vector A)
        {
            int n = A.Count;
            Vector C = new Vector(n);

            for (int i = 0; i < n; i++)
            {
                C[i] = k * A[i];
            }

            return C;
        }
        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator *(Vector A, double k)
        {
            int n = A.Count;
            Vector C = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                C[i] = k * A[i];
            }

            return C;
        }
        /// <summary>
        /// Division
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector operator /(Vector A, Vector B)
        {
            int n1 = A.Count;
            int n2 = B.Count;

            if (n1 != n2)
            {
                throw new InvalidOperationException("Lengths of input vectors mismatche");
            }

            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] / B[i];
            }

            return C;
        }
        /// <summary>
        /// Division
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator /(double k, Vector A)
        {
            int n = A.Count;
            Vector C = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                C[i] = k / A[i];
            }

            return C;
        }
        /// <summary>
        /// Division
        /// </summary>
        /// <param name="A"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector operator /(Vector A, double k)
        {
            double c = 1.0 / k;
            int n = A.Count;
            Vector C = new Vector(n);

            for (int i = 0; i < n; i++)
            {
                C[i] = A[i] * c;
            }

            return C;
        }
        /// <summary>
        /// Remainder of the division
        /// </summary>
        /// <param name="k"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Vector operator %(Vector A, double k)
        {
            int n = A.Count;
            Vector C = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                C[i] = A[i] % k;
            }

            return C;
        }

        /// <summary>
        /// Remainder of the division
        /// </summary>
        /// <param name="k"></param>
        /// <param name="A"></param>
        /// <returns></returns>
        public static Vector operator %(double k, Vector A)
        {
            int n = A.Count;
            Vector C = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                C[i] = k % A[i];
            }

            return C;
        }

        /// <summary>
        /// Remainder of the division for each element
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Vector operator %(Vector A, Vector B)
        {
            int n1 = A.Count;
            int n2 = B.Count;

            if (n1 != n2)
            {
                throw new InvalidOperationException("Lengths of input vectors mismatche");
            }

            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] % B[i];
            }

            return C;
        }

        public static bool operator ==(Vector A, Vector B)
        {
            if (Equals(A, null) && Equals(B, null))
            {
                return true;
            }

            if ((Equals(A, null) && !Equals(B, null)) || (!Equals(A, null) && Equals(B, null)))
            {
                return false;
            }

            if (A.Count != B.Count)
            {
                return false;
            }

            bool flag = true;

            for (int i = 0; i < A.Count; i++)
            {
                if (A[i] != B[i])
                {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        public static bool operator !=(Vector A, Vector B)
        {
            return !(A == B);
        }

        public static bool operator ==(Vector left, IList<double> right)
        {
            return left == FromList(right);
        }

        public static bool operator !=(Vector left, IList<double> right)
        {
            return left != FromList(right);
        }

        public static bool operator ==(List<double> left, Vector right)
        {
            return FromList(left) == right;
        }

        public static bool operator !=(List<double> left, Vector right)
        {
            return FromList(left) != right;
        }

        public static implicit operator double[](Vector vector)
        {
            return vector.ToArray();
        }

        public static implicit operator Vector(double[] data)
        {
            return new Vector(data);
        }

        public static implicit operator Vector(int[] data)
        {
            Vector outp = new Vector(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                outp[i] = data[i];
            }

            return outp;
        }

        public static implicit operator Vector(float[] data)
        {
            return SingleArray2Vector(data);
        }

        public static explicit operator float[](Vector data)
        {
            return Vector2SingleArray(data);
        }

        public static explicit operator int[](Vector vector)
        {
            int[] outp = new int[vector.Count];

            for (int i = 0; i < vector.Count; i++)
            {
                outp[i] = (int)vector[i];
            }

            return outp;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Adding an item (circular buffer)
        /// </summary>
        public void AddCB(double item)
        {
            int len = Count;
            Vector data = new Vector(len);

            for (int i = 1; i < len; i++)
            {
                data[i] = this[i - 1];

            }

            data[0] = item;
            for (int i = 0; i < len; i++) this[i] = data[i];

        }

        /// <summary>
        /// Adding an item (circular buffer to end)
        /// </summary>
        public void AddCBE(double item)
        {
            int len = Count; // Длинна вектора

            for (int i = 1; i < len; i++)
            {
                this[i - 1] = this[i];
            }

            this[len - 1] = item;
        }

        /// <summary>
        /// Replacing uncertainty with a number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector NanToValue(double value = 0)
        {
            Vector outpVect = new Vector(Count);

            for (int i = 0; i < outpVect.Count; i++)
            {
                outpVect[i] = double.IsNaN(this[i]) ? value : this[i];
            }

            return outpVect;
        }
        /// <summary>
        /// Replacing uncertainty with a mean
        /// </summary>
        /// <returns></returns>
        public Vector NanToMean()
        {
            double mean = Mean();
            Vector outpVect = new Vector(Count);

            for (int i = 0; i < outpVect.Count; i++)
            {
                outpVect[i] = double.IsNaN(this[i]) ? mean : this[i];
            }

            return outpVect;
        }
        /// <summary>
        /// Vector repeat
        /// </summary>
        /// <param name="count">Number of repetitions</param>
        public Vector Repeat(int count)
        {
            int k = 0, len = Count * count;
            Vector ret = new Vector(len);

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    ret[k++] = this[j];
                }
            }

            return ret;
        }
        /// <summary>
        /// Cosine of angle between vectors
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public double Cos(Vector vect)
        {
            return AnalyticGeometryFunctions.Cos(vect, this);
        }
        /// <summary>
        /// Convert a vector to a one-hot representation
        /// </summary>
        /// <returns></returns>
        public int MaxOut()
        {
            return MaxElementIndex();
        }
        /// <summary>
        /// Getting a vector with one at the index position with the maximum value and -1 at the rest
        /// </summary>
        /// <param name="max"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        public Vector MaxOutVector(double max = 1, double rest = -1)
        {
            int ind = MaxElementIndex();
            Vector ret = new Vector(Count) + rest;
            ret[ind] = max;
            return ret;
        }
        /// <summary>
        /// Getting a unit vector (direction vector)
        /// </summary>
        /// <returns></returns>
        public Vector GetUnitVector()
        {
            return this / Norm();
        }
        /// <summary>
        /// Rounding
        /// </summary>
        /// <param name="num">Count of digits in the fraction</param>
        /// <returns></returns>
        public Vector Round(int num)
        {
            Vector outp = new Vector(Count);

            for (int i = 0; i < Count; i++)
            {
                outp[i] = Math.Round(this[i], num);
            }

            return outp;
        }
        /// <summary>
        /// Deleting selected items
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(Vector elements)
        {
            List<double> lD = Clone();

            foreach (double element in elements)
            {
                lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Deleting selected items
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(double[] elements)
        {
            List<double> lD = Clone();

            foreach (double element in elements)
            {
                lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Deleting selected items
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(List<double> elements)
        {
            List<double> lD = new List<double>();
            lD.AddRange(Clone());

            foreach (double element in elements)
            {
                lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Returns a vector in the range [a; b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Vector GetIntervalDouble(int a, int b, double[] data)
        {
            double[] interval = new double[b - a];
            int len = 8 * (b - a);

            Buffer.BlockCopy(data, 8 * a, interval, 0, len);

            return new Vector(interval);
        }
        /// <summary>
        /// Returns a vector in the range [a; b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
		public Vector GetInterval(int a, int b)
        {
            double[] interval = new double[b - a];
            int len = 8 * (b - a);
            Buffer.BlockCopy(ToArray(), 8 * a, interval, 0, len);
            return new Vector(interval);
        }
        /// <summary>
        /// Vector cloning
        /// </summary>
        /// <returns></returns>
        public Vector Clone()
        {
            return new Vector(ToArray());
        }
        /// <summary>
        /// Adding a mirrored vector
        /// </summary>
        /// <returns></returns>
        public Vector AddSimmetr()
        {
            int n2 = 2 * Count;
            Vector newVector = new Vector(n2);

            for (int i = 0; i < Count; i++)
            {
                newVector[i] = this[i];
            }

            for (int i = Count; i < n2; i++)
            {
                newVector[i] = this[n2 - i - 1];
            }

            return newVector;
        }
        /// <summary>
        /// Writing the vector in reverse order, for example, the vector {1,2,3} turns into {3,2,1}
        /// </summary>
        /// <returns></returns>
        public Vector Revers()
        {
            double[] newVect = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                newVect[i] = this[Count - i - 1];
            }

            return new Vector(newVect);
        }
        /// <summary>
        /// Zero padding or cropping to the desired vector size
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Vector CutAndZero(int n)
        {
            double[] newVect = new double[n];

            if (n > Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    newVect[i] = this[i];
                }

                for (int i = Count; i < n; i++)
                {
                    newVect[i] = 0;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    newVect[i] = this[i];
                }
            }

            return new Vector(newVect);
        }
        /// <summary>
        /// Shift
        /// </summary>
        /// <param name="valueShift"></param>
        /// <returns></returns>
        public Vector Shift(int valueShift)
        {
            int count = Count + valueShift;
            double[] newVect = new double[count];

            for (int i = 0; i < valueShift; i++)
            {
                newVect[i] = 0.0;
            }

            for (int i = valueShift; i < count; i++)
            {
                newVect[i] = this[i - valueShift];
            }

            return new Vector(newVect);
        }
        /// <summary>
        /// Vector to matrix
        /// </summary>
        public Matrix ToMatrix()
        {
            double[,] matrix = new double[1, Count];
            for (int i = 0; i < Count; i++)
            {
                matrix[0, i] = this[i];
            }

            return new Matrix(matrix);
        }
        /// <summary>
        /// Decimation (without filter) vector
        /// </summary>
        /// <param name="n">Decimation factor</param>
        public Vector Downsampling(int n)
        {
            Vector C = (Count % n == 0) ? new Vector(Count / n) : new Vector((Count / n) + 1);

            for (int i = 0, j = 0; i < Count; i += n, j++)
            {
                C[j] = this[i];
            }

            return C;
        }


        /// <summary>
        /// Decimation (with filter) vector, NWave backend
        /// </summary>
        /// <param name="n">Decimation factor</param>
        public Vector Decimation(int n)
        {
            DiscreteSignal ds = new DiscreteSignal(Count, (float[])this);
            DiscreteSignal outSignal = Operation.Decimate(ds, n);
            return outSignal.Samples;
        }
        /// <summary>
        /// Up sampling, inserting zeros in the middle
        /// </summary>
        /// <param name="kUnPool">The number of zeros between samples of the original vector</param>
        /// <returns></returns>
        public Vector UnPooling(int kUnPool)
        {
            Vector vector = new Vector(Count * kUnPool);

            for (int i = 0, k = 0; i < vector.Count; i += kUnPool)
            {
                vector[i] = this[k++];
            }

            return vector;
        }
        /// <summary>
        /// Interpolation by a polynomial of order zero
        /// </summary>
        /// <param name="kInterp"></param>
        /// <returns></returns>
        public Vector InterpolayrZero(int kInterp)
        {
            Vector C = new Vector(Count * kInterp);

            for (int i = 0; i < C.Count; i++)
            {
                C[i] = this[i / kInterp];
            }


            return C;
        }
        /// <summary>
        /// Adds one to the beginning
        /// </summary>
        public Vector AddOne()
        {
            Vector C = Shift(1);
            C[0] = 1;
            return C;
        }
        /// <summary>
        /// Checks if all elements of the vector are equal to zero
        /// </summary>
        /// <returns></returns>
        public bool IsFilledWithZeros()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Checks if a vector contains more than n zero elements
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsFilledWithZeros(int n)
        {
            int count = 0;

            for (int i = 0; i < Count; i++)
            {
                if (this[i] == 0)
                {
                    count++;
                }
            }

            return count > n;
        }
        /// <summary>
        /// Vector transformation
        /// </summary>
        /// <param name="transformFunc"></param>
        /// <returns></returns>
        public Vector Transform(Func<double, double> transformFunc)
        {
            Vector output = new Vector(Count);

            for (int i = 0; i < Count; i++)
            {
                output[i] = transformFunc(this[i]);
            }

            return output;
        }
        /// <summary>
		/// Vector transformation
		/// </summary>
		/// <param name="transformFunc">Transform function, function from index</param>
        /// <returns></returns>
		public Vector TransformByIndex(Func<int, double> transformFunc)
        {
            Vector output = new Vector(Count);

            for (int i = 0; i < Count; i++)
            {
                output[i] = transformFunc(i);
            }

            return output;
        }
        /// <summary>
		/// Transform vector
		/// </summary>
		/// <param name="transformFunc">Transform function, function from index and value F(int i, double vect_i)</param>
        /// <returns></returns>
		public Vector TransformFromIndexAndValue(Func<int, double, double> transformFunc)
        {
            Vector output = new Vector(Count);

            for (int i = 0; i < Count; i++)
            {
                output[i] = transformFunc(i, this[i]);
            }

            return output;
        }
        /// <summary>
        /// Vector transformation (Use vector of arguments)
        /// </summary>
        /// <param name="transformFunc">Transform function, a function of the value of the vector of arguments and the current vector</param>
        /// <param name="x">Vector of arguments</param>
        /// <returns></returns>
        public Vector TransformWithArguments(Vector x, Func<double, double, double> transformFunc)
        {
            if (x.Count != Count)
            {
                throw new InvalidOperationException("Length of input vector doesn't match the length of current");
            }

            Vector output = new Vector(Count);

            for (int i = 0; i < Count; i++)
            {
                output[i] = transformFunc(x[i], this[i]);
            }

            return output;
        }
        #endregion

        #region Статистика
        /// <summary>
        /// Scaling, brings the vector to a range of 0-1
        /// </summary>
        /// <returns></returns>
        public Vector Scale()
        {
            double max = Max();
            double min = Min();
            double d = 1.0 / (max - min);

            return Transform(x => (x - min) / d);
        }
        /// <summary>
        /// Minimal value
        /// </summary>
        /// <returns></returns>
        public double Min()
        {
            double val = double.MaxValue;

            for (int i = 0; i < Count; i++)
            {
                if (this[i] < val && !double.IsNaN(this[i]))
                {
                    val = this[i];
                }
            }

            return val;
        }
        /// <summary>
        /// Maximum value
        /// </summary>
        /// <returns></returns>
        public double Max()
        {
            double val = double.MinValue;

            for (int i = 0; i < Count; i++)
            {
                if (this[i] > val && !double.IsNaN(this[i]))
                {
                    val = this[i];
                }
            }

            return val;
        }
        /// <summary>
        /// Maximum absolute value
        /// </summary>
        /// <returns></returns>
		public double MaxAbs()
        {
            double[] data = new double[Count];

            for (int i = 0; i < Count; i++)
            {
                data[i] = Math.Abs(this[i]);
            }

            return data.Max(x => double.IsNaN(x) ? double.MinValue : x);
        }
        /// <summary>
        /// Minimal absolute value
        /// </summary>
        /// <returns></returns>
        public double MinAbs()
        {
            double[] data = new double[Count];

            for (int i = 0; i < Count; i++)
            {
                data[i] = Math.Abs(this[i]);
            }

            return data.Min(x => double.IsNaN(x) ? double.MaxValue : x);
        }
        /// <summary>
        /// Arithmetic mean of a vector
        /// </summary>
        /// <returns></returns>
        public double Mean()
        {
            return Statistic.ExpectedValue(this);
        }
        /// <summary>
        /// Vector elements sum
        /// </summary>
        /// <returns></returns>
        public double Sum()
        {
            double sum = 0;

            for (int i = 0; i < Count; i++)
            {
                sum += this[i];
            }

            return sum;
        }
        /// <summary>
        /// Does the uncertainty vector contain
        /// </summary>
        /// <returns></returns>
        public bool ContainsNan()
        {
            for (int i = 0; i < Count; i++)
            {
                if (double.IsNaN(this[i]))
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Dispersion
        /// </summary>
        /// <returns></returns>
        public double Dispersion()
        {
            return Statistic.СalcVariance(this);
        }
        /// <summary>
        /// STD
        /// </summary>
        /// <returns></returns>
        public double Std()
        {
            return Statistic.CalcStd(this);
        }
        /// <summary>
        /// Vector norm
        /// </summary>
        /// <returns></returns>
        public double Norm()
        {
            return AnalyticGeometryFunctions.NormVect(this);
        }
        /// <summary>
        /// Normalization
        /// </summary>
        /// <returns></returns>
        public Vector Normalise()
        {
            return (Clone() - Mean()) / Std();
        }
        /// <summary>
        /// Normalization
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="std"></param>
        /// <returns></returns>
        public Vector Normalise(Vector mean, Vector std)
        {
            return (Clone() - mean) / std;
        }
        #endregion

        #region Поиск индексов
        /// <summary>
        /// Index of max element
        /// </summary>
        /// <returns></returns>
        public int MaxElementIndex()
        {
            int indMax = 0;

            for (int i = 1; i < Count; i++)
            {
                if (this[i] > this[indMax])
                {
                    indMax = i;
                }
            }

            return indMax;
        }
        /// <summary>
        /// Index of absolutely max element
        /// </summary>
        /// <returns></returns>
        public int AbsoluteMaxElementIndex()
        {
            int indMax = 0;
            Vector vector = Transform(x => Math.Abs(x));


            for (int i = 1; i < Count; i++)
            {
                if (vector[i] > vector[indMax])
                {
                    indMax = i;
                }
            }

            return indMax;
        }
        /// <summary>
        /// Index of min element
        /// </summary>
        /// <returns></returns>
        public int MinElementIndex()
        {
            int indMin = 0;

            for (int i = 1; i < Count; i++)
            {
                if (this[i] < this[indMin])
                {
                    indMin = i;
                }
            }

            return indMin;
        }
        /// <summary>
        /// Index of absolutely min element
        /// </summary>
        /// <returns></returns>
        public int AbsoluteMinElementIndex()
        {
            int indMin = 0;
            Vector vector = Transform(x => Math.Abs(x));

            for (int i = 1; i < Count; i++)
            {
                if (vector[i] < vector[indMin])
                {
                    indMin = i;
                }
            }

            return indMin;
        }
        /// <summary>
        /// Index of max element
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int MaxElementIndexInRegion(int a, int b)
        {
            int end = (b < Count) ? b + 1 : Count;

            int indMax = a;

            for (int i = a; i < end; i++)
            {
                if (this[i] > this[indMax])
                {
                    indMax = i;
                }
            }

            return indMax;
        }
        /// <summary>
        /// Index of absolutely max element
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int AbsoluteMaxElementIndexInRegion(int a, int b)
        {
            int end = (b < Count) ? b + 1 : Count;

            int indMax = a;
            Vector vector = Transform(x => Math.Abs(x));

            for (int i = 1; i < end; i++)
            {
                if (vector[i] > vector[indMax])
                {
                    indMax = i;
                }
            }

            return indMax;
        }
        /// <summary>
        /// Index of min element
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int MinElementIndexInRegion(int a, int b)
        {
            int end = (b < Count) ? b + 1 : Count;

            int indMin = a;

            for (int i = a; i < end; i++)
            {
                if (this[i] < this[indMin])
                {
                    indMin = i;
                }
            }

            return indMin;
        }
        /// <summary>
        /// Index of absolutely min element
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int AbsoluteMinElementIndexInRegion(int a, int b)
        {
            int end = (b < Count) ? b + 1 : Count;

            int indMin = a;
            Vector vector = Transform(x => Math.Abs(x));

            for (int i = 1; i < end; i++)
            {
                if (vector[i] < vector[indMin])
                {
                    indMin = i;
                }
            }

            return indMin;
        }
        // TODO: Пофиксить
        /// <summary>
        /// Closest minimal indexc
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public int IndexValueNeighborhoodMin(double value)
        {
            Vector abs = (value - this).Transform(Math.Abs);

            for (int i = 0; i < Count - 1; i++)
            {
                if (abs[i] < abs[i + 1])
                {
                    return i;
                }
            }

            return Count - 1;
        }
        #endregion

        #region Статические методы
        private static float[] Vector2SingleArray(Vector vector)
        {
            float[] array = new float[vector.Count];

            for (int i = 0; i < vector.Count; i++)
            {
                array[i] = (float)vector[i];
            }

            return array;
        }

        private static Vector SingleArray2Vector(float[] array)
        {
            Vector vector = new Vector(array.Length);

            for (int i = 0; i < vector.Count; i++)
            {
                vector[i] = array[i];
            }

            return vector;
        }


        /// <summary>
        /// Creates one vector from two vectors by applying the cross function for each element.
        /// </summary>
        /// <param name="x">First vector</param>
        /// <param name="y">Second vector</param>
        /// <param name="cross">Mixing function(crossing)</param>
        /// <returns></returns>
        public static Vector Crosser(Vector x, Vector y, Func<double, double, double> cross)
        {
            Vector outp = new Vector(x.Count);

            for (int i = 0; i < outp.Count; i++)
            {
                outp[i] = cross(x[i], y[i]);
            }

            return outp;
        }
        /// <summary>
        /// Connection of vectors with overlapping, by summation in the region overlap
        /// </summary>
        /// <param name="data">Vectots</param>
        /// <param name="col">Overlapping areas (collisions)</param>
        /// <returns/>
        public static Vector SummWithCollision(Vector[] data, int col = 0)
        {
            int shiftAll = (data.Length - 1) * col;
            int len = 0;

            for (int i = 0; i < data.Length; i++)
            {
                len += data[i].Count;
            }

            len -= shiftAll;

            Vector outp = new Vector(len);

            int ind = 0;

            for (int i = 0; i < data.Length; i++)
            {
                for (int j = (i != 0) ? col : 0, k = 0; j < data[i].Count; j++)
                {
                    if ((j < data[i].Count - col) || i == data.Length - 1)
                    {
                        outp[ind++] = data[i][j];
                    }
                    else
                    {
                        outp[ind + k] = data[i][j] + data[i + 1][k++]; // любая функция для вычисления отклика в обл. коллизии
                    }
                }
            }

            return outp;
        }
        /// <summary>
        /// Unipolar conversion of index (natural number) to one-hot vector, everywhere value is 0, but at the specified index is 1
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="maxInd">Maximal possible index</param>
        /// <returns>Vector with all zeroes exept the position</returns>
        public static Vector OneHotPol(int index, int maxInd)
        {
            Vector outp = new Vector(maxInd + 1)
            {
                [index] = 1
            };
            return outp;
        }
        /// <summary>
        ///  Unipolar conversion of index (natural number) to one-hot vector, everywhere value is -1, but at the specified index is 1
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="maxInd">Maximal possible index</param>
        /// <returns>Vector with all -1s exept the position</returns>
        public static Vector OneHotBePol(int index, int maxInd)
        {
            Vector outp = new Vector(maxInd + 1) - 1;
            outp[index] = 1;
            return outp;
        }
        /// <summary>
        /// Vector concatenation
        /// </summary>
        /// <param name="vectors">Vectors</param>
        /// <returns></returns>
        public static Vector Concat(Vector[] vectors)
        {
            int n = 0;

            for (int i = 0; i < vectors.Length; i++)
            {
                n += vectors[i].Count;
            }

            Vector resultVector = new Vector(n);

            for (int i = 0, k = 0; i < vectors.Length; i++)
            {
                for (int j = 0; j < vectors[i].Count; j++)
                {
                    resultVector[k++] = vectors[i][j];
                }
            }

            return resultVector;
        }
        /// <summary>
        /// Sequence that begins with zero
        /// </summary>
        /// <param name="step">Step</param>
        /// <param name="end">Last value</param>
        /// <returns></returns>
        public static Vector SeqBeginsWithZero(double step, double end)
        {
            return FunctionsForEachElements.GenerateTheSequence(0, step, end);
        }
        /// <summary>
        /// Sequence
        /// </summary>
        /// <param name="step">Step</param>
        /// <param name="end">Last value</param>
        /// <param name="start">First value</param>
        /// <returns></returns>
        public static Vector Seq(double start, double step, double end)
        {
            return FunctionsForEachElements.GenerateTheSequence(start, step, end);
        }
        /// <summary>
        /// Array os times
        /// </summary>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="t">Time (sec)</param>
        /// <returns></returns>
        public static Vector Time0(double fd, double t)
        {
            double step = 1.0 / fd;
            double end = t;
            return SeqBeginsWithZero(step, end);
        }
        /// <summary>
        /// Split to windows
        /// </summary>
        /// <param name="inp">Input</param>
        /// <param name="w">Window size</param>
        /// <param name="step">Step</param>
        /// <returns></returns>
        public static Vector[] GetWindows(Vector inp, int w, int step = 2)
        {
            List<Vector> list = new List<Vector>();
            double[] dat = inp.ToArray();

            for (int i = 0; i < inp.Count - w; i += step)
            {
                list.Add(Vector.GetIntervalDouble(i, i + w, dat));
            }

            return list.ToArray();
        }
        /// <summary>
        /// Split to windows
        /// </summary>
        /// <param name="transformer">Transformation function</param>
        /// <param name="inp">Input</param>
        /// <param name="w">Window size</param>
        /// <param name="step">Step</param>
        /// <returns></returns>
        public static Vector[] GetWindowsWithFunc(Func<Vector, Vector> transformer, Vector inp, int w, int step = 2)
        {
            Vector[] vects = GetWindows(inp, w, step);

            for (int i = 0; i < vects.Length; i ++)
            {
                vects[i] = transformer(vects[i]);
            }

            return vects;
        }
        /// <summary>
        /// Split to windows
        /// </summary>
        /// <param name="transformer">Transformation function</param>
        /// <param name="inp">Input</param>
        /// <param name="w">Window size</param>
        /// <param name="step">Step</param>
        /// <returns></returns>
        public static Vector GetWindowsWithFuncVect(Func<Vector, double> transformer, Vector inp, int w, int step = 2)
        {
            Vector[] vects = GetWindows(inp, w, step);
            Vector vect = new Vector(vects.Length);

            for (int i = 0; i < vects.Length; i++)
            {
                vect[i] = transformer(vects[i]);
            }

            return vect;
        }
        /// <summary>
        /// Data scaling, ensemble synchronization is performed for each case
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public static Vector[] ScaleData(Vector[] data)
        {
            Vector mean = Statistic.MeanVector(data);
            Vector std = Statistic.EnsembleDispersion(data);
            std = FunctionsForEachElements.Sqrt(std);
            std = std.Transform(x => x == 0 ? 1e-10 : x);


            Vector[] vects = new Vector[data.Length];


            for (int i = 0; i < data.Length; i++)
            {
                vects[i] = data[i] - mean;
                vects[i] /= std;
            }

            return vects;
        }
        /// <summary>
        /// Ensemble averaging
        /// </summary>
        /// <param name="vectors">Vector ensemble</param>
        /// <returns>Avarage vector</returns>
        public static Vector Mean(Vector[] vectors)
        {
            Vector result = new Vector(vectors[0].Count);

            for (int i = 0; i < vectors.Length; i++)
            {
                result += vectors[i];
            }

            return result / vectors.Length;
        }
        /// <summary>
		/// STD by ensemble
		/// </summary>
		/// <param name="vectors">Vector ensemble</param>
		/// <returns>Avarage vector</returns>
		public static Vector Std(Vector[] vectors)
        {
            return Statistic.EnsembleDispersion(vectors).Transform(Math.Abs);
        }
        /// <summary>
        /// Cast string to vector of letters
        /// </summary>
        /// <param name="str">String</param>
        /// <returns></returns>
        public static Vector GetCharVector(string str)
        {
            Vector outp = new Vector(str.Length);

            for (int i = 0; i < outp.Count; i++)
            {
                outp[i] = str[i];
            }

            return outp;
        }
        #endregion

        #region Статические методы инициализации
        /// <summary>
        /// Initialize vector from string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector Parse(string str)
        {
            return Parse(str, AISettings.GetProvider());
        }
        /// <summary>
        /// Initialize vector from string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Vector Parse(string str, NumberFormatInfo provider)
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

            if (!trimmed.StartsWith("[") || !trimmed.EndsWith("]"))
            {
                throw new FormatException("Input string is in the wrong format");
            }

            if (trimmed == "[]")
            {
                Vector res = new Vector();
                res.Clear();
                return res;
            }

            string content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            string[] nums = content.Split(' ');

            return FromStrings(nums, provider);
        }
        /// <summary>
        /// Tries to initialize vector from string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out Vector result)
        {
            return TryParse(str, out result, AISettings.GetProvider());
        }
        /// <summary>
        /// Tries to initialize vector from string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out Vector result, NumberFormatInfo provider)
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

            if (!trimmed.StartsWith("[") || !trimmed.EndsWith("]"))
            {
                result = null;
                return false;
            }

            if (trimmed == "[]")
            {
                Vector empty = new Vector();
                empty.Clear();
                result = empty;
                return true;
            }

            string content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            string[] nums = content.Split(' ');

            Vector res = new Vector();
            res.Clear();

            foreach (string strNum in nums)
            {
                if (!double.TryParse(strNum, NumberStyles.Number, provider, out double num))
                {
                    result = null;
                    return false;
                }

                res.Add(num);
            }

            result = res;
            return true;
        }
        /// <summary>
        /// Initialize vector from string array
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Vector FromStrings(string[] arr)
        {
            return FromStrings(arr, AISettings.GetProvider());
        }
        /// <summary>
        /// Initialize vector from string array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Vector FromStrings(string[] arr, NumberFormatInfo provider)
        {
            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Vector result = new Vector(arr.Length);
            result.Clear();

            foreach (string str in arr)
            {
                result.Add(double.Parse(str, provider));
            }

            return result;
        }
        /// <summary>
        /// Cast list to vector
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Vector FromList(IList<double> list)
        {
            return new Vector(list.ToArray());
        }
        /// <summary>
        /// Cast read-only list to vector
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Vector FromReadOnlyList(IReadOnlyList<double> list)
        {
            return new Vector(list.ToArray());
        }
        #endregion

        #region Технические методы
        public override string ToString()
        {
            return ToString(AISettings.GetProvider());
        }

        public string ToString(NumberFormatInfo numberFormatInfo)
        {
            if (Count == 0)
            {
                return "[]";
            }

            StringBuilder str = new StringBuilder();
            str.Append("[");

            for (int i = 0; i < Count; i++)
            {
                str.Append(this[i].ToString(numberFormatInfo));
                str.Append(" ");
            }

            str.Length--;
            str.Append("]");
            return str.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector vector)
            {
                return vector == this;
            }
            else if (obj is List<double> dList)
            {
                return dList == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Vector other)
        {
            return this == other;
        }

        public bool Equals(List<double> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (double val in this)
                {
                    hash = (hash * 23) + val.GetHashCode();
                }
                return hash;
            }
        }
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Saves vector to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Saves vector to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Saves vector to file in text format
        /// </summary>
        /// <param name="path"></param>
        public void SaveAsText(string path)
        {
            File.WriteAllText(path, ToString());
        }
        /// <summary>
        /// Represents vector as an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.Vector).Write(ToArray()).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Loads vector from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Vector Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File was not found", path);
            }

            return BinarySerializer.Load<Vector>(path);
        }

        /// <summary>
        /// Загрузка вектора из массива double
        /// </summary>
        /// <param name="path">Путь</param>
        public static Vector LoadAsBinary(string path)
        {
            Vector vect = new Vector();
            int len;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                len = (int)(fs.Length / 8);
                BinaryReader br = new BinaryReader(fs);
                vect = new Vector(len);

                for (int i = 0; i < len; i++)
                {
                    vect[i] = br.ReadDouble();
                }

            }


            return vect;
        }
        /// <summary>
        /// Сохранение в массив double
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <param name="vect">Вектор</param>
        public static void SaveAsBinary(string path, Vector vect)
        {

            using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            for (int i = 0; i < vect.Count; i++)
            {
                fs.Write(BitConverter.GetBytes(vect[i]), 0, 8);
            }
        }

        /// <summary>
        /// Loads vector from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Vector Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return BinarySerializer.Load<Vector>(stream);
        }
        /// <summary>
        /// Loads vector from text file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Vector LoadAsText(string path)
        {
            return Parse(File.ReadAllText(path));
        }
        /// <summary>
        /// Initializes vector from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Vector FromBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return FromDataStream(new InMemoryDataStream(data));
        }
        /// <summary>
        /// Initilizes vector from data stream
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public static Vector FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            return dataStream.SkipIfEqual(KeyWords.Vector).ReadDoubles();
        }
        #endregion

        #endregion
    }
}