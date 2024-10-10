using System;
using System.Runtime.CompilerServices;

namespace AI.Algorithms.PriorityQueues
{
    /// <summary>
    /// Простая очередь с приоритетом, с первым максимальным
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SimplePriorityQueueMax<T> : IPriorityQueueMax<T>
        where T : IComparable<T>
    {

        private readonly T[] _data;
        private int size = 0;

        /// <summary>
        /// Простая очередь с приоритетом, с первым максимальным
        /// </summary>
        public SimplePriorityQueueMax(int capasity)
        {
            _data = new T[capasity];
        }

        /// <summary>
        /// Простая очередь с приоритетом, с первым максимальным
        /// </summary>
        public SimplePriorityQueueMax(T[] data)
        {
            _data = new T[data.Length];
            Array.Copy(data, 0, _data, 0, _data.Length);
        }

        /// <summary>
        /// Удаляет и возвращает максимальный элемент
        /// </summary>
        /// <returns></returns>
        public T DelMax()
        {
            int maxInd = 0;

            for (int i = 1; i < size; i++)
            {
                if (IsMax(i, maxInd))
                {
                    maxInd = i;
                }
            }
            ExCh(maxInd, size - 1);
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
        public bool Insert(T element)
        {
            if (_data.Length > size)
            {
                _data[size++] = element;
                return true;
            }
            else return false;
        }

        public T KeyMax()
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
        private bool IsMax(int ind1, int ind2)
        {
            return _data[ind1].CompareTo(_data[ind2]) >= 0;
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
