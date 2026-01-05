using AI.DataStructs.Algebraic;
using System;

namespace AI.HightLevelFunctions
{
    /// <summary>
    /// Математические функции для векторов и матриц
    /// </summary>
    public static class FunctionsForEachElements
    {

        // Коэффициенты для аппроксимации Ланцоша (g=7, n=9).
        // Источник: Paul Godfrey, "A note on the computation of the convergent Lanczos complex Gamma approximation"
        private static readonly double[] LanczosCoefficients = {
        0.99999999999980993,
        676.5203681218851,
        -1259.1392167224028,
        771.32342877765313,
        -176.61502916214059,
        12.507343278686905,
        -0.13857109526572012,
        9.9843695780195716e-6,
        1.5056327351493116e-7
        };
        private const int LANCZOS_G = 7;
        private static readonly long[] _factorials = { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800 };



        /// <summary>
        /// Вычисляет Гамма-функцию Γ(x).
        /// Использует аппроксимацию Ланцоша.
        /// </summary>
        /// <param name="x">Аргумент функции.</param>
        /// <returns>Значение Γ(x).</returns>
        public static double Gamma(double x)
        {
            // Обработка специальных значений
            if (double.IsNaN(x))
            {
                return double.NaN;
            }
            if (double.IsPositiveInfinity(x))
            {
                return double.PositiveInfinity;
            }
            if (double.IsNegativeInfinity(x))
            {
                return double.NaN; // Неопределенность
            }

            // Полюсы в целых неположительных числах
            if (x <= 0)
            {
                if (x == Math.Truncate(x)) // Если x - целое неположительное число
                {
                    return double.PositiveInfinity; // В полюсах функция уходит в бесконечность
                }
            }

            // Для отрицательных аргументов используем формулу отражения Эйлера:
            // Γ(z) * Γ(1-z) = π / sin(πz)  =>  Γ(z) = π / (sin(πz) * Γ(1-z))
            if (x < 0.5)
            {
                return Math.PI / (Math.Sin(Math.PI * x) * Gamma(1 - x));
            }

            // Прямое вычисление с помощью аппроксимации Ланцоша для x >= 0.5
            x -= 1;
            double a = LanczosCoefficients[0];
            for (int i = 1; i < LanczosCoefficients.Length; i++)
            {
                a += LanczosCoefficients[i] / (x + i);
            }

            double t = x + LANCZOS_G + 0.5;

            // Вычисляем через логарифм, чтобы избежать переполнения для больших x
            double logGamma = Math.Log(a)
                            + Math.Log(2 * Math.PI) / 2.0
                            + (x + 0.5) * Math.Log(t)
                            - t;

            return Math.Exp(logGamma);
        }

        /// <summary>
        /// Вычисляет натуральный логарифм Гамма-функции Log(Γ(x)).
        /// Этот метод более предпочтителен для больших аргументов, так как позволяет избежать переполнения.
        /// </summary>
        /// <param name="x">Аргумент функции (должен быть > 0).</param>
        /// <returns>Значение Log(Γ(x)).</returns>
        public static double LogGamma(double x)
        {
            if (double.IsNaN(x) || x <= 0)
            {
                return double.NaN;
            }
            if (double.IsPositiveInfinity(x))
            {
                return double.PositiveInfinity;
            }


            // Для значений < 0.5 используем рекурсию с формулой отражения.
            // Log(Γ(x)) = Log(π) - Log|sin(πx)| - Log(Γ(1-x))
            // Этот вариант сложнее из-за знака sin, поэтому для простоты реализуем только для x > 0.

            double a = LanczosCoefficients[0];
            for (int i = 1; i < LanczosCoefficients.Length; i++)
            {
                a += LanczosCoefficients[i] / (x + i - 1);
            }

            double t = x + LANCZOS_G - 0.5;

            return Math.Log(a)
                 + Math.Log(2 * Math.PI) / 2.0
                 + (x - 0.5) * Math.Log(t)
                 - t;
        }


        /// <summary>
        /// Разворачивание арктангенса
        /// </summary>
        /// <param name="data">Данные с функции atan</param>
        /// <param name="diffMax">Максимальный разрыв</param>
        public static Vector Unwrap(Vector data, double diffMax = 2)
        {
            Vector diff = Functions.Diff(data);
            Vector newDat = new Vector(data.Count);
            double ofset = 0;

            for (int i = 0; i < diff.Count; i++)
            {
                if (diff[i] >= diffMax)
                    ofset += diff[i];

                else if (diff[i] <= -diffMax)
                    ofset += diff[i];

                newDat[i] = ofset - data[i];
            }

            return newDat;
        }
        /// <summary>
        /// Возведение числа в вектор степеней
        /// </summary>
        /// <param name="a">Число</param>
        /// <param name="pow">Вектор степеней</param>
        public static Vector Pow(double a, Vector pow)
        {
            Vector outp = new Vector(pow.Count);

            for (int i = 0; i < pow.Count; i++)
                outp[i] = Math.Pow(a, pow[i]);

            return outp;
        }
        /// <summary>
        /// Устранение выбросов на одном отсчете сигнала(пиков)
        /// </summary>
        /// <param name="data">Сигнал</param>
        /// <returns></returns>
        public static Vector PeakDel(Vector data)
        {
            Vector newDat = new Vector(data.Count)
            {
                [0] = data[0]
            };

            for (int i = 1, max = data.Count - 1; i < max; i++)
            {
                double mean = (data[i - 1] + data[i + 1]) / 2.0;
                if (Math.Abs(data[i - 1] - data[i + 1]) < Math.Abs(mean - data[i]))
                    newDat[i] = mean;

                else
                    newDat[i] = data[i];
            }

            return newDat;
        }
        /// <summary>
        /// Возведение -1 в степень
        /// </summary>
        /// <param name="pow">Степень</param>
        /// <returns></returns>
        public static int MinusOnePow(int pow)
        {
            return (pow % 2 != 0) ? -1 : 1;
        }
        /// <summary>
        /// Функции Радемахера
        /// </summary>
        /// <param name="x">Параметр</param>
        /// <param name="num">Номер функции</param>
        /// <returns></returns>
        public static double Rad(double x, int num)
        {
            return Math.Sign(Math.Sin(Math.Pow(2, num) * Math.PI * x));
        }
        /// <summary>
        /// Функции Радемахера
        /// </summary>
        /// <param name="x">Вектор параметров</param>
        /// <param name="num">Номер функции</param>
        /// <returns></returns>
        public static Vector Rad(Vector x, int num)
        {
            return x.Transform(r => Rad(r, num));
        }
        /// <summary>
        /// Функции Уолша
        /// </summary>
        /// <param name="x">Параметр</param>
        /// <param name="num">Номер функции</param>
        /// <returns></returns>
        public static double Walsh(double x, int num)
        {
            return Math.Sign(Math.Sin(num * Math.PI * x));
        }
        /// <summary>
        /// Функции Радемахера
        /// </summary>
        /// <param name="x">Вектор параметров</param>
        /// <param name="num">Номер функции</param>
        /// <returns></returns>
        public static Vector Walsh(Vector x, int num)
        {
            return x.Transform(r => Walsh(r, num));
        }
        /// <summary>
        /// Функция ошибки
        /// </summary>
        /// <param name="x">Аргумент</param>
        public static double Erf(double x)
        {

            double a = 8 / (3 * Math.PI) * (3 - Math.PI) / (Math.PI - 4),
                exp1 = ((-x * x * (4 / Math.PI)) + (a * x * x)) / (1 + (a * x));
            return Math.Sign(x) * Math.Sqrt(1 - Math.Exp(exp1));
        }

        /// <summary>
        /// Функция ошибок
        /// </summary>
        /// <param name="Inp">Входной вектор</param>
        /// <returns></returns>
        public static Vector Erf(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Erf(Inp[i]);

            return A;
        }
        /// <summary>
        /// Генерирование последовательности
        /// </summary>
        /// <param name="began">Начальное значение</param>
        /// <param name="step">Шаг</param>
        /// <param name="end">Конечное значение</param>
        /// <returns>Возвращает последовательность</returns>
        public static Vector GenerateTheSequence(double began, double step, double end)
        {
            // Исправлено: явно вычисляем количество точек для избежания проблем с floating point
            int Count = (int)Math.Round((end - began) / step);
            
            // Если began + Count * step превышает end, это нормально для последовательности [began, end]
            // Но если не включает end, добавляем точку
            if (Math.Abs(began + Count * step - end) > step * 0.1)
                Count++;

            Vector sequen = new Vector(Count);

            for (int i = 0; i < Count; i++)
                sequen[i] = began + (i * step);

            return sequen;
        }
        /// <summary>
        /// Генерирование последовательности
        /// </summary>
        /// <param name="began">Начальное значение</param>
        /// <param name="end">Конечное значение</param>
        /// <returns>Возвращает послеовательность с шагом 1</returns>
        public static Vector GenerateTheSequence(double began, double end)
        {
            double n = end - began;
            int Count = (n % 1 == 0) ? (int)n : (int)(n + 1);

            Vector sequen = new Vector(Count);

            for (int i = 0; i < Count; i++)
                sequen[i] = began + i;

            return sequen;
        }
        /// <summary>
        /// Перевод градусов в радианы
        /// </summary>
        /// <param name="grad">значение в градусах</param>
        public static double GradToRad(double grad)
        {
            return grad * Math.PI / 180.0;
        }
        /// <summary>
        /// Перевод градусов в радианы
        /// </summary>
        /// <param name="Inp">значения в градусах</param>
        public static Vector GradToRad(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = GradToRad(Inp[i]);

            return A;
        }
        /// <summary>
        /// Перевод радиан в градусы
        /// </summary>
        /// <param name="rad">значение в радианах</param>
        public static double RadToGrad(double rad)
        {
            return rad * 180.0 / Math.PI;
        }
        /// <summary>
        /// Перевод радиан в градусы
        /// </summary>
        /// <param name="Inp">значение в радианах</param>
        public static Vector RadToGrad(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = RadToGrad(Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление факториала числа
        /// </summary>
        /// <param name="x">Число</param>
        /// <returns>Факториал</returns>
        public static long Factorial(int x)
        {
            if (x < 0)
                throw new ArgumentException("Факториал не может быть меньше нуля", "x");
            else if (x < 14)
                return _factorials[x];
            else
            {
                long outp = _factorials[13];
                for (int i = 14; i < x + 1; i++)
                    outp *= i;

                return outp;
            }
        }
        /// <summary>
        /// Вычисление факториала векторов поэлементно
        /// </summary>
        /// <param name="Inp">Входной вектор</param>
        /// <returns>Факториал</returns>
        public static Vector Factorial(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Factorial((int)Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление синусов
        /// </summary>
        /// <param name="Inp">Вектор углов (в радианах)</param>
        /// <returns>Вектор синусов</returns>
        public static Vector Sin(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Sin(Inp[i]);

            return A;
        }
        /// <summary>
        /// Окугление
        /// </summary>
        /// <param name="Inp">Вектор входных данных</param>
        /// <param name="digits">до какого знака</param>
        /// <returns>Вектор выхода</returns>
        public static Vector Round(Vector Inp, int digits)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Round(Inp[i], digits);

            return A;
        }
        /// <summary>
        /// Вычисление косинусов
        /// </summary>
        /// <param name="Inp">Вектор углов (в радианах)</param>
        /// <returns>Вектор косинусов</returns>
        public static Vector Cos(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Cos(Inp[i]);

            return A;
        }
        /// <summary>
        /// Calculating tangents
        /// </summary>
        /// <param name="Inp">Вектор углов (в радианах)</param>
        /// <returns>Вектор тангенсов</returns>
        public static Vector Tan(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Tan(Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление котангенсов
        /// </summary>
        /// <param name="Inp">Вектор углов (в радианах)</param>
        /// <returns>Вектор котангенсов</returns>
        public static Vector ctg(Vector Inp)
        {
            return 1.0 / Tan(Inp);
        }
        /// <summary>
        /// Вычисление арксинусов
        /// </summary>
        /// <param name="Inp">Вектор синусов</param>
        /// <returns>Вектор углов (в радианах)</returns>
        public static Vector Asin(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Asin(Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление арккосинусов
        /// </summary>
        /// <param name="Inp">Вектор косинусов</param>
        /// <returns>Вектор углов (в радианах)</returns>
        public static Vector Acos(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Acos(Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление арктангенсов
        /// </summary>
        /// <param name="Inp">Вектор тангенсов</param>
        /// <returns>Вектор углов (в радианах)</returns>
        public static Vector Atan(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Atan(Inp[i]);

            return A;
        }
        /// <summary>
        /// Дсятичный логарифм
        /// </summary>
        /// <param name="Inp">Подлогарифмическое число</param>
        public static Vector Log10(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Log10(Inp[i]);

            return A;
        }
        /// <summary>
        /// Логарифм по основанию "e"
        /// </summary>
        /// <param name="Inp">Подлогарифмическое число</param>
        public static Vector Ln(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Log(Inp[i]);

            return A;
        }
        /// <summary>
        /// Секанс угла
        /// </summary>
        /// <param name="Inp">углы</param>
        public static Vector Sec(Vector Inp)
        {
            return 1 / Cos(Inp);
        }
        /// <summary>
        /// Косеканс угла
        /// </summary>
        /// <param name="Inp">углы</param>
        public static Vector Cosec(Vector Inp)
        {
            return 1 / Sin(Inp);
        }
        /// <summary>
        /// Экспонента e^x
        /// </summary>
        /// <param name="Inp">показатели степени</param>
        /// <returns>e^Inp - поэлементно</returns>
        public static Vector Exp(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Exp(Inp[i]);

            return A;
        }
        /// <summary>
        /// Гиперболический тангенс
        /// </summary>
        /// <param name="Inp">углы</param>
        public static Vector Tanh(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
            {
                A[i] = Math.Tanh(Inp[i]);
            }

            return A;
        }
        /// <summary>
        /// Определение знака
        /// </summary>
        /// <param name="Inp">Входной вектор</param>
        /// <returns></returns>
        public static Vector Sign(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Sign(Inp[i]);

            return A;
        }
        /// <summary>
        /// Квадратный корень
        /// </summary>
        /// <param name="Inp">числа</param>		
        public static Vector Sqrt(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Sqrt(Inp[i]);

            return A;
        }
        /// <summary>
        /// Вычисление синуса
        /// </summary>
        /// <param name="Inp">Матрица значений для преобразования</param>	
        public static Matrix Sin(Matrix Inp)
        {
            Matrix A = new Matrix(Inp.Height, Inp.Width);
            for (int i = 0; i < Inp.Height; i++)
                for (int j = 0; j < Inp.Width; j++)
                    A[i, j] = Math.Sin(Inp[i, j]);

            return A;
        }
        /// <summary>
        /// e^x
        /// </summary>
        /// <param name="Inp">Матрица значений для преобразования</param>	
        public static Matrix Exp(Matrix Inp)
        {
            Matrix A = new Matrix(Inp.Height, Inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Exp(Inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Гиперболический тангенс
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Tanh(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Tanh(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Косинус
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Cos(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Cos(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Тангенс
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Tan(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Tan(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Котангенс
        /// </summary>
        /// <param name="Inp">Матрица значений для преобразования</param>	
        public static Matrix Ctan(Matrix Inp)
        {
            return 1.0 / Tan(Inp);
        }
        /// <summary>
        /// Арксинус
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Asin(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Asin(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Арккосинус
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Acos(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Acos(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Арктангенс
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Atan(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Atan(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Модуль
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Abs(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Abs(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Квадратный корень
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Sqrt(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Sqrt(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Десятичный логарифм
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Log10(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Log10(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Логарифм по основанию E
        /// </summary>
        /// <param name="inp">Матрица значений для преобразования</param>	
        public static Matrix Ln(Matrix inp)
        {
            Matrix A = new Matrix(inp.Height, inp.Width);
            int len = A.Shape.Count;
            for (int i = 0; i < len; i++)
                A.Data[i] = Math.Log(inp.Data[i]);

            return A;
        }
        /// <summary>
        /// Секонс
        /// </summary>
        /// <param name="Inp">Матрица значений для преобразования</param>	
        public static Matrix Sec(Matrix Inp)
        {
            return 1.0 / Cos(Inp);
        }
        /// <summary>
        /// Косеконс
        /// </summary>
        /// <param name="Inp">Матрица значений для преобразования</param>	
        public static Matrix Cosec(Matrix Inp)
        {
            return 1.0 / Sin(Inp);
        }
        /// <summary>
        /// Модуль
        /// </summary>
        /// <param name="Inp">Комплексный вектор значений для преобразования</param>
        public static Vector Abs(Vector Inp)
        {
            Vector A = new Vector(Inp.Count);
            for (int i = 0; i < Inp.Count; i++)
                A[i] = Math.Abs(Inp[i]);

            return A;
        }
    }
}