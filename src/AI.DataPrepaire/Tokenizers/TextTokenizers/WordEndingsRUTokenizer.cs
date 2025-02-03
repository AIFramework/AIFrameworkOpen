using AI.NLP.Stemmers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    /// <summary>
    /// Токенизатор окончаний для русского языка
    /// </summary>
    [Serializable]
    public class WordEndingsRUTokenizer : TokenizerBase<string>
    {


        /// <summary>
        /// Токенизатор окончаний для русского языка 
        /// </summary>
        public WordEndingsRUTokenizer(string[] decoder, Dictionary<string, int> encoder) : base(decoder, encoder)
        {

        }

        /// <summary>
        /// Токенизатор окончаний для русского языка
        /// </summary>
        public WordEndingsRUTokenizer(string path_to_text)
        {
            TrainFromTextFile(path_to_text);
        }


        /// <summary>
        /// Кодирование текста
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int[] Encode(string data)
        {
            string[] words = WordEndingsRU.Endings(data);
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

            return stringBuilder.ToString();
        }



        /// <summary>
        /// Обучение/создание токенизатора
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void TrainFromTextFile(string path)
        {
            string text = File.ReadAllText(path);
            TrainFromText(text);
        }




        /// <summary>
        /// Обучение/создание токенизатора
        /// </summary>
        public void TrainFromText(string text)
        {
            var data = new HashSet<string>(WordEndingsRU.Endings(text));

            Dictionary<string, int> words = new Dictionary<string, int>();
            string[] decoder = new string[data.Count + 4];

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

            foreach (string word in data)
            {
                if (token_index != UnknowToken && token_index != PadToken && token_index != StartToken && token_index != EndToken)
                {
                    decoder[token_index] = word;
                    words.Add(word, token_index);
                }

                token_index++;
            }

            // загружаем обученный токенизатор
            this.decoder = decoder;
            encoder = words;
        }
    }
}
