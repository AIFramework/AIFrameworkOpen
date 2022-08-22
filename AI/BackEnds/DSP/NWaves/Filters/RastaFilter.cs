using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// RASTA filter (used for robust speech processing)
    /// </summary>
    [Serializable]

    public class RastaFilter : IirFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RastaFilter(double pole = 0.98) : base(new[] { 0.2f, 0.1f, 0, -0.1f, -0.2f }, new[] { 1, -(float)pole })
        {
        }
    }
}
