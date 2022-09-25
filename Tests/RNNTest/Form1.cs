using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork;
using AI.ML.DataSets.Base;

namespace RNNTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            net = GetNN();
            nnw = new NeuralNetworkManager(net);
            int n = 166;


            steps = Vector.SeqBeginsWithZero(1, n);

            
            
            t = steps.Transform(r => Math.Pow(Math.Sin(20*r / n), 3));

            x =  steps.Transform(r =>Math.Sin(20 * r / n));

            y = Forward(x);
        }

        Vector steps;
        Vector x;
        Vector y;
        Vector t;
        NNW net;
        NeuralNetworkManager nnw;

        private void Form1_Load(object sender, EventArgs e)
        {
            chartVisual1.PlotBlack(x);
            Visual();
        }

        /// <summary>
        /// Тест обучения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Train();
            y = Forward(x);
            Visual();
        }

        /// <summary>
        /// Создает нейросеть
        /// </summary>
        //static NNW GetNN()
        //{
        //    NNW lstm = new NNW();
        //    lstm.AddNewLayer(new Shape3D(1), new CopyistLayer(4));
        //    lstm.AddNewLayer(new FilterLayer());
        //    lstm.AddNewLayer(new Agregate());
        //    return lstm;
        //}

        /// <summary>
        /// Создает нейросеть
        /// </summary>
        static NNW GetNN()
        {
            NNW lstm = new NNW();
            lstm.AddNewLayer(new Shape3D(1), new FeedForwardLayer(8, new ReLU()));
            lstm.AddNewLayer(new ControllerLResNet(4));
            lstm.AddNewLayer(new FeedForwardLayer(1));
            return lstm;
        }


        /// <summary>
        /// Прямой проход
        /// </summary>
        Vector Forward(Vector inp) 
        {
            NNWGraphCPU graph = new NNWGraphCPU(false);
            Vector outp = new Vector(inp.Count);
            net.ResetState();

            for (int i = 0; i < inp.Count; i++)
                outp[i] = net.Forward(new Vector(inp[i]), graph).Data[0];

            return outp;
        }

        // Обучение сети
        void Train()
        {
            

            nnw.EpochesToPass = 300;
            nnw.LearningRate = 0.001f;
            nnw.GradientClipValue = 900.1f;
            nnw.ValSplit = 0;
            nnw.Loss = new LossMSE();
            nnw.Optimizer = new Adam();

            List<Vector> inp = x.Decomposition().ToList();
            List<Vector> outp = t.Decomposition().ToList();


            nnw.TrainNet(new[] { inp}, new[] {outp});
        }

        void Visual() 
        {
            chartVisual2.Clear();
            chartVisual2.AddPlot(steps, t, "Идеальный сигнал");
            chartVisual2.AddPlot(steps, y, "Выход сети");
        }

        double ph = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            ph += 0.1;
            x = steps.Transform(r => Math.Sin(20 * r / x.Count + ph));
            y = Forward(x);
            chartVisual1.PlotBlack(x);
            Visual();
        }
    }
}
