using System;

namespace AI.Logic.Fuzzy
{
    /// <summary>
    /// Элемент нечеткого множества.
    /// </summary>
    /// <typeparam name="ElType">Тип элемента.</typeparam>
    /// <typeparam name="ImgType">Тип образа элемента.</typeparam>
    [Serializable]
    public class FuzzySetElement<ElType, ImgType>
    {
        /// <summary>
        /// Коэффициент принадлежности.
        /// </summary>
        public double Mu { get; set; }

        /// <summary>
        /// Значение элемента.
        /// </summary>
        public ElType ElementValue { get; set; }

        /// <summary>
        /// Образ элемента.
        /// </summary>
        public ImgType Image { get; set; }
    }

}
