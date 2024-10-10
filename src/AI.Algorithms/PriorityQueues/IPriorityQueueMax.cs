using System;

namespace AI.Algorithms.PriorityQueues
{
    /// <summary>
    /// Интерфейс очереди с приоритетом, с первым максимальным
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPriorityQueueMax<T> where T : IComparable<T>
    {
        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <returns>Был ли добавлен элемент (true - был, false - нет)</returns>
        bool Insert(T data);
        /// <summary>
        /// Удалить максимальный из очереди и вернуть его
        /// </summary>
        /// <returns></returns>
        T DelMax();
        T KeyMax();
        /// <summary>
        /// Пустая ли очередь
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
        /// <summary>
        /// Размер очереди
        /// </summary>
        /// <returns></returns>
        int Size();
    }
}
