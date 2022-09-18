using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vector = AI.DataStructs.Algebraic.Vector;

namespace AI.DataStructs.WithComplexElements
{
    /// <summary>
    /// Представляет вектор комплексных чисел
    /// </summary>
    [Serializable]
    public class ComplexVector : List<Complex>, IComplexStructure, ISavable, IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Массив комплексных чисел
        /// </summary>
        Complex[] IComplexStructure.Data => ToArray();
        /// <summary>
        /// Форма вектора
        /// </summary>
        public Shape Shape => new Shape1D(Count);
        /// <summary>
        /// Реальная (действительная) часть комплексного вектора 
        /// </summary>
        public Vector RealVector
        {
            get
            {
                Vector ret = new Vector(Count);

                for (int i = 0; i < Count; i++)
                {
                    ret[i] = this[i].Real;
                }

                return ret;
            }
        }
        /// <summary>
        /// Мнимая часть комплексного вектора
        /// </summary>
        public Vector ImaginaryVector
        {
            get
            {
                Vector ret = new Vector(Count);

                for (int i = 0; i < Count; i++)
                {
                    ret[i] = this[i].Imaginary;
                }

                return ret;
            }
        }
        /// <summary>
        /// Модуль комплексного вектора
        /// </summary>
        public Vector MagnitudeVector
        {
            get
            {
                Vector ret = new Vector(Count);

                for (int i = 0; i < Count; i++)
                {
                    ret[i] = this[i].Magnitude;
                }

                return ret;
            }
        }
        /// <summary>
        /// Фаза комплексного вектора
        /// </summary>
        public Vector PhaseVector
        {
            get
            {
                Vector ret = new Vector(Count);

                for (int i = 0; i < Count; i++)
                {
                    ret[i] = this[i].Phase;
                }

                return ret;
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Creates a vector with zeros (0 + 0j) of capacity 3
        /// </summary>
        public ComplexVector() : base(3) { AddRange(new Complex[3]); }
        /// <summary>
        /// Creates a vector with zeros (0 + 0j) of dimension n
        /// </summary>
        /// <param name="n"></param>
        public ComplexVector(int n) : base(n) { AddRange(new Complex[n]); }
        /// <summary>
        /// Creates a vector of dimension 1 with the given value
        /// </summary>
        /// <param name="value"></param>
        public ComplexVector(Complex value)
        {
            Add(value);
        }
        /// <summary>
        /// Creates a vector from the IEnumerable interface of Complex
        /// </summary>
        /// <param name="data"></param>
        public ComplexVector(IEnumerable<Complex> data)
        {
            AddRange(data);
        }
        /// <summary>
        /// Creates a vector based on arrays of real and imaginary parts
        /// </summary>
        /// <param name="vectorReal">Real part</param>
        /// <param name="vectorImg">Imaginary part</param>
        public ComplexVector(double[] vectorReal, double[] vectorImg)
        {
            if (vectorReal == null)
            {
                throw new ArgumentNullException(nameof(vectorReal));
            }

            if (vectorImg == null)
            {
                throw new ArgumentNullException(nameof(vectorImg));
            }

            if (vectorReal.Length != vectorImg.Length)
            {
                throw new InvalidOperationException("Lengths of real and imaginary arrays mismatch");
            }

            Init(vectorReal, vectorImg);
        }
        /// <summary>
        /// Creates a vector based on arrays of real and imaginary parts
        /// </summary>
        /// <param name="vectorReal">Real part</param>
        /// <param name="vectorImg">Imaginary part</param>
        public ComplexVector(Vector vectorReal, Vector vectorImg)
        {
            if (vectorReal == null)
            {
                throw new ArgumentNullException(nameof(vectorReal));
            }

            if (vectorImg == null)
            {
                throw new ArgumentNullException(nameof(vectorImg));
            }

            if (vectorReal.Count != vectorImg.Count)
            {
                throw new InvalidOperationException("Lengths of real and imaginary arrays mismatch");
            }

            Init(vectorReal, vectorImg);
        }
        /// <summary>
        /// Creates a vector based on array of real part, imaginary filled with zeros
        /// </summary>
        /// <param name="vectorReal">Real part</param>
        public ComplexVector(double[] vectorReal)
        {
            if (vectorReal == null)
            {
                throw new ArgumentNullException(nameof(vectorReal));
            }

            Vector vectorImg = new Vector(vectorReal.Length);
            Init(vectorReal, vectorImg);
        }
        /// <summary>
        /// Creates a vector based on vectors of real part, imaginary filled with zeros
        /// </summary>
        /// <param name="vectorReal">Real part</param>
        public ComplexVector(Vector vectorReal)
        {
            if (vectorReal == null)
            {
                throw new ArgumentNullException(nameof(vectorReal));
            }

            Vector vectorImg = new Vector(vectorReal.Count);
            Init(vectorReal, vectorImg);
        }
        #endregion

        #region Операторы
        /// <summary>
        /// Поэлементное сложение
        /// </summary>
        /// <param name="left">Первый вектор</param>
        /// <param name="right">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator +(ComplexVector left, ComplexVector right)
        {
            if (left.Count != right.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(left.Count);

            for (int i = 0; i < left.Count; i++)
            {
                C[i] = left[i] + right[i];
            }

            return C;
        }
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator +(Complex k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k + vector[i];
            }

            return C;
        }
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator +(ComplexVector vector, Complex k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = vector[i] + k;
            }

            return C;
        }
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator +(double k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k + vector[i];
            }

            return C;
        }
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator +(ComplexVector vector, double k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = vector[i] - k;
            }

            return C;
        }
        /// <summary>
        /// Отрицание
        /// </summary>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator -(ComplexVector vector)
        {
            return 0.0 - vector;
        }
        /// <summary>
        /// Поэлементное вычитание
        /// </summary>
        /// <param name="left">Первый вектор</param>
        /// <param name="right">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator -(ComplexVector left, ComplexVector right)
        {
            if (left.Count != right.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(left.Count);

            for (int i = 0; i < left.Count; i++)
            {
                C[i] = left[i] - right[i];
            }

            return C;
        }
        /// <summary>
        /// Вычитание из числа
        /// </summary>
        /// <param name="k">комплексное число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator -(Complex k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k - vector[i];
            }

            return C;
        }
        /// <summary>
        /// Вычитание числа
        /// </summary>
        /// <param name="k">комплексное число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator -(ComplexVector vector, Complex k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = vector[i] - k;
            }

            return C;
        }
        /// <summary>
        /// Вычитание из числа
        /// </summary>
        /// <param name="k">реальное число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator -(double k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k - vector[i];
            }

            return C;
        }
        /// <summary>
        /// Вычитание числа
        /// </summary>
        /// <param name="k"> число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator -(ComplexVector vector, double k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = vector[i] - k;
            }

            return C;
        }
        /// <summary>
        /// Умножение
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator *(ComplexVector vector, Complex k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k * vector[i];
            }

            return C;
        }
        /// <summary>
        /// Поэлементное умножение на реальный вектор
        /// </summary>
        /// <param name="left">Первый вектор</param>
        /// <param name="right">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator *(ComplexVector left, Vector right)
        {
            if (left.Count != right.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(left.Count);

            for (int i = 0; i < left.Count; i++)
            {
                C[i] = left[i] * right[i];
            }

            return C;
        }
        /// <summary>
        /// Поэлементное умножение на реальный вектор
        /// </summary>
        /// <param name="right">Первый вектор</param>
        /// <param name="left">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator *(Vector left, ComplexVector right)
        {
            if (right.Count != left.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(right.Count);

            for (int i = 0; i < right.Count; i++)
            {
                C[i] = right[i] * left[i];
            }

            return C;
        }
        /// <summary>
        /// Multiplication by a number
        /// </summary>
        /// <param name="k">комплексное число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator *(Complex k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k * vector[i];
            }

            return C;
        }
        /// <summary>
        /// Поэлементное умножение
        /// </summary>
        /// <param name="left">Первый вектор</param>
        /// <param name="right">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator *(ComplexVector left, ComplexVector right)
        {
            if (left.Count != right.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(left.Count);

            for (int i = 0; i < left.Count; i++)
            {
                C[i] = left[i] * right[i];
            }

            return C;
        }

        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator /(Complex k, ComplexVector vector)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = k / vector[i];
            }

            return C;
        }
        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="vector">Комплексный вектор</param>
        /// <returns>Комплексный вектор</returns>
        public static ComplexVector operator /(ComplexVector vector, Complex k)
        {
            ComplexVector C = new ComplexVector(vector.Count);

            for (int i = 0; i < vector.Count; i++)
            {
                C[i] = vector[i] / k;
            }

            return C;
        }
        /// <summary>
        /// Поэлементное деление
        /// </summary>
        /// <param name="left">Первый вектор</param>
        /// <param name="right">Второй</param>
        /// <returns>Результат</returns>
        public static ComplexVector operator /(ComplexVector left, ComplexVector right)
        {
            if (left.Count != right.Count)
            {
                throw new InvalidOperationException("Lengths of given vectors mismatch");
            }

            ComplexVector C = new ComplexVector(left.Count);

            for (int i = 0; i < left.Count; i++)
            {
                C[i] = left[i] / right[i];
            }

            return C;
        }
        /// <summary>
        /// Implicit cast ComplexVector -> Complex[]
        /// </summary>
        /// <param name="vect">Вектор</param>
        public static implicit operator Complex[](ComplexVector vect)
        {
            return vect.ToArray();
        }
        /// <summary>
        /// Implicit cast Complex[] -> ComplexVector
        /// </summary>
        /// <param name="dbs">Complex array</param>
        public static implicit operator ComplexVector(Complex[] dbs)
        {
            return new ComplexVector(dbs);
        }
        /// <summary>
        /// Implicit cast double[] -> ComplexVector
        /// </summary>
        /// <param name="dbs">Double array</param>
        public static implicit operator ComplexVector(double[] dbs)
        {
            return new ComplexVector(dbs);
        }
        /// <summary>
        /// Implicit cast Vector -> ComplexVector
        /// </summary>
        /// <param name="dbs"></param>
        public static implicit operator ComplexVector(Vector dbs)
        {
            return new ComplexVector(dbs);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Zero padding or cropping to the desired vector size.
        /// </summary>
        /// <param name="n">New dimension</param>
        public ComplexVector CutAndZero(int n)
        {
            ComplexVector x = new ComplexVector(n);

            if (n > Count)
            {
                for (int i = 0; i < Count; i++)
                {
                    x[i] = this[i];
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    x[i] = this[i];
                }
            }

            return x;
        }
        /// <summary>
        /// Vector cloning
        /// </summary>
        public ComplexVector Clone()
        {
            return new ComplexVector(this);
        }
        /// <summary>
        /// Vector reverse (mirror image)
        /// </summary>
        public ComplexVector Revers()
        {
            Complex[] newVect = new Complex[Count];

            for (int i = 0; i < Count; i++)
            {
                newVect[i] = this[Count - i - 1];
            }

            return new ComplexVector(newVect);
        }
        /// <summary>
        ///  Shift the sequence by a certain number.Example: the sequence 1 2 3 is shifted by 2 - this is {0 0 1 2 3}, by 4 - {0 0 0 0 1 2 3}
        /// </summary>
        /// <param name="valueShift">Shift amount</param>
        public ComplexVector Shift(int valueShift)
        {
            int count = Count + valueShift;
            Complex[] newVect = new Complex[count];

            for (int i = 0; i < valueShift; i++)
            {
                newVect[i] = new Complex(0, 0);
            }

            for (int i = valueShift; i < count; i++)
            {
                newVect[i] = this[i - valueShift];
            }

            return new ComplexVector(newVect);
        }
        /// <summary>
        /// Centering an array of values ​​obtained by the Fourier transform
        /// </summary>
        public ComplexVector FurCentr()
        {
            Complex[] centr = new Complex[Count];
            for (int i = 0; i < Count / 2; i++)
            {
                centr[i] = this[(Count / 2) + i];
                centr[(Count / 2) + i] = this[i];
            }
            return new ComplexVector(centr);
        }
        /// <summary>
        /// Decimation (thinning) vector
        /// </summary>
        /// <param name="kDecim">Decimation factor</param>
        public ComplexVector Decimation(int kDecim)
        {
            ComplexVector ret;

            if (Count % kDecim == 0)
            {
                ret = new ComplexVector(Count / kDecim);
            }
            else
            {
                ret = new ComplexVector((Count / kDecim) + 1);
            }

            int k = 0;

            for (int i = 0; i < Count; i += kDecim)
            {
                ret[k++] = this[i];
            }

            return ret;
        }
        /// <summary>
        /// Adding a reflected Vector
        /// </summary>
        public ComplexVector AddSimmetr()
        {
            int n2 = 2 * Count;
            ComplexVector newVector = new ComplexVector(n2);

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
        /// Interpolation by a polynomial of order zero
        /// </summary>
        /// <param name="kInterp">Interpolation factor</param>
        public ComplexVector InterpolayZero(int kInterp)
        {
            ComplexVector ret = new ComplexVector(Count * kInterp);

            for (int i = 0; i < ret.Count; i++)
            {
                ret[i] = this[i / kInterp];
            }

            return ret;
        }
        /// <summary>
        /// Element-wise vector transformation
        /// </summary>
        /// <param name="func">Conversion function</param>
        public ComplexVector Transform(Func<Complex, Complex> func)
        {
            ComplexVector cVect = new ComplexVector(Count);

            for (int i = 0; i < Count; i++)
            {
                cVect[i] = func(this[i]);
            }

            return cVect;
        }
        /// <summary>
        /// Element-wise vector transformation
        /// </summary>
        /// <param name="func">Conversion function</param>
		public void TransformSelf(Func<Complex, Complex> func)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] = func(this[i]);
            }
        }
        /// <summary>
        /// Complex conjugate number
        /// </summary>
        public ComplexVector ComplexConjugate()
        {
            return Transform(Complex.Conjugate);
        }
        /// <summary>
        /// Complex conjugate number
        /// </summary>
        public void ComplexConjugateSelf()
        {
            TransformSelf(Complex.Conjugate);
        }
        /// <summary>
        /// Arithmetic mean of a complex vector
        /// </summary>
        public Complex Mean()
        {
            Complex av = new Complex(0, 0);

            for (int i = 0; i < Count; i++)
            {
                av += this[i];
            }

            return av / Count;
        }
        #endregion

        #region Статические методы
        /// <summary>
        /// Converting the vector of phases and amplitudes into a complex vector
        /// </summary>
        /// <param name="magn">Amplitude vector</param>
        /// <param name="phase"> Phase vector(rad)</param>
        public static ComplexVector ComplexVectorPhaseMagn(Vector magn, Vector phase)
        {
            ComplexVector complexVector = new ComplexVector(magn.Count);
            Complex j = new Complex(0, 1);

            for (int i = 0; i < complexVector.Count; i++)
            {
                complexVector[i] = magn[i] * Complex.Exp(-j * phase[i]);
            }

            return complexVector;
        }

        /// <summary>
        /// Converting the vector of phases and amplitudes into a complex vector
        /// </summary>
        /// <param name="magnDb">Amplitude vector(db)</param>
        /// <param name="phaseDeg"> Phase vector(deg)</param>
        /// <param name="dbType">Тип дб по энергия/амплитуда</param>
        public static ComplexVector ComplexVectorPhaseDegMagnDb(Vector magnDb, Vector phaseDeg, DbType dbType = DbType.Energy)
        {
            Vector phaseRad = FunctionsForEachElements.GradToRad(phaseDeg);
            Vector magn = (dbType == DbType.Energy) ? magnDb.Transform(x => Math.Pow(10, x / 10.0)) : magnDb.Transform(x => Math.Pow(10, x / 20.0));
            return ComplexVectorPhaseMagn(magn, phaseRad);
        }

        /// <summary>
        /// Vector transformation(A vector of real arguments is used)
        /// </summary>
        /// <param name="transformFunc">Conversion function, a function of the value of a vector of arguments</param>
        /// <param name="x">Argument vector</param>
        public static ComplexVector TransformVectorX(Vector x, Func<double, Complex> transformFunc)
        {
            ComplexVector output = new ComplexVector(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                output[i] = transformFunc(x[i]);
            }

            return output;
        }

        /// <summary>
        /// Vector transformation(Using a vector of complex arguments)
        /// </summary>
        /// <param name="transformFunc"> Conversion function, a function of the value of a vector of arguments</param>
        /// <param name="x">Argument vector</param>
        public static ComplexVector TransformVectorX(ComplexVector x, Func<Complex, Complex> transformFunc)
        {
            ComplexVector output = new ComplexVector(x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                output[i] = transformFunc(x[i]);
            }

            return output;
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
        /// Represents vector as an array of bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.ComplexVector).Write(RealVector).WriteOnlyContent(ImaginaryVector).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Loads vector from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ComplexVector Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File was not found", path);
            }

            return BinarySerializer.Load<ComplexVector>(path);
        }
        /// <summary>
        /// Loads vector from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ComplexVector Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return BinarySerializer.Load<ComplexVector>(stream);
        }
        /// <summary>
        /// Initializes vector from byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ComplexVector FromBytes(byte[] data)
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
        public static ComplexVector FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            int length = dataStream.SkipIfEqual(KeyWords.ComplexVector).ReadInt();

            double[] reals = dataStream.ReadDoubles(length);
            double[] imgs = dataStream.ReadDoubles(length);

            return new ComplexVector(reals, imgs);
        }
        #endregion

        #endregion

        #region Приватные методы
        private void Init(double[] vectorReal, double[] vectorImg)
        {
            Capacity = vectorReal.Length;
            Clear();

            for (int i = 0; i < vectorReal.Length; i++)
            {
                Add(new Complex(vectorReal[i], vectorImg[i]));
            }
        }
        #endregion
    }

    /// <summary>
    /// Decibel type
    /// </summary>
    public enum DbType
    {
        /// <summary>
        /// Energetic
        /// </summary>
        Energy,
        /// <summary>
        /// Amplitude
        /// </summary>
        Ampl
    }
}