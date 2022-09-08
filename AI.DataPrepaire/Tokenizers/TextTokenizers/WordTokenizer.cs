using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    //(ToDo: сделать ограничение по словарю)
    /// <summary>
    /// Токенизатор на уровне слов
    /// </summary>
    [Serializable]
    public class WordTokenizer : TokenizerBase<string>
    {
        /// <summary>
        /// Переводить ли в нижний регистр
        /// </summary>
        public bool IsLower { get; set; } = true;

        /// <summary>
        /// Длинна словаря
        /// </summary>
        public int DictLen { get { return decoder.Length; } }

        /// <summary>
        /// Токенизатор на уровне слов 
        /// </summary>
        public WordTokenizer(string[] decoder, Dictionary<string, int> encoder) : base(decoder, encoder)
        {
        }

        /// <summary>
        /// Токенизатор на уровне слов 
        /// </summary>
        public WordTokenizer(string path_to_text, bool isLower = true)
        {
            IsLower = isLower;
            TrainFromTextFile(path_to_text);
        }



        /// <summary>
        /// Кодирование текста
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int[] Encode(string data)
        {
            string[] words = NLP.TextStandard.OnlyCharsAndDigit(data, IsLower).Split(' ');
            return Encode(words);
        }

        /// <summary>
        /// Декодирование массива индексов в строку
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override string DecodeObj(IEnumerable<int> ids)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] strs = Decode(ids);


            foreach (var str in strs)
            {
                stringBuilder.Append(str);
                stringBuilder.Append(' ');
            }

            return  stringBuilder.ToString();
        }

        /// <summary>
        /// Обучение/создание токенизатора
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void TrainFromTextFile(string path)
        {
            string text = File.ReadAllText(path);
            text = NLP.TextStandard.OnlyCharsAndDigit(text, IsLower);
            NLP.ProbabilityDictionaryHash probability = new NLP.ProbabilityDictionaryHash(false);
            var data = probability.Run(text);

            Dictionary<string, int> words = new Dictionary<string, int>();
            string[] decoder = new string[data.Count+4];

            // Добавляем служебные токены в декодер
            decoder[UnknowToken] = "<UNK>";
            decoder[PadToken] = "<pad>";
            decoder[StartToken] = "<s>";
            decoder[EndToken] = "</s>";

            //Добавляем служебные токены в кодировщик
            words.Add("<UNK>", UnknowToken);
            words.Add("<pad>", PadToken);
            words.Add("<s>", StartToken);
            words.Add("<e>", EndToken);

            int token_index = 0;
            // заполнение токенизатора из вероятностного словаря
            foreach (var item in data)
            {
                for (int i = 0; i < 6; i++) // Пропуск служебных токенов
                {
                    if (token_index != UnknowToken && token_index != PadToken && token_index != StartToken && token_index != EndToken)
                    {
                        decoder[token_index] = item.Key;
                        words.Add(item.Key, token_index);
                        token_index++;
                        break;
                    }
                    token_index++; 
                }
            }

            // загружаем обученный токенизатор
            this.decoder = decoder;
            this.encoder = words;

        }
    }
}
