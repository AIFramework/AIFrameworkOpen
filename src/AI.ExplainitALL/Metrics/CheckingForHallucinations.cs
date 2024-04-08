// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------

using AI.DataPrepaire.NLPUtils.RegexpNLP;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.ExplainitALL.Metrics
{
    /// <summary>
    /// Класс для выявления галлюцинаций в RAG задачах
    /// </summary>
    [Serializable]
    public class CheckingForHallucinations<T>
    {

        public SimMatrix<T> SimMatrixAlg { get; set; }
        SentencesTokenizer Sentences { get; set; } = new SentencesTokenizer();

        /// <summary>
        /// Класс для выявления галлюцинаций в RAG задачах
        /// </summary>
        public CheckingForHallucinations(SimMatrix<T> simMatrixAlg)
        {
            SimMatrixAlg = simMatrixAlg;
        }

        /// <summary>
        /// Загрузка текстов
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string[] LoadDoc(string text)
        {
            return Sentences.Tokenize(text).ToArray();
        }

        /// <summary>
        /// Получение поддерживающих текстов для кусочков ответа
        /// </summary>
        public List<AnalyzeElement> GetSupportSeq(string doc, string answer, double prob = 0.6, int topK = 1)
        {
            string[] answerStrings = LoadDoc(answer);
            string[] docData = LoadDoc(doc);

            // Получение матрицы схожести
            Matrix matrix = SimMatrixAlg.GenerateMatrix(answerStrings, docData);

            topK = Math.Min(topK, docData.Length);

            var results = new List<AnalyzeElement>();
            for (int i = 0; i < answerStrings.Length; i++)
            {
                var slice = Enumerable.Range(0, docData.Length).Select(x => matrix[i, x]).ToArray();
                var topIndexes = slice.Select((value, index) => new { Value = value, Index = index })
                                      .OrderByDescending(x => x.Value)
                                      .Take(topK)
                                      .Where(x => x.Value >= prob)
                                      .Select(x => x.Index)
                                      .ToArray();

                var referenceTexts = new List<string>();
                var indexes = new List<int>();
                foreach (var index in topIndexes)
                {
                    referenceTexts.Add(docData[index]);
                    indexes.Add(index);
                }

                if (indexes.Count > 0)
                {
                    results.Add(new AnalyzeElement
                    {
                        AnswerBlock = answerStrings[i],
                        SupportBlocks = referenceTexts,
                        SupportBlocksIndexInDoc = indexes
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// Уверенность в ответе
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="answer"></param>
        /// <param name="prob"></param>
        /// <returns></returns>
        public double GetConf(string doc, string answer, double prob = 0.6)
        {
            var answerElements = GetSupportSeq(doc, answer, prob);
            int lenAll = answer.Replace(" ", "").Length; 
            int lenWithoutH = answerElements.Sum(a => a.AnswerBlock.Replace(" ", "").Length);

            return (double)lenWithoutH / lenAll;
        }

        /// <summary>
        /// Вероятность галлюцинации
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="answer"></param>
        /// <param name="prob"></param>
        /// <returns></returns>
        public double GetHallucinationsProb(string doc, string answer, double prob = 0.6)
        {
            return 1 - GetConf(doc, answer, prob);
        }

    }
}
