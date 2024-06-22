using AI.DataStructs.Algebraic;
using AI.ML.Regression;
using System.Collections.Generic;
using System.Linq;

namespace AI.DSP.Analyse
{
    /// <summary>
    /// Анализ формант
    /// </summary>
    public static class WelchFormantFinder
    {
        /// <summary>
        /// Минимальные значения 1-й, 2-й и 3-ей формант в Гц
        /// </summary>
        public static Vector MinIntervals = new Vector(50, 300, 800);
        /// <summary>
        /// Максимальные значения 1-й, 2-й и 3-ей формант в Гц
        /// </summary>
        public static Vector MaxIntervals = new Vector(300, 800, 1300);


        /// <summary>
        /// Поиск формант
        /// </summary>
        /// <param name="welchData">СПМ метод Уэлча</param>
        /// <param name="freqs">Массив частот</param>
        /// <param name="thresholdFactor">Множитель адаптивного порога</param>
        /// <param name="smoothParam">Параметр сглаживания</param>
        /// <returns>Вектор пиков</returns>
        public static Vector FindFormants(Vector welchData, Vector freqs, double thresholdFactor = 1, double smoothParam = 0.03)
        {
            int k = (int)(welchData.Count*smoothParam);

            // Сглаживание данных
            Vector smooth = SignalSmooth(welchData, k);
            double sum = smooth.Sum();
            // Средняя мощность
            double averagePower = sum / smooth.Count;
            // Порог для определения пика
            double threshold = averagePower * thresholdFactor;
            // Инициализация списка для результатов. Один пик на интервал.
            List<double> formantFrequencies = new List<double>(MinIntervals.Count);

            // Заполняем список начальными значениями
            for (int i = 0; i < MinIntervals.Count; i++) formantFrequencies.Add(0);

            // Инициализация списка для хранения максимальной амплитуды на интервал
            List<double> maxAmplitudes = new List<double>(MinIntervals.Count);
            maxAmplitudes.AddRange(Enumerable.Repeat(double.MinValue, MinIntervals.Count));

            // Поиск пиков
            for (int i = 1; i < smooth.Count - 1; i++)
                if (smooth[i] > threshold && smooth[i] > smooth[i - 1] && smooth[i] > smooth[i + 1])
                {
                    double currentFrequency = freqs[i];
                    double currentAmplitude = smooth[i];
                    // Проверяем, в какой интервал попадает частота пика
                    for (int j = 0; j < MinIntervals.Count; j++)
                        if (currentFrequency >= MinIntervals[j] && currentFrequency <= MaxIntervals[j])
                        {
                            // Обновляем частоту, если амплитуда больше, чем текущая максимальная для интервала
                            if (currentAmplitude > maxAmplitudes[j])
                            {
                                maxAmplitudes[j] = currentAmplitude;
                                formantFrequencies[j] = currentFrequency;
                            }
                            break; // Частота может попадать только в один интервал, прерываем цикл
                        }
                }

            // Убираем частоты, которые не были обновлены (остались нулевыми)
            formantFrequencies.RemoveAll(freq => freq == 0);
            return formantFrequencies.ToArray();
        }


        /// <summary>
        /// Сглаживание сигнала методом Надарая-Ватсона
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="k">Число соседей</param>
        /// <returns></returns>
        public static Vector SignalSmooth(Vector signal, int k = 10)
        {
            KNNReg kNNReg = new KNNReg();
            kNNReg.Train(Vector.Seq(0,1,signal.Count), signal);
            kNNReg.IsNadrMethod = true;
            kNNReg.K = k;
            return kNNReg.PredictV(signal);
        }
    }
}

