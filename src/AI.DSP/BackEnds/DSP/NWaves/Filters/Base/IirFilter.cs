using AI.BackEnds.DSP.NWaves.Operations;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// 32-х битный БИХ фильтр
    /// </summary>
    [Serializable]
    public class IirFilter : LtiFilter
    {
        /// <summary>
        /// 
        /// Числитель передаточной функции рекурсивного фильтра
        /// Этот массив создан из дублированного ядра фильтра:
        /// 
        ///  числитель                _b
        /// [1 2 3 4 5] -> [1 2 3 4 5 1 2 3 4 5]
        /// 
        /// Такое расположение памяти приводит к значительному ускорению онлайн-фильтраци
        /// </summary>
        public readonly float[] _b;

        /// <summary>
        /// 
        /// Знаменатель передаточной функции рекурсивного фильтра
        /// Этот массив создан из дублированного ядра фильтра:
        /// 
        ///  Знаменатель                _b
        /// [1 2 3 4 5] -> [1 2 3 4 5 1 2 3 4 5]
        /// 
        /// Такое расположение памяти приводит к значительному ускорению онлайн-фильтраци
        /// </summary>
        public readonly float[] _a;

        /// <summary>
        /// Количество коэффициентов числителя
        /// </summary>
        protected readonly int _numeratorSize;

        /// <summary>
        /// Количество коэффициентов знаменателя (обратной связи)
        /// </summary>
        protected readonly int _denominatorSize;

        /// <summary>
        /// Передаточная функция (создается лениво или устанавливается специально, если нужно)
        /// </summary>
        protected TransferFunction _tf;

        /// <summary>
        /// Передаточная функция БИХ фильтра
        /// </summary>
        public override TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b.Take(_numeratorSize).ToDoubles(), _a.Take(_denominatorSize).ToDoubles());
            protected set => _tf = value;
        }

        /// <summary>
        /// Импульсная характеристика по умолчанию
        /// </summary>
        public int DefaultImpulseResponseLength { get; set; } = 512;

        /// <summary>
        /// Внутренние буферы for Линия задержкиs
        /// </summary>
        protected float[] _delayLineA;
        /// <summary>
        /// Линия задержки для коэфициентов B
        /// </summary>
        protected float[] _delayLineB;

        /// <summary>
        /// Смещение в линии задержки (А)
        /// </summary>
        protected int _delayLineOffsetA;
        /// <summary>
        /// Смещение в линии задержки (B)
        /// </summary>
        protected int _delayLineOffsetB;

        /// <summary>
        /// Параметризованный конструктор ( 32-битные коэффициенты)
        /// </summary>
        /// <param name="b">Коэф. в числителе передаточной функции</param>
        /// <param name="a">Коэф. в знаменателе передаточной функции</param>
        public IirFilter(IEnumerable<float> b, IEnumerable<float> a)
        {
            _numeratorSize = b.Count();
            _denominatorSize = a.Count();

            _b = new float[_numeratorSize * 2];

            for (int i = 0; i < _numeratorSize; i++)
            {
                _b[i] = _b[_numeratorSize + i] = b.ElementAt(i);
            }

            _a = new float[_denominatorSize * 2];

            for (int i = 0; i < _denominatorSize; i++)
            {
                _a[i] = _a[_denominatorSize + i] = a.ElementAt(i);
            }

            _delayLineB = new float[_numeratorSize];
            _delayLineA = new float[_denominatorSize];
            _delayLineOffsetB = _numeratorSize - 1;
            _delayLineOffsetA = _denominatorSize - 1;
        }

        /// <summary>
        /// Параметризованный конструктор ( 32-битные коэффициенты)
        /// </summary>
        /// <param name="b">Коэф. в числителе передаточной функции</param>
        /// <param name="a">Коэф. в знамнателе передаточной функции</param>
        public IirFilter(IEnumerable<double> b, IEnumerable<double> a) : this(b.ToFloats(), a.ToFloats())
        {
        }

        /// <summary>
        /// Параметризованный конструктор (Передаточная функция).
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public IirFilter(TransferFunction tf) : this(tf.Numerator, tf.Denominator)
        {
            Tf = tf;
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (оффлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override DiscreteSignal ApplyTo(DiscreteSignal signal,
                                               FilteringMethod method = FilteringMethod.Auto)
        {
            switch (method)
            {
                case FilteringMethod.OverlapAdd:       // are you sure you wanna do this? It's БИХ фильтр!
                case FilteringMethod.OverlapSave:
                    {
                        int length = Math.Max(DefaultImpulseResponseLength, _denominatorSize + _numeratorSize);
                        int fftSize = MathUtilsDSP.NextPowerOfTwo(4 * length);
                        DiscreteSignal ir = new DiscreteSignal(signal.SamplingRate, Tf.ImpulseResponse(length).ToFloats());
                        return Operation.BlockConvolve(signal, ir, fftSize, method);
                    }
                case FilteringMethod.DifferenceEquation:
                    {
                        return ApplyFilterDirectly(signal);
                    }
                default:
                    {
                        return new DiscreteSignal(signal.SamplingRate, signal.Samples.Select(s => Process(s)));
                    }
            }
        }

        /// <summary>
        /// БИХ-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            float output = 0f;

            _delayLineB[_delayLineOffsetB] = sample;
            _delayLineA[_delayLineOffsetA] = 0;

            for (int i = 0, j = _numeratorSize - _delayLineOffsetB; i < _numeratorSize; i++, j++)
            {
                output += _delayLineB[i] * _b[j];
            }

            for (int i = 0, j = _denominatorSize - _delayLineOffsetA; i < _denominatorSize; i++, j++)
            {
                output -= _delayLineA[i] * _a[j];
            }

            _delayLineA[_delayLineOffsetA] = output;

            if (--_delayLineOffsetB < 0)
            {
                _delayLineOffsetB = _numeratorSize - 1;
            }

            if (--_delayLineOffsetA < 0)
            {
                _delayLineOffsetA = _denominatorSize - 1;
            }

            return output;
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (оффлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyFilterDirectly(DiscreteSignal signal)
        {
            float[] input = signal.Samples;

            float[] output = new float[input.Length];

            for (int n = 0; n < input.Length; n++)
            {
                for (int k = 0; k < _numeratorSize; k++)
                {
                    if (n >= k)
                    {
                        output[n] += _b[k] * input[n - k];
                    }
                }
                for (int m = 1; m < _denominatorSize; m++)
                {
                    if (n >= m)
                    {
                        output[n] -= _a[m] * output[n - m];
                    }
                }
            }

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Изменить коэффициенты фильтра онлайн (числитель)
        /// </summary>
        /// <param name="b">Новые коэффициенты</param>
        public void ChangeNumeratorCoeffs(float[] b)
        {
            if (b.Length == _numeratorSize)
            {
                for (int i = 0; i < _numeratorSize; i++)
                {
                    _b[i] = _b[_numeratorSize + i] = b[i];
                }
            }
        }

        /// <summary>
        /// Изменить коэффициенты фильтра онлайн (знаменатель / рекурсивная часть)
        /// </summary>
        /// <param name="a">Новые коэффициенты</param>
        public void ChangeDenominatorCoeffs(float[] a)
        {
            if (a.Length == _denominatorSize)
            {
                for (int i = 0; i < _denominatorSize; i++)
                {
                    _a[i] = _a[_denominatorSize + i] = a[i];
                }
            }
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            _delayLineOffsetB = _numeratorSize - 1;
            _delayLineOffsetA = _denominatorSize - 1;

            for (int i = 0; i < _delayLineB.Length; _delayLineB[i++] = 0) { }
            for (int i = 0; i < _delayLineA.Length; _delayLineA[i++] = 0) { }
        }

        /// <summary>
        /// Нормализует передаточную функцию (делит все коэффициенты на _a[0])
        /// </summary>
        public void Normalize()
        {
            float a0 = _a[0];

            if (Math.Abs(a0 - 1) < 1e-10f)
            {
                return;
            }

            if (Math.Abs(a0) < 1e-30f)
            {
                throw new ArgumentException("The coefficient a[0] can not be zero!");
            }

            for (int i = 0; i < _a.Length; _a[i++] /= a0) { }
            for (int i = 0; i < _b.Length; _b[i++] /= a0) { }

            _tf?.Normalize();
        }

        /// <summary>
        /// Sequential combination of an БИХ фильтр and any LTI filter.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static IirFilter operator *(IirFilter filter1, LtiFilter filter2)
        {
            TransferFunction tf = filter1.Tf * filter2.Tf;

            return new IirFilter(tf.Numerator, tf.Denominator);
        }

        /// <summary>
        /// Parallel combination of an IIR and any LTI filter.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static IirFilter operator +(IirFilter filter1, LtiFilter filter2)
        {
            TransferFunction tf = filter1.Tf + filter2.Tf;

            return new IirFilter(tf.Numerator, tf.Denominator);
        }
    }
}
