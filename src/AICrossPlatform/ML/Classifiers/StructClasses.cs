using AI.DataStructs;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Структура классификатора
    /// </summary>
    [Serializable]
    public class StructClasses : List<VectorClass>
    {

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Добавляет объект <see cref="VectorClass"/> в конец списка.
        /// </summary>
        /// <param name="item">Объект <see cref="VectorClass"/>, который необходимо добавить в список.</param>
        /// <remarks>
        /// Этот метод является потокобезопасным и обеспечивает защиту от одновременного доступа из разных потоков.
        /// </remarks>
        public new void Add(VectorClass item)
        {
            _lock.EnterWriteLock();
            try
            {
                base.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Удаляет первое вхождение указанного объекта <see cref="VectorClass"/> из списка.
        /// </summary>
        /// <param name="item">Объект <see cref="VectorClass"/>, который необходимо удалить из списка.</param>
        /// <remarks>
        /// Этот метод является потокобезопасным и обеспечивает защиту от одновременного доступа из разных потоков.
        /// </remarks>
        public new void Remove(VectorClass item)
        {
            _lock.EnterWriteLock();
            try
            {
                base.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Получает или задает элемент по указанному индексу.
        /// </summary>
        /// <param name="index">Индекс элемента для получения или установки.</param>
        /// <returns>Элемент по указанному индексу.</returns>
        /// <remarks>
        /// Для получения элемента используется потокобезопасный доступ на чтение.
        /// При установке элемента используется потокобезопасный доступ на запись.
        /// </remarks>
        public new VectorClass this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return base[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    base[index] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Сохранить в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранить в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static StructClasses Load(string path)
        {
            return BinarySerializer.Load<StructClasses>(path);
        }
        /// <summary>
        /// Загрузить из потока
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static StructClasses Load(Stream stream)
        {
            return BinarySerializer.Load<StructClasses>(stream);
        }
    }
}
