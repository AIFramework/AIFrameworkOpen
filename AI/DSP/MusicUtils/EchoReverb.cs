using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.MusicUtils
{
    /// <summary>
    /// Class for creating echo and reverb
    /// </summary>
    [Serializable]
    public class EchoReverb
    {
        /// <summary>
        /// Project sampling rate
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Echo and reverb
        /// </summary>
        /// <param name="sr">Project sampling rate</param>
        public EchoReverb(int sr)
        {
            SampleRate = sr;
        }

        /// <summary>
        /// Echo effect
        /// </summary>
        public unsafe void Echo(Vector data, double timeDelay = 0.05, double volume = 0.3)
        {
            int steps = (int)(SampleRate * timeDelay);
            int len = data.Count;

            for (int i = 0; i < len; i++)
            {
                if (i >= steps)
                {
                    data[i] += volume * data[i - steps];
                }
            }
        }


        /// <summary>
        /// Echo effect
        /// </summary>
        public unsafe void EchoInvers(Vector data, double timeDelay = 0.05, double volume = 0.3)
        {
            int steps = (int)(SampleRate * timeDelay);
            int len = data.Count;

            for (int i = 0; i < len; i++)
            {
                if (i >= steps)
                {
                    data[i - steps] += volume * data[i];
                }
            }
        }
    }
}
