/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 21.09.2018
 * Время: 9:30
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */

using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Оптимальный (согласованный) фильтр
    /// </summary>
    [Serializable]
    public class OptimalFilter
    {
        private readonly ComplexVector corFunc;
        private readonly FFT fur;

        /// <summary>
        /// Оптимальный (согласованный) фильтр
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="n"></param>
        public OptimalFilter(Vector signal, int n)
        {
            fur = new FFT(n);
            corFunc = fur.CalcFFT(signal.Revers().CutAndZero(n)) / fur.SemplesCount;
        }

        /// <summary>
        /// Прохождение фильтра
        /// </summary>
        /// <param name="signal">Сигнал на входе</param>
        public Vector Result(Vector signal)
        {
            ComplexVector signalFFT = fur.CalcFFT(signal);
            return fur.RealIFFT(signalFFT * corFunc);
        }

        /// <summary>
        /// Сжатие ЛЧМ по спектру
        /// </summary>
        public Vector SpectrCompressLFM(Vector signal, int fd)
        {
            Vector signal2 = signal * signal;
            return Filters.FilterLow(signal2, 2000, fd);
        }



    }
}
