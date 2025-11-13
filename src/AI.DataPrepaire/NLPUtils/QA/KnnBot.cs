using AI.DataStructs.Algebraic;
using AI.Distances;
using AI.ML.Classifiers;
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
        /// Алгоритм классификации
        /// </summary>
        public ClAlg Alg { get; set; } = ClAlg.NN;

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
        public IClassifier KNN { get; set; }

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


            // Выбор алгоритма
            if (Alg == ClAlg.KNN)
            {
                // Обучение
                KNNCl kNN = new KNNCl();
                kNN.IsParsenMethod = true;
                kNN.K = 2;
                kNN.Dist = BaseDist.CosDistRelu;
                kNN.Train(features, marksTrain);

                KNN = kNN;
            }

            else if (Alg == ClAlg.NN)
            {
                // Обучение
                NN nn = new NN();
                nn.Dist = BaseDist.CosDistRelu;
                nn.Train(features, marksTrain);

                KNN = nn;
            }
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

            if (Alg == ClAlg.NN)
                return NNAns(embd);

            else return WithProbAns(embd);

        }

        // Ответ с помощью knn
        private Answer WithProbAns(Vector embd)
        {
            Vector vect = KNN.ClassifyProbVector(embd);
            int mark = vect.MaxElementIndex();
            double conf = Math.Round(vect[mark] * 100, 1);
            string answer = ClassesToStr[mark];

            return new Answer() { Conf = conf, AnswerStr = answer };
        }

        // Ответ с помощью метода эталонов
        private Answer NNAns(Vector embd)
        {
            int mark = KNN.Classify(embd);
            string answer = ClassesToStr[mark];

            return new Answer() { Conf = 100, AnswerStr = answer };
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


        /// <summary>
        /// Алгоритм классификации
        /// </summary>
        public enum ClAlg
        {
            /// <summary>
            /// Метод k ближ соседей
            /// </summary>
            KNN,
            /// <summary>
            /// Метод эталонов (быстрее, но не дает уверенность)
            /// </summary>
            NN
        }
    }


}
