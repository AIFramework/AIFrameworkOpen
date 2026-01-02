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

            // Ослабление (1/r^2)
            double attenuation = 1.0 / (dist * dist + 0.01);  // +0.01 чтобы не было бесконечности

            // Фаза с задержкой распространения
            double propagation_time = dist / speed;

            return attenuation * t.Transform(x =>
                Math.Sin(2 * Math.PI * F0 * (x - propagation_time))
            );
        }
    }
}
