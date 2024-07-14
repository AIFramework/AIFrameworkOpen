using System;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Butterworth
{
    /// <summary>
    /// Прототип фильтра Баттервордта
    /// </summary>
    public static class PrototypeButterworth
    {
        /// <summary>
        /// Полюса передаточной функции
        /// </summary>
        /// <param name="order">Порядок фильтра</param>
        /// <returns></returns>
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
