using System;
using System.Runtime.CompilerServices;

namespace AI.Algorithms.PriorityQueues
{
    /// <summary>
    /// Простая очередь с приоритетом, с первым минимальным
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SimplePriorityQueueMin<T> : IPriorityQueueMin<T>
        where T : IComparable<T>
    {

        private readonly T[] _data;
        private int size = 0;

        /// <summary>
        /// Простая очередь с приоритетом, с первым минимальным
        /// </summary>
        public SimplePriorityQueueMin(int capasity)
        {
            _data = new T[capasity];
        }

        /// <summary>
        /// Простая очередь с приоритетом, с первым минимальным
        /// </summary>
        public SimplePriorityQueueMin(T[] data)
        {
            _data = new T[data.Length];
            Array.Copy(data, 0, _data, 0, _data.Length);
        }

        /// <summary>
        /// Удаляет и возвращает минимальный элемент
        /// </summary>
        /// <returns></returns>
        public T DelMin()
        {
            int minInd = 0;

            for (int i = 1; i < size; i++)
                if (Less(i, minInd)) minInd = i;

            ExCh(minInd, size - 1);
            return _data[--size];
        }

        /// <summary>
        /// Пустая ли очередь
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return size == 0;
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <returns>Был ли добавлен элемент (true - был, false - нет)</returns>
        public void Insert(T element)
        {
            _data[size++] = element;
        }

        public T KeyMin()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Размер очереди
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return size;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Less(int ind1, int ind2)
        {
            return _data[ind1].CompareTo(_data[ind2]) <= 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExCh(int k, int z)
        {
            T mid = _data[k];
            _data[k] = _data[z];
            _data[z] = mid;
        }
    }
}
