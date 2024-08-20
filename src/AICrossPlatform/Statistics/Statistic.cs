using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AI.Statistics
{
    /// <summary>
    /// Класс содержит методы для статистического анализа. А также генераторы псевдослучайных чисел.
    /// </summary>
    [Serializable]
    public class Statistic
    {
        #region Поля и свойства
        private readonly Vector _vector;
        private readonly int _n;
        private static readonly Random _rand = new Random();

        /// <summary>
        /// Оценка средне-квадратичного отклонение (СКО)
        /// </summary>
        public double STD { get; private set; }
        /// <summary>
        ///  Минимальное значение
        /// </summary>
        public double MinValue { get; private set; }
        /// <summary>
        ///  Максимальное значение
        /// </summary>
        public double MaxValue { get; private set; }
        /// <summary>
        /// Оценка дисперсии
        /// </summary>
        public double Variance { get; private set; }
        /// <summary>
        /// Оценка мат. ожидания
        /// </summary>
        public double Expected { get; private set; }
        #endregion

        /// <summary>
        /// Создает объект класса Statistic, принимает вектор входных значений случайной величины
        /// </summary>
        public Statistic(IAlgebraicStructure<double> data)
        {
            _vector = data.Data;
            _n = _vector.Count;
            ExpectedValue();
            СalcVariance();
            Std();
            MaxMinValue();
        }

        /// <summary>
        /// Генератор псевдо-случайных чисел с равномерным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <returns>Возвращает вектор случайных чисел</returns>
        public static Vector UniformDistribution(int n)
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
		/// Генератор псевдо-случайных чисел с равномерным распределением
		/// </summary>
		/// <param name="n">Длинна вектора</param>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
		/// <returns>Возвращает вектор случайных чисел</returns>
		public static Vector UniformDistribution(int n, Random random)
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
        /// Генератор псевдо-случайных чисел с нормальным распределением
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
        /// Генератор псевдо-случайных чисел с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
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
        /// Генератор псевдо-случайных чисел с нормальным распределением
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
        /// Генератор псевдо-случайных чисел с нормальным распределением
        /// </summary>
        /// <param name="n">Длинна вектора</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
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
        public static Matrix UniformDistribution(int m, int n)
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
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public static Matrix UniformDistribution(int m, int n, Random random)
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
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        public static Tensor UniformDistribution(int h, int w, int d)
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
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Tensor UniformDistribution(int h, int w, int d, Random random)
        {
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        tensor[i, j, k] = random.NextDouble();

            return tensor;
        }

        /// <summary>
        /// Тензор
        /// </summary>
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <returns></returns>
        public static Tensor RandNorm(int h, int w, int d)
        {
            Random rn = new Random();
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        tensor[i, j, k] = Gauss(rn);

            return tensor;
        }

        /// <summary>
        /// Cоздает матрицу с нормально распределенными значениями
        /// размерности m на n
        /// </summary>
        /// <param name="m">Количество строк</param>
        /// <param name="n">Количество столбцов</param>
        /// <param name="rn">Генератор псевдо-случайных чисел</param>
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
        /// <param name="h"> Высота</param>
        /// <param name="w">Ширина</param>
        /// <param name="d">Глубина</param>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public static Tensor RandNorm(int h, int w, int d, Random random)
        {
            Tensor tensor = new Tensor(h, w, d);

            for (int k = 0; k < d; k++)
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        tensor[i, j, k] = Gauss(random);

            return tensor;
        }


        /// <summary>
        ///  Максимальное значение
        /// </summary>
        /// <param name="array">Значения</param>
        public static double MaximalValue(IAlgebraicStructure<double> array)
        {
            double max = double.MinValue;
            int len = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    if (max < data[i])
                    {
                        max = data[i];
                    }
                }
            }
            return max;
        }

        /// <summary>
        ///  Минимальное значение
        /// </summary>
        /// <param name="array">Значения</param>
        public static double MinimalValue(IAlgebraicStructure<double> array)
        {
            double min = double.MaxValue;
            int len = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    if (min > data[i])
                    {
                        min = data[i];
                    }
                }
            }
            return min;
        }





        #region Оценка математического ожидания
        /// <summary>
        /// Оценка математического ожидания
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
        /// Оценка математического ожидания
        /// </summary>
        /// <param name="array"> Значения </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValue(IAlgebraicStructure<double> array)
        {
            double summ = 0;
            double n = 0;
            int len = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < len; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    summ += data[i];
                    n++;
                }
            }

            double expected = summ / n;
            return double.IsNaN(expected) ? 0 : expected;

        }

        /// <summary>
        ///  Оценка математического ожидания от модуля случайной величины
        /// </summary>
        /// <param name="array"> Значения </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueAbs(IAlgebraicStructure<double> array)
        {
            double summ = 0;
            double n = 0;
            int count = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < count; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    summ += Math.Abs(data[i]);
                    n++;
                }
            }

            double expected = summ / n;
            return double.IsNaN(expected) ? 0 : expected;

        }

        /// <summary>
        /// Estimation of mathematical expectation
        /// </summary>
        /// <param name="array"> Значения </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueNotCheckNaN(IAlgebraicStructure<double> array)
        {
            double summ = 0;
            int count = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < count; i++) summ += data[i];

            return summ / count;

        }

        /// <summary>
        /// Оценка математического ожидания от модуля случайной величины
        /// </summary>
        /// <param name="array"> Вектор значений </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ExpectedValueAbsNotCheckNaN(IAlgebraicStructure<double> array)
        {
            double summ = 0;
            int count = array.Shape.Count;
            double[] data = array.Data;

            for (int i = 0; i < count; i++)
                summ += Math.Abs(data[i]);

            return summ / count;

        }



        #endregion

        #region Оценка СКО и дисперсии

        /// <summary>
        /// Оценка дисперсии
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
        /// Оценка дисперсии
        /// </summary>
        public static double СalcVariance(IAlgebraicStructure<double> array)
        {
            double dispers, eV = ExpectedValue(array);
            double summ = 0;
            double n = 0;

            double[] data = array.Data;
            int count = data.Length;

            for (int i = 0; i < count; i++)
            {
                if (!double.IsNaN(data[i]))
                {
                    double q = data[i] - eV;
                    summ += q * q;
                    n++;
                }
            }

            dispers = summ / (n - 1);
            return double.IsNaN(dispers) ? 0 : dispers;

        }

        /// <summary>
        /// Расчет оценки СКО
        /// </summary>
        public static double CalcStd(IAlgebraicStructure<double> array)
        {
            return Math.Sqrt(СalcVariance(array));
        }
        #endregion


        /// <summary>
        /// Генератор псевдо-случайных чисел с нормальным распределением
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
        public static Matrix UniformDistribution(short n)
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
            double step = (MaxValue - MinValue) / n;
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


            A.Y /= _vector.Count * step;
            A.X = new Vector(A.Y.Count);

            for (int i = 0; i < A.X.Count; i++)
            {
                A.X[i] = MinValue + (i * step);
            }

            return A;
        }

        /// <summary>
        /// Начальный момент
        /// </summary>
        /// <param name="n">Номер начального момента</param>
        public double InitialMoment(int n)
        {
            return ExpectedValue(_vector.Transform(x => Math.Pow(x, n)));
        }
        /// <summary>
        /// Центральный момент
        /// </summary>
        /// <param name="n">Номер центрального момента</param>
        public double CentralMoment(int n)
        {
            return ExpectedValue((_vector - Expected).Transform(x => Math.Pow(x, n)));
        }
        /// <summary>
        /// Ассиметрия распределения
        /// </summary>
        public double Asymmetry()
        {
            return CentralMoment(3) / (STD * STD * STD);
        }
        /// <summary>
        /// Крутизна распределения (CM(4)/D^2 - 3)
        /// </summary>
        public double Excess()
        {
            return (CentralMoment(4) / (STD * STD * STD * STD)) - 3;
        }


        /// <summary>
        /// Ковариация (корреляционный момент, линейная зависимость) двух векторов
        /// </summary>
        public static double Cov(IAlgebraicStructure<double> xS, IAlgebraicStructure<double> yS)
        {
            Vector x = xS.Data;
            Vector y = yS.Data;
            int n1 = x.Count;
            int n2 = y.Count;
            string exceptionStr = string.Format("Невозможно выполнить ковариацию, длинна одного вектора {0}, а второго {1}", n1, n2);

            if (n1 != n2)
            {
                throw new ArgumentException(exceptionStr, "Ковариация");
            }

            double Mx = 0, My = 0, cov = 0;

            for (int i = 0; i < x.Count; i++)
            {
                Mx += x[i];
                My += y[i];
            }

            Mx /= n1;
            My /= n1;

            for (int i = 0; i < x.Count; i++)
            {
                cov += (x[i] - Mx) * (y[i] - My);
            }

            cov /= n1 - 1;

            return cov;
        }

        /// <summary>
        /// Коэффициент корреляции Пирсона
        /// </summary>
        public static double CorrelationCoefficient(Vector x, Vector y)
        {
            string exceptionStr = string.Format("Невозможно рассчитать корреляцию, длинна одного вектора {0}, а второго {1}", x.Count, y.Count);
            if (x.Count != y.Count)
            {
                throw new ArgumentException(exceptionStr, "Корреляция");
            }

            int n = x.Count;

            double Mx = 0, My = 0, cor = 0, Dx = 0, Dy = 0, dx, dy;

            for (int i = 0; i < x.Count; i++)
            {
                Mx += x[i];
                My += y[i];
            }

            Mx /= n;
            My /= n;


            for (int i = 0; i < x.Count; i++)
            {
                cor += (x[i] - Mx) * (y[i] - My);
                dx = x[i] - Mx;
                dy = y[i] - My;
                Dx += dx * dx;
                Dy += dy * dy;
            }

            cor /= Math.Sqrt((Dx * Dy) + 1e-8);

            return cor;
        }

        /// <summary>
        /// Коэффициент корреляции Пирсона
        /// </summary>
        public static double CorrelationCoefficient(IAlgebraicStructure<double> X, IAlgebraicStructure<double> Y)
        {
            return CorrelationCoefficient(X.Data, Y.Data);
        }


        /// <summary>
        /// Усреднение по выборке(ансамблю)
        /// </summary>
        /// <param name="vectors">Выборка</param>
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
        /// <param name="mean">Предю рассчитанный вектор средних</param>
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
        /// <param name="fd">Частота дискретизации</param>
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
        /// Средний шаг, насколько x[i] отличается от x[i + 1] в среднем
        /// </summary>
        /// <param name="vector">Последовательность</param>
        /// <param name="eps">Минимально-возможный шаг</param>
        public static double MeanStep(Vector vector, double eps = double.Epsilon)
        {
            Vector difVec = Functions.Diff(vector);
            difVec[0] = 0;
            double mStep = Functions.Summ(difVec) / (difVec.Count - 1);
            return double.IsNaN(mStep) ? eps : mStep;
        }

        /// <summary>
        /// Средний шаг в последовательности (Max(vector) - Min(vector))/N + eps
        /// </summary>
        /// <param name="vector">Последовательность</param>
        /// <param name="eps">Минимально-возможный шаг</param>
        public static double MeanStep2(Vector vector, double eps = double.Epsilon)
        {
            double max = vector.Max();
            double min = vector.Min();

            return ((max - min) / vector.Count) + eps;
        }


    }

}
