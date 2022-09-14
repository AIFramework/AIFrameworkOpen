/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 05.01.2019
 * Время: 21:40
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Description of GetSpectrEnerge.
    /// </summary>
    [Serializable]
    public class GetSpectrEnerge
    {
        private readonly List<double> bFI = new List<double>();
        private readonly List<double> eFI = new List<double>();
        private IntervalData iD;
        private readonly double _fd;

        /// <summary>
        /// Анализ формант
        /// </summary>
        /// <param name="fd">Частота дискретизации</param>
        public GetSpectrEnerge(double fd)
        {
            _fd = fd;
        }

        /// <summary>
        /// Добавление диапозона частот
        /// </summary>
        /// <param name="b">Начальная частота форманты</param>
        /// <param name="e">Конечная частота форманты</param>
        public void Add(double b, double e)
        {
            bFI.Add(b);
            eFI.Add(e);
        }

        /// <summary>
        ///  Возвращает суммарные амплитуды в формантах
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        /// <returns>Вектор амплитуд</returns>
        public Vector GetAmplFreq(Vector inp)
        {
            FFT fft = new FFT(inp.Count);
            Vector vect = fft.GetSpectrum(inp, WindowForFFT.BlackmanWindow);
            GetIntervalData(vect.Count * 2);
            return iD.GetVect(Functions.Summ, vect);
        }

        private void GetIntervalData(int Count)
        {
            iD = new IntervalData();

            for (int i = 0; i < bFI.Count; i++)
            {
                iD.Add((int)(bFI[i] * Count / _fd), (int)(eFI[i] * Count / _fd)); // Перевод частот в отсчеты
            }
        }




    }
}
