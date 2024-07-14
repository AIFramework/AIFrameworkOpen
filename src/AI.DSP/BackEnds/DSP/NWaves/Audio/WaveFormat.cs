using System;

namespace AI.BackEnds.DSP.NWaves.Audio
{
    /// <summary>
    /// Стандартный заголовок WAVE
    /// </summary>
    [Serializable]
    public struct WaveFormat
    {
        /// <summary>
        /// PCM = 1
        /// </summary>
        public short AudioFormat;

        /// <summary>
        /// 1 - моно, 2 - стерео звук
        /// </summary>
        public short ChannelCount;

        /// <summary>
        /// 8000 Hz, 11025 Hz, 16000 Hz, 22050 Hz, 44100 Hz
        /// </summary>
        public int SamplingRate;

        /// <summary>
        /// SamplingRate * NumChannels * BitsPerSample / 8
        /// </summary>
        public int ByteRate;

        /// <summary>
        /// ChannelCount * BitsPerSample / 8
        /// </summary>
        public short Align;

        /// <summary>
        /// 8, 16, 24, 32 (Разрядность квантования)
        /// </summary>
        public short BitsPerSample;
    }
}
