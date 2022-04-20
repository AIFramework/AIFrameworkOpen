using AI.DataStructs.Algebraic;
using AI.DSP.FIR;
using System;

namespace AI.DSP.Modulation
{
    /// <summary>
    /// Амплитудная модуляция с подавленной центральной частотой
    /// </summary>
    [Serializable]
    public class DSB : IModulator
    {
        private readonly double _f0;
        private readonly int _fd;

        /// <summary>
        /// Инициализация модулятора ампл. модуляции с подавленной несущей
        /// </summary>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="f0">Carrier frequency</param>
        public DSB(int fd, double f0)
        {
            _fd = fd;
            _f0 = f0;
        }

        /// <summary>
        /// Демодулятор
        /// </summary>
        /// <param name="channel">Channel</param>
        public Channel Demodulate(Channel channel)
        {
            FilterLowButterworth filter = new FilterLowButterworth((int)(_f0), _fd, channel.ChData.Count);
            Vector cos = Vector.SeqBeginsWithZero(1, channel.ChData.Count) * (Math.PI * 2 * _f0 / _fd);
            cos = cos.Transform(Math.Cos);
            Vector outp = filter.FilterOutp(cos * channel.ChData);
            return new Channel(outp, _fd);

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
        /// Модуляция для сигналов с одинаковой частотой дискретизации
        /// </summary>
        /// <param name="signalIn">Входной сигнал</param>
        private Channel ModulateSimple(Channel signalIn)
        {
            Vector cos = Vector.SeqBeginsWithZero(1, signalIn.ChData.Count) * (Math.PI * 2 * _f0 / _fd);
            cos = cos.Transform(Math.Cos);
            return new Channel(cos * (signalIn.ChData - signalIn.ChData.Mean()), _fd);
        }
    }

}
