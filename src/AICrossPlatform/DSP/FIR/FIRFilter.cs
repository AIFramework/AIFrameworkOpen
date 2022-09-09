using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;

namespace AI.DSP.FIR
{
    /// <summary>
    /// Фильтр с конечной импульсной характеристикой
    /// </summary>
    [Serializable]
    public class FIRFilter : IFilter
    {
        private readonly Vector _ht;
        private readonly int transientsInd; // отсчеты переходного процесса
        private readonly FIRCalcConvType convType;
        private readonly int _fd;
        /// <summary>
        /// Имя фильтра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Инициализация фильтра
        /// </summary>
        /// <param name="ht">Impulse response фильтра</param>
        /// <param name="fd">Sampling frequency</param>
        /// <param name="calcConvType">Метод расчета свертки</param>
        public FIRFilter(Vector ht, int fd, FIRCalcConvType calcConvType = FIRCalcConvType.WithFFT)
        {
            _ht = ht;
            convType = calcConvType;
            transientsInd = _ht.Count / 2;
            _fd = fd;
        }


        /// <summary>
        /// Расчет отклика фильтра на сигнал
        /// </summary>
        /// <param name="input">Сигнал</param>
        public Vector FilterOutp(Vector input)
        {
            Vector outp = new Vector(0);

            switch (convType)
            {
                case FIRCalcConvType.Simple:
                    outp = Convolution.DirectConvolution(input, _ht, _fd);
                    break;
                case FIRCalcConvType.WithFFT:
                    outp = FastConv.FastConvolution(input, _ht, _fd);
                    break;
                case FIRCalcConvType.Sectional:
                    outp = FastConv.SectionalConvolution(_ht, input) / _fd;
                    break;
                case FIRCalcConvType.Sectional4:
                    outp = FastConv.SectionalConvolution(_ht, input, 4) / _fd;
                    break;
            }


            return outp.GetInterval(transientsInd * 2, input.Count + transientsInd);
        }
    }

    /// <summary>
    /// Метод расчета свертки
    /// </summary>
    public enum FIRCalcConvType
    {
        /// <summary>
        /// Простая свертка
        /// </summary>
        Simple,
        /// <summary>
        /// Быстрая с исп. БПФ
        /// </summary>
        WithFFT,
        /// <summary>
        /// Секционная
        /// </summary>
        Sectional,
        /// <summary>
        /// Секционная 4 потока
        /// </summary>
        Sectional4

    }
}
