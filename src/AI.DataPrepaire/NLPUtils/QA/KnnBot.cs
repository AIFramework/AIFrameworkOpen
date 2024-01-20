using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using AI.ML.Distances;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AI.DataPrepaire.NLPUtils.QA
{
    /// <summary>
    /// Экстрактивный бот на базе knn
    /// </summary>
    [Serializable]
    public class KnnBot
    {
        /// <summary>
        /// Вектор средних
        /// </summary>
        public Vector Mean { get; set; }
        /// <summary>
        /// СКО
        /// </summary>
        public Vector Std { get; set; }

        /// <summary>
        /// Преобразование меток класса в ответы
        /// </summary>
        public List<string> ClassesToStr { get; set; }

        /// <summary>
        /// Метод ближайшего соседа
        /// </summary>
        public KNNCl KNN { get; set; }

        /// <summary>
        /// Функция преобразования текста в вектор
        /// </summary>
        public Func<string, Vector> TextTransform { get; set; }

        /// <summary>
        /// Экстрактивный бот на базе knn
        /// </summary>
        public KnnBot(Func<string, Vector> textTransform) 
        {
            TextTransform = textTransform;
        }


        /// <summary>
        /// Обучение бота
        /// </summary>
        /// <param name="textQ"></param>
        /// <param name="textAns"></param>
        public void Train(IEnumerable<string> textQ, IEnumerable<string> textAns)
        {
            string[] qs = textQ.ToArray();
            string[] answers = textAns.ToArray();

            // Создание векторов вопросов
            Vector[] features = new Vector[qs.Length];

            Parallel.For(0, features.Length, i =>
            {
                features[i] = TextTransform(qs[i]);
            });

            Mean = Vector.Mean(features);
            Std = Vector.Std(features) + AISettings.GlobalEps;

            for (int i = 0; i < features.Length; i++)
                features[i] = (features[i] - Mean) / Std;


            // Обработка меток
            Dictionary<string, int> classesDict = new Dictionary<string, int>();
            ClassesToStr = new List<string>(answers.Length);
            int cl = 0;

            foreach (var answer in answers)
            {
                if (!classesDict.ContainsKey(answer))
                {
                    classesDict.Add(answer, cl);
                    ClassesToStr.Add(answer);
                    cl++;
                }
            }

            int[] marksTrain = new int[classesDict.Count];

            for (int i = 0; i < answers.Length; i++)
                marksTrain[i] = classesDict[answers[i]];

            
            // Обучение
            KNN = new KNNCl();
            KNN.IsParsenMethod = true;
            KNN.K = 2;
            KNN.Dist = BaseDist.CosDistRelu;
            KNN.Train(features, marksTrain);

        }

        /// <summary>
        /// Получение ответа
        /// </summary>
        /// <param name="text">Текст вопроса</param>
        /// <returns></returns>
        public Answer GetAnswer(string text)
        {
            var embd = TextTransform(text);
            embd = (embd - Mean) / Std;
            Vector vect = KNN.ClassifyProbVector(embd);
            int mark = vect.MaxElementIndex();
            double conf = Math.Round(vect[mark] * 100, 1);
            string answer = ClassesToStr[mark];

            return new Answer() { Conf = conf, AnswerStr = answer };
        }


        /// <summary>
        /// Класс ответа
        /// </summary>
        public class Answer
        {
            /// <summary>
            /// Уверенность в ответе
            /// </summary>
            public double Conf { get; set; }

            /// <summary>
            /// Ответ строкой
            /// </summary>
            public string AnswerStr { get; set; }
        }
    }
}
