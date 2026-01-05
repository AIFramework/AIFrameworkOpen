using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.Multiray
{
    /// <summary>
    /// Источник синусоидального сигнала
    /// </summary>
    public class SinSource : Source
    {
        public double F0 = 300;


        public SinSource()
        { }

        public SinSource(double sr, params double[] coords) : base(sr, coords)
        { }

        public override Vector GetSignal(double dist, double speed)
        {
            Vector t = Vector.Time0(SR, T);

            // Затухание по модели 1/r (цилиндрическая волна или дальнее поле сферической волны)
            // Это соответствует формуле расчета r1, r2 в TwoMicro
            double attenuation = 1.0 / dist;
            double phase_shift = 2 * Math.PI * F0 * dist / speed;

            return attenuation * t.Transform(x =>
                Math.Sin(2 * Math.PI * F0 * x - phase_shift)
            );
        }
    }
}
