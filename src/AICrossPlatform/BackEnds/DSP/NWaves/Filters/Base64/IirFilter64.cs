using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base64
{
    /// <summary>
    /// 64-х битный БИХ фильтр
    /// </summary>
    [Serializable]
    public class IirFilter64 : IFilter64, IOnlineFilter64
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
        public readonly double[] _b;

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
        public readonly double[] _a;

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
        /// Передаточная функция
        /// </summary>
        public TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b.Take(_numeratorSize).ToArray(), _a.Take(_denominatorSize).ToArray());
            protected set => _tf = value;
        }

        /// <summary>
        /// Импульсная характеристика по умолчанию
        /// </summary>
        public int DefaultImpulseResponseLength { get; set; } = 512;

        /// <summary>
        /// Буффер линии задержки коэф. А
        /// </summary>
        protected double[] _delayLineA;
        /// <summary>
        /// Буффер линии задержки коэф. B
        /// </summary>
        protected double[] _delayLineB;

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
        public IirFilter64(IEnumerable<double> b, IEnumerable<double> a)
        {
            _numeratorSize = b.Count();
            _denominatorSize = a.Count();

            _b = new double[_numeratorSize * 2];

            for (int i = 0; i < _numeratorSize; i++)
            {
                _b[i] = _b[_numeratorSize + i] = b.ElementAt(i);
            }

            _a = new double[_denominatorSize * 2];

            for (int i = 0; i < _denominatorSize; i++)
            {
                _a[i] = _a[_denominatorSize + i] = a.ElementAt(i);
            }

            _delayLineB = new double[_numeratorSize];
            _delayLineA = new double[_denominatorSize];
            _delayLineOffsetB = _numeratorSize - 1;
            _delayLineOffsetA = _denominatorSize - 1;
        }

        /// <summary>
        /// Параметризованный конструктор (Передаточная функция).
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public IirFilter64(TransferFunction tf) : this(tf.Numerator, tf.Denominator)
        {
            Tf = tf;
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (оффлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public double[] ApplyTo(double[] signal, FilteringMethod method = FilteringMethod.Auto)
        {
            switch (method)
            {
                case FilteringMethod.OverlapAdd:       // are you sure you wanna do this? It's БИХ фильтр!
                case FilteringMethod.OverlapSave:
                    {
                        int length = Math.Max(DefaultImpulseResponseLength, _denominatorSize + _numeratorSize);
                        int fftSize = MathUtils.NextPowerOfTwo(4 * length);
                        double[] ir = Tf.ImpulseResponse(length);
                        return new OlsBlockConvolver64(ir, fftSize).ApplyTo(signal);
                    }
                default:
                    {
                        return signal.Select(s => Process(s)).ToArray();
                    }
            }
        }

        /// <summary>
        /// БИХ-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public virtual double Process(double sample)
        {
            double output = 0.0;

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
        /// Изменить коэффициенты фильтра онлайн (числитель)
        /// </summary>
        /// <param name="b">Новые коэффициенты</param>
        public void ChangeNumeratorCoeffs(double[] b)
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
        public void ChangeDenominatorCoeffs(double[] a)
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
        public virtual void Reset()
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
            double a0 = _a[0];

            if (Math.Abs(a0 - 1) < 1e-10)
            {
                return;
            }

            if (Math.Abs(a0) < 1e-30)
            {
                throw new ArgumentException("The coefficient a[0] can not be zero!");
            }

            for (int i = 0; i < _a.Length; _a[i++] /= a0) { }
            for (int i = 0; i < _b.Length; _b[i++] /= a0) { }

            _tf?.Normalize();
        }
    }
}
