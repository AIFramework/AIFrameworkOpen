using AI.DataPrepaire.DataLoader.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.NLP;
using AI.NLP.Stemmers;
using System.IO;
using AI.DataPrepaire.Tokenizers.TextTokenizers;

namespace DataFrameTestCSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var csv = CSVLoader.Read("iris.csv",','); // Загрузка csv
            //csv.ColumnToCategorical(4); // Получение категорий
            //var dataLine = csv.GetRow(3); // 3я строка
            //var colums = csv.GetColums(); // Имена столбцов
            //var df = csv.GetSubTable(new[] { colums[1], colums[2] }); // Построение подтаблицы с указанными столбцами

            //Console.WriteLine(csv);


            WordEndingsRUTokenizer wordEndingsRU = new WordEndingsRUTokenizer("ва.txt");

            var a = wordEndingsRU.Encode("Построение подтаблицы с указанными столбцами");
            var dec = wordEndingsRU.Decode(a);
        }
    }
}
