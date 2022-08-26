using AI.DataStructs;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Структура классификатора
    /// </summary>
    [Serializable]
    public class StructClasses : List<VectorClass>
    {
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Load from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static StructClasses Load(string path)
        {
            return BinarySerializer.Load<StructClasses>(path);
        }
        /// <summary>
        /// Load from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static StructClasses Load(Stream stream)
        {
            return BinarySerializer.Load<StructClasses>(stream);
        }
    }
}
