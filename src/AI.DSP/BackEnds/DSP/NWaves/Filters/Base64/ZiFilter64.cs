using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base64
{
    /// <summary>
    /// 64-х битный фильтр
    /// </summary>
    [Serializable]
    public class ZiFilter64 : IFilter64, IOnlineFilter64
    {
        /// <summary>
        /// Числитель передаточной функции фильтра
        /// (Не рекусивная часть разностного фильтра)
        /// </summary>
        protected readonly double[] _b;

        /// <summary>
        /// Знаменатель передаточной функции фильтра 
        /// (Рекусивная часть разностного фильтра)
        /// </summary>
        protected readonly double[] _a;

        /// <summary>
        /// Вектор состояний
        /// </summary>
        protected readonly double[] _zi;

        /// <summary>
        /// Вектор состояний линии задержки
        /// </summary>
        public double[] Zi => _zi;

        /// <summary>
        /// Передаточная функция
        /// </summary>
        protected TransferFunction _tf;

        /// <summary>
        /// Передаточная функция
        /// </summary>
        public TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b, _a);
            protected set => _tf = value;
        }

        /// <summary>
        /// Параметризованный конструктор (Массив 64х битных коэффициентов)
        /// </summary>
        /// <param name="b">Коэф. в числителе передаточной функции</param>
        /// <param name="a">Коэф. в знаменателе передаточной функции</param>
        public ZiFilter64(IEnumerable<double> b, IEnumerable<double> a)
        {
            _b = b.ToArray();
            _a = a.ToArray();

            int maxLength = _a.Length;

            if (_a.Length > _b.Length)
            {
                maxLength = _a.Length;
                _b = _b.PadZeros(maxLength);
            }
            else if (_a.Length < _b.Length)
            {
                maxLength = _b.Length;
                _a = _a.PadZeros(maxLength);
            }
            // don't check for equality

            _zi = new double[maxLength];
        }

        /// <summary>
        /// Параметризованный конструктор (Передаточная функция).
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public ZiFilter64(TransferFunction tf) : this(tf.Numerator, tf.Denominator)
        {
            Tf = tf;
        }

        /// <summary>
        /// Инициализация фильтра
        /// </summary>
        /// <param name="zi"></param>
        public virtual void Init(double[] zi)
        {
            Array.Copy(zi, 0, _zi, 0, Math.Min(zi.Length, _zi.Length));
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (оффлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public double[] ApplyTo(double[] signal, FilteringMethod method = FilteringMethod.Auto)
        {
            return signal.Select(s => Process(s)).ToArray();
        }

        /// <summary>
        /// Онлайн-фильтрация с начальными условиями
        /// </summary>
        /// <param name="input">Входной отсчет</param>
        /// <returns>Выходной отсчет</returns>
        public double Process(double input)
        {
            double output = (_b[0] * input) + _zi[0];

            for (int j = 1; j < _zi.Length; j++)
            {
                _zi[j - 1] = (_b[j] * input) - (_a[j] * output) + _zi[j];
            }

            return output;
        }

        /// <summary>
        /// Нуль-фазовая фильтрация (аналог filtfilt() в MATLAB/sciPy)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="padLength"></param>
        /// <returns></returns>
        public double[] ZeroPhase(double[] signal, int padLength = 0)
        {
            if (padLength <= 0)
            {
                padLength = 3 * (Math.Max(_a.Length, _b.Length) - 1);
            }

            Guard.AgainstInvalidRange(padLength, signal.Length, "pad length", "Signal length");

            double[] output = new double[signal.Length];
            double[] edgeLeft = new double[padLength];
            double[] edgeRight = new double[padLength];


            // forward filtering: ============================================================

            double[] initialZi = Tf.Zi;
            double[] zi = initialZi.FastCopy();
            double baseSample = (2 * signal[0]) - signal[padLength];

            for (int i = 0; i < zi.Length; zi[i++] *= baseSample)
            {
                ;
            }

            Init(zi);

            baseSample = signal[0];

            for (int k = 0, i = padLength; i > 0; k++, i--)
            {
                edgeLeft[k] = Process((2 * baseSample) - signal[i]);
            }

            for (int i = 0; i < signal.Length; i++)
            {
                output[i] = Process(signal[i]);
            }

            baseSample = Enumerable.Last<double>(signal);

            for (int k = 0, i = signal.Length - 2; i > signal.Length - 2 - padLength; k++, i--)
            {
                edgeRight[k] = Process((2 * baseSample) - signal[i]);
            }


            // backward filtering: ============================================================

            zi = initialZi;
            baseSample = edgeRight.Last();

            for (int i = 0; i < zi.Length; zi[i++] *= baseSample)
            {
                ;
            }

            Init(zi);

            for (int i = padLength - 1; i >= 0; i--)
            {
                Process(edgeRight[i]);
            }
            for (int i = output.Length - 1; i >= 0; i--)
            {
                output[i] = Process(output[i]);
            }
            for (int i = padLength - 1; i >= 0; i--)
            {
                Process(edgeLeft[i]);
            }

            return output;
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public void Reset()
        {
            Array.Clear(_zi, 0, _zi.Length);
        }
    }
}
