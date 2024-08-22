using System;

namespace AI.Algorithms.PriorityQueues
{
    /// <summary>
    /// Интерфейс очереди с приоритетом, с первым минимальным
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPriorityQueueMin<T> where T : IComparable<T>
    {
        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <returns>Был ли добавлен элемент (true - был, false - нет)</returns>
        void Insert(T data);
        /// <summary>
        /// Удалить минимальный из очереди и вернуть его
        /// </summary>
        /// <returns></returns>
        T DelMin();
        T KeyMin();
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
