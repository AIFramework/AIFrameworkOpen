using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.Analyse;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;

namespace AI.DSP.DSPCore
{
    /// <summary>
	/// Основной класс для одномерного сигнала
	/// </summary>
    [Serializable]
    public class Signal1D : List<Channel>
    {
        /// <summary>
        /// Имя сигнала
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание сигнала
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Частота дискретизации
        /// </summary>
        public int Fd { get; set; }
        private FFT fur;
        private int _n;
        /// <summary>
        /// Шаг по времени
        /// </summary>
        public double Dt => 1.0 / Fd;

        /// <summary>
        /// Масштаб в вольтах
        /// </summary>
        public TypeScaleVolt ScaleVolt
        {
            get => this[0].ScaleVolt;
            set
            {
                for (int i = 0; i < Count; i++)
                    this[i].ScaleVolt = value;
            }
        }




        /// <summary>
        /// Инициализация многоканальным сигналом
        /// </summary>
        /// <param name="channels">Сигнал</param>
        /// <param name="fd">Частота дискретизации</param>
        public Signal1D(Vector[] channels, int fd)
        {
            AddRange(Channel.GetChannels(channels, fd));
            fur = new FFT(channels[0].Count);
            _n = fur.SemplesCount;
            ScaleVolt = this[0].ScaleVolt;
        }
        /// <summary>
        /// Создает пустой список каналов
        /// </summary>
        public Signal1D()
        {
        }
        /// <summary>
        /// Инициализация многоканальным сигналом
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="fd">Частота дискретизации</param>
        public Signal1D(Vector signal, int fd)
        {
            Add(new Channel(signal, fd));
        }
        /// <summary>
        /// Инициализация многоканальным сигналом
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public Signal1D(Channel signal)
        {
            Add(signal);
        }


        /// <summary>
        /// Добавление сигнала
        /// </summary>
        /// <param name="signal">Сигнал</param>
        public new void Add(Channel signal)
        {
            if (Count == 0)
            {
                fur = new FFT(signal.ChData.Count);
                _n = fur.SemplesCount;
                Fd = signal.Fd;
                ScaleVolt = signal.ScaleVolt;
                base.Add(signal);
            }
            else
            {
                if (signal.ScaleVolt != ScaleVolt)
                    _ = signal.ConvertVolt(ScaleVolt);

                base.Add(signal);
            }
        }
        /// <summary>
        /// Тренды сигнала
        /// </summary>
        public Signal1D Trends()
        {
            Vector[] vcs = new Vector[Count];
            Vector time = Time();
            Trend lr;

            for (int i = 0; i < vcs.Length; i++)
            {
                lr = new Trend(time, this[i].ChData);
                vcs[i] = lr.Predict(time);
            }


            return new Signal1D(vcs, Fd);
        }
        /// <summary>
        /// Сигнал без тренда
        /// </summary>
        public Signal1D SignalWithoutTrend()
        {
            Signal1D trends = Trends();
            Vector[] vcs = new Vector[Count];

            for (int i = 0; i < Count; i++)
                vcs[i] = this[i].ChData - trends[i].ChData;

            return new Signal1D(vcs, Fd);
        }
        /// <summary>
        /// Сигнал нулевым мат. ожиданием и средне квадратичным отклонением равным 1 (вычитается тренд)
        /// </summary>
        public Signal1D SignalWithM0Std1Trend()
        {
            Signal1D withOutTrends = SignalWithoutTrend();
            Vector[] vcs = new Vector[Count];

            for (int i = 0; i < Count; i++)
                vcs[i] = withOutTrends[i].ChData / Statistic.CalcStd(withOutTrends[i].ChData);

            return new Signal1D(vcs, Fd);
        }
        /// <summary>
        /// Сигнал нулевым мат. ожиданием и средне квадратичным отклонением равным 1 (вычитается среднее)
        /// </summary>
        public Signal1D SignalWithM0Std1()
        {
            Vector[] vcs = new Vector[Count];

            for (int i = 0; i < Count; i++)
            {
                vcs[i] = this[i].ChData - Statistic.ExpectedValue(this[i].ChData);
                vcs[i] /= Statistic.CalcStd(vcs[i]);
            }

            return new Signal1D(vcs, Fd);
        }
        /// <summary>
        /// Рассчитывает спектр
        /// </summary>
        /// <param name="numCh">Номер канала</param>
        /// <returns>Амплитудный спектр частоты 0 .. fd/2</returns>
        public Vector GetSpectr(int numCh = 0)
        {
            ComplexVector cv = fur.CalcFFT(this[numCh].ChData);
            Vector sp = cv.MagnitudeVector / _n;
            sp *= 2;
            sp = sp.CutAndZero(_n / 2);
            return sp;
        }
        /// <summary>
        /// Рассчитывает спектр по всем каналам
        /// </summary>
        /// <returns>Спектры</returns>
        public Vector[] GetSpectrAll()
        {
            Vector[] vcs = new Vector[Count];

            for (int i = 0; i < Count; i++)
                vcs[i] = GetSpectr(i);

            return vcs;
        }
        /// <summary>
        /// Корреляционная матрица по каналам
        /// </summary>
        /// <returns>Матрица</returns>
        public Matrix CorrelationMatrix()
        {
            return Matrix.GetCorrelationMatrixNorm(Channel.ChansToVects(ToArray()));
        }
        /// <summary>
        /// Корреляционная матрица амплитудных спектров
        /// </summary>
        /// <returns>Матрица</returns>
        public Matrix CorrelationMatrixSpectr()
        {
            return Matrix.GetCorrelationMatrixNorm(GetSpectrAll());
        }
        /// <summary>
        /// Коэффициент связи между каналами рассчитывается как, единица минус определитель корреляционной матрицы
        /// </summary>
        /// <returns>Коэфициент связи [1,0] близко к 1 связь сильная, к 0 слабая</returns>
        public double CouplingCoefficient()
        {
            return 1 - CorrelationMatrix().Determinant;
        }
        /// <summary>
        /// Коэффициент связи между сперктрами каналов рассчитывается как, единица минус определитель корреляционной матрицы амплитудных спектров
        /// </summary>
        /// <returns>Коэфициент связи [1,0] близко к 1 связь сильная, к 0 слабая</returns>
        public double CouplingCoefficientSp()
        {
            return 1 - CorrelationMatrixSpectr().Determinant;
        }
        /// <summary>
        /// Генерация отсчетов времени
        /// </summary>
        /// <returns>Отсчеты времени</returns>
        public Vector Time()
        {
            double endT = this[0].ChData.Count / (double)Fd;
            return FunctionsForEachElements.GenerateTheSequence(0, Dt, endT).CutAndZero(this[0].ChData.Count);
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
        /// Конвертирование шкалы напряжения 
        /// </summary>
        /// <param name="typeScaleVolt">Новый масштаб</param>
        public Signal1D ConvertVolt(TypeScaleVolt typeScaleVolt)
        {
            Signal1D retObj = new Signal1D();
            for (int i = 0; i < Count; i++)
                retObj.Add(this[i].ConvertVolt(typeScaleVolt));

            return retObj;
        }
        /// <summary>
        /// Фильтрация сигнала
        /// </summary>
        /// <param name="filter">Фильтр</param>
        public Signal1D Filtration(IFilter filter)
        {
            Signal1D retObj = new Signal1D();
            for (int i = 0; i < Count; i++)
                retObj.Add(this[i].Filtration(filter));

            return retObj;
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
        /// Список имен каналов
        /// </summary>
        public string[] ChannelNames()
        {
            string[] names = new string[Count];

            for (int i = 0; i < names.Length; i++)
                names[i] = this[i].Name;

            return names;
        }
    }
}
