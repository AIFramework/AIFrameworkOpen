using AI.DataPrepaire.Tokenizers.TextTokenizers;
using AI.ML.SeqAnalyze;
using AI.NLP;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.NLPUtils.TextClassification
{
    /// <summary>
    /// Классификатор текста на базе правил
    /// </summary>
    [Serializable]
    public class TextRuleClassifier
    {

        WordTokenizer wordTokenizer = new WordTokenizer();
        /// <summary>
        /// Классификатор
        /// </summary>
        public ClassifierS2V classifier;

        /// <summary>
        /// Число правил
        /// </summary>
        public int CountRules => classifier.CountRules;

        /// <summary>
        /// Классификатор текста на базе правил
        /// </summary>
        public TextRuleClassifier(int count_of_classes, double top_p, int max_n_gramm) 
        {
            classifier = new ClassifierS2V(count_of_classes, max_n_gramm, top_p);
        }


        /// <summary>
        /// Обучение 
        /// </summary>
        /// <param name="texts">Тексты</param>
        /// <param name="cls">Классы</param>
        public void Train(string[] texts, int[] cls) 
        {
            wordTokenizer.TrainFromText(texts.Concatinate());
            int[][] data_tr = new int[texts.Length][];

            for (int i = 0; i < texts.Length; i++)
                data_tr[i] = wordTokenizer.Encode(texts[i]);

            classifier.Train(data_tr, cls);
        }

        /// <summary>
        /// Классификация
        /// </summary>
        /// <returns></returns>
        public int Predict(string text)
        {
            int[] inp =  wordTokenizer.Encode(text);
            return classifier.Classify(inp)[0];
        }


        /// <summary>
        /// Добавление правила
        /// </summary>
        /// <param name="rull"></param>
        /// <param name="class_mark"></param>
        public void AddRule(string rull, int class_mark) 
        {
            int[] tokens = wordTokenizer.Encode(rull);
            classifier.AddRuleCl(tokens, class_mark);
        }

    }
}
