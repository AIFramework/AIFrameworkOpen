using AI.BackEnds.DSP.NWaves.Utils;
using System;

namespace AI.BackEnds.DSP.NWaves.Operations.Tsm
{
    /// <summary>
    /// Фазовый вокодер с идентификацией фазы [Puckette].
    /// </summary>
    [Serializable]
    public class PhaseLockingVocoder : PhaseVocoder
    {
        /// <summary>
        /// Массив значений спектра (на текущем шаге)
        /// </summary>
        private readonly double[] _mag;

        /// <summary>
        /// Массив фаз спектра (на текущем шаге)
        /// </summary>
        private readonly double[] _phase;

        /// <summary>
        /// Массив фазовых дельт
        /// </summary>
        private readonly double[] _delta;

        /// <summary>
        /// Массив пиковых позиций (индексов)
        /// </summary>
        private readonly int[] _peaks;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="stretch"></param>
        /// <param name="hopAnalysis"></param>
        /// <param name="fftSize"></param>
        public PhaseLockingVocoder(double stretch, int hopAnalysis, int fftSize = 0)
            : base(stretch, hopAnalysis, fftSize)
        {
            _mag = new double[(_fftSize / 2) + 1];
            _phase = new double[(_fftSize / 2) + 1];
            _delta = new double[(_fftSize / 2) + 1];
            _peaks = new int[_fftSize / 4];
        }

        /// <summary>
        /// Спектр процесса с фазовой синхронизацией на каждом шаге STFT
        /// </summary>
        public override void ProcessSpectrum()
        {
            for (int j = 0; j < _mag.Length; j++)
            {
                _mag[j] = Math.Sqrt((_re[j] * _re[j]) + (_im[j] * _im[j]));
                _phase[j] = Math.Atan2(_im[j], _re[j]);
            }

            // spectral peaks in magnitude spectrum

            int peakCount = 0;

            for (int j = 2; j < _mag.Length - 3; j++)
            {
                if (_mag[j] <= _mag[j - 1] || _mag[j] <= _mag[j - 2] ||
                    _mag[j] <= _mag[j + 1] || _mag[j] <= _mag[j + 2])
                {
                    continue;   // if not a peak
                }

                _peaks[peakCount++] = j;
            }

            _peaks[peakCount++] = _mag.Length - 1;

            // assign phases at peaks to all neighboring frequency bins

            int leftPos = 1;

            for (int j = 0; j < peakCount - 1; j++)
            {
                int peakPos = _peaks[j];
                double peakPhase = _phase[peakPos];

                _delta[peakPos] = peakPhase - _prevPhase[peakPos];

                double deltaUnwrapped = _delta[peakPos] - (_hopAnalysis * _omega[peakPos]);
                double deltaWrapped = MathUtilsDSP.Mod(deltaUnwrapped + Math.PI, 2 * Math.PI) - Math.PI;

                double freq = _omega[peakPos] + (deltaWrapped / _hopAnalysis);

                _phaseTotal[peakPos] = _phaseTotal[peakPos] + (_hopSynthesis * freq);

                int rightPos = (_peaks[j] + _peaks[j + 1]) / 2;

                for (int k = leftPos; k < rightPos; k++)
                {
                    _phaseTotal[k] = _phaseTotal[peakPos] + _phase[k] - _phase[peakPos];

                    _prevPhase[k] = _phase[k];

                    _re[k] = (float)(_mag[k] * Math.Cos(_phaseTotal[k]));
                    _im[k] = (float)(_mag[k] * Math.Sin(_phaseTotal[k]));
                }

                leftPos = rightPos;
            }
        }
    }
}
