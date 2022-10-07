using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// LTI filter based on state space representation
    /// </summary>
    [Serializable]
    public class ZiFilter : LtiFilter
    {
        /// <summary>
        /// Numerator part coefficients in filter's Передаточная функция 
        /// (non-recursive part in difference equations)
        /// </summary>
        protected readonly float[] _b;

        /// <summary>
        /// Denominator part coefficients in filter's Передаточная функция 
        /// (recursive part in difference equations).
        /// </summary>
        protected readonly float[] _a;

        /// <summary>
        /// Вектор состояний
        /// </summary>
        protected readonly float[] _zi;
        /// <summary>
        /// Данные линии задержки
        /// </summary>
        public float[] Zi => _zi;

        /// <summary>
        /// Передаточная функция
        /// </summary>
        protected TransferFunction _tf;

        /// <summary>
        /// Передаточная функция
        /// </summary>
        public override TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b.ToDoubles(), _a.ToDoubles());
            protected set => _tf = value;
        }

        /// <summary>
        /// Параметризованный конструктор (from arrays of 32-bit coefficients)
        /// </summary>
        /// <param name="b">Коэф. в числителе передаточной функции</param>
        /// <param name="a">Коэф. в знаминателе передаточной функции</param>
        public ZiFilter(IEnumerable<float> b, IEnumerable<float> a)
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

            _zi = new float[maxLength];
        }

        /// <summary>
        /// Параметризованный конструктор (from arrays of 64-bit coefficients)
        /// 
        /// NOTE.
        /// It will simply cast values to floats!
        /// If you need to preserve precision for filter design  analysis, use constructor with TransferFunction!
        /// 
        /// </summary>
        /// <param name="b">Коэф. в числителе передаточной функции</param>
        /// <param name="a">Коэф. в знаминателе передаточной функции</param>
        public ZiFilter(IEnumerable<double> b, IEnumerable<double> a) : this(b.ToFloats(), a.ToFloats())
        {
        }

        /// <summary>
        /// Параметризованный конструктор (from Передаточная функция).
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public ZiFilter(TransferFunction tf) : this(tf.Numerator, tf.Denominator)
        {
            Tf = tf;
        }

        /// <summary>
        /// Инициализация фильтра
        /// </summary>
        /// <param name="zi"></param>
        public virtual void Init(float[] zi)
        {
            Array.Copy(zi, 0, _zi, 0, Math.Min(zi.Length, _zi.Length));
        }

        /// <summary>
        /// Инициализация фильтра (cast double precision to single precision)
        /// </summary>
        /// <param name="zi"></param>
        public virtual void Init(double[] zi)
        {
            Array.Copy(zi.ToFloats(), 0, _zi, 0, Math.Min(zi.Length, _zi.Length));
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (офлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override DiscreteSignal ApplyTo(DiscreteSignal signal,
                                               FilteringMethod method = FilteringMethod.Auto)
        {
            return new DiscreteSignal(signal.SamplingRate, signal.Samples.Select(s => Process(s)));
        }

        /// <summary>
        /// Фильтрация всего сигнала with initial conditions (for tests)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal FilterIc(DiscreteSignal signal)
        {
            float[] input = signal.Samples;
            float[] output = new float[signal.Length];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (_b[0] * input[i]) + _zi[0];

                for (int j = 1; j < _zi.Length; j++)
                {
                    _zi[j - 1] = (_b[j] * input[i]) - (_a[j] * output[i]) + _zi[j];
                }
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Онлайн-фильтрация с начальными условиями
        /// </summary>
        /// <param name="input">Входной отсчет</param>
        /// <returns>Выходной отсчет</returns>
        public override float Process(float input)
        {
            float output = (_b[0] * input) + _zi[0];

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
        public DiscreteSignal ZeroPhase(DiscreteSignal signal, int padLength = 0)
        {
            if (padLength <= 0)
            {
                padLength = 3 * (Math.Max(_a.Length, _b.Length) - 1);
            }

            Guard.AgainstInvalidRange(padLength, signal.Length, "pad length", "Signal length");

            float[] input = signal.Samples;
            float[] output = new float[signal.Length];
            float[] edgeLeft = new float[padLength];
            float[] edgeRight = new float[padLength];


            // forward filtering: ============================================================

            double[] initialZi = Tf.Zi;
            double[] zi = initialZi.FastCopy();
            float baseSample = (2 * input[0]) - input[padLength];

            for (int i = 0; i < zi.Length; zi[i++] *= baseSample)
            {
                ;
            }

            Init(zi);

            baseSample = input[0];

            for (int k = 0, i = padLength; i > 0; k++, i--)
            {
                edgeLeft[k] = Process((2 * baseSample) - input[i]);
            }

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = Process(input[i]);
            }

            baseSample = input.Last();

            for (int k = 0, i = input.Length - 2; i > input.Length - 2 - padLength; k++, i--)
            {
                edgeRight[k] = Process((2 * baseSample) - input[i]);
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

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            Array.Clear(_zi, 0, _zi.Length);
        }
    }
}
