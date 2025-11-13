using AI.DataStructs.Shapes;
using System.Numerics;

namespace AI.DataStructs.WithComplexElements
{
    /// <summary>
    /// Интефейс комплексной структуры
    /// </summary>
    public interface IComplexStructure
    {
        /// <summary>
        /// Одномерный массив комплексных компонент
        /// </summary>
        Complex[] Data { get; }
        /// <summary>
        /// Форма структуры
        /// </summary>
        Shape Shape { get; }
    }
}