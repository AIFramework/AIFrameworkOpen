using System;
using System.Collections.Generic;
using System.Linq;
using Many2Many = AI.ML.DataSets.Base.Many2ManyVectorClassifierDataset;
using M2MSample = AI.ML.DataSets.Base.Many2ManyVectorClassifier;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System.IO;
using AI.Extensions;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.DataPrepaire.Tokenizers.TextTokenizers;
using System.Reflection;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;

namespace ControllerTest
{
    class Program
    {
        static void Main(string[] args)
        {

            int n_tr = 1;

            WordTokenizer wordTokenizer = new WordTokenizer("cat.txt");

            string[] texts = File.ReadAllText("cat.txt").Split('.');
            List<int[]> data = new List<int[]>(n_tr);


            for (int i = 0; i < texts.Length&&i< n_tr; i++)
                data.Add(wordTokenizer.Encode(
                    "<s> "+ texts[i]+" <e>" ));
            

            Ex2(data.ToArray(), wordTokenizer);
        }

      
        static void Train(NeuralNetworkManager nnw, Many2Many dataset, int count)
        {
            nnw.EpochesToPass = 235;
            nnw.LearningRate = 0.001f;
            nnw.GradientClipValue = 0.21f;
            nnw.ValSplit = 0;
            nnw.Loss = new CrossEntropyWithSoftmax();
            nnw.Optimizer = new RMSProp();
           
            nnw.TrainNet(dataset.GetFeatures(), dataset.GetVectorLabels(count));
        }


        // Emb --------------------------------
        #region Embeding
        private static void Ex2(int[][] texts, WordTokenizer tokenizer)
        {
        
            Many2Many dataset = DataSetEmb(texts);
            NNW lstm = GetNNWEmb(tokenizer.DictLen);
           // NNW lstm = NNW.Load("pretrain1.net");//
            NeuralNetworkManager neuralNetwork = new NeuralNetworkManager(lstm);
            Train(neuralNetwork, dataset, tokenizer.DictLen);

            lstm.Save("pretrain1.net");
            while (true)
            {
                Console.WriteLine("\nВведите фразу: ");
                string inp = "<s> " + Console.ReadLine().ToLower();
                Console.WriteLine("Генерация нейронкой: ");
                var outp = PredictEmb(tokenizer.Encode(inp), lstm, tokenizer.EndToken);

                Console.WriteLine($"{tokenizer.DecodeObj(outp.Item1)}, \t уверенность: {outp.Item2}");
            }
        }
        
        /// <summary>
        /// Создание датасета
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static Many2Many DataSetEmb(int[][] data)
        {
            Many2Many dataset = new Many2Many();
            for (int i = 0; i < data.Length; i++)
            {
                List<Vector> inp = new List<Vector>();
                List<int> outp = new List<int>();

                for (int j = 0; j < data[i].Length - 1; j++)
                {
                    inp.Add(new Vector((double)data[i][j])); //Добавление индекса
                    outp.Add(data[i][j + 1]);
                }

                dataset.Add(new M2MSample(outp, inp));
            }

            return dataset;
        }


        static NNW GetNNWEmb(int inps)
        {
            NNW lstm = new NNW();
            lstm.AddNewLayer(new Shape3D(1), new EmbedingLayer(inps, 105));
            lstm.AddNewLayer(new ControllerL(23));
            lstm.AddNewLayer(new FeedForwardLayer(inps, new SoftmaxUnit()));
            return lstm;
        }
        // Возвращает спрогнозированную строку(токены) и вероятность
        static Tuple<int[], double> PredictEmb(int[] start, NNW nNW, int end_token)
        {
            Random rnd = new Random();
            List<int> listChar = new List<int>();
            nNW.ResetState();
            Vector st;
            GraphCPU graph = new GraphCPU();

            for (int i = 0; i < start.Length - 1; i++)
            {
                st = new Vector((double)start[i]);
                nNW.Forward(new NNValue(st), graph);
            }

            st = new Vector((double)start[start.Length - 1]);
            Vector outp = nNW.Forward(new NNValue(st), graph).ToVector();

            double prob;// вероятности на каждом шаге

            int ind = AI.Statistics.RandomItemSelection<int>.GetIndex(outp * outp, rnd);
            listChar.Add(ind);
            int count = 0;

            prob = outp[ind];

            while (ind != end_token && count < 15)
            {
                outp = nNW.Forward(new NNValue(new Vector((double)ind)), graph).ToVector();
                ind = AI.Statistics.RandomItemSelection<int>.GetIndex(outp * outp, rnd);
                prob *= outp[ind];
                listChar.Add(ind);
                count++;
            }


            return new Tuple<int[], double>(listChar.ToArray(), prob);
        }
        #endregion
    }
}

