using System;

namespace AI.DSP
{
    /// <summary>
    /// Масштаб в вольтах
    /// </summary>
    [Serializable]
    public enum TypeScaleVolt
    {
        /// <summary>
        /// Киловольты 
        /// </summary>
        kV,
        /// <summary>
        /// Вольты
        /// </summary>
        V,
        /// <summary>
        /// Милливольты
        /// </summary>
        mV,
        /// <summary>
        /// Микровольты
        /// </summary>
        uV,
        /// <summary>
        /// Нановольты
        /// </summary>
        nV
    }
}
