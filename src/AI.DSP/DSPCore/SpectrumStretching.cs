﻿using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Растяжение и сжатие спектра
    /// </summary>
    [Serializable]
    public class SpectrumStretching
    {
        /// <summary>
        /// Растяжение спектра
        /// </summary>
        /// <param name="signal">Входной сигнал</param>
        /// <param name="k">Коэффициент растяжения</param>
        /// <param name="lFilt"></param>
        public static Vector SpectrumStretch(Vector signal, double k, int lFilt = 15)
        {
            Vector s2 = Filters.MAv(signal, lFilt);

            int len = (int)(s2.Count / k);

            Vector ret = new Vector(len);

            for (int i = 0; i < len; i++)
                ret[i] = s2[(int)(k * i)];


            return ret;
        }
    }
}
