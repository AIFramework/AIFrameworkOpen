
using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI
{
    /// <summary>
    /// Description of Sound.
    /// </summary>
    [Serializable]
    public class Sound
    {
        /// <summary>
        /// ID
        /// </summary>
        public int chunkID;
        /// <summary>
        /// Размер файла
        /// </summary>
        public int fileSize;
        /// <summary>
        /// Тип
        /// </summary>
        public int riffType;
        /// <summary>
        /// 
        /// </summary>
        public int fmtID;
        /// <summary>
        /// 
        /// </summary>
        public int fmtSize;
        /// <summary>
        /// 
        /// </summary>
        public int fmtCode;
        /// <summary>
        /// Каналы (число)
        /// </summary>
        public int channels;
        /// <summary>
        /// Sampling frequency
        /// </summary>
        public int sampleRate;
        /// <summary>
        /// средний битрейт
        /// </summary>
        public int fmtAvgBPS;
        /// <summary>
        /// 
        /// </summary>
        public int fmtBlockAlign;
        /// <summary>
        /// 
        /// </summary>
        public int bitDepth;
        /// <summary>
        /// 
        /// </summary>
        public int dataID;
        /// <summary>
        /// 
        /// </summary>
        public int dataSize;

        /// <summary>
        /// Звук
        /// </summary>
        public Sound()
        {
        }


        /// <summary>
        /// Загрузка звука
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns></returns>
        public Vector SoundLoad(string path)
        {

            Stream waveFileStream = File.OpenRead(path);
            BinaryReader reader = new BinaryReader(waveFileStream);

            chunkID = reader.ReadInt32();
            fileSize = reader.ReadInt32();
            riffType = reader.ReadInt32();
            fmtID = reader.ReadInt32();
            fmtSize = reader.ReadInt32();
            fmtCode = reader.ReadInt16();
            channels = reader.ReadInt16();
            sampleRate = reader.ReadInt32();
            fmtAvgBPS = reader.ReadInt32();
            fmtBlockAlign = reader.ReadInt16();
            bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            dataID = reader.ReadInt32();
            dataSize = reader.ReadInt32();

            List<double> fl = new List<double>();

            while (true)
            {
                try
                {
                    fl.Add(reader.ReadInt16() / 32000.0);
                }
                catch
                {
                    break;
                }
            }

            return Vector.FromList(fl);
        }


        /// <summary>
        /// Save vector как звука
        /// </summary>
        /// <param name="path"></param>
        /// <param name="vector"></param>
        /// <param name="fd"></param>
        public void SaveVector(string path, Vector vector, int fd)
        {
            File.Delete(path);
            Stream waveFileStream = File.OpenWrite(path);
            BinaryWriter br = new BinaryWriter(waveFileStream);
            br.Write(1179011410);
            br.Write(4 * vector.Count + 36);
            br.Write(1163280727);
            br.Write(544501094);
            br.Write(16);
            br.Write((short)1);
            br.Write((short)1);
            br.Write(2 * fd);
            br.Write(2 * fd);
            br.Write((short)2);
            br.Write((short)16);
            br.Write(1635017060);
            br.Write(4 * vector.Count);


            double max = Statistic.MaximalValue(FunctionsForEachElements.Abs(vector));
            vector /= max;

            for (int i = 0; i < vector.Count; i++)
            {
                br.Write((int)(vector[i] * 32000));
            }

            br.Close();

        }




    }
}
