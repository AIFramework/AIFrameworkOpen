using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;

namespace AI.DSP.Modulation
{
    /// <summary>
    /// Amplitude modulation
    /// </summary>
    [Serializable]
    public class AM : IModulator
    {
        private readonly double _dt, _f0, _m, _2pi;
        private readonly int _fd;

        /// <summary>
        /// Инициализация модулятора ампл. модуляции
        /// </summary>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="f0">Carrier frequency</param>
        /// <param name="m">Modulation rate</param>
        public AM(int fd, double f0, double m = 1)
        {
            _2pi = Math.PI * 2;
            _fd = fd;
            _dt = 1.0 / fd;
            _f0 = f0;
            _m = m;

        }



        /// <summary>
        /// Модуляция
        /// </summary>
        /// <param name="signalIn"></param>
        /// <returns></returns>
        public Channel Modulate(Channel signalIn)
        {
            if (Math.Abs(_fd - signalIn.Fd) < 0.3)
            {
                return ModulateSimple(signalIn);
            }
            else
            {
                throw new ArgumentException("не совпадают частоты дискретизации");
            }
        }

        /// <summary>
        /// Демодуляция
        /// </summary>
        /// <param name="channel">Channel с модулированным сигналом</param>
        public Channel Demodulate(Channel channel)
        {
            Vector dat = FastHilbert.EnvelopeIQ(channel.ChData, _fd, _f0);
            dat -= dat.Min();
            dat /= dat.Max();
            Channel ret = new Channel(dat, channel.Fd);
            return ret;
        }

        /// <summary>
        /// Модуляция для сигналов с одинаковой частотой дискретизации
        /// </summary>
        /// <param name="signalIn">Входной сигнал</param>
        private Channel ModulateSimple(Channel signalIn)
        {
            Channel signal = signalIn;
            Vector data = signal.ChData.Clone();
            double mDivDataMax = _m / data.MaxAbs();
            double n = 1.0 + _m;

            Vector outp = new Vector(data.Count);

            for (int i = 0; i < outp.Count; i++)
            {
                outp[i] = (1 + (data[i] * mDivDataMax)) * Math.Sin(_2pi * i * _dt * _f0) / n;
            }

            Channel retCh = new Channel(outp, _fd, signal.Name, signal.Description)
            {
                ScaleVolt = signal.ScaleVolt
            };

            return retCh;
        }
    }
}
