using System;

namespace AI.NeuralSymbolic
{
    /// <summary>
    /// Преобразование между системами счисления
    /// </summary>
    [Serializable]
    public static class NumConverter
    {
        /// <summary>
        /// Из двоичной в десятичную
        /// </summary>
        public static int BinaryToDecimal(this bool[] binary)
        {
            int ret = 0;

            for (int i = 0; i < binary.Length; i++)
                if (binary[i])
                    ret += (int)(Math.Pow(2, binary.Length - i - 1));

            return ret;
        }

        /// <summary>
        /// Из двоичной в десятичную
        /// </summary>
        public static int BinaryToDecimal(this string binary)
        {
            var arr = binary.StringToBitArray();
            return arr.BinaryToDecimal();
        }

        /// <summary>
        /// Строка в массив бит
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static bool[] StringToBitArray(this string binary)
        {
            bool[] ret = new bool[binary.Length];

            for (int i = 0; i < binary.Length; i++)
                ret[i] = binary[i] == '1';

            return ret;
        }

        /// <summary>
        /// Перевод из десятичной в двоичную систему
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        public static string DecimalToBinaryStr(this int decim)
        {
            return Convert.ToString(decim, 2);
        }

        /// <summary>
        /// Перевод из десятичной в двоичную систему
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        public static bool[] DecimalToBinaryBits(this int decim)
        {
            return decim.DecimalToBinaryStr().StringToBitArray();
        }


        /// <summary>
        /// Перевод из десятичной системы в код Грея
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        public static string DecimalToGrayStr(this int decim)
        {
            return Convert.ToString(decim ^ (decim >> 1), 2);
        }

        /// <summary>
        /// Перевод из десятичной системы в код Грея
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        public static bool[] DecimalToGrayBits(this int decim)
        {
            return decim.DecimalToGrayStr().StringToBitArray();
        }

        /// <summary>
        /// Перевод из десятичной системы в код Грея
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        /// <param name="count">Количество цифр в представлении числа</param>
        public static string DecimalToGrayStr(this int decim, int count)
        {
            var str = Convert.ToString(decim ^ (decim >> 1), 2);
            if (count < str.Length)
                throw new Exception("Необходимая размерность меньше числа символов кода");

            if (count > str.Length)
            {
                string zeros = new string('0', count - str.Length);
                str = zeros + str;
            }

            return str;
        }

        /// <summary>
        /// Перевод из десятичной системы в код Грея
        /// </summary>
        /// <param name="decim">Число в десятичной системе</param>
        /// <param name="count">Количество цифр в представлении числа</param>
        public static bool[] DecimalToGrayBits(this int decim, int count)
        {
            return decim.DecimalToGrayStr(count).StringToBitArray();
        }

        /// <summary>
        /// Декодирование кода Грея в десятичный 
        /// </summary>
        /// <param name="binary">Бинарный код Грея</param>
        /// <returns></returns>
        public static int GrayDecoder(this string binary)
        {
            var ret = binary.StringToBitArray();
            return ret.GrayDecoder();
        }

        /// <summary>
        /// Декодирование кода Грея в десятичный 
        /// </summary>
        /// <param name="binary">Бинарный код Грея</param>
        /// <returns></returns>
        public static int GrayDecoder(this bool[] binary)
        {
            long bin = 0;
            string code = "";

            for (int i = 0; i < binary.Length; i++)
            {
                bin ^= binary[i] ? 1 : 0;
                code += bin;
            }



            return code.BinaryToDecimal();
        }

    }
}
