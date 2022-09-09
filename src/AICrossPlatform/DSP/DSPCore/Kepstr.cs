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
using AI.HightLevelFunctions;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    ///Кепстральный анализ
    /// </summary>
    [Serializable]
    public static class Kepstr
    {
        /// <summary>
        /// Быстрое кепстральное преобразование
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns></returns>
		public static Vector FKT(Vector signal)
        {
            ComplexVector spectr = FFT.CalcFFT(signal);
            Vector Aspectr = FunctionsForEachElements.Ln(spectr.MagnitudeVector.Transform(x => Math.Pow(x, 2)));
            return FFT.CalcFFT(Aspectr).RealVector / Aspectr.Count;
        }



        /// <summary>
        /// Быстрое кепстральное преобразование
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns></returns>
        public static Vector FKT(ComplexVector signal)
        {
            ComplexVector spectr = FFT.CalcFFT(signal);
            Vector Aspectr = FunctionsForEachElements.Ln(spectr.MagnitudeVector.Transform(x => Math.Pow(x, 2)));
            return FFT.CalcFFT(Aspectr).RealVector / Aspectr.Count;
        }
    }
}
