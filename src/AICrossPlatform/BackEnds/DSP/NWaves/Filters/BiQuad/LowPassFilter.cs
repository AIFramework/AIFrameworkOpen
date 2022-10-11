﻿using System;

namespace AI.BackEnds.DSP.NWaves.Filters.BiQuad
{
    /// <summary>
    /// Биквадратный фильтр нижних частот
    /// The coef "A"re calculated automatically according to 
    /// audio-eq-cookbook by R.Bristow-Johnson and WebAudio API.
    /// </summary>
    [Serializable]
    public class LowPassFilter : BiQuadFilter
    {
        /// <summary>
        /// Частота
        /// </summary>
        public double Freq { get; protected set; }

        /// <summary>
        /// Добротность
        /// </summary>
        public double Q { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        public LowPassFilter(double freq, double q = 1)
        {
            SetCoefficients(freq, q);
        }

        /// <summary>
        /// Установить коэффициенты фильтра
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        private void SetCoefficients(double freq, double q)
        {
            Freq = freq;
            Q = q;

            double omega = 2 * Math.PI * freq;
            double alpha = Math.Sin(omega) / (2 * q);
            double cosw = Math.Cos(omega);

            _b[0] = (float)((1 - cosw) / 2);
            _b[1] = (float)(1 - cosw);
            _b[2] = _b[0];

            _a[0] = (float)(1 + alpha);
            _a[1] = (float)(-2 * cosw);
            _a[2] = (float)(1 - alpha);

            Normalize();
        }

        /// <summary>
        /// Изменить параметры фильтра (с сохранением его состояния)
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="q"></param>
        public void Change(double freq, double q = 1)
        {
            SetCoefficients(freq, q);
        }
    }
}
