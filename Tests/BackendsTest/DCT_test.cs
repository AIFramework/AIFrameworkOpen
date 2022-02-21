using AI.BackEnds.DSP.NWaves.Transforms;
using AI.DataStructs.Algebraic;
using AI.DSP.DSPCore;
using System;
using System.Windows.Forms;

namespace BackendsTest
{
    public partial class DCT_test : Form
    {
        public DCT_test()
        {
            InitializeComponent();


            t = Vector.SeqBeginsWithZero(0.0001, 1).CutAndZero(4096);

            st = t.Transform(x=>Math.Sin(6.28*160*x));
        }

        Vector st;
        Vector t;

        private void DCT_test_Load(object sender, EventArgs e)
        {
            
        }

        // Сигнал
        private void button1_Click(object sender, EventArgs e)
        {
            chartVisual1.PlotBlack(t, st);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dct2 dct2 = new Dct2(st.Count);
            float[] input = (float[])st;
            float[] dctArr = new float[input.Length];
            dct2.Direct(input, dctArr);
            chartVisual1.PlotBlack(dctArr);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DCT dct = new DCT(st.Count, st.Count/10);
            chartVisual1.PlotBlack(dct.DirectDCTNorm(st));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DCT2NWaveWrapper dct = new DCT2NWaveWrapper(st.Count);
            chartVisual1.PlotBlack(dct.DirectDCTNorm(st));
        }
    }
}
