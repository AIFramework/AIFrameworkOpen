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
            steps = Vector.SeqBeginsWithZero(1, 50);


            x =  new Vector(50) { [0]=1, [37] = 0};
            
            t = steps.Transform(r => Math.Pow(Math.Sin(r / 6), 3));

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
        static NNW GetNN()
        {
            NNW lstm = new NNW();
            lstm.AddNewLayer(new Shape3D(1), new FeedForwardLayer(3, new ReLU(0.1)));
            lstm.AddNewLayer(new GRURegression(10));
            lstm.AddNewLayer(new FeedForwardLayer(1, new LinearUnit()));
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
            nnw.GradientClipValue = 0.1f;
            nnw.ValSplit = 0;
            nnw.Loss = new LossMSE();
            nnw.Optimizer = new RMSProp();

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

        
    }
}
