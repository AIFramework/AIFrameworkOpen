namespace AI.DataStructs
{
    /// <summary>
    /// Объект который может быть сохранен в текстовом виде
    /// </summary>
    public interface ITextSavable
    {
        /// <summary>
        /// Сохранение в файл
        /// </summary>
        /// <param name="path">Путь</param>
        void SaveAsText(string path);
    }
}