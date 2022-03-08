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
            Vector ifV = new Vector(0.3, 0.1, 0.2);
            Vector then = new Vector(0.1, 0.2, 0.1);

            var impl = FuzzyAnalogyInference.GetMatrixG(ifV, then);
            Console.WriteLine(impl);


            Console.WriteLine("\n\n"+FuzzyAnalogyInference.Inference(impl, new Vector(0.3, 0.1, 0.2)));
        }
    }
}
