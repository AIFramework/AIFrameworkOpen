using AI.DataStructs.Algebraic;
using AI.Fuzzy;
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
            Vector ifV = new Vector(0.3, 0.1, 0.2, 0.6);
            Vector then = new Vector(0.1, 0.2, 0.3, 0.21);

            Console.WriteLine($"Обучение:\nВектор условия: {ifV}\nВектор следствия: {then}");

            Console.WriteLine("\n\nМатрица импликаций: ");
            var impl = FuzzyAnalogyInference.GetMatrixG(ifV, then);
            Console.WriteLine(impl.Round(2).ToString());

            Vector x = new Vector(0.3, 0.1, 0.2, 0.9);

            Console.WriteLine($"\n\nВектор условия: {x}\nВектор следствия: {FuzzyAnalogyInference.Inference(impl, x)}");
        }
    }
}
