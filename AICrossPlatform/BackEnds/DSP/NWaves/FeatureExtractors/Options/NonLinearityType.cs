namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    /// <summary>
    /// Нелинейные схемы постобработки
    /// </summary>
    public enum NonLinearityType
    {
        /// <summary>
        /// Лгарифм по основанию e
        /// </summary>
        LogE,
        /// <summary>
        /// Десятичный логарифм
        /// </summary>
        Log10,
        /// <summary>
        /// Перевод в децибелы
        /// </summary>
        ToDecibel,
        /// <summary>
        /// Получение кубического корня
        /// </summary>
        CubicRoot,
        /// <summary>
        /// Ничего не делать
        /// </summary>
        None
    }
}
