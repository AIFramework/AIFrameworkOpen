using AI.DataStructs.Algebraic;
using AI.ML.Regression;
using AI.ML.SeqPredict;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AutoRegr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int wSize = 100;
            int time = 600;
            int timeNew = 600;
            int fullTime = time + timeNew;


            Random random = new Random();

            Vector t = Vector.Seq(0, 1, time);
            Vector x = t.Transform(r1 => Math.Sin(2 * 10 * r1 * Math.PI / time) + Math.Sin(2 * 13 * r1 * Math.PI / time) + Math.Cos(2 * 2 * r1 * Math.PI / time));
            x = x.Minimax();
            var xBinary = x.Transform(r1 => (int)(15 *  r1));


            Vector t1 = Vector.Seq(0, 1, fullTime);
            Vector xTest = t1.Transform(r1 => Math.Sin(2 * 10 * r1 * Math.PI / time) + Math.Sin(2 * 13 * r1 * Math.PI / time) + Math.Cos(2 * 2 * r1 * Math.PI / time));
            xTest = xTest.Minimax();
            var xBinaryTest = xTest.Transform(r1 => (int)(15 * r1));


            var xNoise = xBinary.Transform(r1 => r1 + random.Next(-3, 4));
            var ar = new AR(wSize);// SeqPrediction(reg, wSize);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ar.Train(xNoise);
            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds / 1000.0}");


            var pred = ar.Predict(xNoise, timeNew);

            var y_pred = new Vector(pred.GetRange(time, timeNew - 1));
            var y = new Vector(xBinaryTest.GetRange(time, timeNew - 1));
            var r = Statistic.CorrelationCoefficient(y_pred, y);

            Console.WriteLine(r*r);

            chartVisual1.AddPlot(t, xNoise, "Исходные данные");
            chartVisual1.AddPlot(t1, xBinaryTest, "Сигнал без шума данные");
            chartVisual1.AddPlotBlack(pred);

            


        }
    }
}
