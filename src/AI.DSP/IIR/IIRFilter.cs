﻿using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;

namespace AI.DSP.IIR
{
    /// <summary>
    /// БИХ фильтр
    /// </summary>
    [Serializable]
    public class IIRFilter : IFilter
    {
        private int aLen, bLen, ofA, ofB, bL2, aL2;
        private Vector inps, outps;
        /// <summary>
        /// Имя фильтра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Коэффициенты "A"
        /// </summary>
        public Vector A { get; set; }
        /// <summary>
        /// Коэффициенты B (числитель)
        /// </summary>
        public Vector B { get; set; }

        /// <summary>
        /// Ограничение сигнала
        /// </summary>
        public double Treshold { get; set; } = 1e+300;

        /// <summary>
        /// БИХ фильтр
        /// </summary>
        /// <param name="a">Коэффициенты "A"</param>
        /// <param name="b">Коэффициенты B (числитель)</param>
        public IIRFilter(Vector a, Vector b)
        {
            Init(a, b);
        }

        private void Init(Vector a, Vector b)
        {
            aLen = a.Count;
            bLen = b.Count;

            A = a.Repeat(2);
            B = b.Repeat(2);

            aL2 = A.Count;
            bL2 = B.Count;

            Reset();
        }

        /// <summary>
        /// Filter output
        /// </summary>
        /// <param name="inp">Вход</param>
        public double FilterOutp(double inp)
        {

            // MIT License

            //Copyright(c) 2017 Tim

            //Permission is hereby granted, free of charge, to any person obtaining a copy
            //of this software and associated documentation files(the "Software"), to deal
            //in the Software without restriction, including without limitation the rights
            //to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell
            // copies of the Software, and to permit persons to whom the Software is
            // furnished to do so, subject to the following conditions:

            //The above copyright notice and this permission notice shall be included in all
            //copies or substantial portions of the Software.

            //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
            //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
            //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
            //AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
            //LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
            //OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
            //SOFTWARE.

            double outp = 0;
            inps[ofB] = inp;
            outps[ofA] = 0;

            for (int i = 0, j = bLen - ofB; i < bLen; i++, j++)
                outp += inps[i] * B[j];

            for (int i = 0, j = aLen - ofA; i < aLen; i++, j++)
                outp -= outps[i] * A[j];

            //Ограничение сигнала
            if (outp > Treshold) outp = Treshold;
            else if (outp < -Treshold) outp = -Treshold;

            outps[ofA] = outp;

            if (--ofB < 0)
                ofB = bLen - 1;


            if (--ofA < 0)
                ofA = aLen - 1;



            return outp;
        }

        /// <summary>
        /// Выход рекурсивного фильтра
        /// </summary>
        /// <param name="signal">Входной сигнал</param>
        public Vector FilterOutp(Vector signal)
        {
            Reset();
            return signal.Transform(FilterOutp);
        }

        /// <summary>
        /// Выход рекурсивного фильтра
        /// </summary>
        /// <param name="signal">Входной сигнал</param>
        /// <param name="iteration">Итерации фильтрации</param>
        public Vector FilterOutp(Vector signal, int iteration)
        {
            Vector outp = signal.Clone();

            for (int i = 0; i < iteration; i++)
            {
                Reset();
                outp = outp.Transform(FilterOutp);
            }

            return outp;
        }

        /// <summary>
        /// Сброс состояния нейронной сети
        /// </summary>
        public void Reset()
        {
            inps = new Vector(bL2);
            outps = new Vector(aL2);
            ofA = aLen;
            ofB = bLen;
        }

        /// <summary>
        /// Экспорт состояния
        /// </summary>
        public Tuple<Vector, Vector, int, int> ExportState()
        {
            return new Tuple<Vector, Vector, int, int>(inps, outps, ofA, ofB);
        }

        /// <summary>
        /// Импорт состояния фильтра
        /// </summary>
        /// <param name="inputs">Входы</param>
        /// <param name="outputs">Выходы (целевые значения)</param>
        /// <param name="offsetA">Смещение выходов</param>
        /// <param name="offsetB">Смещение входов</param>
        public void ImportState(Vector inputs, Vector outputs, int offsetA, int offsetB)
        {
            if (inputs.Count != bLen || outputs.Count != aLen)
            {
                throw new ArgumentException("Размерности не совпадают, импорт невозможен");
            }

            ofA = offsetA;
            ofB = offsetB;
            inps = inputs.Clone();
            outps = outputs.Clone();
        }

        /// <summary>
        /// Созранение фильтра
        /// </summary>
        /// <param name="path">Путь</param>
        public void Save(string path)
        {
            /*
            * Структура:
            * Проверочное слово "iir"
            * Название Unicode
            * Коэффициенты "A"
            * Коэффициенты B (числитель)
            */
            InMemoryDataStream bs = new InMemoryDataStream();
            bs.Write("iir").Write(Name).Write(A.CutAndZero(aLen)).Write(B.CutAndZero(bLen)).Zip().Save(path);
        }

        /// <summary>
        /// Загрузка фильтра
        /// </summary>
        /// <param name="path">Путь</param>
        public static IIRFilter Load(string path)
        {
            /*
             * Структура:
             * Проверочное слово "iir"
             * Название Unicode
             * Коэффициенты "A"
             * Коэффициенты B (числитель)
             */
            InMemoryDataStream bs = new InMemoryDataStream(path, isZipped: true);
            _ = bs.UnZip();
            _ = bs.ReadString();
            string name = bs.ReadString();
            Vector A = bs.ReadDoubles();
            Vector B = bs.ReadDoubles();

            IIRFilter iir = new IIRFilter(A, B)
            {
                Name = name
            };

            return iir;
        }

        /// <summary>
        /// Загрузка фильтра
        /// </summary>
        /// <param name="data">Буфер данных</param>
        public static IIRFilter Load(byte[] data)
        {
            /*
             * Структура:
             * Проверочное слово "iir"
             * Название Unicode
             * Коэффициенты "A"
             * Коэффициенты B (числитель)
             */
            InMemoryDataStream bs = new InMemoryDataStream(data, isZipped: true);
            _ = bs.UnZip();
            _ = bs.ReadString();
            string name = bs.ReadString();
            Vector A = bs.ReadDoubles();
            Vector B = bs.ReadDoubles();

            IIRFilter iir = new IIRFilter(A, B)
            {
                Name = name
            };

            return iir;
        }
    }
}
