﻿using System;

namespace AI.BackEnds.DSP.NWaves.Audio
{
    /// <summary>
    /// Статический класс, предоставляющий методы для преобразования между байтами PCM и данными float[].
    /// </summary>
    [Serializable]
    public static class ByteConverter
    {
        /// <summary>
        /// Конвертирование из Pcm_8bit в float[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="floats"></param>
        /// <param name="normalize"></param>
        public static void ToFloats8Bit(byte[] bytes, float[][] floats, bool normalize = true)
        {
            int channelCount = floats.Length;

            if (normalize)
            {
                for (int n = 0; n < channelCount; n++)
                {
                    for (int i = n, j = 0; i < bytes.Length; i += channelCount, j++)
                    {
                        floats[n][j] = (bytes[i] - 128) / 128f;
                    }
                }
            }
            else
            {
                for (int n = 0; n < channelCount; n++)
                {
                    for (int i = n, j = 0; i < bytes.Length; i += channelCount, j++)
                    {
                        floats[n][j] = bytes[i];
                    }
                }
            }
        }

        /// <summary>
        /// Конвертирование из float[] в Pcm_8bit
        /// </summary>
        /// <param name="floats"></param>
        /// <param name="bytes"></param>
        /// <param name="normalized"></param>
        public static void FromFloats8Bit(float[][] floats, byte[] bytes, bool normalized = true)
        {
            int channelCount = floats.Length;

            if (normalized)
            {
                for (int n = 0; n < channelCount; n++)
                {
                    for (int i = n, j = 0; j < floats[n].Length; i += channelCount, j++)
                    {
                        bytes[i] = (byte)((floats[n][j] * 128) + 128);
                    }
                }
            }
            else
            {
                for (int n = 0; n < channelCount; n++)
                {
                    for (int i = n, j = 0; j < floats[n].Length; i += channelCount, j++)
                    {
                        bytes[i] = (byte)floats[n][j];
                    }
                }
            }
        }

        /// <summary>
        /// Конвертирование из Pcm_16bit в float[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="floats"></param>
        /// <param name="normalize"></param>
        /// <param name="bigEndian"></param>
        public static void ToFloats16Bit(byte[] bytes, float[][] floats, bool normalize = true, bool bigEndian = false)
        {
            int channelCount = floats.Length;
            int step = channelCount * 2;

            if (bigEndian)
            {
                if (normalize)
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; i < bytes.Length; i += step, j++)
                        {
                            floats[n][j] = (short)((bytes[i] << 8) | bytes[i + 1]) / 32768f;
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; i < bytes.Length; i += step, j++)
                        {
                            floats[n][j] = (short)((bytes[i] << 8) | bytes[i + 1]);
                        }
                    }
                }
            }
            else
            {
                if (normalize)
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; i < bytes.Length; i += step, j++)
                        {
                            floats[n][j] = (short)(bytes[i] | (bytes[i + 1] << 8)) / 32768f;
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; i < bytes.Length; i += step, j++)
                        {
                            floats[n][j] = (short)(bytes[i] | (bytes[i + 1] << 8));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Конвертирование из float[] в Pcm_16bit
        /// </summary>
        /// <param name="floats"></param>
        /// <param name="bytes"></param>
        /// <param name="normalized"></param>
        /// <param name="bigEndian"></param>
        public static void FromFloats16Bit(float[][] floats, byte[] bytes, bool normalized = true, bool bigEndian = false)
        {
            int channelCount = floats.Length;
            int step = channelCount * 2;

            if (bigEndian)
            {
                if (normalized)
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; j < floats[n].Length; i += step, j++)
                        {
                            short s = (short)(floats[n][j] * 32768);

                            bytes[i] = (byte)(s >> 8);
                            bytes[i + 1] = (byte)s;
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; j < floats[n].Length; i += step, j++)
                        {
                            short s = (short)floats[n][j];

                            bytes[i] = (byte)(s >> 8);
                            bytes[i + 1] = (byte)s;
                        }
                    }
                }
            }
            else
            {
                if (normalized)
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; j < floats[n].Length; i += step, j++)
                        {
                            short s = (short)(floats[n][j] * 32768);

                            bytes[i] = (byte)s;
                            bytes[i + 1] = (byte)(s >> 8);
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < channelCount; n++)
                    {
                        for (int i = 2 * n, j = 0; j < floats[n].Length; i += step, j++)
                        {
                            short s = (short)floats[n][j];

                            bytes[i] = (byte)s;
                            bytes[i + 1] = (byte)(s >> 8);
                        }
                    }
                }
            }
        }
    }
}
