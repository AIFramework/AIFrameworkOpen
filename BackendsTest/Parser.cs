using System;
using System.Windows.Forms;
using AI.BackEnds.MathLibs.MathNet.Numerics.Statistics;
using AI.DataStructs.Algebraic;

namespace BackendsTest
{
    public partial class TestNumerics : Form
    {
        public TestNumerics()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vector x = Vector.Seq(-10, 0.1, 10);
            Vector y = x.Transform(Math.Sin) + 4;
            MessageBox.Show(""+ArrayStatistics.HarmonicMean(y));

            chartVisual1.PlotBlack(x, y);
            
        }

        private void chartVisual1_Load(object sender, EventArgs e)
        {

        }
    }
}
