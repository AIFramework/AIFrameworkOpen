using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.Analyse;
using AI.DSP.DSPCore;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;

namespace AI.DSP
{
    /// <summary>
    /// Канал
    /// </summary>
    [Serializable]
    public class Channel
    {
        /// <summary>
        /// Отсчеты сигнала
        /// </summary>
        public Vector ChData { get; set; }
        /// <summary>
        /// Частота дискретизации
        /// </summary>
        public int Fd { get; set; }
        /// <summary>
        /// Шаг по времени
        /// </summary>
        public double Dt => 1.0 / Fd;
        /// <summary>
        /// Имя канала
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Доступ по индексам к данным канала
        /// </summary>
        /// <param name="i">Индекс</param>
        public double this[int i]
        {
            get => ChData[i];

            set => ChData[i] = value;
        }
        /// <summary>
        /// Масштаб по напряжению
        /// </summary>
        public TypeScaleVolt ScaleVolt { get; set; }
        private readonly FFT fur;
        private readonly int _n;

        /// <summary>
        /// Канал
        /// </summary>
        public Channel()
        {
            ChData = new Vector();
            Fd = 1;
            Name = "Ch";
            Description = "";
            ScaleVolt = TypeScaleVolt.V;
        }
        /// <summary>
        /// Канал, задается через вектор данных и частоту дискретизации
        /// </summary>
        /// <param name="vectorData">Вектор</param>
        /// <param name="fd">Частота</param>
        public Channel(Vector vectorData, int fd)
        {
            ChData = vectorData;
            Fd = fd;
            Name = "Ch";
            Description = "";
            fur = new FFT(vectorData.Count);
            _n = fur.SemplesCount;
            ScaleVolt = TypeScaleVolt.V;
        }
        /// <summary>
        /// Канал, задается через вектор данных, частоту дискретизации и имя
        /// </summary>
        /// <param name="vectorData">Вектор</param>
        /// <param name="fd">Частота</param>
        /// <param name="name">Имя</param>
        public Channel(Vector vectorData, int fd, string name)
        {
            ChData = vectorData;
            Fd = fd;
            Name = name;
            Description = "";
            fur = new FFT(vectorData.Count);
            _n = fur.SemplesCount;
            ScaleVolt = TypeScaleVolt.V;
        }
        /// <summary>
        /// Канал, задается через вектор данных, частоту дискретизации, имя и  описание
        /// </summary>
        /// <param name="vectorData">Вектор</param>
        /// <param name="fd">Частота</param>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        public Channel(Vector vectorData, int fd, string name, string description)
        {
            ChData = vectorData;
            Fd = fd;
            Name = name;
            Description = description;
            fur = new FFT(vectorData.Count);
            _n = fur.SemplesCount;
            ScaleVolt = TypeScaleVolt.V;
        }
        /// <summary>
        /// Преобразует масив векторов в массив каналов
        /// </summary>
        /// <param name="vects">Массив векторов</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <returns></returns>
        public static Channel[] GetChannels(Vector[] vects, int fd)
        {
            Channel[] retObj = new Channel[vects.Length];

            for (int i = 0; i < vects.Length; i++)
            {
                retObj[i] = new Channel(vects[i], fd, "Ch: " + i);
            }

            return retObj;
        }
        /// <summary>
        /// Массив каналов в массив векторов
        /// </summary>
        /// <param name="channels">Массив каналов</param>
        public static Vector[] ChansToVects(Channel[] channels)
        {
            Vector[] vs = new Vector[channels.Length];

            for (int i = 0; i < channels.Length; i++)
            {
                vs[i] = channels[i].ChData;
            }

            return vs;
        }
        /// <summary>
        /// Тренды сигнала
        /// </summary>
        public Channel SignalTrend()
        {
            Vector vcs;
            Vector time = Time();
            Trend lr;

            lr = new Trend(time, ChData);
            vcs = lr.Predict(time);


            return new Channel(vcs, Fd);
        }
        /// <summary>
        /// Сигнал без тренда
        /// </summary>
        public Channel SignalWithoutTrend()
        {
            Channel trends = SignalTrend();
            Vector vcs;

            vcs = ChData - trends.ChData;


            return new Channel(vcs, Fd);
        }
        /// <summary>
        /// Сигнал нулевым мат. ожиданием и средне квадратичным отклонением равным 1 (вычитается тренд)
        /// </summary>
        public Channel SignalWithM0Std1Trend()
        {
            Channel withOutTrends = SignalWithoutTrend();
            Vector vcs;

            vcs = withOutTrends.ChData / Statistic.CalcStd(withOutTrends.ChData);


            return new Channel(vcs, Fd);
        }
        /// <summary>
        /// Сигнал нулевым мат. ожиданием и средне квадратичным отклонением равным 1 (вычитается среднее)
        /// </summary>
        public Channel SignalWithM0Std1()
        {
            Vector vcs;

            vcs = ChData - Statistic.ExpectedValue(ChData);
            vcs /= Statistic.CalcStd(vcs);
            return new Channel(vcs, Fd);
        }
        /// <summary>
        /// Рассчитывает спектр
        /// </summary>
        /// <returns>Амплитудный спектр частоты 0 .. fd/2</returns>
        public Vector GetSpectr()
        {
            ComplexVector cv = fur.CalcFFT(ChData);
            Vector sp = cv.MagnitudeVector / _n;
            sp *= 2;
            sp = sp.CutAndZero(_n / 2);
            return sp;
        }
        /// <summary>
        /// Рассчитывает спектр с использованием оконных функций
        /// </summary>
        /// <returns>Амплитудный спектр частоты 0 .. fd/2</returns>
        public Vector GetSpectr(Func<int, Vector> windowWFunc)
        {
            ComplexVector cv = fur.CalcFFT(ChData * windowWFunc(ChData.Count));
            Vector sp = cv.MagnitudeVector / _n;
            sp *= 2;
            sp = sp.CutAndZero(_n / 2);
            return sp;
        }
        /// <summary>
        /// Генерация отсчетов времени
        /// </summary>
        /// <returns>Отсчеты времени</returns>
        public Vector Time()
        {
            double endT = ChData.Count / Fd;
            return FunctionsForEachElements.GenerateTheSequence(0, Dt, endT).CutAndZero(ChData.Count);
        }
        /// <summary>
        /// Генерация отсчетов частоты
        /// </summary>
        /// <returns>Отсчеты частоты</returns>
        public Vector Freq()
        {
            return Signal.Frequency(_n, Fd).CutAndZero(_n / 2);
        }
        /// <summary>
        /// Коэффициент для перевода в вольты
        /// </summary>
        /// <param name="scaleVolt">Масштаб</param>
        /// <returns></returns>
        public double KoefScaleToVolt(TypeScaleVolt scaleVolt)
        {
            switch (scaleVolt)
            {
                case TypeScaleVolt.kV:
                    return 1e+3;
                case TypeScaleVolt.V:
                    return 1;
                case TypeScaleVolt.mV:
                    return 1e-3;
                case TypeScaleVolt.uV:
                    return 1e-6;
                case TypeScaleVolt.nV:
                    return 1e-9;
            }
            return 0;
        }
        /// <summary>
        /// Единица измерения шкалы Y
        /// </summary>
        public string YName()
        {
            switch (ScaleVolt)
            {
                case TypeScaleVolt.kV:
                    return "кВ";
                case TypeScaleVolt.V:
                    return "В";
                case TypeScaleVolt.mV:
                    return "мВ";
                case TypeScaleVolt.uV:
                    return "мкВ";
                case TypeScaleVolt.nV:
                    return "нВ";
            }
            return "";
        }
        /// <summary>
        /// Конвертирование масштаба по Y
        /// </summary>
        /// <param name="newScale">Новый масштаб</param>
        public Channel ConvertVolt(TypeScaleVolt newScale)
        {
            double k = KoefScaleToVolt(ScaleVolt) / KoefScaleToVolt(newScale);
            Channel channel = new Channel(ChData * k, Fd, Name, Description)
            {
                ScaleVolt = newScale
            };
            return channel;
        }
        /// <summary>
        /// Фильтрация канала
        /// </summary>
        /// <param name="filter">Фильтр</param>
        public Channel Filtration(IFilter filter)
        {
            Channel channel = new Channel(filter.FilterOutp(ChData), Fd, Name, Description);
            return channel;
        }
    }
}
