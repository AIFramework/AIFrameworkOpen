using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using AI.ML.FeaturesTransforms;
using AI.Statistics;
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
            /* 
             * числа
            [-5.5812697 ,  4.08967563,  7.49159407]

            векторы
            |-0.07586913| | 0.792579560| | -0.49272970|,
            |-0.17826201| | -0.59690186| | -0.65635818|,
            | 0.98105379| | -0.12460262| | -0.57132423| */

            int f = 7; // Число признаков
            Random random = new Random(110);
            Matrix c = Statistic.UniformDistribution(f, f, random); // Для создания значимой корреляции
            matrix = Statistic.UniformDistribution(16090, f, random)*c;
            //PCA pca = new PCA(2) { Iterations = 200, Eps = 1};
            //pca.Train(matrix);

            //Console.WriteLine("Ковариационная матрица до МГК(PCA): \n" + Matrix.GetCovMatrixFromColumns(matrix).Round(3));
            //Console.WriteLine("\n\nКовариационная матрица после МГК(PCA): \n" + Matrix.GetCovMatrixFromColumns(pca.Transform(matrix, true)).Round(3));
            //Console.WriteLine($"\n\nПроцент сохраненной энергии: {pca.Info.InfoSaveEnergy*100}% \n");

            AutoEncoder ae = new AutoEncoder(f,2);
            ae.Train(matrix);

            Console.WriteLine("Ковариационная матрица до автокодировщика: \n" + Matrix.GetCovMatrixFromColumns(matrix).Round(3));
            Console.WriteLine("\n\nКовариационная матрица после автокодировщика: \n" + Matrix.GetCovMatrixFromColumns(ae.Transform(matrix)).Round(3));

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
