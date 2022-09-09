using AI.BackEnds.DSP.NWaves.Filters.Fda;
using AI.BackEnds.DSP.NWaves.Windows;
using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;
using System.Threading.Tasks;

namespace AI.DSP.Decomposition
{
    /// <summary>
    /// Декомпозиция сигнала при помощи КИХ фильтров
    /// </summary>
    [Serializable]
    public class FiltersDecomp
    {
        private readonly Vector[] _responses;

        /// <summary>
        /// Декомпозиция сигнала при помощи КИХ фильтров
        /// </summary>
        public FiltersDecomp(double[] fStart, double[] fEnd, int fd, int order = 157)
        {
            if (fStart.Length != fEnd.Length)
            {
                throw new Exception("fStart.Length != fEnd.Length");
            }

            _responses = new Vector[fStart.Length];
            

            for (int i = 0; i < fStart.Length; i++)
            {
                _responses[i] = FIRSincImpulseResponse(fd, fStart[i], fEnd[i], order);
            }
        }

        /// <summary>
        /// Декомпозиция сигнала по полосам
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public Vector[] Decomosition(Vector signal)
        {
            Vector[] signals = new Vector[_responses.Length];

            Parallel.For(0, _responses.Length, i =>
            {
                signals[i] = RunFirFilter(signal, _responses[i]);
            });

            return signals;
        }

        /// <summary>
        /// Декомпозиция и нормировка сигнала по полосам
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public Tuple<Vector[], Vector> DecomositionBalanceSTD(Vector signal)
        {
            Vector[] signals = new Vector[_responses.Length];
            Vector std = new Vector(_responses.Length);

            Parallel.For(0, _responses.Length, i =>
            {
                signals[i] = RunFirFilter(signal, _responses[i]);
                std[i] = signals[i].Std();//(_fe[i] - _fs[i]);
                signals[i] /= std[i];
            });

            return new Tuple<Vector[], Vector>(signals, std);
        }

        /// <summary>
        /// Сумма декопозированного сигнала
        /// </summary>
        public Vector SumDecomosition(Vector signal)
        {
            Vector[] s = Decomosition(signal);

            Vector signal_ = new Vector(s[0].Count);

            for (int i = 0; i < s.Length; i++)
            {
                signal_ += s[i];
            }

            return signal_;
        }

        /// <summary>
        /// Разложение сигнала с помощью фильтров
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns></returns>
        public Tuple<Vector, Vector> SumDecomositionBalanceSTD(Vector signal)
        {
            Tuple<Vector[], Vector> d = DecomositionBalanceSTD(signal);
            Vector[] s = d.Item1;

            Vector signal_ = new Vector(s[0].Count);

            for (int i = 0; i < s.Length; i++)
            {
                signal_ += s[i];
            }

            return new Tuple<Vector, Vector>(signal_, d.Item2);
        }

        /// <summary>
        /// КИХ фильтр типа sin(x)/x (Синтезирует импульсную характеристику)
        /// </summary>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="f1">Нижняя частота среза</param>
        /// <param name="f2">Верхняя частота среза</param>
        /// <param name="order">Порядок</param>
        /// <param name="windowType">Тип весового окна</param>
        public static Vector FIRSincImpulseResponse(int fd, double f1 = 2, double f2 = 4, int order = 37, WindowTypes windowType = WindowTypes.Blackman)
        {
            if (order % 2 == 0)
            {
                order++;
            }

            return DesignFilter.FirWinBp(order, f1 / fd, f2 / fd, windowType);
        }

        /// <summary>
        /// Синтезирует импульсную характеристику методом Ремеза
        /// </summary>
        /// <param name="fd">Частота дискретизации</param>
        /// <param name="f1_stop">Нижняя частота подавление</param>
        /// <param name="f1_pass">Нижняя частота пропускания</param>
        /// <param name="f2_stop">Верхняя частота подавление</param>
        /// <param name="f2_pass">Верхняя частота пропускания</param>
        /// <param name="w_stop">Коэф. в полосе подавления</param>
        /// <param name="w_pass">Коэф. в полосе пропускания</param>
        /// <param name="order">Порядок</param>
        /// <param name="windowType">Тип весового окна</param>
        public static Vector FIRRemezImpulseResponse(int fd, double f1_stop = 2, double f1_pass = 2.1, double f2_stop = 4.2, double f2_pass = 4, double w_stop = 0.05, double w_pass = 0.95, int order = 37, WindowTypes windowType = WindowTypes.Kaiser)
        {
            if (order % 2 == 0)
            {
                order++;
            }

            return DesignFilter.FirEquirippleBp(order, f1_stop / fd, f1_pass / fd, f2_stop / fd, f2_pass / fd, w_stop, w_pass, w_stop);
        }

        /// <summary>
        /// Прямой проход КИХ фильтра
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="impulseResponse">Импульсная характеристика</param>
        public static Vector RunFirFilter(Vector signal, Vector impulseResponse)
        {
            int orderHalf = impulseResponse.Count / 2;
            return FastConv.FastConvolution(signal, impulseResponse).GetInterval(orderHalf, signal.Count + orderHalf);
        }
    }
}
