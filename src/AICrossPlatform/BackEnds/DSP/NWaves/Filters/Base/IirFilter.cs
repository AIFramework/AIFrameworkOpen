﻿using AI.BackEnds.DSP.NWaves.Operations;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Class representing Infinite Impulse Response filters
    /// </summary>
    [Serializable]
    public class IirFilter : LtiFilter
    {
        /// <summary>
        /// Numerator part coefficients in filter's transfer function 
        /// (non-recursive part in difference equations)
        /// 
        /// These coefficients have single precision since they are used for filtering!
        /// For filter design analysis specify transfer function (Tf property).
        /// 
        /// Note.
        /// This array is created from duplicated coefficients:
        /// 
        ///  numerator              _b
        /// [1 2 3 4 5] -> [1 2 3 4 5 1 2 3 4 5]
        /// 
        /// Such memory layout leads to speed-up of online filtering.
        /// </summary>
        public readonly float[] _b;

        /// <summary>
        /// Denominator part coefficients in filter's transfer function 
        /// (recursive part in difference equations).
        /// 
        /// These coefficients have single precision since they are used for filtering!
        /// For filter design  analysis specify transfer function (Tf property).
        /// 
        /// Note.
        /// This array is created from duplicated coefficients:
        /// 
        ///  denominator             _a
        ///  [1 2 3 4 5] -> [1 2 3 4 5 1 2 3 4 5]
        /// 
        /// Such memory layout leads to speed-up of online filtering.
        /// </summary>
        public readonly float[] _a;

        /// <summary>
        /// Number of numerator coefficients
        /// </summary>
        protected readonly int _numeratorSize;

        /// <summary>
        /// Number of denominator (feedback) coefficients
        /// </summary>
        protected readonly int _denominatorSize;

        /// <summary>
        /// Transfer function (created lazily or set specifically if needed)
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
        /// Default length of truncated impulse response
        /// </summary>
        public int DefaultImpulseResponseLength { get; set; } = 512;

        /// <summary>
        /// Internal buffers for delay lines
        /// </summary>
        protected float[] _delayLineA;
        /// <summary>
        /// Линия задержки для коэфициентов B
        /// </summary>
        protected float[] _delayLineB;

        /// <summary>
        /// Current offsets in delay lines
        /// </summary>
        protected int _delayLineOffsetA;
        /// <summary>
        /// 
        /// </summary>
        protected int _delayLineOffsetB;

        /// <summary>
        /// Parameterized constructor (from arrays of 32-bit coefficients)
        /// </summary>
        /// <param name="b">TF numerator coefficients</param>
        /// <param name="a">TF denominator coefficients</param>
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
        /// Parameterized constructor (from arrays of 64-bit coefficients)
        /// 
        /// NOTE.
        /// It will simply cast values to floats!
        /// If you need to preserve precision for filter design  analysis, use constructor with TransferFunction!
        /// 
        /// </summary>
        /// <param name="b">TF numerator coefficients</param>
        /// <param name="a">TF denominator coefficients</param>
        public IirFilter(IEnumerable<double> b, IEnumerable<double> a) : this(b.ToFloats(), a.ToFloats())
        {
        }

        /// <summary>
        /// Parameterized constructor (from transfer function).
        /// 
        /// Coefficients (used for filtering) will be cast to floats anyway,
        /// but filter will store the reference to TransferFunction object for FDA.
        /// 
        /// </summary>
        /// <param name="tf">Transfer function</param>
        public IirFilter(TransferFunction tf) : this(tf.Numerator, tf.Denominator)
        {
            Tf = tf;
        }

        /// <summary>
        /// Apply filter to entire signal (offline)
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
                        int fftSize = MathUtils.NextPowerOfTwo(4 * length);
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
        /// IIR online filtering (sample-by-sample)
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
        /// The most straightforward implementation of the difference equation:
        /// code the difference equation as it is
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
        /// Change filter coefficients online (numerator part)
        /// </summary>
        /// <param name="b">New coefficients</param>
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
        /// Change filter coefficients online (denominator / recursive part)
        /// </summary>
        /// <param name="a">New coefficients</param>
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
        /// Reset filter
        /// </summary>
        public override void Reset()
        {
            _delayLineOffsetB = _numeratorSize - 1;
            _delayLineOffsetA = _denominatorSize - 1;

            for (int i = 0; i < _delayLineB.Length; _delayLineB[i++] = 0) { }
            for (int i = 0; i < _delayLineA.Length; _delayLineA[i++] = 0) { }
        }

        /// <summary>
        /// Divide all filter coefficients by _a[0] and normalize TF
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
