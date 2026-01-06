using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;
using System.Collections.Generic;

namespace AI.DSP.Multiray.Sources
{
    /// <summary>
    /// Источник синусоидального сигнала с белым гауссовым шумом
    /// </summary>
    public class NoisySinSource : Source
    {
        public double F0 = 1200;
        /// <summary>
        /// Отношение сигнал/шум (SNR) в разах (не в дБ)
        /// </summary>
        public double SNR = 5.0;

        public NoisySinSource()
        { }

        public NoisySinSource(double sr, params double[] coords) : base(sr, coords)
        { }

        public NoisySinSource(double sr, double snr, params double[] coords) : base(sr, coords)
        {
            SNR = snr;
        }

        public override Vector GetSignal(double dist, double speed, IEnumerable<Source> sources = null)
        {
            Vector t = Vector.Time0(SR, T);

            // Затухание по модели 1/r
            double attenuation = 1.0 / dist;
            double phase_shift = 2 * Math.PI * F0 * dist / speed;

            // Чистый синусоидальный сигнал
            Vector cleanSignal = attenuation * t.Transform(x =>
                Math.Sin(2 * Math.PI * F0 * x - phase_shift)
            );

            // Вычисляем RMS чистого сигнала
            double signalRMS = 0;
            for (int i = 0; i < cleanSignal.Count; i++)
            {
                signalRMS += cleanSignal[i] * cleanSignal[i];
            }
            signalRMS = Math.Sqrt(signalRMS / cleanSignal.Count);

            // Генерируем белый гауссов шум (mean=0, std=1)
            Vector noise = Statistic.RandNorm(cleanSignal.Count);

            // Вычисляем RMS шума
            double noiseRMS = 0;
            for (int i = 0; i < noise.Count; i++)
            {
                noiseRMS += noise[i] * noise[i];
            }
            noiseRMS = Math.Sqrt(noiseRMS / noise.Count);

            // Масштабируем шум для достижения желаемого SNR
            // SNR = RMS_signal / RMS_noise
            // Нужно: RMS_noise_scaled = RMS_signal / SNR
            double noiseScale = signalRMS / (SNR * noiseRMS);
            Vector scaledNoise = noise * noiseScale;

            // Возвращаем сигнал + шум
            return cleanSignal + scaledNoise;
        }
    }
}

