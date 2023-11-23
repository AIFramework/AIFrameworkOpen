using System;

namespace AI.DataPrepaire.DataLoader.Sound
{

    /// <summary>
    /// A helper class for audio data conversion.
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Converts a byte array to an array of shorts, assuming 16-bit mono audio.
        /// </summary>
        /// <param name="buffer">The byte array containing 16-bit audio data.</param>
        /// <returns>An array of shorts representing the audio data.</returns>
        /// <remarks>
        /// This method assumes that each pair of bytes in the input array represents a single audio sample in little-endian format.
        /// </remarks>
        public static short[] ConvertByteToShort(byte[] buffer)
        {
            int shortCount = buffer.Length / 2;
            short[] shortArray = new short[shortCount];
            for (int index = 0, i = 0; i < shortCount; i++)
            {
                shortArray[i] = BitConverter.ToInt16(buffer, index);
                index += 2;
            }
            return shortArray;
        }

        /// <summary>
        /// Converts an array of shorts to an array of floats.
        /// </summary>
        /// <param name="shortArray">The array of shorts representing audio data.</param>
        /// <returns>An array of floats normalized to the range of -1.0f to 1.0f.</returns>
        /// <remarks>
        /// This method normalizes the 16-bit audio samples to the range of -1.0f to 1.0f suitable for processing or playback.
        /// </remarks>
        public static float[] ConvertShortToFloat(short[] shortArray)
        {
            float[] floatArray = new float[shortArray.Length];
            float maxShort = short.MaxValue;

            for (int i = 0; i < shortArray.Length; i++)
            {
                // Normalize the short to the range of -1.0f to 1.0f;
                floatArray[i] = shortArray[i] / maxShort;
            }

            return floatArray;
        }
    }
}
