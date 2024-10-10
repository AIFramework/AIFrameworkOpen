namespace AI.Logic.Fuzzy
{
    /// <summary>
    /// Интерфейс для получения коэффициентов принадлежности.
    /// </summary>
    /// <typeparam name="ElType">Тип элемента.</typeparam>
    /// <typeparam name="ImgType">Тип образа элемента.</typeparam>
    public interface IMu<ElType, ImgType>
    {
        /// <summary>
        /// Начальный коэффициент принадлежности.
        /// </summary>
        double Mu0 { get; set; }

        /// <summary>
        /// Получение коэффициента принадлежности для элемента.
        /// </summary>
        /// <param name="data">Элемент.</param>
        /// <param name="image">Образ элемента.</param>
        /// <returns>Коэффициент принадлежности.</returns>
        double GetCoef(ElType data, ImgType image);
    }

}
