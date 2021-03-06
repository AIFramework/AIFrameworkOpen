using AI.NLP;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TextGenTest
{
    public partial class Form1 : Form
    {
        private readonly HMMFast _hmm = new HMMFast();
        private int _n;

        public Form1()
        {
            InitializeComponent();
        }

        private void nGr_ValueChanged(object sender, EventArgs e)
        {
            SetN();
        }

        private void train_Click(object sender, EventArgs e)
        {
            SetN();
            _hmm.NGram = _n;
            _hmm.Train(richTextBox1.Text, true);
        }

        private void SetN()
        {
            _n = (int)nGr.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] strArr = textBox1.Text.Split(' ');
            List<string> inp = new List<string>();
            int count = _n - strArr.Length - 1;

            for (int i = count; i > 0; i--)
            {
                inp.Add("<start>");
            }

            inp.AddRange(strArr);

            richTextBox2.Text = textBox1.Text + " " + _hmm.Generate(120, inp.ToArray());
        }
    }
}
