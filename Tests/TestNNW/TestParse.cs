using AI.DataStructs.Algebraic;
using System;
using System.Text;

namespace TestNNW
{
    public static class TestParse
    {
        private static readonly Random _rand = new Random();

        private static void TestVector()
        {
            Vector vector = new Vector(3);
            vector.Clear();

            for (int i = 0; i < _rand.Next(4, 10); i++)
            {
                vector.Add(_rand.Next(10));
            }

            string str = vector.ToString();
            Console.WriteLine(str);

            Console.WriteLine(Vector.Parse(str).ToString());

            string norm = "[5 7.2 3.4 50]";
            Console.WriteLine($"Parse result from \"{norm}\": {Vector.TryParse(norm, out Vector res)}, result: \"{res ?? new Vector(3)}\"");

            string bad = "[5 7.2 3.4 50}";
            Console.WriteLine($"Parse result from \"{bad}\": {Vector.TryParse(bad, out Vector res2)}, result: \"{res2 ?? new Vector(3)}\"");
        }

        private static void TestMatrix()
        {
            Matrix matrix = new Matrix(_rand.Next(4, 10), _rand.Next(4, 10));

            for (int i = 0; i < matrix.Data.Length; i++)
            {
                matrix[i] = _rand.Next(4, 10);
            }

            string str = matrix.ToString();
            Console.WriteLine(str);
            Console.WriteLine("====================");
            Console.WriteLine(Matrix.Parse(str));

            StringBuilder sb = new StringBuilder();

            int width = _rand.Next(4, 10);

            for (int i = 0; i < _rand.Next(4, 10); i++)
            {
                Vector v = new Vector(3);
                v.Clear();

                for (int j = 0; j < width; j++)
                {
                    v.Add(_rand.Next(4, 10));
                }

                sb.AppendLine(v.ToString());
            }

            string norm = sb.ToString();

            Console.WriteLine($"Parse result from:{Environment.NewLine}{norm}{Matrix.TryParse(norm, out Matrix res)}, result: {Environment.NewLine}{res ?? new Matrix()}");

            sb.Clear();

            for (int i = 0; i < _rand.Next(4, 10); i++)
            {
                Vector v = new Vector(3);
                v.Clear();

                for (int j = 0; j < _rand.Next(4, 10); j++)
                {
                    v.Add(_rand.Next(4, 10));
                }

                sb.AppendLine(v.ToString());
            }

            string bad = sb.ToString();

            Console.WriteLine($"Parse result from:{Environment.NewLine}{bad}{Matrix.TryParse(bad, out Matrix res2)}, result: {Environment.NewLine}{res2 ?? new Matrix()}");
        }

        public static void Execute()
        {
            TestVector();
            TestMatrix();
        }
    }
}