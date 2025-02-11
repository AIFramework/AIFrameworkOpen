using System.Globalization;

namespace AI
{
    /// <summary>
    /// Глобальные настройки
    /// </summary>
    public static class AISettings
    {
        private static readonly NumberFormatInfo s_provider;
        private static readonly NumberFormatInfo s_providerComa;
        /// <summary>
        /// Глобальный эпсилон(смещение) (по-умолчанию = 1e-80)
        /// </summary>
        public static double GlobalEps { get; set; } = 1e-200;

        static AISettings()
        {
            s_provider = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = string.Empty,
            };

            s_providerComa = new NumberFormatInfo
            {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = string.Empty,
            };
        }

        /// <summary>
        /// Получить провайдер для конвертирования чисел в строку и наоборот с точкой в кач. разделителя
        /// </summary>
        public static NumberFormatInfo GetProvider()
        {
            return s_provider;
        }
        /// <summary>
        ///  Получить провайдер для конвертирования чисел в строку и наоборот с запятой в кач. разделителя
        /// </summary>
        public static NumberFormatInfo GetProviderComa()
        {
            return s_providerComa;
        }
    }
}