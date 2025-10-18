using AI.DataPrepaire.DataNormalizers;
using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string str =
             @"Vector[] vectors =
            {
                new Vector(100, 1000, 200, 0.1),
                new Vector(140, 0, 150, 0.3),
                new Vector(170, -1000, 100, 10.1),
            };

            Vector test = new Vector(100, 1000, 200, 2);


            bool Z = true; // Z - нормализация
            bool MM = false;// минимакс - нормализация

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
";

            //using (FileStream fileStream = new FileStream("1.png", FileMode.Open)) 
            //{
                byte[] image = Encoding.UTF8.GetBytes(str);
                //fileStream.Read(image, 0, image.Length);
                InMemoryDataStream dataStream = image.ToDataStream();
                dataStream = dataStream.Zip();
                var data = dataStream.AsByteArray();
            Console.WriteLine(data);
            //}
        }
    }
}
