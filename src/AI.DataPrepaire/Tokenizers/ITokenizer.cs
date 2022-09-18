using System.Collections.Generic;

namespace AI.DataPrepaire.Tokenizers
{
    /// <summary>
    /// Интерфейс токенизатора
    /// </summary>
    public interface ITokenizer<T>
    {
        /// <summary>
        /// Неизвестный токен
        /// </summary>
        int UnknowToken { get; set; }

        /// <summary>
        /// Токен заполнения
        /// </summary>
        int PadToken { get; set; }

        /// <summary>
        /// Токен начала
        /// </summary>
        int StartToken { get; set; }

        /// <summary>
        /// Токен окончания
        /// </summary>
        int EndToken { get; set; }

        /// <summary>
        /// Максимальная длинна последовательности токенов 
        /// </summary>
        int MaxSize { get; set; }




        /// <summary>
        /// Кодирование токенов
        /// </summary>
        /// <param name="data">Данные</param>
        int[] Encode(IEnumerable<T> data);

        /// <summary>
        /// Кодирование токенов
        /// </summary>
        /// <param name="data">Данные</param>
        int[] Encode(T data);

        /// <summary>
        /// Кодирование батча
        /// </summary>
        /// <param name="data">Данные</param>
        int[,] EncodeBatch(IEnumerable<IEnumerable<T>> data);


        /// <summary>
        /// Кодирование батча
        /// </summary>
        /// <param name="data">Данные</param>
        int[,] EncodeBatch(IEnumerable<T> data);


        /// <summary>
        /// Декодирование токенов
        /// </summary>
        /// <param name="ids">Индексы токенов</param>
        T[] Decode(IEnumerable<int> ids);

        /// <summary>
        /// Декодирование токенов в один объект
        /// </summary>
        /// <param name="ids">Индексы токенов</param>
        T DecodeObj(IEnumerable<int> ids);

        /// <summary>
        /// Декодирование батча токенов
        /// </summary>
        /// <param name="ids">Индексы токенов</param>
        T[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids);

        /// <summary>
        /// Декодирование батча токенов
        /// </summary>
        /// <param name="ids">Индексы токенов</param>
        T[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids);
    }


    /// <summary>
    /// Стратегия заполнения батча
    /// </summary>
    public enum TokenizerPadStratege
    {
        /// <summary>
        /// Максимальная длинна в батче
        /// </summary>
        MaximumSeq,
        /// <summary>
        /// Максимально возможная длинна последовательности
        /// </summary>
        MaxSize
    }
}
