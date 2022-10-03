using AI.BackEnds.DSP.NWaves.Filters.OnePole;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters
{
    /// <summary>
    /// Standard de-emphasis БИХ фильтр
    /// </summary>
    [Serializable]

    public class DeEmphasisFilter : OnePoleFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="a">De-emphasis coefficient</param>
        /// <param name="normalize">Normalize freq response to unit gain</param>
        public DeEmphasisFilter(double a = 0.97, bool normalize = false) : base(1.0, -a)
        {
            if (normalize)
            {
                _b[0] = (float)(1 - a);
            }
        }
    }
}
