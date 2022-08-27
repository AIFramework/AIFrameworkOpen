using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClPPLDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаем выборку
            Random random = new Random(2);

            //Класс 1
            Vector cl1 = new Vector(1, 2, 9, 11, -2);
            //Класс 2
            Vector cl2 = new Vector(3, 6, 1, 13, -2);

            List<Vector> xList = new List<Vector>();
            List<int> yList = new List<int>();

            //Выборка
            for (int i = 0; i < 300; i++)
            {
                xList.Add(cl1 + 4*AI.Statistics.Statistic.RandNorm(5, random));
                xList.Add(cl2 + 4*AI.Statistics.Statistic.RandNorm(5, random));
                yList.Add(0);
                yList.Add(1);
            }

            // Классификатор
            Classifier classifier = new Classifier();
            Console.WriteLine(classifier.TrainTest(xList.ToArray(), yList.ToArray()));
        }
    }

    /// <summary>
    /// Классификатор
    /// </summary>
    public class Classifier: ObjectClassifierPipeline<Vector> 
    {
        /// <summary>
        /// Классификатор
        /// </summary>
        public Classifier() 
        {
            Normalizer = new ZNormalizer();
            Detector = new NoDetector<Vector>();
            Classifier = new BayesianClassifier();//new KNNCl() { IsParsenMethod = true, K = 3 };
            Extractor = new NoExtractor();
        }
    }
}
