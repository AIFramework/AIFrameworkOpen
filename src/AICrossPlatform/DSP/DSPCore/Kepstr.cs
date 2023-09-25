/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 07.06.2017
 * Время: 11:46
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.Analyse;
using AI.HightLevelFunctions;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Кепстральный анализ
    /// </summary>
    [Serializable]
    public static class Kepstr
    {
        /// <summary>
        /// Быстрое кепстральное преобразование
        /// </summary>
        /// <param name="signal">Сигнал</param>
		public static Vector FKT(Vector signal)
        {
            ComplexVector spectr = FFT.CalcFFT(signal);
            Vector spectrPow = 2 * spectr.MagnitudeVector.Transform(Math.Log);
            return FFT.CalcFFT(spectrPow).RealVector / spectrPow.Count;
        }

        /// <summary>
        /// Быстрое кепстральное преобразование
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="window_size_1">Размер окна для преобразования Welch1</param>
		public static Vector WelchFKT(Vector signal, int window_size_1)
        {
            Vector window_1 = WindowForFFT.BlackmanWindow(window_size_1);

            Vector spectrPow = Welch.WelchRun(signal, window_size_1, 0.5, window_1).Transform(Math.Log);
            return FFT.CalcFFT(spectrPow).RealVector / spectrPow.Count;
        }



        /// <summary>
        /// Быстрое кепстральное преобразование
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public static Vector FKT(ComplexVector signal)
        {
            ComplexVector spectr = FFT.CalcFFT(signal);
            Vector spectrPow = 2 * spectr.MagnitudeVector.Transform(Math.Log);
            return FFT.CalcFFT(spectrPow).RealVector / spectrPow.Count;
        }


    }
}
