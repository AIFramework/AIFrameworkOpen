using AI.ComputerVision;
using AI.ComputerVision.ImgFeatureExtractions;
using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.Classifiers;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ONNX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DogCatClassif
{
    public partial class Form1 : Form
    {
        Classifier imCl = new Classifier();
        OpenFileDialog imgLoad = new OpenFileDialog();
        string[] files;
        Bitmap currentBmp;

        List<int> cls = new List<int>();
        List<Bitmap> bitmaps = new List<Bitmap>();

        public Form1()
        {
            InitializeComponent();
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if(browserDialog.ShowDialog() == DialogResult.OK)
            {
                files = Directory.GetFiles(browserDialog.SelectedPath);
                pointer = 0;
                NextImg();
            }
        }

        private void predictBtn_Click(object sender, EventArgs e)
        {
            if(DialogResult.OK == imgLoad.ShowDialog()) 
            {
                Bitmap bitmap = new Bitmap(imgLoad.FileName);
                pictureBox1.Image = bitmap;
                resultTxt.Text = imCl.Classify(bitmap) == 0 ? "Кот" : "Собака";
            }
        }

        //Кот
        private void isCatBtn_Click(object sender, EventArgs e)
        {
            SetClass(0);
            NextImg();
        }

        //Пес
        private void isDogBtn_Click(object sender, EventArgs e)
        {
            SetClass(1);
            NextImg();
        }

        //Обучить
        private void TrainBtn_Click(object sender, EventArgs e)
        {
            imCl.Train(bitmaps, cls);
        }

        int pointer = 0;
        //Показать следующее изображение
        private void NextImg()
        {
            if (pointer < files.Length)
            {
                currentBmp = new Bitmap(files[pointer++]);
                pictureBox1.Image = currentBmp;
            }
            else { MessageBox.Show("Разметка завершена"); }
        }

        // Установить класс
        private void SetClass(int classN)
        {
            bitmaps.Add(currentBmp);
            cls.Add(classN);
        }

        
    }

    /// <summary>
    /// Классификатор
    /// </summary>
    public class Classifier : ObjectClassifierPipeline<Bitmap>
    {
        /// <summary>
        /// Классификатор
        /// </summary>
        public Classifier()
        {
            Normalizer = new ZNormalizer();
            Detector = new NoDetector<Bitmap>();
            Classifier = new KNNCl() { IsParsenMethod = true, K = 2 };
            DataAugmetation = new NoAugmentation<Vector>();
            Vector mean = new Vector(0.485, 0.456, 0.406), std = new Vector(0.229, 0.224, 0.225);
            Extractor = new ImgOnnxExtractor("resnet18-v2-7.onnx", mean, std, 224, 224, LibType.InverseCh);

            //Classifier = new NeuralClassifier(GetNNW())
            //{
            //    EpochesToPass = 100,
            //    LearningRate = 0.001f,
            //    Loss = new CrossEntropyWithSoftmax(),
            //    Optimizer = new Adam(),
            //    ValSplit = 0
            //};
        }


        NNW GetNNW()
        {
            NNW net = new NNW();
            net.AddNewLayer(new Shape3D(1000), new FeedForwardLayer(4, new ReLU(0.1)));
            net.AddNewLayer(new FeedForwardLayer(2, new SoftmaxUnit()));
            return net;
        }

    }

   
}
