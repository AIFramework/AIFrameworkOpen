using AI.DataPrepaire.DataLoader.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFrameTestCSV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var csv = CSVLoader.Read("iris.csv",',');
            var dataLine = csv.GetRow(3);
            var colums = csv.GetColums();
            var df = csv.GetSubTable(new[] { colums[1], colums[2] });

            Console.WriteLine(csv);
        }
    }
}
