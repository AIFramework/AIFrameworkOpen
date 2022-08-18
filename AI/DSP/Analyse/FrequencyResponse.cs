using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.Analyse
{
    /// <summary>
    /// АЧХ
    /// </summary>
    [Serializable]
    public class FrequencyResponse
    {
        private readonly int _fd;
        /// <summary>
        /// Начальная частота
        /// </summary>
        public int FStart { get; set; } = 0;
        /// <summary>
        /// Конечная частота
        /// </summary>
        public int FEnd { get; set; } = 10;
        /// <summary>
        /// Шаг по частоте
        /// </summary>
        public double Step { get; set; } = 1;
        /// <summary>
        /// Время реализации
        /// </summary>
        public double Time { get; set; } = 1;
        /// <summary>
        /// Отсчеты по частоте
        /// </summary>
        public Vector Freq
        {
            get => Vector.Seq(FStart, Step, FEnd);
            private set { }
        }


        /// <summary>
        /// АЧХ
        /// </summary>
        /// <param name="fd">Sampling frequency</param>
        public FrequencyResponse(int fd)
        {
            _fd = fd;
        }



        /// <summary>
        /// Проверка АЧХ алгоритма
        /// </summary>
        /// <param name="alg">Алгоритм</param>
        public Vector Test(Func<Vector, Vector> alg)
        {
            Vector time = Vector.Time0(_fd, Time);
            Vector freq = Freq;
            Vector fResp = new Vector(freq.Count);

            for (int i = 0; i < freq.Count; i++)
            {
                Vector cos = time.Transform(t => Math.Cos(2 * Math.PI * freq[i] * t));
                Vector sin = time.Transform(t => Math.Sin(2 * Math.PI * freq[i] * t));
                double stdSin = sin.Std(), stdCos = cos.Std();

                Vector cosOut = alg(cos);
                Vector sinOut = alg(sin);
                double stdSinOut = sinOut.Std(), stdCosOut = cosOut.Std();
                if (stdCos == 0 || stdSin == 0)
                {
                    fResp[i] = cosOut.Mean() / cos.Mean();
                }
                else
                {
                    fResp[i] = ((stdCosOut / stdCos) + (stdSinOut / stdSin)) / 2.0;
                }
            }

            return fResp.NanToValue();
        }

    }
}
