using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// КИХ фильтр
    /// </summary>
    [Serializable]
    public class FirFilter : LtiFilter
    {
        /// <summary>
        /// Ядро фильтра (импульсная характеристика)
        /// </summary>
        public float[] Kernel => _b.Take(_kernelSize).ToArray();

        /// <summary>
        /// 
        /// Числитель передаточной функции нерекурсивного фильтра
        /// Этот массив создан из дублированного ядра фильтра:
        /// 
        ///   Ядро                _b
        /// [1 2 3 4 5] -> [1 2 3 4 5 1 2 3 4 5]
        /// 
        /// Такое расположение памяти приводит к значительному ускорению онлайн-фильтраци
        /// </summary>
        protected readonly float[] _b;

        /// <summary>
        /// Размер ядра
        /// </summary>
        protected int _kernelSize;

        /// <summary>
        /// Передаточная функция
        /// </summary>
        protected TransferFunction _tf;

        /// <summary>
        /// Передаточная функция КИХ-фильтра
        /// </summary>
        public override TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b.Take(_kernelSize).ToDoubles());
            protected set => _tf = value;
        }

        /// <summary>
        /// Если _kernelSize превышает это значение, код фильтрации всегда будет вызывать процедуру Overlap-Save.
        /// </summary>
        public int KernelSizeForBlockConvolution { get; set; } = 64;

        /// <summary>
        /// Внутренний буфер для линии задержки
        /// </summary>
        protected float[] _delayLine;

        /// <summary>
        /// Текущее смещение в линии задержки
        /// </summary>
        protected int _delayLineOffset;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="kernel"></param>
        public FirFilter(IEnumerable<float> kernel)
        {
            _kernelSize = kernel.Count();

            _b = new float[_kernelSize * 2];

            for (int i = 0; i < _kernelSize; i++)
            {
                _b[i] = _b[_kernelSize + i] = kernel.ElementAt(i);
            }

            _delayLine = new float[_kernelSize];
            _delayLineOffset = _kernelSize - 1;
        }

        /// <summary>
        /// Создание фильтра при помощи установки ядра.
        /// </summary>
        /// <param name="kernel"></param>
        public FirFilter(IEnumerable<double> kernel) : this(kernel.ToFloats())
        {
        }

        /// <summary>
        /// Конструктор, принимающий передаточную функцию
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public FirFilter(TransferFunction tf) : this(tf.Numerator.ToFloats())
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
            if (_kernelSize >= KernelSizeForBlockConvolution && method == FilteringMethod.Auto)
            {
                method = FilteringMethod.OverlapSave;
            }

            switch (method)
            {
                case FilteringMethod.OverlapAdd:
                    {
                        int fftSize = MathUtils.NextPowerOfTwo(4 * _kernelSize);
                        OlaBlockConvolver blockConvolver = OlaBlockConvolver.FromFilter(this, fftSize);
                        return blockConvolver.ApplyTo(signal);
                    }
                case FilteringMethod.OverlapSave:
                    {
                        int fftSize = MathUtils.NextPowerOfTwo(4 * _kernelSize);
                        OlsBlockConvolver blockConvolver = OlsBlockConvolver.FromFilter(this, fftSize);
                        return blockConvolver.ApplyTo(signal);
                    }
                case FilteringMethod.DifferenceEquation:
                    {
                        return ApplyFilterDirectly(signal);
                    }
                default:
                    {
                        return new DiscreteSignal(signal.SamplingRate, ProcessAllSamples(signal.Samples));
                    }
            }
        }

        /// <summary>
        /// КИХ-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override float Process(float sample)
        {
            _delayLine[_delayLineOffset] = sample;

            float output = 0f;

            for (int i = 0, j = _kernelSize - _delayLineOffset; i < _kernelSize; i++, j++)  output += _delayLine[i] * _b[j];

            if (--_delayLineOffset < 0) _delayLineOffset = _kernelSize - 1;

            return output;
        }

        /// <summary>
        /// Обрабатывает все отсчеты сигнала в цикле. Код Process() встроен в цикл для повышения производительности (особенно для небольших ядер).
        /// </summary>
        /// <param name="samples">Отсчеты сигнала</param>
        /// <returns></returns>
        public float[] ProcessAllSamples(float[] samples)
        {
            float[] filtered = new float[samples.Length + _kernelSize - 1];

            int k = 0;
            foreach (float sample in samples)
            {
                _delayLine[_delayLineOffset] = sample;

                float output = 0f;

                for (int i = 0, j = _kernelSize - _delayLineOffset; i < _kernelSize; i++, j++)
                    output += _delayLine[i] * _b[j];

                if (--_delayLineOffset < 0)
                    _delayLineOffset = _kernelSize - 1;

                filtered[k++] = output;
            }

            while (k < filtered.Length)
                filtered[k++] = Process(0);

            return filtered;
        }

        /// <summary>
        /// Применить фильтр ко всему сигналу (оффлайн)
        /// </summary>
        /// <param name="signal"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyFilterDirectly(DiscreteSignal signal)
        {
            float[] input = signal.Samples;

            float[] output = new float[input.Length + _kernelSize - 1];

            for (int n = 0; n < output.Length; n++)
                for (int k = 0; k < _kernelSize; k++)
                    if (n >= k && n < input.Length + k)
                        output[n] += _b[k] * input[n - k];

            return new DiscreteSignal(signal.SamplingRate, output);
        }

        /// <summary>
        /// Изменить ядро фильтра
        /// </summary>
        /// <param name="kernel">Новое ядро</param>
        public void ChangeKernel(float[] kernel)
        {
            if (kernel.Length == _kernelSize)
            {
                for (int i = 0; i < _kernelSize; i++)
                {
                    _b[i] = _b[_kernelSize + i] = kernel[i];
                }
            }
        }

        /// <summary>
        /// Перезапуск фильтра
        /// </summary>
        public override void Reset()
        {
            _delayLineOffset = _kernelSize - 1;
            Array.Clear(_delayLine, 0, _kernelSize);
        }


        /// <summary>
        /// Sequential combination of two FIR filters (also an FIR filter)
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static FirFilter operator *(FirFilter filter1, FirFilter filter2)
        {
            TransferFunction tf = filter1.Tf * filter2.Tf;

            return new FirFilter(tf.Numerator);
        }

        /// <summary>
        /// Sequential combination of an FIR and БИХ фильтр
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static IirFilter operator *(FirFilter filter1, IirFilter filter2)
        {
            TransferFunction tf = filter1.Tf * filter2.Tf;

            return new IirFilter(tf.Numerator, tf.Denominator);
        }

        /// <summary>
        /// Parallel combination of two FIR filters
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static FirFilter operator +(FirFilter filter1, FirFilter filter2)
        {
            TransferFunction tf = filter1.Tf + filter2.Tf;

            return new FirFilter(tf.Numerator);
        }

        /// <summary>
        /// Parallel combination of an FIR and БИХ фильтр
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static IirFilter operator +(FirFilter filter1, IirFilter filter2)
        {
            TransferFunction tf = filter1.Tf + filter2.Tf;

            return new IirFilter(tf.Numerator, tf.Denominator);
        }
    }
}
