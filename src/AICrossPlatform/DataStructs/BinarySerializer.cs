using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace AI.DataStructs
{
    /// <summary>
    /// Вспомогательный класс для бинарной сериализации
    /// </summary>
    /// <remarks>
    /// ⚠️ ВАЖНО: Данный класс использует BinaryFormatter, который является УСТАРЕВШИМ и НЕБЕЗОПАСНЫМ.
    /// Microsoft официально признал BinaryFormatter deprecated начиная с .NET 5.
    /// 
    /// РИСКИ БЕЗОПАСНОСТИ:
    /// - Десериализация недоверенных данных может привести к выполнению произвольного кода (RCE)
    /// - Уязвимости к атакам типа deserialization attacks
    /// 
    /// РЕКОМЕНДАЦИИ:
    /// 1. НЕ используйте этот класс для десериализации данных из ненадежных источников
    /// 2. Рассмотрите миграцию на:
    ///    - System.Text.Json для простых объектов
    ///    - protobuf-net или MessagePack для бинарной сериализации
    ///    - Кастомную сериализацию для сложных структур
    /// 
    /// Этот класс сохранён для обратной совместимости с существующими проектами.
    /// </remarks>
    [Obsolete("BinaryFormatter is deprecated and insecure. Consider migrating to System.Text.Json or protobuf-net. " +
              "Only use with trusted data sources.")]
    public static class BinarySerializer
    {
        /// <summary>
        /// Загрузка из потока
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Load<T>(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (File.Exists(filePath))
            {
                using FileStream fs = new FileStream(filePath, FileMode.Open);
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
                BinaryFormatter formatter = new BinaryFormatter
                {
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                };

                if (fs.Length == 0)
                {
                    throw new ArgumentException("File is empty", nameof(filePath));// Файл пуст
                }

                object data = formatter.Deserialize(fs);
#pragma warning restore SYSLIB0011

                if (data is T t)
                {
                    return t;
                }
                else
                {
                    throw new InvalidOperationException($"Not a {typeof(T)}"); // не является нужным типом
                }
            }
            else
            {
                throw new FileNotFoundException("File was not found", filePath);
            }
        }
        /// <summary>
        /// Загрузка из потока
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static T Load<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (stream.Length == 0)
            {
                throw new ArgumentException("Stream is empty", nameof(stream));
            }

#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
            BinaryFormatter formatter = new BinaryFormatter
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };

            object data = formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011

            if (data is T t)
            {
                return t;
            }
            else
            {
                throw new InvalidOperationException($"Not a {typeof(T)}"); // не является нужным типом
            }
        }
        /// <summary>
        /// Сохранение в бинарный файл
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public static void Save<T>(string filePath, T data)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            FileInfo fileInfo = new FileInfo(filePath);

            string dir = fileInfo.DirectoryName;

            if (!Directory.Exists(dir))
            {
                _ = Directory.CreateDirectory(dir);
            }

            using Stream stream = File.Open(filePath, FileMode.Create);
            Save(stream, data);
        }
        /// <summary>
        /// Сохранение в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <param name="data"></param>
        public static void Save<T>(Stream stream, T data)
        {
#pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
            BinaryFormatter formatter = new BinaryFormatter()
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };

            formatter.Serialize(stream, data);
#pragma warning restore SYSLIB0011
        }
    }
}