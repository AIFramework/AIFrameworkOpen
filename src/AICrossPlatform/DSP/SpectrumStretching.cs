using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;

namespace AI.DSP
{
    /// <summary>
    /// Spectrum stretching and compression class
    /// </summary>
    [Serializable]
    public class SpectrumStretching
    {
        /// <summary>
        /// Spectrum stretching
        /// </summary>
        /// <param name="signal">Input signal</param>
        /// <param name="k">Stretch ratio</param>
        /// <param name="lFilt"></param>
        public static Vector SpectrumStretch(Vector signal, double k, int lFilt = 15)
        {
            Vector s2 = Filters.MAv(signal, lFilt);

            int len = (int)(s2.Count / k);

            Vector ret = new Vector(len);

            for (int i = 0; i < len; i++)
            {
                ret[i] = s2[(int)(k * i)];
            }


            return ret;
        }
    }
}
