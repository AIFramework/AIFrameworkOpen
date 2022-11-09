using AI;
using AI.Charts.Control;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpectrumAnalyzer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            signal = AI.DSP.DSPCore.Signal.LFM(df, f, sr, 10);
            spectrumWelchAnalyzer1.FFTBlock = 4096;
            spectrumWelchAnalyzer1.SR = sr;
            spectrumWelchAnalyzer1.WelchPSDTypeData = AI.DSP.Analyse.WelchPSDType.Db;
        }

        Vector signal;
        int f = 500;
        int df = 500;
        int sr = 4096;

        private void button1_Click(object sender, EventArgs e)
        {
            spectrumWelchAnalyzer1.Analyze(signal);
        }
    }

    
}
