using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base64
{
    /// <summary>
    /// КИХ фильтр
    /// </summary>
    [Serializable]
    public class FirFilter64 : IFilter64, IOnlineFilter64
    {
        /// <summary>
        /// Ядро фильтра (импульсная характеристика)
        /// </summary>
        public double[] Kernel => _b.Take(_kernelSize).ToArray();

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
        protected readonly double[] _b;

        /// <summary>
        /// Размер ядра
        /// </summary>
        protected int _kernelSize;

        /// <summary>
        /// Передаточная функция (создается лениво или устанавливается специально, если нужно)
        /// </summary>
        protected TransferFunction _tf;
        /// <summary>
        /// Передаточная функция
        /// </summary>
        public TransferFunction Tf
        {
            get => _tf ?? new TransferFunction(_b.Take(_kernelSize).ToArray());
            protected set => _tf = value;
        }

        /// <summary>
        /// Если _kernelSize превышает это значение, код фильтрации всегда будет вызывать процедуру Overlap-Save.
        /// </summary>
        public int KernelSizeForBlockConvolution { get; set; } = 64;

        /// <summary>
        /// Внутренний буфер для линии задержки
        /// </summary>
        protected double[] _delayLine;

        /// <summary>
        /// Текущее смещение в линии задержки
        /// </summary>
        protected int _delayLineOffset;

        /// <summary>
        /// Конструктор, прием 64-битного ядра фильтра
        /// </summary>
        /// <param name="kernel"></param>
        public FirFilter64(IEnumerable<double> kernel)
        {
            _kernelSize = kernel.Count();

            _b = new double[_kernelSize * 2];

            for (int i = 0; i < _kernelSize; i++)
            {
                _b[i] = _b[_kernelSize + i] = kernel.ElementAt(i);
            }

            _delayLine = new double[_kernelSize];
            _delayLineOffset = _kernelSize - 1;
        }

        /// <summary>
        /// Конструктор, принимающий передаточную функцию
        /// </summary>
        /// <param name="tf">Передаточная функция</param>
        public FirFilter64(TransferFunction tf) : this(tf.Numerator)
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
            if (_kernelSize >= KernelSizeForBlockConvolution && method == FilteringMethod.Auto)
            {
                method = FilteringMethod.OverlapSave;
            }

            switch (method)
            {
                case FilteringMethod.OverlapAdd:
                    {
                        int fftSize = MathUtilsDSP.NextPowerOfTwo(4 * _kernelSize);
                        OlaBlockConvolver64 blockConvolver = OlaBlockConvolver64.FromFilter(this, fftSize);
                        return blockConvolver.ApplyTo(signal);
                    }
                case FilteringMethod.OverlapSave:
                    {
                        int fftSize = MathUtilsDSP.NextPowerOfTwo(4 * _kernelSize);
                        OlsBlockConvolver64 blockConvolver = OlsBlockConvolver64.FromFilter(this, fftSize);
                        return blockConvolver.ApplyTo(signal);
                    }
                default:
                    {
                        return ProcessAllSamples(signal);
                    }
            }
        }

        /// <summary>
        /// КИХ-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public double Process(double sample)
        {
            _delayLine[_delayLineOffset] = sample;

            double output = 0.0;

            for (int i = 0, j = _kernelSize - _delayLineOffset; i < _kernelSize; i++, j++)
            {
                output += _delayLine[i] * _b[j];
            }

            if (--_delayLineOffset < 0)
            {
                _delayLineOffset = _kernelSize - 1;
            }

            return output;
        }

        /// <summary>
        /// Обрабатывает все отсчеты сигнала в цикле. Код Process() встроен в цикл для повышения производительности (особенно для небольших ядер).
        /// </summary>
        /// <param name="samples">Отсчеты сигнала</param>
        /// <returns></returns>
        public double[] ProcessAllSamples(double[] samples)
        {
            double[] filtered = new double[samples.Length + _kernelSize - 1];

            int k = 0;
            foreach (double sample in samples)
            {
                _delayLine[_delayLineOffset] = sample;

                double output = 0.0;

                for (int i = 0, j = _kernelSize - _delayLineOffset; i < _kernelSize; i++, j++)
                {
                    output += _delayLine[i] * _b[j];
                }

                if (--_delayLineOffset < 0)
                {
                    _delayLineOffset = _kernelSize - 1;
                }

                filtered[k++] = output;
            }

            while (k < filtered.Length)
            {
                filtered[k++] = Process(0);
            }

            return filtered;
        }

        /// <summary>
        /// Изменить ядро фильтра
        /// </summary>
        /// <param name="kernel">Новое ядро</param>
        public void ChangeKernel(double[] kernel)
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
        public void Reset()
        {
            _delayLineOffset = _kernelSize - 1;
            Array.Clear(_delayLine, 0, _kernelSize);
        }
    }
}
