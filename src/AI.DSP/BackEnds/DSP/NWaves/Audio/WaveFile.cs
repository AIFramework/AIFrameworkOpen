using AI.BackEnds.DSP.NWaves.Audio.Interfaces;
using AI.BackEnds.DSP.NWaves.Signals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Audio
{
    /// <summary>
    /// Wav файл
    /// </summary>
    [Serializable]
    public class WaveFile : IAudioContainer
    {
        /// <summary>
        /// Сигналы со всех каналов.
        ///  
        ///     Signals.Count = 1 (Моно)
        /// или
        ///     Signals.Count = 2 (Стерео)
        /// 
        /// </summary>
        public List<DiscreteSignal> Signals { get; }

        /// <summary>
        /// Структура Wav заголовка
        /// </summary>
        public WaveFormat WaveFmt { get; }

        /// <summary>
        /// Поддерживаемые разрядности квантования
        /// </summary>
        public short[] SupportedBitDepths = { 8, 16, 24, 32 };

        /// <summary>
        /// Этот конструктор загружает сигналы из волнового файла.
        ///
        /// Поскольку NWaves является библиотекой .NET Standard 2.0, универсального класса FileStream не существует.
        /// </summary>
        /// <param name="waveStream">Поток входа</param>
        /// <param name="normalized">Нормализовать ли данные</param>
        public WaveFile(Stream waveStream, bool normalized = true)
        {
            using BinaryReader reader = new BinaryReader(waveStream);
            if (reader.ReadInt32() != 0x46464952)     // "RIFF"
            {
                throw new FormatException("NOT RIFF!");
            }

            // ignore file size
            _ = reader.ReadInt32();

            if (reader.ReadInt32() != 0x45564157)     // "WAVE"
            {
                throw new FormatException("NOT WAVE!");
            }

            // try to find "fmt " header in the file:

            long fmtPosition = reader.BaseStream.Position;
            while (fmtPosition != reader.BaseStream.Length - 1)
            {
                reader.BaseStream.Position = fmtPosition;
                int fmtId = reader.ReadInt32();
                if (fmtId == 0x20746D66)
                {
                    break;
                }
                fmtPosition++;
            }

            if (fmtPosition == reader.BaseStream.Length - 1)
            {
                throw new FormatException("NOT fmt !");
            }

            int fmtSize = reader.ReadInt32();

            WaveFormat waveFmt;
            waveFmt.AudioFormat = reader.ReadInt16();
            waveFmt.ChannelCount = reader.ReadInt16();
            waveFmt.SamplingRate = reader.ReadInt32();
            waveFmt.ByteRate = reader.ReadInt32();
            waveFmt.Align = reader.ReadInt16();
            waveFmt.BitsPerSample = reader.ReadInt16();

            WaveFmt = waveFmt;

            if (fmtSize == 18)
            {
                short fmtExtraSize = reader.ReadInt16();
                _ = reader.ReadBytes(fmtExtraSize);
            }

            // there may be some wavefile meta info here,
            // so try to find "data" header in the file:

            long dataPosition = reader.BaseStream.Position;
            while (dataPosition != reader.BaseStream.Length - 1)
            {
                reader.BaseStream.Position = dataPosition;
                int dataId = reader.ReadInt32();
                if (dataId == 0x61746164)
                {
                    break;
                }
                dataPosition++;
            }

            if (dataPosition == reader.BaseStream.Length - 1)
            {
                throw new FormatException("NOT data!");
            }

            int length = reader.ReadInt32();

            length /= waveFmt.ChannelCount;
            length /= waveFmt.BitsPerSample / 8;

            Signals = new List<DiscreteSignal>();

            for (int i = 0; i < waveFmt.ChannelCount; i++)
            {
                Signals.Add(new DiscreteSignal(waveFmt.SamplingRate, length));
            }

            switch (waveFmt.BitsPerSample)
            {
                case 8:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < waveFmt.ChannelCount; j++)
                            {
                                Signals[j][i] = reader.ReadByte() - 128;
                                if (normalized)
                                {
                                    Signals[j][i] /= 128;
                                }
                            }
                        }
                        break;
                    }

                case 16:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < waveFmt.ChannelCount; j++)
                            {
                                Signals[j][i] = reader.ReadInt16();
                                if (normalized)
                                {
                                    Signals[j][i] /= 32768;
                                }
                            }
                        }
                        break;
                    }

                case 32:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < waveFmt.ChannelCount; j++)
                            {
                                Signals[j][i] = reader.ReadInt32();
                                if (normalized)
                                {
                                    Signals[j][i] /= 2147483648;
                                }
                            }
                        }
                        break;
                    }

                case 24:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < waveFmt.ChannelCount; j++)
                            {
                                byte b1 = reader.ReadByte();
                                byte b2 = reader.ReadByte();
                                byte b3 = reader.ReadByte();

                                Signals[j][i] = (b1 << 8) | (b2 << 16) | (b3 << 24);
                                if (normalized)
                                {
                                    Signals[j][i] /= 2147483648;
                                }
                            }
                        }
                        break;
                    }

                default:
                    throw new ArgumentException(
                        "Неправильная битовая глубина! Поддерживаются следующие значения: " + string.Join(", ", SupportedBitDepths));
            }
        }

        /// <summary>
        /// Этот конструктор загружает сигналы в контейнер.
        /// </summary>
        /// <param name="signals">Сигналы для загрузки в контейнер</param>
        /// <param name="bitsPerSample">Биты квантования</param>
        public WaveFile(IList<DiscreteSignal> signals, short bitsPerSample = 16)
        {
            if (signals == null || !signals.Any())
            {
                throw new ArgumentException("Должен быть как минимум один сигнал");
            }

            int samplingRate = signals[0].SamplingRate;
            if (signals.Any(s => s.SamplingRate != samplingRate))
            {
                throw new ArgumentException("Сигналы должны быть дискретизированы с одинаковой частотой дискретизации");
            }

            int length = signals[0].Length;
            if (signals.Any(s => s.Length != length))
            {
                throw new ArgumentException("Сигналы должны иметь одинаковую длину");
            }

            if (!SupportedBitDepths.Contains(bitsPerSample))
            {
                throw new ArgumentException(
                            "Неправильная битовая глубина! Поддерживаются следующие значения: " + string.Join(", ", SupportedBitDepths));
            }

            WaveFormat waveFmt;
            waveFmt.AudioFormat = 1;                        // PCM
            waveFmt.ChannelCount = (short)signals.Count;    // number of channels
            waveFmt.BitsPerSample = bitsPerSample;          // 8, 16, 24 or 32

            waveFmt.Align = (short)(waveFmt.ChannelCount * waveFmt.BitsPerSample / 8);
            waveFmt.SamplingRate = samplingRate;
            waveFmt.ByteRate = waveFmt.SamplingRate * waveFmt.ChannelCount * waveFmt.BitsPerSample / 8;

            WaveFmt = waveFmt;

            Signals = signals.ToList();
        }

        /// <summary>
        /// Этот конструктор загружает сигналы в контейнер.
        /// </summary>
        /// <param name="signal">Сигналы для загрузки в контейнер</param>
        /// <param name="bitsPerSample">Биты квантования</param>
        public WaveFile(DiscreteSignal signal, short bitsPerSample = 16) : this(new[] { signal }, bitsPerSample)
        {
        }

        /// <summary>
        /// Сохранение файла в поток
        /// </summary>
        /// <param name="waveStream">Поток для сохранения</param>
        /// <param name="normalized">Номализовать ли данные</param>
        public void SaveTo(Stream waveStream, bool normalized = true)
        {
            using BinaryWriter writer = new BinaryWriter(waveStream);
            int length = Signals[0].Length;

            writer.Write(0x46464952);     // "RIFF"

            int dataSize = length * WaveFmt.ChannelCount * WaveFmt.BitsPerSample / 8;

            int fileSize = 36 + dataSize;
            writer.Write(fileSize);

            writer.Write(0x45564157);     // "WAVE"
            writer.Write(0x20746D66);     // "fmt "
            writer.Write(16);             // fmtSize = 16 for PCM

            writer.Write(WaveFmt.AudioFormat);
            writer.Write(WaveFmt.ChannelCount);
            writer.Write(WaveFmt.SamplingRate);
            writer.Write(WaveFmt.ByteRate);
            writer.Write(WaveFmt.Align);
            writer.Write(WaveFmt.BitsPerSample);

            writer.Write(0x61746164);      // "data"
            writer.Write(dataSize);

            switch (WaveFmt.BitsPerSample)
            {
                case 8:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < WaveFmt.ChannelCount; j++)
                            {
                                float sample = normalized ? (Signals[j][i] * 128) + 128 : Signals[j][i];
                                writer.Write((sbyte)sample);
                            }
                        }
                        break;
                    }

                case 16:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < WaveFmt.ChannelCount; j++)
                            {
                                float sample = normalized ? Signals[j][i] * 32768 : Signals[j][i];
                                writer.Write((short)sample);
                            }
                        }
                        break;
                    }

                case 32:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < WaveFmt.ChannelCount; j++)
                            {
                                float sample = normalized ? Signals[j][i] * 2147483648 : Signals[j][i];
                                writer.Write((int)sample);
                            }
                        }
                        break;
                    }

                case 24:
                    {
                        for (int i = 0; i < length; i++)
                        {
                            for (int j = 0; j < WaveFmt.ChannelCount; j++)
                            {
                                float sample = normalized ? Signals[j][i] * 2147483648 : Signals[j][i];
                                int s = (int)sample;

                                byte b = (byte)(s >> 8); writer.Write(b);
                                b = (byte)(s >> 16); writer.Write(b);
                                b = (byte)(s >> 24); writer.Write(b);
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Легенда индексов:
        /// 
        ///     waveFile[Channels.Left] -> waveFile.Signals[0]
        ///     waveFile[Channels.Right] -> waveFile.Signals[1]
        ///     waveFile[Channels.Average] -> returns channel-averaged (Новый) signal
        ///     waveFile[Channels.Interleave] -> Возвращает чередующийся (Новый) сигнал
        /// 
        /// </summary>
        /// <param name="channel">Перечисление каналов</param>
        /// <returns>Сигнал из канала или чередующийся сигнал</returns>
        public DiscreteSignal this[Channels channel]
        {
            get
            {
                if (channel != Channels.Interleave && channel != Channels.Sum && channel != Channels.Average)
                {
                    return Signals[(int)channel];
                }

                // in case of averaging or interleaving first check if our signal is mono

                if (WaveFmt.ChannelCount == 1)
                {
                    return Signals[0];
                }

                int length = Signals[0].Length;

                // 1) SUMMING

                if (channel == Channels.Sum)
                {
                    float[] sumSamples = new float[length];

                    for (int i = 0; i < sumSamples.Length; i++)
                    {
                        for (int j = 0; j < Signals.Count; j++)
                        {
                            sumSamples[i] += Signals[j][i];
                        }
                    }

                    return new DiscreteSignal(WaveFmt.SamplingRate, sumSamples);
                }

                // 2) AVERAGING

                if (channel == Channels.Average)
                {
                    float[] avgSamples = new float[length];

                    for (int i = 0; i < avgSamples.Length; i++)
                    {
                        for (int j = 0; j < Signals.Count; j++)
                        {
                            avgSamples[i] += Signals[j][i];
                        }
                        avgSamples[i] /= Signals.Count;
                    }

                    return new DiscreteSignal(WaveFmt.SamplingRate, avgSamples);
                }

                // 3) if it ain't mono, we start ACTUALLY interleaving:

                float[] samples = new float[WaveFmt.ChannelCount * length];

                int idx = 0;
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < WaveFmt.ChannelCount; j++)
                    {
                        samples[idx++] = Signals[j][i];
                    }
                }

                return new DiscreteSignal(WaveFmt.SamplingRate, samples);
            }
        }
    }
}
