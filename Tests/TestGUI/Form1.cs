using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.DataSets;
using AI.ML.NeuralNetwork.CoreNNW.Events;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Train;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TestGUI
{
    public partial class TestGui : Form
    {
        private readonly GraphCPU _graph = new GraphCPU();
        private readonly NNW _net = new NNW();
        private Trainer _tr;
        private static DataSetNoRecurrent s_dataSet;
        private static CancellationTokenSource s_cancellationTokenSource = new CancellationTokenSource();

        static TestGui()
        {
            Vector inp1 = new double[] { 0.9, 0.1, 0.9, 0.1, 0.9, 0.1, 0.9, 0.1 };
            Vector inp2 = new double[] { 0.1, 0.9, 0.1, 0.9, 0.1, 0.9, 0.1, 0.9 };

            Vector outp1 = new double[] { 0.23, -0.1, 0.6 };
            Vector outp2 = new double[] { -0.9, 0.8, 0.4 };

            s_dataSet = new DataSetNoRecurrent(new[] { inp1, inp2 }, new[] { outp1, outp2 }, new LossMSE());
        }

        public TestGui()
        {
            InitializeComponent();

            _tr = new Trainer(_graph);
            _tr.ReportType = ReportType.EventReport;
            _tr.ReportElementCreated += Tr_ReportElementCreated;

            _net.AddNewLayer(new Shape3D(8), new FeedForwardLayer(2130, new ReLU(0.1)));
            _net.AddNewLayer(new FeedForwardLayer(130, new ReLU(0.1)));
            _net.AddNewLayer(new FeedForwardLayer(3, new LinearUnit()));
            textLog.Text = _net.ToString() + Environment.NewLine + Environment.NewLine;

        }

        private void Tr_ReportElementCreated(object sender, ReportElementEventArgs e)
        {
            textLog.Text += e.Message + Environment.NewLine;
        }

        private async void startBtn_Click(object sender, EventArgs e)
        {
            await _tr.TrainAsync(30, 1, 0.001f, _net, s_dataSet, cancellationToken: s_cancellationTokenSource.Token);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            s_cancellationTokenSource.Cancel();
            s_cancellationTokenSource.Dispose();
            s_cancellationTokenSource = new CancellationTokenSource();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FImg fImg = new FImg();
            fImg.ShowDialog();
        }
    }
}