using AI.DataStructs.Algebraic;
using AI.Statistics;
using AI.Statistics.RandomGenerator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCMCTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            montecarlo = new MCMC_1D(log_d, 3000);
        }

        MCMC_1D montecarlo;

        /// <summary>
        /// Логарифм ненормированного распределения
        /// </summary>
        double log_d(double x) 
        { 
            return -(x*x*x*x-2*x*x)/2; 
        }


       

        private void button1_Click(object sender, EventArgs e)
        {
            double min = -3;
            double max = 3;
            double step = 0.1;

            // Выход MCMC
            Vector doubles = montecarlo.Generate(15000, min, max);
            doubles = doubles[null, null, -1]; // Переворачиваем последовательность (тест срезов)
            // Гистограмма на выходе MCMC
            Statistic stat = new Statistic(doubles);
            var hist = stat.Histogramm(70);

            // Плотность
            Vector x_v = Vector.Seq(min, step, max);
            Vector prob = x_v.Transform(x => Math.Exp(log_d(x)));
            prob /= (prob.Sum()*step);

            // Визуализация
            chartVisual1.Clear();
            chartVisual1.AddArea(hist.X, hist.Y, "Гистограмма MCMC", Color.Black);
            chartVisual1.AddPlot(x_v, prob, "Реальная плотность", Color.Red);
        }
    }
}
