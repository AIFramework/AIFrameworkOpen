using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using AI.ML.NeuralNetwork.CoreNNW;
using System;

namespace TestNNW
{
    public static class TestIO
    {
        private static readonly Random _rand = new Random();

        private static void TestVector()
        {
            Console.WriteLine("===================================");
            Console.WriteLine("Vector test:");
            Vector vector = new Vector(3);
            for (int i = 0; i < _rand.Next(10); i++)
            {
                vector.Add(_rand.Next(10));
            }
            Console.Write($"Vector: {vector}");
            Console.WriteLine();
            InMemoryDataStream stream = vector.ToDataStream();
            byte[] bytes = stream.AsByteArray();
            Console.WriteLine($"Bytes: [{bytes.Length}]");
            Console.WriteLine($"Bytes zipped: [{stream.Zip().AsByteArray().Length}]");
            Console.WriteLine($"Bytes unzipped: [{stream.UnZip().AsByteArray().Length}]");
            Vector fromBytes = stream.ReadVector();
            Console.Write($"Vector from bytes: {fromBytes}");
            Console.WriteLine();
        }

        private static void TestMatrix()
        {
            Console.WriteLine("===================================");
            Console.WriteLine("Matrix test:");
            Matrix matrix = new Matrix(_rand.Next(2, 7), _rand.Next(2, 7));
            for (int i = 0; i < matrix.Data.Length; i++)
            {
                matrix.Data[i] = _rand.Next(10);
            }
            Console.WriteLine("Matrix: ");
            Console.WriteLine(matrix);
            byte[] bytes = matrix.GetBytes();
            Console.Write($"Bytes: [{bytes.Length}]");
            Console.WriteLine();
            Matrix fromBytes = Matrix.FromBytes(bytes);
            Console.WriteLine("Matrix from bytes: ");
            Console.WriteLine(fromBytes);
        }

        private static void TestTensor()
        {
            Console.WriteLine("===================================");
            Console.WriteLine("Tensor test:");
            Tensor tensor = new Tensor(_rand.Next(2, 5), _rand.Next(2, 5), _rand.Next(1, 3));
            for (int i = 0; i < tensor.Data.Length; i++)
            {
                tensor.Data[i] = _rand.Next(10);
            }
            Console.WriteLine("Tensor: ");
            Console.WriteLine(tensor);
            byte[] bytes = tensor.GetBytes();
            Console.Write($"Bytes: [{bytes.Length}]");
            Console.WriteLine();
            Tensor fromBytes = Tensor.FromBytes(bytes);
            Console.WriteLine("Tensor from bytes: ");
            Console.WriteLine(fromBytes);
        }

        private static void TestNNValue()
        {
            Console.WriteLine("===================================");
            Console.WriteLine("NNValue test:");
            NNValue val = NNValue.RandomR(_rand.Next(2, 7), _rand.Next(2, 7), _rand.Next(1, 3), 0.5, _rand);
            Console.WriteLine(val);
            Console.WriteLine($"Shape: {val.Shape}");
            byte[] bytes = val.GetBytes();
            Console.WriteLine($"Bytes: [{bytes.Length}]");
            Console.WriteLine();
            NNValue fromBytes = NNValue.FromBytes(bytes);
            Console.WriteLine("NNValue from bytes: ");
            Console.WriteLine(fromBytes);
            Console.WriteLine($"Shape: {fromBytes.Shape}");
        }

        public static void Execute()
        {
            TestVector();
            TestMatrix();
            TestTensor();
            TestNNValue();
        }
    }
}