using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using AI.ML.MatrixUtils;
using System;

namespace MLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            QRTest();
        }



        static void QRTest() 
        {
            Matrix matrix = new Matrix(3, 3);

            matrix[0, 0] = 5;
            matrix[0, 1] = 1;
            matrix[0, 2] = 1;

            matrix[1, 0] = 1;
            matrix[1, 1] = 5;
            matrix[1, 2] = 2;

            matrix[2, 0] = 4;
            matrix[2, 1] = 7;
            matrix[2, 2] = -4;


            // Реальные значения 
            // 7.49, 4.08, -5.58

            var eigenvalues = QR.GetEigenvalues(matrix);
        }


        static void SVMTest() 
        {
            int[] t = new[] { 0, 1 };
            Vector x = new[] { 222.0, 993, 110 };
            Vector x2 = new[] { 222.0, 993, 109 };
            Vector[] X = new[] { x, x2 };

            X = Vector.ScaleData(X);

            SVMBinary svm = new SVMBinary(3)
            {
                MinimalMargin = 0.2,
                L2 = 0.1,
                L1 = 0.01,
                C = 2,
                LearningRate = 0.1,
                NumSupportVectors = 1,
                EpochesToPass = 200
            };

            svm.Train(X, t);


            var o_1 = svm.ClassifyProbVector(X[0]);

            double y = svm.Classify(X[0]);
            double y2 = svm.Classify(X[1]);
            Console.WriteLine($"cl:0 y = {y}; cl:1 y = {y2};");
        }
    }
}
