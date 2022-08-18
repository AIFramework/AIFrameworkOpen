using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AI.Statistics
{
    /// <summary>
    /// The class contains methods for statistical analysis.As well as pseudo random number generators
    /// </summary>
    [Serializable]
    public class Statistic
    {
        #region Fields and properties
        private readonly Vector _vector;
        private readonly int _n;
        private static readonly Random _rand = new Random();

        /// <summary>
        /// Root mean square deviation
        /// </summary>
        public double STD { get; private set; }
        /// <summary>
        /// Minimum value
        /// </summary>
        public double MinValue { get; private set; }
        /// <summary>
        /// Maximum value
        /// </summary>
        public double MaxValue { get; private set; }
        /// <summary>
        /// Dispersion
        /// </summary>
        public double Variance { get; private set; }
        /// <summary>
        /// Expected value
        /// </summary>
        public double Expected { get; private set; }
        #endregion

        /// <summary>
        /// Creates an object of class Statistic, takes a vector of input values ​​of a random variable
        /// </summary>
        public Statistic(IAlgebraicStructure data)
        {
            _vector = data.Data;
            _n = _vector.Count;
            ExpectedValue();
            СalcVariance();
            Std();
            MaxMinValue();
        }

        /// <summary>
        /// Pseudo-random number generator с равномерным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector Rand(int n)
        {
            Random A = new Random();
            Vector vect = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                vect[i] = A.NextDouble();
            }

            return vect;
        }

        /// <summary>
		/// Pseudo-random number generator с равномерным распределением
		/// </summary>
		/// <param name="n">Длинна вектора</param>
        /// <param name="random">Pseudo-random number generator</param>
		/// <returns>Возвращает вектор случайных чисел</returns>
		public static Vector Rand(int n, Random random)
        {
            Vector vect = new Vector(n);

            for (int i = 0; i < n; i++)
            {
                vect[i] = random.NextDouble();
            }

            return vect;
        }


        /// <summary>
        /// Гауссовское распределение
        /// </summary>
        /// <returns>Возвращает норм. распред величину СКО = 1, M = 0</returns>
        public static double Gauss(Random A)
        {
            double a = (2 * A.NextDouble()) - 1,
            b = (2 * A.NextDouble()) - 1,
            s = (a * a) + (b * b);

            if (a == 0 && b == 0)
            {
                a = 0.000001;
                s = (a * a) + (b * b);
            }

            return b * Math.Sqrt(Math.Abs(-2 * Math.Log(s) / s));
        }


        /// <summary>
        /// Гауссовское распределение
        /// </summary>
        /// <returns>Возвращает норм. распред величину СКО = 1, M = 0</returns>
        public static double Gauss2(Random A, int n)
        {

            double a = 0;

            for (int i = 0; i < n; i++)
            {
                a += 2 * (A.NextDouble() - 0.5);
            }

            return a;
        }




        /// <summary>
        /// Pseudo-random number generator с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <param name="iter">Число итераций</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector RandNormP(int n, int iter = 100)
        {
            Random A = new Random();
            Vector vect = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                vect[i] = Gauss2(A, iter);
            }

            return vect / vect.Std();
        }


        /// <summary>
        /// Pseudo-random number generator с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <param name="random">Pseudo-random number generator</param>
        /// <param name="iter">Число итераций</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector RandNormP(int n, Random random, int iter = 10)
        {
            Vector[] vects = new Vector[iter];
            Vector sum = new Vector(n);


            for (int j = 0; j < iter; j++)
            {
                vects[j] = new Vector(n);

                for (int i = 0; i < n; i++)
                {
                    vects[j][i] = random.NextDouble();
                }

                sum += vects[j];
            }

            Vector std = Vector.Std(vects);
            Vector mean = Vector.Mean(vects);

            return (mean - mean.Mean()) / std;
        }

        /// <summary>
        /// Pseudo-random number generator с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector RandNorm(int n)
        {
            Random A = new Random();
            Vector vect = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                vect[i] = Gauss(A);
            }

            return vect;
        }



        /// <summary>
        /// Pseudo-random number generator с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector RandNorm(int n, Random rnd)
        {
            Vector vect = new Vector(n);
            for (int i = 0; i < n; i++)
            {
                vect[i] = Gauss(rnd);
            }

            return vect;
        }


        // Минимальное и макимальное значения
        private void MaxMinValue()
        {
            MaxValue = _vector.Max();
            MinValue = _vector.Min();
        }

        /// <summary>
        /// Cоздает матрицу с равномерно распределенными значениями
        /// размерности m на n
        /// </summary>
        /// <param name="m">Количество строк</param>
        /// <param name="n">Количество столбцов</param>
        public static Matrix Rand(int m, int n)
        {
            Random rn = new Random();
            Matrix C = new Matrix(m, n);
            int count = C.Shape.Count;

            for (int i = 0; i < count; i++)
            {
                C.Data[i] = rn.NextDouble();
            }

            return C;
        }

        /// <summary>
        /// Cоздает матрицу с равномерно распределенными значениями
        /// размерности m на n
        /// </summary>
        /// <param name="m">Количество строк</param>
        /// <param name="n">Количество столбцов</param>
        /// <param name="random">Pseudo-random number generator</param>
        public static Matrix Rand(int m, int n, Random random)
        {
            Matrix C = new Matrix(m, n);
            int count = C.Shape.Count;

            for (int i = 0; i < count; i++)
            {
                C.Data[i] = random.NextDouble();
            }

            return C;
        }

        /// <summary>
        /// Тензор
        /// </summary>
        /// <param name="h"> Height </param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        public static Tensor Rand(int h, int w, int d)
        {
            Random rn = new Random();
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        tensor[i, j, k] = rn.NextDouble();
                    }
                }
            }

            return tensor;
        }

        /// <summary>
        /// Тензор
        /// </summary>
        /// <param name="h"> Height </param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Tensor Rand(int h, int w, int d, Random random)
        {
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        tensor[i, j, k] = random.NextDouble();
                    }
                }
            }

            return tensor;
        }

        /// <summary>
        /// Тензор
        /// </summary>
        /// <param name="h"> Height </param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <returns></returns>
        public static Tensor RandNorm(int h, int w, int d)
        {
            Random rn = new Random();
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        tensor[i, j, k] = Gauss(rn);
                    }
                }
            }

            return tensor;
        }

        /// <summary>
        /// Cоздает матрицу с нормально распределенными значениями
        /// размерности m на n
        /// </summary>
        /// <param name="m">Количество строк</param>
        /// <param name="n">Количество столбцов</param>
        /// <param name="rn">Pseudo-random number generator</param>
        public static Matrix RandNorm(int m, int n, Random rn)
        {
            Matrix C = new Matrix(m, n);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = Gauss(rn);
            }

            return C;
        }
        /// <summary>
        /// Тензор
        /// </summary>
        /// <param name="h"> Height </param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <param name="random">Pseudo-random number generator</param>
        public static Tensor RandNorm(int h, int w, int d, Random random)
        {
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
            {
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        tensor[i, j, k] = Gauss(random);
                    }
                }
            }

            return tensor;
        }


        /// <summary>
        /// Maximum value
        /// </summary>
        /// <param name="data">Values</param>
        public static double MaximalValue(IAlgebraicStructure data)
        {
            double max = double.MinValue;

            for (int i = 0; i < data.Shape.Count; i++)
            {
                if (!double.IsNaN(data.Data[i]))
                {
                    if (max < data.Data[i])
                    {
                        max = data.Data[i];
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Minimum value
        /// </summary>
        /// <param name="data">Values</param>
        public static double MinimalValue(IAlgebraicStructure data)
        {
            double min = double.MaxValue;

            for (int i = 0; i < data.Shape.Count; i++)
            {
                if (!double.IsNaN(data.Data[i]))
                {
                    if (min > data.Data[i])
                    {
                        min = data.Data[i];
                    }
                }
            }
            return min;
        }





        #region ExpectedValue
        /// <summary>
        /// Expected value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExpectedValue()
        {
            double summ = 0;
            double n = 0;

            for (int i = 0; i < _n; i++)
            {
                if (!double.IsNaN(_vector[i]))
                {
                    summ += _vector[i];
                    n++;
                }
            }

            Expected = summ / n;

            Expected = double.IsNaN(Expected) ? 0 : Expected;
        }

        /// <summary>
        /// Estimation of mathematical expectation
        /// </summary>
        /// <param name="vector">Vector containing samples of a random variable</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValue(IAlgebraicStructure vector)
        {
            double summ = 0;
            double n = 0;

            for (int i = 0; i < vector.Shape.Count; i++)
            {
                if (!double.IsNaN(vector.Data[i]))
                {
                    summ += vector.Data[i];
                    n++;
                }
            }

            double expected = summ / n;
            return double.IsNaN(expected) ? 0 : expected;

        }

        /// <summary>
        /// Estimation of the mathematical expectation from the modulus of a random variable
        /// </summary>
        /// <param name="vector">Vector containing samples of a random variable</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueAbs(IAlgebraicStructure vector)
        {
            double summ = 0;
            double n = 0;

            for (int i = 0; i < vector.Shape.Count; i++)
            {
                if (!double.IsNaN(vector.Data[i]))
                {
                    summ += Math.Abs(vector.Data[i]);
                    n++;
                }
            }

            double expected = summ / n;
            return double.IsNaN(expected) ? 0 : expected;

        }

        /// <summary>
        /// Estimation of mathematical expectation
        /// </summary>
        /// <param name="vector">Vector containing samples of a random variable</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueNotCheckNaN(IAlgebraicStructure vector)
        {
            double summ = 0;

            for (int i = 0; i < vector.Shape.Count; i++)
            {
                summ += vector.Data[i];
            }
            return summ / vector.Shape.Count;

        }

        /// <summary>
        /// Estimation of the mathematical expectation from the modulus of a random variable
        /// </summary>
        /// <param name="vector">Vector containing samples of a random variable</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueAbsNotCheckNaN(IAlgebraicStructure vector)
        {
            double summ = 0;

            for (int i = 0; i < vector.Shape.Count; i++)
            {
                summ += Math.Abs(vector.Data[i]);
            }
            return summ / vector.Shape.Count;

        }



        #endregion

        #region Variance calculation and Std

        /// <summary>
        /// Variance calculation
        /// </summary>
        private void СalcVariance()
        {
            double summ = 0;
            double n = 0;

            for (int i = 0; i < _n; i++)
            {
                if (!double.IsNaN(_vector[i]))
                {
                    double q = _vector[i] - Expected;
                    summ += q * q;
                    n++;
                }
            }

            Variance = summ / (n - 1);
            Variance = double.IsNaN(Variance) ? 0 : Variance;
        }

        /// <summary>
        /// СКО
        /// </summary>
        private void Std()
        {
            STD = Math.Sqrt(Variance);
        }

        /// <summary>
        /// Variance calculation
        /// </summary>
        public static double СalcVariance(IAlgebraicStructure vector)
        {
            double dispers, eV = ExpectedValue(vector);
            double summ = 0;
            double n = 0;

            for (int i = 0; i < vector.Shape.Count; i++)
            {
                if (!double.IsNaN(vector.Data[i]))
                {
                    double q = vector.Data[i] - eV;
                    summ += q * q;
                    n++;
                }
            }

            dispers = summ / (n - 1);
            return double.IsNaN(dispers) ? 0 : dispers;

        }

        /// <summary>
        /// Calculating standard deviation
        /// </summary>
        public static double CalcStd(Vector vector)
        {
            return Math.Sqrt(СalcVariance(vector));
        }
        #endregion


        /// <summary>
        /// Pseudo-random number generator с нормальным распределением
        /// </summary>
        /// <returns>Возвращает случайные числа</returns>
        public double RandNorm()
        {
            return Gauss(_rand);
        }


        /// <summary>
        /// Cоздает матрицу с нормально распределенными значениями
        /// размерности m на n
        /// </summary>
        /// <param name="m">Количество строк</param>
        /// <param name="n">Количество столбцов</param>
        public static Matrix RandNorm(int m, int n)
        {
            Random rn = new Random();
            Matrix C = new Matrix(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = Gauss(rn);
                }
            }

            return C;
        }

        /// <summary>
        /// Cоздает матрицу с равномерно распределенными значениями
        /// размерности n на n
        /// </summary>
        public static Matrix Rand(short n)
        {
            Random rn = new Random();
            Matrix C = new Matrix(n, n);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = rn.NextDouble();
            }

            return C;
        }


        /// <summary>
        /// Cоздает матрицу с нормально распределенными значениями
        /// размерности n на n
        /// </summary>
        public static Matrix RandNorm(short n)
        {
            Random rn = new Random();
            Matrix C = new Matrix(n, n);
            int len = C.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                C.Data[i] = rn.NextDouble();
            }

            return C;
        }


        /// <summary>
        /// Строит гистограмму
        /// </summary>
        /// <param name="n">Количество разрядов гистограммы</param>
        /// <returns>возращает вектор длинной nRazr, содержащий отсчеты для построения гистограммы</returns>
        public Histogramm Histogramm(int n)
        {
            double step = (MaxValue - MinValue) / n, step2 = (MaxValue - MinValue) / _vector.Count;
            Histogramm A = new Histogramm(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < _vector.Count; j++)
                {
                    if (_vector[j] >= (MinValue + (i * step)) && _vector[j] <= MinValue + ((i + 1) * step))// попадение в интервал					
                    {
                        A.Y[i]++;
                    }
                }

            }


            A.Y /= _vector.Count;
            A.Y = A.Y.InterpolayrZero(_vector.Count / n);
            A.X = new Vector(A.Y.Count);

            for (int i = 0; i < A.X.Count; i++)
            {
                A.X[i] = MinValue + (i * step2);
            }

            return A;
        }

        /// <summary>
        /// Initial moment
        /// </summary>
        /// <param name="n">Initial moment number 1,2,3 ...</param>
        public double InitialMoment(int n)
        {
            return ExpectedValue(_vector.Transform(x => Math.Pow(x, n)));
        }
        /// <summary>
        /// Central moment
        /// </summary>
        /// <param name="n">Central moment number 1,2,3 ...</param>
        public double CentralMoment(int n)
        {
            return ExpectedValue((_vector - Expected).Transform(x => Math.Pow(x, n)));
        }
        /// <summary>
        /// Distribution asymmetry
        /// </summary>
        public double Asymmetry()
        {
            return CentralMoment(3) / (STD * STD * STD);
        }
        /// <summary>
        /// Kurtosis, "steepness" of the distribution
        /// </summary>
        /// <returns>Returns the kurtosis coefficient</returns>
        public double Excess()
        {
            return (CentralMoment(4) / (STD * STD * STD * STD)) - 3;
        }


        /// <summary>
        /// Covariance (correlation moment, linear dependence) of two vectors
        /// </summary>
        public static double Cov(Vector X, Vector Y)
        {
            int n1 = X.Count;
            int n2 = Y.Count;
            string exceptionStr = string.Format("Невозможно выполнить ковариацию, длинна одного вектора {0}, а второго {1}", n1, n2);
            if (n1 != n2)
            {
                throw new ArgumentException(exceptionStr, "Ковариация");
            }

            double Mx = 0, My = 0, cov = 0;

            for (int i = 0; i < X.Count; i++)
            {
                Mx += X[i];
                My += Y[i];
            }

            Mx /= n1;
            My /= n1;

            for (int i = 0; i < X.Count; i++)
            {
                cov += (X[i] - Mx) * (Y[i] - My);
            }

            cov /= n1 - 1;

            return cov;
        }

        /// <summary>
        /// Pearson's correlation coefficient
        /// </summary>
        public static double CorrelationCoefficient(Vector X, Vector Y)
        {
            int n = X.Count;

            double Mx = 0, My = 0, cor = 0, Dx = 0, Dy = 0, dx, dy;

            for (int i = 0; i < X.Count; i++)
            {
                Mx += X[i];
                My += Y[i];
            }

            Mx /= n;
            My /= n;


            for (int i = 0; i < X.Count; i++)
            {
                cor += (X[i] - Mx) * (Y[i] - My);
                dx = X[i] - Mx;
                dy = Y[i] - My;
                Dx += dx * dx;
                Dy += dy * dy;
            }

            cor /= Math.Sqrt((Dx * Dy) + 1e-8);

            return cor;
        }

        /// <summary>
        /// Pearson's correlation coefficient
        /// </summary>
        public static double CorrelationCoefficient(IAlgebraicStructure X, IAlgebraicStructure Y)
        {
            return CorrelationCoefficient(X.Data, Y.Data);
        }


        /// <summary>
        /// Усреднение по выборке(ансамблю)
        /// </summary>
        /// <param name="vectors">Dataset</param>
        /// <returns>Средний вектор</returns>
        public static Vector MeanVector(IEnumerable<Vector> vectors)
        {
            Vector[] data = vectors.ToArray();
            Vector output = Functions.Summ(data);
            output /= data.Length;
            return output;
        }

        /// <summary>
        /// Среднее геометрическое 
        /// </summary>
        public static double MeanGeom(Vector vect)
        {
            int numMinus = 0;

            for (int i = 0; i < vect.Count; i++)
            {
                if (vect[i] < 0)
                {
                    numMinus++;
                }
            }

            Vector res = FunctionsForEachElements.Ln(FunctionsForEachElements.Abs(vect));
            double summ = Functions.Summ(res);
            summ /= vect.Count;

            switch (numMinus % 2)
            {
                case 1:
                    return -Math.Exp(summ);
                default:
                    return Math.Exp(summ);
            }

        }
        /// <summary>
        /// Среднее гармоническое
        /// </summary>
        /// <returns></returns>
        public static double MeanGarmonic(Vector vect)
        {
            Vector res = 1 / vect;
            double summ = Functions.Summ(res);
            return vect.Count / summ;
        }
        /// <summary>
        /// Среднeквадратичное значение
        /// </summary>
        /// <returns></returns>
        public static double RMS(Vector vect)
        {
            Vector res = vect.Transform(x => Math.Pow(x, 2));
            double summ = Functions.Summ(res);
            return Math.Sqrt(summ / vect.Count);
        }
        /// <summary>
        /// Дисперсия по ансамлю
        /// </summary>
        /// <param name="vectors">Ансамбль векторов</param>
        public static Vector EnsembleDispersion(IEnumerable<Vector> vectors)
        {
            Vector[] ensemble = vectors.ToArray();
            Vector res = new Vector(ensemble[0].Count);
            Vector mean = MeanVector(ensemble);

            for (int i = 0; i < ensemble.Length; i++)
            {
                res += (ensemble[i] - mean).Transform(x => Math.Pow(x, 2));
            }

            res /= ensemble.Length - 1;

            return res;
        }

        /// <summary>
        /// Дисперсия по ансамлю(использует предрассчитанный вектор средних)
        /// </summary>
        /// <param name="ensemble">Ансамбль векторов</param>
        /// <param name="mean">Вектор средних</param>
        public static Vector EnsembleDispersion(Vector[] ensemble, Vector mean)
        {
            Vector res = new Vector(ensemble[0].Count);

            for (int i = 0; i < ensemble.Length; i++)
            {
                res += (ensemble[i] - mean).Transform(x => Math.Pow(x, 2));
            }

            res /= ensemble.Length - 1;

            return res;
        }

        /// <summary>
        /// СКО по ансамлю
        /// </summary>
        /// <param name="ensemble">Ансамбль векторов</param>
        public static Vector EnsembleStd(IEnumerable<Vector> ensemble)
        {
            Vector res = EnsembleDispersion(ensemble);

            return res.Transform(Math.Sqrt);
        }

        /// <summary>
        /// СКО по ансамлю
        /// </summary>
        /// <param name="ensemble">Ансамбль векторов</param>
        public static Vector EnsembleStd(Vector[] ensemble, Vector mean)
        {
            Vector res = EnsembleDispersion(ensemble, mean);

            return res.Transform(Math.Sqrt);
        }
        /// <summary>
        /// Максимум по ансамлю
        /// </summary>
        /// <param name="ensemble">Ансамбль векторов</param>
        public static Vector MaxEns(Vector[] ensemble)
        {
            Vector res = new Vector(ensemble[0].Count);

            for (int i = 0; i < ensemble[0].Count; i++)
            {
                res[i] = ensemble[0][i];

                for (int j = 1; j < ensemble.Length; j++)
                {
                    if (ensemble[j][i] > res[i])
                    {
                        res[i] = ensemble[j][i];
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// Возвращает вектор с максимальной энергией
        /// </summary>
        /// <param name="ens">Ансамбль векторов</param>
        /// <returns>Вектор с максимальной энергией</returns>
        public static Vector MaxEnergeVector(Vector[] ens)
        {
            Vector res = new Vector(ens.Length);

            for (int i = 0; i < res.Count; i++)
            {
                res[i] = AnalyticGeometryFunctions.NormVect(ens[i]);
            }

            double max = MaximalValue(res);
            int ind = res.FindIndex(el => el == max);

            return ens[ind].Clone();
        }
        /// <summary>
        /// Средняя частота (не нормированная, зависит от кол-ва точек)
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns></returns>
        public static double SimpleMeanFreq(Vector signal)
        {
            Vector signal2 = signal.Clone();
            signal2 -= ExpectedValue(signal2);
            Vector signalQR = signal2.Transform(x => Math.Pow(x, 2));
            Vector signalDQR = Functions.Diff(signal2);
            signalDQR *= signalDQR;
            double mq1 = 0, mq2 = 0;

            for (int i = 0; i < signalQR.Count; i++)
            {
                mq1 += signalQR[i];
                mq2 += signalDQR[i];
            }

            return Math.Sqrt(mq2 / mq1);

        }
        /// <summary>
        /// Средняя частота сигнала
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="fd">Sampling frequency</param>
        /// <returns>Средняя частота [Гц]</returns>
        public static double MeanFreq(Vector signal, double fd)
        {
            double k = fd / (2 * Math.PI);
            double W = SimpleMeanFreq(signal);
            return Math.Round(k * W, 3);

        }
        /// <summary>
        /// Изменение частоты
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns>Дивиация средней частоты</returns>
        public static double DivFreq(Vector signal)
        {
            Vector dif = Functions.Diff(signal);

            return SimpleMeanFreq(dif) / SimpleMeanFreq(signal);
        }
        /// <summary>
        /// Average step, how much x[i] differs from x[i + 1] on average
        /// </summary>
        /// <param name="vector">Sequence</param>
        public static double MeanStep(Vector vector, double eps = double.Epsilon)
        {
            Vector difVec = Functions.Diff(vector);
            difVec[0] = 0;
            double mStep = Functions.Summ(difVec) / (difVec.Count - 1);
            return double.IsNaN(mStep) ? eps : mStep;
        }

        /// <summary>
        /// (Max(vector) - Min(vector))/N + eps
        /// </summary>
        /// <param name="vector">Sequence</param>
        public static double MeanStep2(Vector vector, double eps = double.Epsilon)
        {
            double max = vector.Max();
            double min = vector.Min();

            return ((max - min) / vector.Count) + eps;
        }


    }

}
