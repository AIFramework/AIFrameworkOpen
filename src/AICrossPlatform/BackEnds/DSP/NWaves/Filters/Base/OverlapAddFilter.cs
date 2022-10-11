using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// The base class for all filters working by the STFT overlap-add scheme:
    /// 
    /// - short-time frame analysis
    /// - short-time frame processing
    /// - short-time frame synthesis (overlap-add)
    /// 
    /// Subclasses must implement ProcessSpectrum() method
    /// that corresponds to the second stage.
    /// 
    /// Also, it implements IMixable interface,
    /// since audio effects can be built based on this class.
    /// 
    /// </summary>
    [Serializable]
    public abstract class OverlapAddFilter : IFilter, IOnlineFilter, IMixable
    {
        /// <summary>
        /// Влажная смесь
        /// </summary>
        public float Wet { get; set; } = 1f;

        /// <summary>
        /// Сухая смесь
        /// </summary>
        public float Dry { get; set; } = 0f;

        /// <summary>
        /// Hop size
        /// </summary>
        protected readonly int _hopSize;

        /// <summary>
        /// Размер блока БПФ для анализа и синтеза
        /// </summary>
        protected readonly int _fftSize;

        /// <summary>
        /// Коэффициент нормализации ISTFT
        /// </summary>
        protected float _gain;

        /// <summary>
        /// Размер перекрытия (в отсчетах)
        /// </summary>
        protected readonly int _overlapSize;

        /// <summary>
        /// Внутренний алгоритм для выполнения БПФ
        /// </summary>
        protected readonly RealFft _fft;

        /// <summary>
        /// Весовое окно
        /// </summary>
        protected readonly float[] _window;

        /// <summary>
        /// Линия задержки
        /// </summary>
        private readonly float[] _dl;

        /// <summary>
        /// Смещение входа в линии задержки
        /// </summary>
        private int _inOffset;

        /// <summary>
        /// Смещение в выходном буфере
        /// </summary>
        private int _outOffset;

        /// <summary>
        /// Внутренние буферы
        /// </summary>
        private readonly float[] _re;
        private readonly float[] _im;
        private readonly float[] _filteredRe;
        private readonly float[] _filteredIm;
        private readonly float[] _lastSaved;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="hopSize"></param>
        /// <param name="fftSize"></param>
        public OverlapAddFilter(int hopSize, int fftSize = 0)
        {
            _hopSize = hopSize;
            _fftSize = (fftSize > 0) ? fftSize : 8 * hopSize;
            _overlapSize = _fftSize - _hopSize;

            Guard.AgainstInvalidRange(_hopSize, _fftSize, "Hop size", "FFT size");

            _fft = new RealFft(_fftSize);

            _window = Window.OfType(WindowTypes.Hann, _fftSize);

            _gain = 1 / (_fftSize * _window.Select(w => w * w).Sum() / _hopSize);

            _dl = new float[_fftSize];
            _re = new float[_fftSize];
            _im = new float[_fftSize];
            _filteredRe = new float[_fftSize];
            _filteredIm = new float[_fftSize];
            _lastSaved = new float[_overlapSize];

            _inOffset = _overlapSize;
        }

        /// <summary>
        /// Онлайн-обработка
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public virtual float Process(float sample)
        {
            _dl[_inOffset++] = sample;

            if (_inOffset == _fftSize)
            {
                ProcessFrame();
            }

            return _filteredRe[_outOffset++];
        }

        /// <summary>
        /// Обработка одного БПФ блока
        /// </summary>
        public virtual void ProcessFrame()
        {
            // analysis =========================================================

            _dl.FastCopyTo(_re, _fftSize);
            _re.ApplyWindow(_window);
            _fft.Direct(_re, _re, _im);

            // processing =======================================================

            ProcessSpectrum(_re, _im, _filteredRe, _filteredIm);

            // synthesis ========================================================

            _fft.Inverse(_filteredRe, _filteredIm, _filteredRe);
            _filteredRe.ApplyWindow(_window);

            for (int j = 0; j < _overlapSize; j++)
            {
                _filteredRe[j] += _lastSaved[j];
            }

            _filteredRe.FastCopyTo(_lastSaved, _overlapSize, _hopSize);

            for (int i = 0; i < _filteredRe.Length; i++)  // Wet/Сухая смесь
            {
                _filteredRe[i] *= Wet * _gain;
                _filteredRe[i] += _dl[i] * Dry;
            }

            _dl.FastCopyTo(_dl, _overlapSize, _hopSize);

            _inOffset = _overlapSize;
            _outOffset = 0;
        }

        /// <summary>
        /// Обработка одного спектра на каждом шаге STFT
        /// </summary>
        /// <param name="re">Реальная часть спектра</param>
        /// <param name="im">Мнимая часть входного спектра</param>
        /// <param name="filteredRe">Реальная часть выходного спектра</param>
        /// <param name="filteredIm">Мнимая часть выходного спектра</param>
        public abstract void ProcessSpectrum(float[] re, float[] im,
                                             float[] filteredRe, float[] filteredIm);

        /// <summary>
        /// Перезапуск фильтра 
        /// </summary>
        public virtual void Reset()
        {
            _inOffset = _overlapSize;
            _outOffset = 0;

            Array.Clear(_dl, 0, _dl.Length);
            Array.Clear(_re, 0, _re.Length);
            Array.Clear(_im, 0, _im.Length);
            Array.Clear(_filteredRe, 0, _filteredRe.Length);
            Array.Clear(_filteredIm, 0, _filteredIm.Length);
            Array.Clear(_lastSaved, 0, _lastSaved.Length);
        }

        /// <summary>
        /// Оффлайн обработка
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyTo(DiscreteSignal signal,
                                      FilteringMethod method = FilteringMethod.Auto)
        {
            return new DiscreteSignal(signal.SamplingRate, signal.Samples.Select(s => Process(s)));
        }
    }
}
