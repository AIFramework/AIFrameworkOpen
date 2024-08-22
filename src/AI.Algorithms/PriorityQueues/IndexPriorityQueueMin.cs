using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AI.Algorithms.PriorityQueues
{
    /// <summary>
    /// Минимальная очередь с приоритетом, индекс значение
    /// </summary>
    /// <typeparam name="T"></typeparam>

    [Serializable]
    public class IndexPriorityQueueMin<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// Карта, индекс в данных -> индекс в массиве
        /// </summary>
        private readonly Dictionary<int, int> _mapIndexElementIndexInArray;
        private readonly Tuple<int, T>[] _data;
        private int size = 0;

        public IndexPriorityQueueMin(int capasity)
        {
            _data = new Tuple<int, T>[capasity];
            _mapIndexElementIndexInArray = new Dictionary<int, int>(capasity);
        }


        public Tuple<int, T> DelMin()
        {
            int minInd = 0;

            for (int i = 1; i < size; i++)
                if (Less(i, minInd))
                    minInd = i;

            ExCh(minInd, size - 1);
            var el = _data[--size];
            _ = _mapIndexElementIndexInArray.Remove(el.Item1);
            return el;
        }

        public int DelMinGetIndex()
        {
            return DelMin().Item1;
        }

        public T DelMinGetValue()
        {
            return DelMin().Item2;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public void Insert(int index, T element)
        {
            _data[size] = new Tuple<int, T>(index, element);
            _mapIndexElementIndexInArray.Add(index, size);
            size++;
        }

        /// <summary>
        /// Обновить значение
        /// </summary>
        /// <param name="index"></param>
        /// <param name="element"></param>
        public void Update(int index, T element)
        {
            var ind = _mapIndexElementIndexInArray[index];
            _data[ind] = new Tuple<int, T>(index, element);
        }

        /// <summary>
        /// Проверяет есть ли индекс
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsContain(int index)
        {
            return _mapIndexElementIndexInArray.ContainsKey(index);
        }

        public T KeyMin()
        {
            throw new NotImplementedException();
        }

        public int Size()
        {
            return size;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Less(int ind1, int ind2)
        {
            return _data[ind1].Item2.CompareTo(_data[ind2].Item2) <= 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExCh(int k, int z)
        {
            Tuple<int, T> mid = _data[k];
            _data[k] = _data[z];
            _data[z] = mid;
        }

    }
}
