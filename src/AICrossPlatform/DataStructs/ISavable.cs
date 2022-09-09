using System.IO;

namespace AI.DataStructs
{
    /// <summary>
    /// Объект, который может быть сохранен
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// Сохранение в файл
        /// </summary>
        /// <param name="path"></param>
        void Save(string path);
        /// <summary>
        /// Сохранение в поток
        /// </summary>
        /// <param name="stream"></param>
        void Save(Stream stream);
    }
}