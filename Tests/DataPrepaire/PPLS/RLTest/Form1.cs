using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines;
using AI.DataPrepaire.Pipelines.RL;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.Classifiers;
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

namespace RLTest
{
    public partial class Form1 : Form
    {
        List<Vector> xList = new List<Vector>();
        List<int> yList = new List<int>();
        RLEnv rL = new RLEnv();
        Vector scores = new Vector();
        Vector x = new Vector();
        int allCount = 0;
        int lenM = 40; // Длинна матча

        public Form1()
        {
            InitializeComponent();

            // Создаем выборку
            Random random = new Random(1);

            //Класс 1
            Vector cl1 = new Vector(2, 2, 8, 11, -2);
            //Класс 2
            Vector cl2 = new Vector(1, 6, 8, 11, -2);

            //Выборка
            for (int i = 0; i < lenM; i++)
            {
                xList.Add(cl1 + 2 * AI.Statistics.Statistic.RandNorm(5, random));
                xList.Add(cl2 + 2 * AI.Statistics.Statistic.RandNorm(5, random));
                yList.Add(0);
                yList.Add(1);
            }
        }




        // Сыграть несколько партий
        private void button1_Click(object sender, EventArgs e)
        {
            double mScore = 0;
            int count = 15;

            for (int i = 0; i < count; i++)
            {
                mScore += RunMatch();
            }

            int[] cls = rL.RewardData[rL.RewardData.Count-1].Actions;

            ShowScatter(cls); // Показываем классификацию на случайной партии
            rL.Train(3);

            if (rL.RewardData.Count > 1500) rL.ClearData();

            mScore /= count;
            x.Add(allCount);
            scores.Add((mScore - lenM) / lenM);

            chartVisual1.PlotBlack(x, scores);
            allCount += count;
        }


        /// <summary>
        /// Посчитать оценку сети
        /// </summary>
        public double GetScore(int[] pred, int[] trueData) 
        {
            double score = 0;
            for (int i = 0; i < pred.Length; i++)
                if(pred[i] == trueData[i]) score++;

            return score;
        }

        // Сыграть один матч
        public double RunMatch() 
        {
            int[] pred = new int[yList.Count];

            for (int i = 0; i < xList.Count; i++)
            {
                pred[i] = rL.GetAction(xList[i], 0, 1.8);
            }

            double score = GetScore(pred, yList.ToArray());

            rL.SetReward(score);

            return score;
        }


        /// <summary>
        /// Показывает распределение классов
        /// </summary>
        /// <param name="marks"></param>
        public void ShowScatter(int[] marks) 
        {
            Vector cl1X = new Vector();
            Vector cl1Y = new Vector();

            Vector cl2X = new Vector();
            Vector cl2Y = new Vector();
            

            for (int i = 0; i < marks.Length-1; i++)
            {
                if (marks[i] == 0)
                {
                    cl1X.Add(xList[i][0]);
                    cl1Y.Add(xList[i][1]);
                }
                else
                {
                    cl2X.Add(xList[i][0]);
                    cl2Y.Add(xList[i][1]);
                }
            }

            chartVisual2.Clear();
            chartVisual2.AddScatter(cl1X, cl1Y, "Класс 1", Color.Blue);
            chartVisual2.AddScatter(cl2X, cl2Y, "Класс 2", Color.Gray);
        }

    }


    // Инваремент для RL
    public class RLEnv : RLWithoutCriticPipeline<Vector>
    {
        public RLEnv()
        {
            Actor = new Agent();
        }
    }


    /// <summary>
    /// Интеллектуальный агент
    /// </summary>
    public class Agent : ObjectClassifierPipeline<Vector>
    {
        /// <summary>
        /// Интеллектуальный агент
        /// </summary>
        public Agent()
        {
            Normalizer = new ZNormalizer();
            Detector = new NoDetector<Vector>();
            Extractor = new NoExtractor();
            DataAugmetation = new NoAugmentation<Vector>();

            Classifier = new NeuralClassifier(GetNNW())
            {
                EpochesToPass = 1,
                LearningRate = 0.001f,
                Loss = new CrossEntropyWithSoftmax(),
                Optimizer = new Adam(),
                ValSplit = 0
            };
        }

        /// <summary>
        /// Неройнная сеть
        /// </summary>
        NNW GetNNW()
        {
            NNW net = new NNW();
            net.AddNewLayer(new Shape3D(5), new FeedForwardLayer(15, new ReLU(0.1)));
            net.AddNewLayer(new FeedForwardLayer(2, new SoftmaxUnit()));
            return net;
        }
    }
}
