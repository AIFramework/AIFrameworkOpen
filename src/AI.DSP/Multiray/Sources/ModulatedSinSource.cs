using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.DSP.Multiray.Sources
{
    /// <summary>
    /// Источник синусоидального сигнала с линейной модуляцией амплитуды: sin(2πft) * t
    /// Это всё ещё узкополосный сигнал, но с изменяющейся во времени амплитудой
    /// </summary>
    public class ModulatedSinSource : Source
    {
        public double F0 = 300;

        public ModulatedSinSource()
        { }

        public ModulatedSinSource(double sr, params double[] coords) : base(sr, coords)
        { }

        public override Vector GetSignal(double dist, double speed, IEnumerable<Source> sources = null)
        {
            Vector t = Vector.Time0(SR, T);

            // Затухание по модели 1/r
            double attenuation = 1.0 / dist;
            double phase_shift = 2 * Math.PI * F0 * dist / speed;

            // Модулированный синус: sin(ωt - φ) * t
            // Амплитуда растёт линейно со временем
            return attenuation * t.Transform(x =>
                Math.Sin(2 * Math.PI * F0 * x - phase_shift) * x
            );
        }
    }
}

