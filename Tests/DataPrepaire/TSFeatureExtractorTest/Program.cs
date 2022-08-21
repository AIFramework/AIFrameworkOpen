using AI.DataPrepaire.FeatureExtractors.TimeSeq;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFeatureExtractorTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            double sr = 780;
            double f0 = 8;
            double f1 = 12;

            double halfBand = 3;

            Vector sin = Vector.Time0(sr, 2);
            sin = sin.Transform(x => Math.Sin(x * 2 * Math.PI * f0) + 1.5* Math.Cos(x * 2 * f1 * Math.PI));
            sin += AI.Statistics.Statistic.RandNorm(sin.Count, random); // Добавление шума


            //Преобразователь
            double[] down = { f0 - halfBand, f1 - halfBand, 30};
            double[] up = { f0 + halfBand, f1 + halfBand, 32};

            FilterExtractor fs = new FilterExtractor(down, up, sr);
            var featur = fs.GetFeatures(sin, (int)(sr/2));
        }
    }
}
