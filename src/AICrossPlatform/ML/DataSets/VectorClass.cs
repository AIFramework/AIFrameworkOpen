using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Представляет структуру вектор-класс
    /// </summary>
    [Serializable]
    public class VectorClass
    {
        /// <summary>
        /// Вектор для классификации
        /// </summary>
        public Vector Features;
        /// <summary>
        /// Метка класса
        /// </summary>
        public int ClassMark;

        /// <summary>
        /// Радиус(схожесть)
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Представляет структуру вектор-класс
        /// </summary>
        /// <param name="vector">Вектор</param>
        /// <param name="mark">Метка класса</param>
        public VectorClass(Vector vector, int mark)
        {
            Features = vector;
            ClassMark = mark;
        }

        /// <summary>
        /// Представляет структуру вектор-класс
        /// </summary>
        public VectorClass() { }
    }
}
