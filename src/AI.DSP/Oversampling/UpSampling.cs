using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;

namespace AI.DSP.Oversampling
{
    /// <summary>
    /// Увеличение частоты дискретизации
    /// </summary>
    public static class UpSampling
    {
        /// <summary>
        /// Увеличение частоты дискретизации(Фильтр с прямоугольной АЧХ)
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="kUpSemp">Во сколько раз увеличить</param>
        public static Vector UpSamplingRectFilter(Vector signal, int fd, int kUpSemp)
        {
            int newfD = fd * kUpSemp;
            Vector newSignal = signal.UnPooling(kUpSemp);
            int fFiltr = newfD / (2 * kUpSemp);
            newSignal = Filters.FilterLow(newSignal, fFiltr, newfD) * kUpSemp;
            return newSignal;
        }

        /// <summary>
        /// Увеличение частоты дискретизации(Фильтр Баттерворта)
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="kUpSemp">Во сколько раз увеличить</param>
        /// <param name="order">Порядок фильтра</param>
        public static Vector UpSamplingButterworthFilter(Vector signal, int fd, int kUpSemp, int order = 7)
        {
            int newfD = fd * kUpSemp;
            Vector newSignal = signal.UnPooling(kUpSemp);
            int fFiltr = newfD / (2 * kUpSemp);
            newSignal = Filters.FilterLowButterworthCFH(newSignal, fFiltr, newfD, order) * kUpSemp;
            return newSignal;
        }


        /// <summary>
        /// Увеличение частоты дискретизации(Фильтр Баттерворта)
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="kUpSemp">Во сколько раз увеличить</param>
        /// <param name="order">Порядок фильтра</param>
        public static Vector UpSamplingButterworthFilterW(Vector signal, int fd, int kUpSemp, int order = 7)
        {
            int newfD = fd * kUpSemp;
            Vector newSignal = signal.UnPooling(kUpSemp);
            Vector blac = WindowForFFT.HannWindow(newSignal.Count) + 1e-2;
            newSignal *= blac;
            int fFiltr = newfD / (2 * kUpSemp);
            newSignal = Filters.FilterLowButterworthCFH(newSignal, fFiltr, newfD, order) * kUpSemp;
            return newSignal / blac;
        }


        /// <summary>
        /// Увеличение частоты дискретизации квадратичными сплайнами
        /// </summary>
        /// <param name="signal">Исходный сигнал</param>
        /// <param name="kUpSemp">Во сколько раз увеличить</param>
        /// <returns>Сигнал с увеличенной частотой дискретизации</returns>
        public static Vector UpSamplingQudratic(Vector signal, int kUpSemp)
        {
            int origCount = signal.Count;
            int newCount = (origCount - 1) * kUpSemp + 1;
            Vector newSignal = new Vector(newCount);

            for (int n = 0; n < origCount - 1; n++)
            {
                double prev = (n == 0) ? signal[0] : signal[n - 1];
                double cur = signal[n];
                double next = signal[n + 1];

                for (int i = 0; i < kUpSemp; i++)
                {
                    double a = i / (double)kUpSemp; // нормированная позиция в сегменте
                                                    // Вычисление по квадратичной интерполяции
                    double interp = cur
                        + 0.5 * (next - prev) * a
                        + 0.5 * (next + prev - 2 * cur) * a * a;
                    newSignal[n * kUpSemp + i] = interp;
                }
            }

            newSignal[newCount - 1] = signal[origCount - 1];

            return newSignal;
        }


    }
}
