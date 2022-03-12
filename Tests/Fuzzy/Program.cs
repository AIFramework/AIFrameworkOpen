using AI.DataStructs.Algebraic;
using AI.Fuzzy;
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
            double f1 = 2;
            double f2 = 1.5;
            Vector t = Vector.SeqBeginsWithZero(0.01, 2);
            int N = 10000;
            double k = 1.0;


            Vector cl_inp1 = t.Transform(x => Math.Sin(x * 2 * f1 * Math.PI));
            Vector cl_inp2 = t.Transform(x => Math.Sin(x * 2 * f2 * Math.PI));

            Vector cl_output_1 = new Vector(1, 0.1);
            Vector cl_output_2 = new Vector(0.1, 1);

            Vector[] cli = new Vector[2*N];
            Vector[] clo = new Vector[2*N];

            // Создание выборки
            for (int i = 0; i < N; i++)
            {
                cli[i] = cl_inp1 + k * Statistic.Rand(cl_inp1.Count);
                cli[i] = (cli[i] - cli[i].Min()) / (cli[i].Max() - cli[i].Min());

                cli[i + N] = cl_inp2 + k * Statistic.Rand(cl_inp1.Count);
                cli[i+N] = (cli[i+N] - cli[i+N].Min()) / (cli[i+N].Max() - cli[i+N].Min());


                clo[i] = cl_output_1;
                clo[i + N] = cl_output_2;
            }


            var impl = FuzzyAnalogyInference.GetImplicationMatrixG(cli, clo);
            
            // Тестовые векторы

            Vector test1 = cl_inp1 + k * Statistic.Rand(cl_inp1.Count);
            test1 = (test1 - test1.Min()) / (test1.Max() - test1.Min());
            Vector test2 = cl_inp2 + k * Statistic.Rand(cl_inp1.Count);
            test2 = (test2 - test2.Min()) / (test2.Max() - test2.Min());

            Console.WriteLine("\ncl_1: "+FuzzyAnalogyInference.Inference(impl, test1).Round(1));
            Console.WriteLine("cl_2: "+FuzzyAnalogyInference.Inference(impl, test2).Round(1));
        }

    }
}
