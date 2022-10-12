//using AI.DataStructs.Algebraic;
//using AI.DSP.DSPCore;
//using AI.DSP.FIR;
//using AI.HightLevelFunctions;
//using System;

//namespace AI.DSP.Modulation
//{
//    /// <summary>
//    /// Частотная модуляция
//    /// </summary>
//    [Serializable]
//    public class FM : IModulator
//    {
//        private readonly double _dt, _f0, _deltF, _2pi;
//        private readonly int _fd;

//        /// <summary>
//        /// Инициализация модулятора частотной модуляции
//        /// </summary>
//        /// <param name="fd">Частота дискретизации</param>
//        /// <param name="f0">Несущая частота</param>
//        /// <param name="deltF">Дифиация частоты</param>
//        public FM(int fd, double f0, double deltF)
//        {
//            _2pi = Math.PI * 2;
//            _fd = fd;
//            _dt = 1.0 / fd;
//            _f0 = f0;
//            _deltF = deltF;

//        }

//        /// <summary>
//        /// Демодуляция ЧМК
//        /// </summary>
//        /// <param name="channel">Channel</param>
//        /// <returns></returns>
//        public Channel Demodulate(Channel channel)
//        {
//            Vector dat = FastHilbert.PhaseIQ(channel.ChData, _fd, _f0);
//            dat = Functions.Diff(dat);
//            dat -= dat.Min();
//            dat /= dat.Max();
//            dat = FunctionsForEachElements.PeakDel(dat);
//            Channel ret = new Channel(dat, channel.Fd);
//            return ret;
//        }

//        /// <summary>
//        /// Модуляция
//        /// </summary>
//        /// <param name="signalIn"></param>
//        /// <returns></returns>
//        public Channel Modulate(Channel signalIn)
//        {
//            if (Math.Abs(_fd - signalIn.Fd) < 0.3)
//                return ModulateSimple(signalIn);

//            else
//                throw new ArgumentException("не совпадают частоты дискретизации");
//        }


//        /// <summary>
//        /// Модуляция для сигналов с одинаковой частотой дискретизации
//        /// </summary>
//        /// <param name="signalIn">Входной сигнал</param>
//        private Channel ModulateSimple(Channel signalIn)
//        {
//            FilterLowButterworth filter = new FilterLowButterworth((int)_deltF, _fd, signalIn.ChData.Count, 9);
//            Channel signal = signalIn.Filtration(filter);
//            Vector data = signal.ChData;

//            data /= data.MaxAbs();
//            data -= data.Mean();
//            data = Functions.Integral(data, _fd);


//            Vector outp = new Vector(data.Count);

//            for (int i = 0; i < outp.Count; i++)
//                outp[i] = Math.Sin(_2pi * ((i * _dt * _f0) + (_deltF * data[i])));
            
//            Channel retCh = new Channel(outp, _fd, signal.Name, signal.Description)
//            {
//                ScaleVolt = signal.ScaleVolt
//            };

//            return retCh;
//        }
//    }
//}
