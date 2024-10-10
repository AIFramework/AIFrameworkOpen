namespace AI.Logic.Fuzzy
{
    /// <summary>
    /// Интерфейс для набора образов элементов.
    /// </summary>
    /// <typeparam name="ElType">Тип элемента.</typeparam>
    /// <typeparam name="ImgType">Тип образа элемента.</typeparam>
    public interface IImageSet<ElType, ImgType>
    {
        /// <summary>
        /// Получение образа для элемента.
        /// </summary>
        /// <param name="type">Элемент.</param>
        /// <returns>Образ элемента.</returns>
        ImgType GetImage(ElType type);
    }

}
