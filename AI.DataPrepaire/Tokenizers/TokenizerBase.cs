using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Tokenizers
{
    /// <summary>
    /// Базовый класс для токенизаторов
    /// </summary>
    [Serializable]
    public class TokenizerBase<T> : ITokenizer<T>
    {

        protected Dictionary<T, int> encoder = new Dictionary<T, int>();
        protected T[] decoder;

        /// <summary>
        /// Токенизатор
        /// </summary>
        /// <param name="decoder">Массив токенов для декодирования</param>
        /// <param name="encoder">Словарь для кодирования</param>
        public TokenizerBase(T[] decoder, Dictionary<T, int> encoder) 
        {
            this.decoder = decoder;
            this.encoder = encoder;
        }

        /// <summary>
        /// Неизвестный токен
        /// </summary>
        public int UnknowToken { get; set; } = 0;

        /// <summary>
        /// Токен заполнения
        /// </summary>
        public int PadToken { get; set; } = 1;

        /// <summary>
        /// Токен начала
        /// </summary>
        public int StartToken { get; set; } = 2;

        /// <summary>
        /// Токен окончания
        /// </summary>
        public int EndToken { get; set; } = 3;

        /// <summary>
        /// Максимальная длинна последовательности токенов 
        /// </summary>
        public int MaxSize { get; set; } = 1024;

        /// <summary>
        /// Декодирование токенов
        /// </summary>
        /// <param name="ids">Индексы</param>
        public virtual T[] Decode(IEnumerable<int> ids)
        {
            int[] idsArray = ids.ToArray();
            T[] decoderArray = new T[idsArray.Length];

            for (int i = 0; i < idsArray.Length; i++)
            {
                decoderArray[i] = decoder[idsArray[i]];
            }

            return decoderArray;
        }

        /// <summary>
        /// Декодирование батча токенов
        /// </summary>
        /// <param name="ids">Индексы токенов</param>
        public virtual T[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids)
        {
            IEnumerable<int>[] ints = ids.ToArray();
            int[][] intsIds = new int[ints.Length][];

            for (int i = 0; i < ints.Length; i++) 
                intsIds[i] = ints[i].ToArray(); 

            T[][] decoderArray = new T[ints.Length][];

            for (int i = 0; i < ints.Length; i++)
                decoderArray[i] = Decode(ints[i]);

            return decoderArray;
        }

        /// <summary>
        /// Кодирование токенов
        /// </summary>
        /// <param name="data">Данные</param>
        public virtual int[] Encode(IEnumerable<T> data)
        {
            T[] dataArr = data.ToArray();
            int[] tokens = new int[dataArr.Length];
            int len = dataArr.Length <= MaxSize ? dataArr.Length : MaxSize;

            for (int i = 0; i < len; i++)
                tokens[i++] = encoder.Keys.Contains(dataArr[i]) ? encoder[dataArr[i]] : UnknowToken;

            return tokens;
        }






        public virtual T[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }

        public virtual T DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public virtual int[] Encode(T data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Токенизация батча
        /// </summary>
        public virtual int[,] EncodeBatch(IEnumerable<IEnumerable<T>> data)
        {
            var dArr = data.ToArray();
            int batch_size = dArr.Length;
            int len = dArr[0].Count();

            for (int i = 1; i < dArr.Length; i++)
                if (len < dArr[i].Count()) len = dArr[i].Count();

            len = len <= MaxSize ? len : MaxSize;

            int[,] batch_tokens = new int[batch_size, len];

            for (int batch_count = 0; batch_count < batch_size; batch_count++)
            {
                int[] tokens = Encode(dArr[batch_count]);

                for (int token_ids = 0; token_ids < tokens.Length; token_ids++)
                {
                    batch_tokens[batch_count, token_ids] = tokens[token_ids];
                }
            }

            return batch_tokens;
        }

        public virtual int[,] EncodeBatch(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }
    }
}
