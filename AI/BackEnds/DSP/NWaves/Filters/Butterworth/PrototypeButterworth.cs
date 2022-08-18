using System;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Butterworth
{
    public static class PrototypeButterworth
    {
        public static Complex[] Poles(int order)
        {
            Complex[] poles = new Complex[order];

            for (int k = 0; k < order; k++)
            {
                double theta = Math.PI * ((2 * k) + 1) / (2 * order);

                poles[k] = new Complex(-Math.Sin(theta), Math.Cos(theta));
            }

            return poles;
        }
    }
}
