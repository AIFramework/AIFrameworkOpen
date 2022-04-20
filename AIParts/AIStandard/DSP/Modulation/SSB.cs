using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using AI.DSP.FIR;
using System;

namespace AI.DSP.Modulation
{
    /// <summary>
    /// Однополосная амплитудная модуляция
    /// </summary>
    [Serializable]
    public class SSB : IModulator
    {
        private readonly double _dt, _f0, _2pi;
        private readonly int _fd;
        private readonly SSBType sBType;

        /// <summary>
        /// Инициализация модулятора однополосной ампл. модуляции
        /// </summary>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="f0">Carrier frequency</param>
        /// <param name="ssbType">Какая полоса будет подавлена</param>
        public SSB(int fd, double f0, SSBType ssbType = SSBType.Down)
        {
            _2pi = Math.PI * 2;
            _fd = fd;
            _dt = 1.0 / fd;
            _f0 = f0;
            sBType = ssbType;
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
        /// <param name="channel">Channel</param>
        public Channel Demodulate(Channel channel)
        {

            FilterLowButterworth filter = new FilterLowButterworth(_fd / 2, _fd, channel.ChData.Count);
            Tuple<Vector, Vector> iq = FastHilbert.IQ(channel.ChData, _fd, _f0);
            Vector inphase = iq.Item1;
            Vector qud = iq.Item2;
            Vector arg = Vector.SeqBeginsWithZero(1, inphase.Count) * (Math.PI / 2.0);
            Vector cos = arg.Transform(Math.Cos);
            Vector sin = arg.Transform(Math.Sin);



            if (sBType == SSBType.Down)
            {
                Vector v1 = (cos * inphase + sin * qud);
                Vector v2 = (cos * qud - sin * inphase);//v1 * cos - v2 * sin
                return new Channel(v1 * cos - v2 * sin, _fd);
            }
            else
            {
                Vector v1 = (sin * qud - cos * inphase);
                Vector v2 = (cos * qud + sin * inphase);
                return new Channel(-(v1 * cos - v2 * sin), _fd);
            }
        }

        /// <summary>
        /// Модуляция для сигналов с одинаковой частотой дискретизации
        /// </summary>
        /// <param name="signalIn">Входной сигнал</param>
        private Channel ModulateSimple(Channel signalIn)
        {
            Channel signal = signalIn;
            Vector data = signal.ChData.Clone();
            data -= data.Mean();
            Vector datHilb = FastHilbert.ConjugateToTheHilbert(data); // сопряженный по Гильберту сигнал
            double normal = 1.0 / data.MaxAbs(); // 0.5

            Vector outp = new Vector(data.Count);

            // Подавление нижней полосы
            if (sBType == SSBType.Up)
            {
                for (int i = 0; i < outp.Count; i++)
                {
                    outp[i] = normal * (data[i] * Math.Cos(_2pi * i * _dt * _f0) - datHilb[i] * Math.Sin(_2pi * i * _dt * _f0));
                }
            }
            // Подавление веррхней полосы
            else
            {
                for (int i = 0; i < outp.Count; i++)
                {
                    outp[i] = normal * (data[i] * Math.Cos(_2pi * i * _dt * _f0) + datHilb[i] * Math.Sin(_2pi * i * _dt * _f0));
                }
            }

            Channel retCh = new Channel(outp, _fd, signal.Name, signal.Description)
            {
                ScaleVolt = signal.ScaleVolt
            };

            return retCh;
        }
    }

    /// <summary>
    /// Какая полоса будет подавлена
    /// </summary>
    public enum SSBType
    {
        /// <summary>
        /// Верхняя
        /// </summary>
        Up,
        /// <summary>
        /// Нижняя
        /// </summary>
        Down
    }
}
