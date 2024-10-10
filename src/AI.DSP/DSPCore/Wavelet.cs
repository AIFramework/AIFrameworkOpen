/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 06.06.2017
 * Время: 17:56
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.Statistics;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Вейвлеты
    /// </summary>
    [Serializable]
    public class Wavelet
    {
        private readonly PerentWavelet _pw;

        /// <summary>
        /// Непрерывное вейвлет преобразование
        /// </summary>
        /// <param name="pw">Порождение вейвлетов</param>
        public Wavelet(PerentWavelet pw)
        {
            _pw = pw;
        }

        /// <summary>
        /// Поиск патернов в сигнале
        /// </summary>
        /// <param name="sig">Сигнал</param>
        /// <returns>Максимумы патернов</returns>
        public Vector SerchPatern(Vector sig)
        {
            ComplexVector spectr = _pw.fur.CalcFFT(sig - Statistic.ExpectedValue(sig));
            Vector[] output = new Vector[_pw.waveletSpectrs.Length];
            double std = Statistic.CalcStd(sig);


            for (int i = 0; i < output.Length; i++)
            {
                output[i] = _pw.fur.RealIFFT(_pw.waveletSpectrs[i] * spectr);
                output[i] /= std * _pw.std[i];
                output[i] *= 6 * _pw.scals[i];
            }

            Vector res = Statistic.MaxEns(output);

            return res;
        }


    }

    /// <summary>
    /// Функция порождения вейвлетов
    /// </summary>
    public class PerentWavelet
    {
        /// <summary>
        /// Спектры ф-й
        /// </summary>
        public ComplexVector[] waveletSpectrs;
        /// <summary>
        /// Фурье
        /// </summary>
        public FFT fur;
        /// <summary>
        /// Вектор СКО
        /// </summary>
        public Vector std;
        /// <summary>
        /// Масштабы
        /// </summary>
        public Vector scals;

        /// <summary>
        /// Порождения вейвлетов
        /// </summary>
        /// <param name="wavelet">Порождающая функция</param>
        /// <param name="scales">Масштабы</param>
        /// <param name="n">Размерность входа</param>
        public PerentWavelet(Func<double, Vector> wavelet, Vector scales, int n)
        {
            fur = new FFT(n);
            scals = scales.Clone();

            waveletSpectrs = new ComplexVector[scales.Count];
            Vector wavReal;
            std = new Vector(scales.Count);

            for (int i = 0; i < waveletSpectrs.Length; i++)
            {
                wavReal = wavelet(scales[i]).Revers();
                wavReal -= Statistic.ExpectedValue(wavReal);
                //wavReal *= scales[i];
                waveletSpectrs[i] = fur.CalcFFT(wavReal);
                waveletSpectrs[i] /= wavReal.Count;
                std[i] = Statistic.CalcStd(wavReal);
            }

        }



    }
}
