using AI.DataPrepaire.DataNormalizers;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Vector[] vectors =
            {
                new Vector(100, 1000, 200, 0.1),
                new Vector(140, 0, 150, 0.3),
                new Vector(170, -1000, 100, 10.1),
            };

            Vector test = new Vector(100, 1000, 200, 2);


            bool Z = false; // Z - нормализация
            bool MM = true;// минимакс - нормализация

            if (Z)
            {

                ZNormalizer z = new ZNormalizer();
                z.Train(vectors);
                var dataPrep = z.Transform(vectors);
            }
            if (MM)
            {
                MinimaxNomalizer minimax = new MinimaxNomalizer();
                minimax.Train(vectors);
                var dataPrep = minimax.Transform(vectors);
            }
        }
    }
}
