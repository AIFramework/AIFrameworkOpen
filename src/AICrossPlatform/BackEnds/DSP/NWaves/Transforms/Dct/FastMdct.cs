using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]

    public class FastMdct : Mdct
    {
        /// <summary>
        /// 
        /// </summary>
        public FastMdct(int dctSize) : base(dctSize, new FastDct4(dctSize))
        {
        }
    }
}
