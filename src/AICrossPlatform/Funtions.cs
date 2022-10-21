using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector = AI.DataStructs.Algebraic.Vector;

namespace AI
{
    /// <summary>
    /// Математические фукции
    /// </summary>
    [Serializable]
    public static class Functions
    {
        #region Вспомогательные функции
        /// <summary>
        /// Реализация оконных функций
        /// </summary>
        /// <param name="vect">входной вектор</param>
        /// <param name="Function">функция</param>
        /// <param name="window">окно</param>
        /// <returns>Результат применения ф-и</returns>
        public static Vector WindowFunc(Vector vect, Func<Vector, Vector> Function, int window)
        {
            Vector input, vect1 = vect.Shift(window);
            int n = vect1.Count - window;
            List<double> DoubList = new List<double>();
            double[] data;


            for (int i = 0; i < n; i += window)
            {
                data = new double[window];
                input = vect1.CutAndZero(i + window);
                Array.Copy(input.ToArray(), i, data, 0, window);
                input = new Vector(data);
                DoubList.AddRange(Function(input));
            }

            return Vector.FromList(DoubList);

        }

        /// <summary>
        /// Реализация оконных функций
        /// </summary>
        /// <param name="vect">входной вектор</param>
        /// <param name="Function">функция</param>
        /// <param name="window">окно</param>
        /// <returns>Результат применения ф-и</returns>
        public static Vector WindowFuncDouble(Vector vect, Func<Vector, double> Function, int window)
        {
            Vector input, vect1 = vect.Shift(window);

            int n = vect1.Count - window;

            List<double> DoubList = new List<double>();
            double[] data = new double[window];

            for (int i = 0; i < n; i++)
            {
                data = new double[window];
                input = vect1.CutAndZero(i + window);
                Array.Copy(input.ToArray(), i, data, 0, window);
                input = new Vector(data);
                DoubList.Add(Function(input));
            }

            return Vector.FromList(DoubList);

        }

        /// <summary>
        /// Реализация оконных функций
        /// </summary>
        /// <param name="vect">входной вектор</param>
        /// <param name="Function">функция</param>
        /// <param name="window">окно</param>
        /// <param name="stride">Шаг</param>
        /// <returns>Результат применения ф-и</returns>
        public static Vector WindowFuncDouble(Vector vect, Func<Vector, double> Function, int window, int stride)
        {
            Vector input, vect1 = vect.Shift(window);
            int n = vect1.Count - window;
            List<double> DoubList = new List<double>();
            double[] data;


            for (int i = 0; i < n; i += stride)
            {
                data = new double[window];
                input = vect1.CutAndZero(i + window);
                Array.Copy(input.ToArray(), i, data, 0, window);
                input = new Vector(data);
                DoubList.Add(Function(input));


            }

            return Vector.FromList(DoubList).InterpolayrZero(stride);

        }

        /// <summary>
        /// Следующая степень числа 2
        /// </summary>
        /// <param name="n">входное число</param>
        /// <returns></returns>
        public static int NextPow2(int n)
        {
            double log2 = Math.Log(n, 2);
            int pow = log2 % 1 == 0 ? n : 1 << (int)(log2 + 1); // Возведение битовым сдвигом
            return pow;
        }
        #endregion

        #region Сумма
        /// <summary>
        /// Суммирование всех элементов массива типа double
        /// </summary>
        public static double Summ(double[] mass)
        {
            double summ = 0;

            for (int i = 0; i < mass.Length; i++)
                summ += mass[i];

            return summ;
        }

        /// <summary>
        /// Суммирование всех элементов массива типа double
        /// </summary>
        public static float Summ(float[] mass)
        {
            float summ = 0;

            for (int i = 0; i < mass.Length; i++)
                summ += mass[i];

            return summ;
        }


        /// <summary>
        /// Поэлементная сумма
        /// </summary>
        /// <param name="vectors">Массив векторов</param>
        /// <returns>Результирующий вектор</returns>
        public static Vector Summ(Vector[] vectors)
        {
            Vector vect = new Vector(vectors[0].Count);

            for (int i = 0; i < vectors.Length; i++)
                vect += vectors[i];

            return vect;
        }


        /// <summary>
        /// Суммирование всех элементов действительного вектора
        /// </summary>
        public static double Summ(Vector vect)
        {
            int n = vect.Count;
            double summ = 0;

            for (int i = 0; i < n; i++)
                summ += vect[i];

            return summ;
        }

        /// <summary>
        /// Суммирование всех элементов массива типа int
        /// </summary>
        public static int Summ(int[] mass)
        {
            int summ = 0;

            for (int i = 0; i < mass.Length; i++)
                summ += mass[i];

            return summ;
        }

        /// <summary>
        /// Суммирование всех элементов комплексного вектора
        /// </summary>
        public static Complex Summ(ComplexVector vect)
        {
            int n = vect.Count;
            Complex summ = 0;

            for (int i = 0; i < n; i++)
                summ += vect[i];

            return summ;
        }

        #endregion

        #region Интеграл
        /// <summary>
        /// Вычисляет интегральную функцию действительный вектор
        /// Входной вектор апроксиммирован полиномом 0-го порядка
        /// с коэфициентом 2
        /// </summary>
        /// <returns></returns>
        public static Vector IntegralInterp(Vector A)
        {

            int kRasshR = 2;
            Vector B = new Vector(A.Count * kRasshR), C, D = A.InterpolayrZero(kRasshR);

            for (int i = 0; i < B.Count; i++)
            {
                C = D.CutAndZero(i + 1);
                B[i] = Summ(C / kRasshR);
            }

            return B.Downsampling(kRasshR);
        }

        /// <summary>
        /// Вычисляет интегральную функцию действительный вектор
        /// </summary>
        /// <returns></returns>
        public static Vector Integral(Vector A)
        {
            Vector B = new Vector(A.Count);
            double sum = 0;

            for (int i = 0; i < B.Count; i++)
            {
                sum += A[i];
                B[i] = sum;
            }

            return B;
        }

        /// <summary>
        /// Вычисляет интегральную функцию действительный вектор
        /// </summary>
        /// <param name="A">Входной вектор</param>
        /// <param name="fd">Частота дискретизации</param>
        public static Vector Integral(Vector A, double fd)
        {
            Vector B = new Vector(A.Count);
            double sum = 0;

            for (int i = 0; i < B.Count; i++)
            {
                sum += A[i];
                B[i] = sum;
            }

            return B / fd;
        }

        #endregion

        #region Производные
        /// <summary>
        /// Вычисляет диференциальную функцию действительный вектор
        /// </summary>
        /// <param name="A"> Входной вектор</param>
        public static Vector Diff(Vector A)
        {
            Vector B = new Vector(A.Count)
            {
                [0] = A[0]
            };

            for (int i = 1; i < B.Count; i++)
            {
                B[i] = A[i] - A[i - 1];
            }
            B[0] = B[1];


            return B;
        }

        /// <summary>
        /// Вычисляет диференциальную функцию действительный вектор
        /// </summary>
        /// <param name="A"> Входной вектор</param>   
        /// <param name="fd"> Частота дискретизации</param>
        public static Vector Diff(Vector A, double fd)
        {
            Vector B = new Vector(A.Count)
            {
                [0] = A[0]
            };

            for (int i = 1; i < B.Count; i++)
            {
                B[i] = (A[i] - A[i - 1]) * fd;
            }

            return B;
        }

        /// <summary>
        /// Вычисляет диференциальную функцию действительный вектор (без первого отсчета)
        /// </summary>
        /// <param name="A"> Входной вектор</param>
        /// <param name="fd"> Частота дискретизации</param>
        public static Vector DiffWithOutF(Vector A, double fd)
        {
            Vector B = new Vector(A.Count - 1);

            for (int i = 1; i < B.Count; i++)
            {
                B[i - 1] = (A[i] - A[i - 1]) * fd;
            }

            return B;
        }




        /// <summary>
        /// Вычисляет i-ю производную по dx
        /// </summary>
        /// <param name="A">Входной вектор</param>
        /// <param name="i">Порядок производной 1, 2, 3 ....</param>
        /// <returns>Действительный вектор</returns>
        public static Vector Diff(Vector A, int i)
        {
            Vector B = A.Clone();
            for (int j = 0; j < i; j++)
            {
                B = Diff(B);
            }

            return B;
        }
        #endregion

        #region Перемножение
        /// <summary>
        /// Перемножение всех элементов массива типа double
        /// </summary>
        public static double Multiplication(double[] mass)
        {
            double multipl = 1;

            for (int i = 0; i < mass.Length; i++)
            {
                multipl *= mass[i];
            }

            return multipl;
        }



        /// <summary>
        /// Перемножение всех элементов действительного вектора
        /// </summary>
        public static double Multiplication(Vector vect)
        {
            int n = vect.Count;
            double multipl = 1;


            for (int i = 0; i < n; i++)
            {
                multipl *= vect[i];
            }

            return multipl;
        }


        /// <summary>
        /// Перемножение всех элементов действительного вектора
        /// </summary>
        public static Vector MultiplicationFunction(Vector vect)
        {
            int n = vect.Count;
            Vector multipl = new Vector(vect.Count)
            {
                [0] = vect[0]
            };

            for (int i = 1; i < n; i++)
            {
                multipl[i] = multipl[i - 1] * vect[i];
            }

            return multipl;
        }

        /// <summary>
        /// Возвращает набор степеней данного числа
        /// </summary>
        /// <param name="inp">Число</param>
        /// <param name="pow">максимальная степень</param>
        public static Vector PowFunction(double inp, int pow)
        {
            int n = pow + 1;
            Vector multipl = new Vector(n)
            {
                [0] = 1
            };

            for (int i = 1; i < n; i++)
            {
                multipl[i] = multipl[i - 1] * inp;
            }

            return multipl;
        }


        /// <summary>
        /// Перемножение всех элементов массива типа int
        /// </summary>
        public static int Multiplication(int[] mass)
        {
            int multipl = 1;

            for (int i = 0; i < mass.Length; i++)
            {
                multipl *= mass[i];
            }

            return multipl;
        }

        #endregion
    }
}