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
    /// Класс, реализующий вектор и операции над ним
    /// </summary>
    [Serializable]
    public class Vector : List<double>, IAlgebraicStructure<double>, IEquatable<Vector>, IEquatable<List<double>>, ISavable, ITextSavable, IByteConvertable
    {
        #region Поля и свойства
        /// <summary>
        /// Данные вектора
        /// </summary>
        double[] IAlgebraicStructure<double>.Data => ToArray();
        /// <summary>
        /// Форма (размерность вектора)
        /// </summary>
        public Shape Shape => new Shape1D(Count);


        /// <summary>
        /// Получение значения по индексу, аналогично как Python(поддержка отрицательных индексов)
        /// </summary>
        /// <param name="i">Индекс</param>
        public new double this[int i]
        {
            get
            {
                if (i >= 0) return base[i];
                else return base[Count + i];
            }
            set
            {
                if (i >= 0) base[i] = value;
                else base[Count + i] = value;
            }
        }
        /// <summary>
        /// Получение или установка значений по маске, аналогично как Python
        /// </summary>
        /// <param name="mask">Маска (true - позиции для вставки или извлечения)</param>
        /// <exception cref="Exception">Возникает исключение при несоответствии числа позиций для вставки и размерности вектора для вставки</exception>
        public Vector this[bool[] mask]
        {
            get
            {
                int count = 0;
                for (int i = 0; i < mask.Length; i++) if (mask[i]) count++;

                Vector result = new Vector(count);

                for (int i = 0, j = 0; i < mask.Length; i++)
                    if (mask[i]) result[j++] = this[i];

                return result;
            }
            set
            {
                int count = 0;
                for (int i = 0; i < mask.Length; i++) if (mask[i]) count++;

                if (value.Count != count)
                    throw new Exception("Число позиций для вставки в маске должно совпадать с размерностью вектора");



                for (int i = 0, j = 0; i < mask.Length; i++)
                {
                    if (mask[i]) this[i] = value[j++];
                }
            }
        }
        /// <summary>
        /// Получение среза, аналогично как Python(поддержка отрицательных индексов и шагов)
        /// </summary>
        /// <param name="start">Начало</param>
        /// <param name="end">Конец</param>
        /// <param name="step">Шаг (если отрицательный, то последовательность переворачивается)</param>
        public Vector this[int? start, int? end, int step = 1]
        {
            get
            {
                int a = 0;
                int b = Count;

                if (start != null)
                    a = start.Value >= 0 ? start.Value : Count + start.Value;

                if (end != null)
                    b = end.Value >= 0 ? end.Value : Count + end.Value;

                int s = Math.Abs(step);

                Vector ret = new Vector((b - a) / s);

                for (int i = a, j = 0; i < b; i += s)
                    if (j < ret.Count) ret[j++] = base[i];

                return step < 0 ? ret.Revers() : ret;
            }

            set
            {
                int a = 0;
                int b = Count;

                if (start != null)
                    a = start.Value >= 0 ? start.Value : Count + start.Value;

                if (end != null)
                    b = end.Value >= 0 ? end.Value : Count + end.Value;

                int s = Math.Abs(step);
                Vector inp = step >= 0 ? value : value.Revers();

                for (int i = a, j = 0; i < b; i += s)
                    if (j < inp.Count)
                        base[i] = inp[j++];
            }
        }

        #endregion

        #region Конструкторы
        /// <summary>
        /// Создает вектор емкости 0
        /// </summary>
        public Vector() : base(0) { AddRange(new double[0]); }
        /// <summary>
        /// Создает вектор емкости n
        /// </summary>
        /// <param name="n"></param>
        public Vector(int n) : base(n) { AddRange(new double[n]); }
        /// <summary>
        /// Создает вектор размерности 1 с заданным значением
        /// </summary>
        /// <param name="value"></param>
        public Vector(double value)
        {
            Add(value);
        }
        /// <summary>
        /// Создает вектор из массива чисел типа double
        /// </summary>
        /// <param name="vector"></param>
        public Vector(params double[] vector)
        {
            AddRange(vector);
        }
        /// <summary>
        /// Создает вектор из интерфейса IEnumerable double
        /// </summary>
        /// <param name="data"></param>
        public Vector(IEnumerable<double> data)
        {
            AddRange(data);
        }
        /// <summary>
        /// Создает вектор из интерфейса IEnumerable float
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
                throw new InvalidOperationException("Размерности векторов не совпадают");
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
        /// Вычитание
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
        /// Вычитание
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
        /// Вычитание
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
                throw new InvalidOperationException("Размерности векторов не совпадают");
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
                throw new InvalidOperationException("Размерности векторов не совпадают");
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
                throw new InvalidOperationException("Размерности векторов не совпадают");
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
                throw new InvalidOperationException("Размерности векторов не совпадают");
            }

            Vector C = new Vector(n1);
            for (int i = 0; i < n1; i++)
            {
                C[i] = A[i] % B[i];
            }

            return C;
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
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

            if (A!.Count != B!.Count)
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

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator !=(Vector A, Vector B)
        {
            return !(A == B);
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator ==(Vector left, IList<double> right)
        {
            return left == FromList(right);
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator !=(Vector left, IList<double> right)
        {
            return left != FromList(right);
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator ==(List<double> left, Vector right)
        {
            return FromList(left) == right;
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public static bool operator !=(List<double> left, Vector right)
        {
            return FromList(left) != right;
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
        public static implicit operator double[](Vector vector)
        {
            return vector.ToArray();
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
        public static implicit operator Vector(double[] data)
        {
            return new Vector(data);
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
        public static implicit operator Vector(int[] data)
        {
            Vector outp = new Vector(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                outp[i] = data[i];
            }

            return outp;
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
        public static implicit operator Vector(float[] data)
        {
            return SingleArray2Vector(data);
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
        public static explicit operator float[](Vector data)
        {
            return Vector2SingleArray(data);
        }

        /// <summary>
        /// Преобразование типа
        /// </summary>
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
        /// Декомпозиция вектора, где каждая компонента представляется отдельным вектором
        /// [a, b, c, d] -> [[a], [b], [c], [d]]
        /// </summary>
        /// <returns></returns>
        public Vector[] Decomposition()
        {
            Vector[] vects = new Vector[Count];

            for (int i = 0; i < Count; i++)
                vects[i] = new Vector(this[i]);

            return vects;
        }

        /// <summary>
        /// Добавление числа в циклический буфер
        /// </summary>
        public void AddCB(double item)
        {
            int len = Count;
            Vector data = new Vector(len);

            for (int i = 1; i < len; i++)
                data[i] = this[i - 1];

            data[0] = item;
            for (int i = 0; i < len; i++) this[i] = data[i];

        }

        /// <summary>
        /// Добавление в конец циклического буфера
        /// </summary>
        public void AddCBE(double item)
        {
            int len = Count; // Длинна вектора

            for (int i = 1; i < len; i++)
                base[i - 1] = base[i];

            base[len - 1] = item;
        }

        /// <summary>
        /// Замена неопределенности на указанное число
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector NanToValue(double value = 0)
        {
            Vector outpVect = new Vector(Count);

            for (int i = 0; i < outpVect.Count; i++)
                outpVect[i] = double.IsNaN(this[i]) ? value : this[i];

            return outpVect;
        }
        /// <summary>
        /// Замена неопределенности на среднее
        /// </summary>
        /// <returns></returns>
        public Vector NanToMean()
        {
            double mean = Mean();
            Vector outpVect = new Vector(Count);

            for (int i = 0; i < outpVect.Count; i++)
                outpVect[i] = double.IsNaN(this[i]) ? mean : this[i];


            return outpVect;
        }
        /// <summary>
        /// Повтор вектора
        /// </summary>
        /// <param name="count">Число повторов</param>
        public Vector Repeat(int count)
        {
            int k = 0, len = Count * count;
            Vector ret = new Vector(len);

            for (int i = 0; i < count; i++)
                for (int j = 0; j < Count; j++)
                    ret[k++] = this[j];

            return ret;
        }
        /// <summary>
        /// Косинусное расстояние между векторами
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public double Cos(Vector vect)
        {
            return AnalyticGeometryFunctions.Cos(vect, this);
        }

        /// <summary>
        /// Получение вектора с единицей в позиции индекса с максимальным значением и -1 в остальных
        /// </summary>
        /// <param name="max">Значение в максимуме</param>
        /// <param name="rest">Значение в на остальных позициях</param>
        /// <returns></returns>
        public Vector MaxOutVector(double max = 1, double rest = -1)
        {
            int ind = MaxElementIndex();
            Vector ret = new Vector(Count) + rest;
            ret[ind] = max;
            return ret;
        }
        /// <summary>
        /// Вектор направления (с единичной длинной) 
        /// </summary>
        /// <returns></returns>
        public Vector GetUnitVector()
        {
            return this / NormL2();
        }
        /// <summary>
        /// Окугление
        /// </summary>
        /// <param name="num"></param>
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
        /// Удаление выбранных элементов
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(Vector elements)
        {
            List<double> lD = Clone();

            foreach (double element in elements)
            {
                _ = lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Удаление выбранных элементов
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(double[] elements)
        {
            List<double> lD = Clone();

            foreach (double element in elements)
            {
                _ = lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Удаление выбранных элементов
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Vector ElementsDel(List<double> elements)
        {
            List<double> lD = new List<double>();
            lD.AddRange(Clone());

            foreach (double element in elements)
            {
                _ = lD.Remove(element);
            }

            return FromList(lD);
        }
        /// <summary>
        /// Вернуть регион [a; b)
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
        /// Вернуть регион [a; b)
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
        /// Клонирование(копирование) вектора
        /// </summary>
        /// <returns></returns>
        public Vector Clone()
        {
            return new Vector(ToArray());
        }
        /// <summary>
        /// Добавление зеркально отраженного вектора к текущему
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
        /// Изменение порядка следования компонент вектора {1,2,3} -> {3,2,1}
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
        /// Обрезка или заполнение нулями вектора до нужного размера 
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
        /// Сдвиг на несколько единиц {1,0, 2, 3} -3-> {0, 0, 0, 1, 0, 2, 3}
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
        /// Перевод вектора в матрицу
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
        /// Прореживание (без фильтра)
        /// </summary>
        /// <param name="n"></param>
        public Vector Downsampling(int n)
        {
            Vector C = (Count % n == 0) ? new Vector(Count / n) : new Vector((Count / n) + 1);

            for (int i = 0, j = 0; i < Count; i += n, j++)
                C[j] = this[i];

            return C;
        }
        /// <summary>
        /// Увеличение размерности (аналог Up Sampling) 
        /// </summary>
        /// <param name="kUnPool">Восколько раз увеличить размерность</param>
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
        /// Ступенчатая интерполяция
        /// </summary>
        /// <param name="kInterp"></param>
        /// <returns></returns>
        public Vector InterpolayrZero(int kInterp)
        {
            Vector C = new Vector(Count * kInterp);

            for (int i = 0; i < C.Count; i++)
                C[i] = this[i / kInterp];


            return C;
        }
        /// <summary>
        /// Добавить единицу в начало
        /// </summary>
        public Vector AddOne()
        {
            Vector C = Shift(1);
            C[0] = 1;
            return C;
        }
        /// <summary>
        /// Является ли вектор нулевым
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
        /// Проверяет, содержит ли вектор более n нулевых элементов
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
        /// Поэлементное изменение вектора с помощью функции transformFunc
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
		/// Преобразование вектора
		/// </summary>
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
        /// Преобразование вектора
        /// </summary>
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
        /// Преобразование вектора с использование вектора параметра(аргумента)
        /// output[i] = transformFunc(x[i], this[i]);
        /// </summary>
        public Vector TransformWithArguments(Vector x, Func<double, double, double> transformFunc)
        {
            if (x.Count != Count)
            {
                throw new InvalidOperationException("Length of Вектор входа doesn't match the length of current");
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
        /// Нормализация вектора от 0 до 1
        /// </summary>
        /// <returns></returns>
        public Vector Minimax()
        {
            double max = Max();
            double min = Min();
            double d = 1.0 / (max - min + double.Epsilon);

            return Transform(x => (x - min) * d);
        }
        /// <summary>
        /// Минимальное значение
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
        ///  Максимальное значение
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
        /// Максимальное(по модулю) значение
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
        /// Минимальное(по модулю) значение
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
        /// Среднее арифметическое
        /// </summary>
        /// <returns></returns>
        public double Mean()
        {
            return Statistic.ExpectedValue(this);
        }
        /// <summary>
        /// Сумма компонент вектора
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
        /// Содержит ли вектор Nan
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
        /// Дисперсия
        /// </summary>
        /// <returns></returns>
        public double Dispersion()
        {
            return Statistic.СalcVariance(this);
        }
        /// <summary>
        ///  Среднеквадратичное отклонение
        /// </summary>
        /// <returns></returns>
        public double Std()
        {
            return Statistic.CalcStd(this);
        }
        /// <summary>
        /// L2 норма вектора
        /// </summary>
        /// <returns></returns>
        public double NormL2()
        {
            return AnalyticGeometryFunctions.NormVect(this);
        }
        /// <summary>
        /// Нормализация (ско = 1, среднее = 0)
        /// </summary>
        /// <returns></returns>
        public Vector ZNormalise()
        {
            return (Clone() - Mean()) / (Std() + AISettings.GlobalEps);
        }
        /// <summary>
        /// Нормализация (ско = 1, среднее = 0)
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="std"></param>
        /// <returns></returns>
        public Vector Normalise(Vector mean, Vector std)
        {
            return (Clone() - mean) / (std + AISettings.GlobalEps);
        }
        #endregion

        #region Поиск индексов
        /// <summary>
        /// Индекс элемента с максимальным значением
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
        /// Индекс элемента с максимальным по модулю значением
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
        /// Индекс элемента с минимальным значением
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
        /// Индекс элемента с минимальным по модулю значением
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
        /// Индекс элемента с максимальным значением
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
        /// Индекс элемента с максимальным по модулю значением
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
        /// Индекс элемента с минимальным значением
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
        /// Индекс элемента с минимальным по модулю значением
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
        /// Индекс элемента с ближайшим минимальным значением
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
        /// Смешивает два вектора, используя функцию смешивания
        /// </summary>
        /// <param name="x">Первый вектор</param>
        /// <param name="y">Второй вектор</param>
        /// <param name="cross">Функция смешивания</param>
        /// <returns></returns>
        public static Vector Crosser(Vector x, Vector y, Func<double, double, double> cross)
        {
            Vector outp = new Vector(x.Count);

            for (int i = 0; i < outp.Count; i++)
                outp[i] = cross(x[i], y[i]);

            return outp;
        }
        /// <summary>
        /// Соединение векторов с перекрытием, суммированием в области перекрытия
        /// </summary>
        /// <param name="data">Векторы</param>
        /// <param name="col">Область перекрытия (коллизии)</param>
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
                for (int j = (i != 0) ? col : 0, k = 0; j < data[i].Count; j++)
                    if ((j < data[i].Count - col) || i == data.Length - 1)
                        outp[ind++] = data[i][j];
                    else
                        outp[ind + k] = data[i][j] + data[i + 1][k++]; // любая функция для вычисления отклика в обл. коллизии

            return outp;
        }
        /// <summary>
        /// Преобразование индекса в one-hot вектор, на позиции индекса 1, на остальных 0
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="maxInd">Максимально возможный индекс</param>
        public static Vector OneHotPol(int index, int maxInd)
        {
            Vector outp = new Vector(maxInd + 1)
            {
                [index] = 1
            };
            return outp;
        }
        /// <summary>
        ///  Преобразование индекса в one-hot вектор, на позиции индекса 1, на остальных -1
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="maxInd">Максимально возможный индекс</param>
        public static Vector OneHotBePol(int index, int maxInd)
        {
            Vector outp = new Vector(maxInd + 1) - 1;
            outp[index] = 1;
            return outp;
        }
        /// <summary>
        /// Конкатенация (последовательное соединение) векторов
        /// </summary>
        /// <param name="vectors">Векторы</param>
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
        /// Последовательность, начинающаяся с нуля
        /// </summary>
        /// <param name="step">Шаг</param>
        /// <param name="end">Последнее значение</param>
        /// <returns></returns>
        public static Vector SeqBeginsWithZero(double step, double end)
        {
            return FunctionsForEachElements.GenerateTheSequence(0, step, end);
        }
        /// <summary>
        /// Последовательность
        /// </summary>
        /// <param name="step">Шаг</param>
        /// <param name="end">Последнее значение</param>
        /// <param name="start">Первое значение</param>
        /// <returns></returns>
        public static Vector Seq(double start, double step, double end)
        {
            return FunctionsForEachElements.GenerateTheSequence(start, step, end);
        }
        /// <summary>
        /// Вектор отсчетов времени
        /// </summary>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="t">Время в секундах</param>
        /// <returns></returns>
        public static Vector Time0(double fd, double t)
        {
            double step = 1.0 / fd;
            double end = t;
            return SeqBeginsWithZero(step, end);
        }
        /// <summary>
        /// Разделить на окна (участки)
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <param name="w">Размер окна</param>
        /// <param name="step">Шаг</param>
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
        /// Разделить на окна (участки)
        /// </summary>
        /// <param name="transformer">Функция преобразования</param>
        /// <param name="inp">Вход</param>
        /// <param name="w">Размер окна</param>
        /// <param name="step">Шаг</param>
        /// <returns></returns>
        public static Vector[] GetWindowsWithFunc(Func<Vector, Vector> transformer, Vector inp, int w, int step = 2)
        {
            Vector[] vects = GetWindows(inp, w, step);

            for (int i = 0; i < vects.Length; i++)
            {
                vects[i] = transformer(vects[i]);
            }

            return vects;
        }
        /// <summary>
        /// Разделить на окна (участки)
        /// </summary>
        /// <param name="transformer">Функция преобразования</param>
        /// <param name="inp">Вход</param>
        /// <param name="w">Размер окна</param>
        /// <param name="step">Шаг</param>
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
        /// Масштабирование векторов (z-нормализация)
        /// </summary>
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
        /// Усреднение по ансамблю
        /// </summary>
        /// <param name="vectors">Ансамбль векторов</param>
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
		///  Среднеквадратичное отклонение в ансамбле
		/// </summary>
		/// <param name="vectors">Ансамбль векторов</param>
		public static Vector Std(Vector[] vectors)
        {
            return Statistic.EnsembleDispersion(vectors).Transform(Math.Abs);
        }

        #endregion

        #region Статические методы инициализации
        /// <summary>
        /// Инициализация вектора с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector Parse(string str)
        {
            return Parse(str, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация вектора с помощью строки
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
                Vector res = new Vector(3);
                res.Clear();
                return res;
            }

            string content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            string[] nums = content.Split(' ');

            return FromStrings(nums, provider);
        }
        /// <summary>
        /// Инициализация вектора с помощью строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out Vector result)
        {
            return TryParse(str, out result, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация вектора с помощью строки
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
                Vector empty = new Vector(3);
                empty.Clear();
                result = empty;
                return true;
            }

            string content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            string[] nums = content.Split(' ');

            Vector res = new Vector(3);
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
        /// Инициализация вектора с помощью массива строк
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Vector FromStrings(string[] arr)
        {
            return FromStrings(arr, AISettings.GetProvider());
        }
        /// <summary>
        /// Инициализация вектора с помощью массива строк
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
        /// Создание вектора с помощью перечисления
        /// </summary>
        /// <param name="dbs"></param>
        /// <returns></returns>
        public static Vector FromList(IEnumerable<double> dbs)
        {
            return new Vector(dbs.ToArray());
        }

        #endregion

        #region Технические методы

        /// <summary>
        /// Перевод вектора в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(AISettings.GetProvider());
        }

        /// <summary>
        /// Перевод вектора в строку
        /// </summary>
        public string ToString(NumberFormatInfo numberFormatInfo)
        {
            if (Count == 0)
            {
                return "[]";
            }

            StringBuilder str = new StringBuilder();
            _ = str.Append("[");

            for (int i = 0; i < Count; i++)
            {
                _ = str.Append(this[i].ToString(numberFormatInfo));
                _ = str.Append(" ");
            }

            str.Length--;
            _ = str.Append("]");
            return str.ToString();
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
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

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public bool Equals(Vector other)
        {
            return this == other;
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public bool Equals(IEnumerable<double> other)
        {
            return this == other.ToList();
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public bool Equals(List<double> other)
        {
            return this == other.ToList();
        }

        /// <summary>
        /// Получение хэш кода
        /// </summary>
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
        /// Сохранить в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранениеs vector to stream
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Сохранить в текстовый файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void SaveAsText(string path)
        {
            File.WriteAllText(path, ToString());
        }
        /// <summary>
        /// Представить в виде массива байт
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.Vector).Write(ToArray()).AsByteArray();
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
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
            Vector vect = new Vector(3);
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
        /// Загрузить из потока
        /// </summary>
        /// <param name="stream">Поток</param>
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
        ///Загрузить из текстового файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static Vector LoadAsText(string path)
        {
            return Parse(File.ReadAllText(path));
        }
        /// <summary>
        /// Инициализировать массивом байт
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
        /// Инициализировать потоком данных
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