using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.MusicUtils
{
    /// <summary>
    /// Эхо и реверберация
    /// </summary>
    [Serializable]
    public class EchoReverb
    {
        /// <summary>
        /// Частота дискретизации проекта
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Эхо и реверберация
        /// </summary>
        /// <param name="sr">Частота дискретизации проекта</param>
        public EchoReverb(int sr)
        {
            SampleRate = sr;
        }

        /// <summary>
        /// Эхо
        /// </summary>
        public unsafe void Echo(Vector data, double timeDelay = 0.05, double volume = 0.3)
        {
            int steps = (int)(SampleRate * timeDelay);
            int len = data.Count;

            for (int i = 0; i < len; i++)
                if (i >= steps)
                    data[i] += volume * data[i - steps];

        }


        /// <summary>
        /// Эхо
        /// </summary>
        public unsafe void EchoInvers(Vector data, double timeDelay = 0.05, double volume = 0.3)
        {
            int steps = (int)(SampleRate * timeDelay);
            int len = data.Count;

            for (int i = 0; i < len; i++)
                if (i >= steps) data[i - steps] += volume * data[i];
        }
    }
}
