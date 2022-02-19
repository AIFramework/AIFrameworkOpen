using System;
using System.Windows.Forms;
using AI.ComputerVision;
using AI.ComputerVision.SpatialFilters;
using AI.DataStructs.Algebraic;

namespace TestGUI
{
    public partial class FImg : Form
    {
        public FImg()
        {
            InitializeComponent();
        }

        string path = "";
        
        // Загрузка
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                path = openFile.FileName;
                var m = ImageMatrixConverter.LoadAsMatrix(path);
                pictureBox1.Image = ImageMatrixConverter.ToBitmap(m);
            }
        }

        // Набросок карандашом
        private void button2_Click(object sender, EventArgs e)
        {
            var m = ImageMatrixConverter.LoadAsMatrix(path);

            var f = new Matrix(3, 3) - 1.0/9.0;
            f[1, 1] = 8 / 9.0;

            m = new CustomFilter(f).Filtration(m);
            var std = m.Std();
            m = m/(3*std);

            pictureBox1.Image = ImageMatrixConverter.ToBitmap(255*(1-m));
        }

        // Вертикальные линии
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new HLine().Filtration(path);
        }

        // Горизонтальные линии
        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new WLine().Filtration(path);
        }

        // Резкость
        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Sharpness().Filtration(path);
        }

        // Сглаживание (АЧХ |sin(x)/x|)
        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Smoothing().Filtration(path);
        }

        // СКО фильтр
        private void button7_Click(object sender, EventArgs e)
        {
            var m = ImageMatrixConverter.LoadAsMatrix(path);

            var f = new Matrix(3, 3) + 1.0;
            m = ImgFilters.StdFilter(m, f);


            pictureBox1.Image = ImageMatrixConverter.ToBitmap(255-m);
        }

        // Медианный фильтр
        private void button8_Click(object sender, EventArgs e)
        {
            var m = ImageMatrixConverter.LoadAsMatrix(path);

            var f = new Matrix(3, 3) + 1.0;
            m = ImgFilters.MedianFilter(m, f);


            pictureBox1.Image = ImageMatrixConverter.ToBitmap(m);
        }
    }
}
