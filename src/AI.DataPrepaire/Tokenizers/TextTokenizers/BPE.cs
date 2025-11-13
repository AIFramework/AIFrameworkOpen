using AI.DataStructs.Data;
using AI.NeuralSymbolic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    /// <summary>
    /// Токенизация на уровне байт
    /// </summary>
    [Serializable]
    public class BPE<T> : ITokenizer<T>
    {
        /// <summary>
        /// Словарь значение -> токен
        /// </summary>
        protected Dictionary<T, int> encoder = new Dictionary<T, int>();
        /// <summary>
        /// Массив для декодирования
        /// </summary>
        protected T[] decoder;

        /// <summary>
        /// Токенизатор
        /// </summary>
        /// <param name="decoder">Массив токенов для декодирования</param>
        /// <param name="encoder">Словарь для кодирования</param>
        public BPE(T[] decoder, Dictionary<T, int> encoder)
        {
            this.decoder = decoder;
            this.encoder = encoder;
        }


        /// <summary>
        /// Токенизатор
        /// </summary>
        public BPE()
        { }

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

            if (decoder == null)
                throw new Exception("Обучите токенизатор");

            for (int i = 0; i < idsArray.Length; i++)
                decoderArray[i] = decoder[idsArray[i]];

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
                tokens[i] = encoder.Keys.Contains(dataArr[i]) ? encoder[dataArr[i]] : UnknowToken;

            return tokens;
        }





        /// <summary>
        /// Не реализовано
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual T[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public virtual T DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
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
            int batch_size = dArr.Length; // Вычисление размера батча
            int len = dArr[0].Count();

            for (int i = 1; i < dArr.Length; i++)
                if (len < dArr[i].Count()) len = dArr[i].Count();

            len = len <= MaxSize ? len : MaxSize; // Вычисление максимальной длинны

            int[,] batch_tokens = new int[batch_size, len]; // Создание батча

            for (int batch_count = 0; batch_count < batch_size; batch_count++)
            {
                int[] tokens = Encode(dArr[batch_count]); // Кодирование каждой последовательности

                for (int token_ids = 0; token_ids < tokens.Length; token_ids++)
                    batch_tokens[batch_count, token_ids] = tokens[token_ids]; // Заполнение батча
            }

            return batch_tokens;
        }

        // ToDo: Дописать логику
        /// <summary>
        /// Не реализовано
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int[,] EncodeBatch(IEnumerable<T> data)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Ядро (логика) BPE
    /// </summary>
    [Serializable]
    public class BPECore
    {
        private Dictionary<int[], int> encoder; // Словарь для токенизатора

        /// <summary>
        /// Максимальный размер n-граммы
        /// </summary>
        public int MaxNGrammSize { get; set; } = 2;

        /// <summary>
        /// Ядро (логика) BPE
        /// </summary>
        public BPECore() { }


        /// <summary>
        /// Обучение BPE
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        public void TrainBPE(byte[][] bytes)
        {
            encoder = new Dictionary<int[], int>(new IntArrayEqualityComparer());

            for (int i = 0; i < 256; i++)
                encoder.Add(new[] { i }, i); // Добавление всех байт

            var data = new int[bytes.Length][];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new int[bytes[i].Length];
                for (int j = 0; j < data[i].Length; j++)
                    data[i][j] = bytes[i][j];
            }

            TrainCandidateSearch(data);

        }

        // Обучение на одной последовательности с вектором меток
        private void TrainCandidateSearch(int[][] seqs)
        {
            int indexater = 256;

            for (int i = 1; i < MaxNGrammSize + 1; i++)
                indexater = TrainCalcNG(seqs, (short)i, indexater);
        }

        // Обучение с определенной длинной n-граммы
        private int TrainCalcNG(int[][] seqs, short nGram, int indexater)
        {
            int ind = indexater;

            for (int i = 0; i < seqs.Length; i++)
            {
                int[] seq = seqs[i];

                if (seq.Length < nGram) return ind; // Если последовательность меньше анализируемой n-граммы выходим из метода
                RingBuffer<int> ringBuffer = new RingBuffer<int>(nGram);
                int ngOfset = 0; // Смещение, чтобы в буффере не было 0

                for (int j = 0; j < seq.Length; j++)
                {
                    ringBuffer.AddElement(seq[j]);
                    if (ngOfset >= nGram) // Добавление nGramm
                    {
                        int[] d = CopyKey(ringBuffer.Data);

                        if (!encoder.ContainsKey(d))
                            encoder.Add(d, ind++);
                    }

                    ngOfset++;
                }
            }

            return ind;
        }


        /// <summary>
        /// Преобразование строки в байты
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns></returns>
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Токенизация на уровне байт
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public int[] Tokenize(byte[] bytes)
        {
            int[] data = new int[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
                data[i] = (int)bytes[i];

            List<int> list = new List<int>();

            // Проход по всем длиннам буфера
            for (int i = MaxNGrammSize; i > 0; i--)
            {
                RingBuffer<int> ringBuffer = new RingBuffer<int>(i);
                List<int[]> used = new List<int[]>();

                // Проход по последовательности
                for (int j = 0; j < data.Length; j++)
                {
                    ringBuffer.AddElement(data[j]);
                    var key = CopyKey(ringBuffer.Data);


                    if (encoder.ContainsKey(key))
                    {
                        used.Add(ringBuffer.Data); // Добавление n-gramm
                        var dat = encoder[key];
                        list.Add(dat);
                    }
                }

                // Удаление групп
                foreach (var item in used)
                    data = ArrayUtils<int>.DeleteSubArray(data, item);

            }

            return list.ToArray();
        }

        /// <summary>
        /// Токенизация строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int[] Tokenize(string str)
        {
            byte[] bytes = GetBytes(str);
            return Tokenize(bytes);
        }

        private int[] CopyKey(int[] key)
        {
            int[] keyCopy = new int[key.Length];
            Array.Copy(key, 0, keyCopy, 0, key.Length);
            return keyCopy;
        }
    }
}
