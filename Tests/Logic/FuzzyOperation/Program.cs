using AI.Logic.Fuzzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyOperation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создание генераторов нечётких множеств для высокой и низкой температуры
            FuzzySetGenerator<double, int> highTempSet = new FuzzySetGenerator<double, int>(new ImgSet(), new HighTempMu()) { Name = "Высокая температура" };
            FuzzySetGenerator<double, int> lowTempSet = new FuzzySetGenerator<double, int>(new ImgSet(), new LowTempMu()) { Name = "Низкая температура" };

            // Набор данных
            var dataSet = new[] { 1.0, 10, 20, 30, 31, 33, 34, 37, 39, 42 };

            // Генерация нечётких множеств для высокой и низкой температуры
            var fuzzySetHigh = highTempSet.SetGenerate(dataSet);
            var fuzzySetLow = lowTempSet.SetGenerate(dataSet);

            // Логические операции "И" и "ИЛИ" для нечётких множеств
            var andOperation = FuzzySetGenerator<double, int>.And(dataSet, new[] { highTempSet, lowTempSet }, 2);
            var orOperation = FuzzySetGenerator<double, int>.Or(dataSet, new[] { highTempSet, lowTempSet }, 2);
        }
    }

    // Реализация интерфейса IImageSet для типа double и int
    class ImgSet : IImageSet<double, int>
    {
        public int GetImage(double type) => (int)type;
    }

    // Реализация функции принадлежности для высокой температуры
    class HighTempMu : IMu<double, int>
    {
        public double Mu0 { get; set; } = 0;

        public double GetCoef(double data, int image) =>
            1.0 / (1 + Math.Pow((data - 40) / 2.4, 2));
    }

    // Реализация функции принадлежности для низкой температуры
    class LowTempMu : IMu<double, int>
    {
        public double Mu0 { get; set; } = 0;

        public double GetCoef(double data, int image) =>
            1.0 / (1 + Math.Pow((data - 35) / 2.4, 2));
    }

}
