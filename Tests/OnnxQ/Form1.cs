using AI.DataStructs.Algebraic;
using AI.ONNX.Classifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnnxQ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Clear();
            brushDr = new SolidBrush(Color.Black);
            brushCl = new SolidBrush(Color.White);
            
            classifier = new GrayScaleClassifier(@"Onnx mnist\model_q.onnx");
        }

        Bitmap bitmap;
        Brush brushDr, brushCl;
        GrayScaleClassifier classifier;

        void Clear() 
        {
            int h = pictureBox1.Height;
            int w = pictureBox1.Width;
            bitmap = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bitmap)) 
            {
                g.Clear(Color.White);
            }
            
            pictureBox1.Image = bitmap;
        }

        Bitmap Crop() 
        {
            List<int> xList = new List<int>();
            List<int> yList = new List<int>();

            int w = bitmap.Width;
            int h = bitmap.Height;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (bitmap.GetPixel(i, j).R < 70)
                    {
                        xList.Add(i);
                        yList.Add(j);
                    }
                }
            }

            try
            {
                int x = xList.Min(), y = yList.Min(), wR = xList.Max() - x, hR = yList.Max() - y;
                Rectangle rectangle = new Rectangle(x, y, wR, hR);

                return bitmap.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }

            catch 
            {
                return bitmap;
            }
        }

        Vector Predict(Bitmap bmp) 
        {
            Bitmap bitmapRec = new Bitmap(bmp, 28, 28);

            Matrix m = 1 - AI.ComputerVision.ImageMatrixConverter.BmpToMatr(bitmapRec) / 255;
            Vector outp = classifier.Classify(m);
            return outp;
            
        }

        // Стереть
        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        // Распознать
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp = Crop();
            var v = Predict(bmp);
            pictureBox1.Image = bmp;
            label1.Text = $"Это цифра: {v.MaxElementIndex()}";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //Рисование
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            if (e.Button == MouseButtons.Left|| e.Button == MouseButtons.Right)
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    Brush brush = e.Button == MouseButtons.Left ? brushDr : brushCl;
                    g.FillEllipse(brush, x - 10, y - 10, 20, 20);
                }

                pictureBox1.Image = bitmap;
                chartVisual1.BarBlack(Predict(bitmap));
            }

            

        }
    }
}
