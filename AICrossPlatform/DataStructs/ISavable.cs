using System.IO;

namespace AI.DataStructs
{
    /// <summary>
    /// Object that can be saved
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        /// <summary>
        /// Save to stream
        /// </summary>
        /// <param name="stream"></param>
        void Save(Stream stream);
    }
}