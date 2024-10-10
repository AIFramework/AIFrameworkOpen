using AI.DataStructs.Algebraic;
using System;
using System.Drawing;
using System.Windows.Forms;
using AI.DSP;

namespace BackendsTest
{
    public partial class DecimationTest : Form
    {
        public DecimationTest()
        {
            InitializeComponent();
            t = Vector.SeqBeginsWithZero(0.0001, 1).CutAndZero(4096);
            st = t.Transform(x => Math.Sin(6.28 * 60 * x)>0?1:-1);

            chartVisual1.Clear();
            chartVisual1.AddPlot(t, st, "Сигнал", Color.Red, 1);

        }

        Vector st;
        Vector t;
        int k = 32;

        private void chartVisual1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           var t1 = Vector.SeqBeginsWithZero(0.0001/k, 1).CutAndZero(4096/k);
           
            chartVisual1.Clear();
            chartVisual1.AddPlot(t1, st.Downsampling(k), "Прореживание");
            chartVisual1.AddPlot(t1, st.Decimation(k), "Децимация");
        }
    }
}
