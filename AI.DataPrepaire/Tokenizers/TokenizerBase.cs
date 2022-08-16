using System;
using System.Collections.Generic;
using System.Linq;
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
            int[] outps = new int[data.Count()];
            int i = 0;

            foreach (var item in data)
                outps[i++] = encoder.Keys.Contains(item) ? encoder[item] : UnknowToken;

            return outps;
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

        public virtual int[,] EncodeBatch(IEnumerable<IEnumerable<T>> data)
        {
            throw new NotImplementedException();
        }

        public virtual int[,] EncodeBatch(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }
    }
}
