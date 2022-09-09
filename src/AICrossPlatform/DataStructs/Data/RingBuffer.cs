using System;

namespace AI.DataStructs.Data
{

    /// <summary>
    /// Циклический буфер
    /// </summary>
    [Serializable]
    public class RingBuffer<T>
    {
        /// <summary>
        /// Данные буфера
        /// </summary>
        public T[] Data { get; set; }

        /// <summary>
        /// Число элементов буфера
        /// </summary>
        public int Length
        {
            get { return Data.Length; }
            set { Data = new T[value]; }
        }

        /// <summary>
        /// Создание буфера нужной длинны
        /// </summary>
        /// <param name="length">Длинна</param>
        public RingBuffer(int length)
        {
            Length = length;
        }


        /// <summary>
        /// Создание буфера нужной длинны
        /// </summary>
        /// <param name="length">Длинна</param>
        /// <param name="defoultElement">Элемент поумолчанию, до заполнения</param>
        public RingBuffer(int length, T defoultElement)
        {
            Length = length;
            for (int i = 0; i < length; i++)
            {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                Data[i] = defoultElement;
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
            }
        }

        /// <summary>
        /// Преобразование циклического буфера в массив
        /// </summary>
        /// <param name="buffer">Буффер</param>
        public static implicit operator T[](RingBuffer<T> buffer)
        {
            return buffer.Data;
        }

        /// <summary>
        /// Преобразование в циклический буффер
        /// </summary>
        /// <param name="elements"></param>
        public static explicit operator RingBuffer<T>(T[] elements)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            RingBuffer<T> ringBuffer = new RingBuffer<T>(elements.Length);
            ringBuffer.Data = elements;
            return ringBuffer;
        }


        /// <summary>
        /// Добавление элемента в буфер(в конец)
        /// </summary>
        public void AddElement(T element)
        {
            int len = Length;

            for (int i = 1; i < len; i++)
            {
                Data[i - 1] = Data[i];
            }

            Data[len - 1] = element;
        }
    }
}
