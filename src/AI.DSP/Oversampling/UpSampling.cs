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


    }
}
