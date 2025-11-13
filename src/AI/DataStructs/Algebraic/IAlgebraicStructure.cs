using AI.DataStructs.Shapes;

namespace AI.DataStructs.Algebraic
{
    /// <summary>
    /// Интерфейс алгебраической структуры
    /// </summary>
    public interface IAlgebraicStructure<T>
    {
        /// <summary>
        /// Представление данных структуры как одномерного массива
        /// </summary>
        T[] Data { get; }
        /// <summary>
        /// Форма структуры
        /// </summary>
        Shape Shape { get; }
    }
}