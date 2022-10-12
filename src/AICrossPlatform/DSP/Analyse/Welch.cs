using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.Analyse
{
    /// <summary>
    /// Метода Уэлча
    /// </summary>
    [Serializable]
    public class Welch
    {
        /// <summary>
        /// Запуск метода Уэлча
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="window">Окно</param>
        /// <param name="overlap">Перекрытие</param>
        /// <param name="windowForFFT">Функция взвешивания окна</param>
        public static Vector WelchRun(Vector signal, int window, double overlap, Vector windowForFFT)
        {
            FFT fft = new FFT(window);
            int step = (int)((1.0 - overlap) * window);

            int len = signal.Count - window;
            Vector outp = new Vector(window);
            double n = 0;
            double[] s = signal;

            for (int i = 0; i < len; i += step)
            {
                n++;
                double[] outpSignal = new double[window];
                Buffer.BlockCopy(s, 8 * i, outpSignal, 0, 8 * window);
                Vector data = outpSignal;
                data *= windowForFFT;
                data = fft.CalcFFT(data).MagnitudeVector;
                outp += data * data;
            }

            outp /= n;

            return outp;
        }

        /// <summary>
        /// Запуск метода периодограммы
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="window">Окно</param>
        /// <param name="windowForFFT">Функция взвешивания окна</param>
        public static Vector BartlettRun(Vector signal, int window, Vector windowForFFT)
        {
            return WelchRun(signal, window, 0, windowForFFT);
        }


    }
}
