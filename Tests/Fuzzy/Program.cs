using AI.DataStructs.Algebraic;
using AI.Fuzzy;
using AI.Fuzzy.Fuzzyficators.FVector;
using AI.ML.Classifiers;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\n\t****************** Простой тест аналогий   ************************\n");
            SimpleTest();
            Console.WriteLine("\n\n\t****************** Классификатор   ************************\n");
            CLTest();
        }

        // Простой тест аналогий
        static void SimpleTest()
        {
            Vector ifV = new Vector(0.3, 0.1, 0.2, 0.6);
            Vector then = new Vector(0.1, 0.2, 0.3, 0.21);

            Console.WriteLine($"Обучение:\nВектор условия: {ifV}\nВектор следствия: {then}");

            Console.WriteLine("\n\nМатрица импликаций: ");
            var impl = FuzzyAnalogyInference.GetImplicationMatrixG(ifV, then);
            Console.WriteLine(impl.Round(2).ToString());

            Vector x = new Vector(0.3, 0.1, 0.2, 0.6);

            Console.WriteLine($"\n\nВектор условия: {x}\nВектор следствия: {FuzzyAnalogyInference.Inference(impl, x)}");
        }

        // Классификатор
        static void CLTest() 
        {
            // Параметры
            double f1 = 2;
            double f2 = 1.6;
            Vector t = Vector.SeqBeginsWithZero(0.01, 1);
            int N = 10000;
            double k = 1.0;
            // Эталонные сигналы
            Vector cl_inp1 = t.Transform(x => Math.Sin(x * 2 * f1 * Math.PI));
            Vector cl_inp2 = t.Transform(x => Math.Sin(x * 2 * f2 * Math.PI));

            // Создание выборки
            Vector[] cli = new Vector[2 * N];
            int[] clo = new int[2 * N];
            for (int i = 0; i < N; i++)
            {
                cli[i] = cl_inp1 + k * Statistic.Rand(cl_inp1.Count);
                cli[i + N] = cl_inp2 + k * Statistic.Rand(cl_inp1.Count);

                clo[i] = 0;
                clo[i + N] = 1;
            }

            // Тестовые векторы
            Vector test1 = cl_inp1 + k * Statistic.Rand(cl_inp1.Count);
            Vector test2 = cl_inp2 + k * Statistic.Rand(cl_inp1.Count);



            // -------------- Классификатор ----------------- //
            FuzzyClassifier fuzzyCl = new FuzzyClassifier();
            fuzzyCl.Train(cli, clo);


            Console.WriteLine("\ncl_1: " + fuzzyCl.Classify(test1));
            Console.WriteLine("cl_2: " + fuzzyCl.Classify(test2));

            Console.WriteLine("\ncl_1 probs: " + fuzzyCl.ClassifyProbVector(test1).Round(2));
            Console.WriteLine("cl_2 probs: " + fuzzyCl.ClassifyProbVector(test2).Round(2));
        }

    }
}
