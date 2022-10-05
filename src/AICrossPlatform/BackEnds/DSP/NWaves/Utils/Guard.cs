using System;
using System.Diagnostics;

namespace AI.BackEnds.DSP.NWaves.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerStepThrough]
    public static class Guard
    {
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstNonPositive(double arg, string argName = "argument")
        {
            if (arg < 1e-30)
            {
                throw new ArgumentException($"{argName} должен быть положительным!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstInequality(double arg1, double arg2, string arg1Name = "argument1", string arg2Name = "argument2")
        {
            if (Math.Abs(arg2 - arg1) > 1e-30)
            {
                throw new ArgumentException($"{arg1Name} должен быть равен {arg2Name}!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstInvalidRange(double low, double high, string lowName = "low", string highName = "high")
        {
            if (high - low < 1e-30)
            {
                throw new ArgumentException($"{highName} должен быть больше {lowName}!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstExceedance(double low, double high, string lowName = "low", string highName = "high")
        {
            if (low > high)
            {
                throw new ArgumentException($"{lowName} не должен превышать {highName}!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstNotPowerOfTwo(int n, string argName = "Parameter")
        {
            int pow = (int)Math.Log(n, 2);

            if (n != 1 << pow)
            {
                throw new ArgumentException($"{argName} должен быть степенью 2!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstEvenNumber(int n, string argName = "Parameter")
        {
            if (n % 2 == 0)
            {
                throw new ArgumentException($"{argName} должен быть нечетным!");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AgainstIncorrectFilterParams(double[] freqs, double[] desired, double[] weights)
        {
            int n = freqs.Length;

            if (n < 4 || n % 2 != 0)
            {
                throw new ArgumentException("Массив частот должен иметь четное количество значений не менее 4!");
            }

            if (freqs[0] != 0 || freqs[n - 1] != 0.5)
            {
                throw new ArgumentException("Массив частот должен начинаться с 0 и заканчиваться 0,5!");
            }

            Guard.AgainstInequality(desired.Length, n / 2, "Размер желаемого массива", "половинный размер массива частот");
            Guard.AgainstInequality(weights.Length, n / 2, "Размер массива весов", "половинный размер массива частот");
        }
    }
}
