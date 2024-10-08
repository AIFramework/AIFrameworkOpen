﻿using AI.BackEnds.DSP.NWaves.Filters.Base;
using AI.BackEnds.DSP.NWaves.Filters.Base64;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations.Convolution
{
    /// <summary>
    /// Class responsible for OLA block convolution (double precision).
    /// It can be used as a filter (online filter as well).
    /// </summary>
    [Serializable]
    public class OlaBlockConvolver64 : IFilter64, IOnlineFilter64
    {
        /// <summary>
        /// Ядро фильтра
        /// </summary>
        private readonly double[] _kernel;

        /// <summary>
        /// Размер блока (число отсчетов) для преобразования Фурье
        /// </summary>
        private readonly int _fftSize;

        /// <summary>
        /// Метод вычисления БПФ
        /// </summary>
        private readonly RealFft64 _fft;

        /// <summary>
        /// Смещение входа в линии задержки
        /// </summary>
        private int _bufferOffset;

        /// <summary>
        /// Смещение в линии задержки
        /// </summary>
        private int _outputBufferOffset;

        /// <summary>
        /// Внутренние буферы
        /// </summary>
        private readonly double[] _kernelSpectrumRe;
        private readonly double[] _kernelSpectrumIm;
        private readonly double[] _blockRe;
        private readonly double[] _blockIm;
        private readonly double[] _convRe;
        private readonly double[] _convIm;
        private readonly double[] _lastSaved;

        /// <summary>
        /// Hop size
        /// </summary>
        public int HopSize => _fftSize - _kernel.Length + 1;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="fftSize"></param>
        public OlaBlockConvolver64(IEnumerable<double> kernel, int fftSize)
        {
            _kernel = kernel.ToArray();

            _fftSize = MathUtilsDSP.NextPowerOfTwo(fftSize);

            Guard.AgainstExceedance(_kernel.Length, _fftSize, "Размер ядра", "the Размер блока БПФ");

            _fft = new RealFft64(_fftSize);

            _kernelSpectrumRe = _kernel.PadZeros(_fftSize);
            _kernelSpectrumIm = new double[_fftSize];
            _convRe = new double[_fftSize];
            _convIm = new double[_fftSize];
            _blockRe = new double[_fftSize];
            _blockIm = new double[_fftSize];
            _lastSaved = new double[_kernel.Length - 1];

            _fft.Direct(_kernelSpectrumRe, _kernelSpectrumRe, _kernelSpectrumIm);

            Reset();
        }

        /// <summary>
        /// Construct BlockConvolver from a specific FIR filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fftSize"></param>
        /// <returns></returns>
        public static OlaBlockConvolver64 FromFilter(FirFilter64 filter, int fftSize)
        {
            fftSize = MathUtilsDSP.NextPowerOfTwo(fftSize);
            return new OlaBlockConvolver64(filter.Kernel, fftSize);
        }

        /// <summary>
        /// OLA Онлайн-фильтрация (отсчет за отсчетом)
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public double Process(double sample)
        {
            _blockRe[_bufferOffset++] = sample;

            if (_bufferOffset == HopSize)
            {
                ProcessFrame();
            }

            return _convRe[_outputBufferOffset++];
        }

        /// <summary>
        /// Process one frame (block)
        /// </summary>
        public void ProcessFrame()
        {
            int M = _kernel.Length;

            int halfSize = _fftSize / 2;

            Array.Clear(_blockRe, HopSize, M - 1);

            _fft.Direct(_blockRe, _blockRe, _blockIm);
            for (int j = 0; j <= halfSize; j++)
            {
                _convRe[j] = ((_blockRe[j] * _kernelSpectrumRe[j]) - (_blockIm[j] * _kernelSpectrumIm[j])) / halfSize;
                _convIm[j] = ((_blockRe[j] * _kernelSpectrumIm[j]) + (_blockIm[j] * _kernelSpectrumRe[j])) / halfSize;
            }
            _fft.Inverse(_convRe, _convIm, _convRe);

            for (int j = 0; j < M - 1; j++)
            {
                _convRe[j] += _lastSaved[j];
            }

            _convRe.FastCopyTo(_lastSaved, M - 1, HopSize);

            _outputBufferOffset = 0;
            _bufferOffset = 0;
        }

        /// <summary>
        /// Offline OLA filtering
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public double[] ApplyTo(double[] signal, FilteringMethod method = FilteringMethod.Auto)
        {
            int firstCount = Math.Min(HopSize - 1, signal.Length);

            int i = 0, j = 0;

            for (; i < firstCount; i++)    // first HopSize-1 samples are just placed in the Линия задержки
            {
                _ = Process(signal[i]);
            }

            double[] filtered = new double[signal.Length + _kernel.Length - 1];

            for (; i < signal.Length; i++, j++)    // process
            {
                filtered[j] = Process(signal[i]);
            }

            int lastCount = firstCount + _kernel.Length - 1;

            for (i = 0; i < lastCount; i++, j++)    // get last 'late' samples
            {
                filtered[j] = Process(0.0f);
            }

            return filtered;
        }

        /// <summary>
        /// Перезапуск фильтра internals
        /// </summary>
        public void Reset()
        {
            _bufferOffset = 0;
            _outputBufferOffset = 0;

            Array.Clear(_lastSaved, 0, _lastSaved.Length);
            Array.Clear(_blockRe, 0, _blockRe.Length);
            Array.Clear(_blockIm, 0, _blockIm.Length);
            Array.Clear(_convRe, 0, _convRe.Length);
            Array.Clear(_convIm, 0, _convIm.Length);
        }
    }
}
