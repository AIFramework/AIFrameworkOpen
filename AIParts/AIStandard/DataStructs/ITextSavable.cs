namespace AI.DataStructs
{
    /// <summary>
    /// Object that can be saved in text format
    /// </summary>
    public interface ITextSavable
    {
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        void SaveAsText(string path);
    }
}