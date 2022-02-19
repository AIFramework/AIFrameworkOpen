using System.Windows.Forms;

namespace BackendsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TestNumerics parser = new TestNumerics();
            parser.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DCT_test dct = new DCT_test();
            dct.ShowDialog();
        }

        // Децимация
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DecimationTest decimation = new DecimationTest();
            decimation.ShowDialog();
        }
    }
}
