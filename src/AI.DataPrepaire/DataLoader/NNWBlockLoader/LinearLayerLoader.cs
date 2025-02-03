using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AI.DataPrepaire.DataLoader.NNWBlockLoader
{
    /// <summary>
    /// Загрузчик линейного слоя
    /// </summary>
    [Serializable]
    public class LinearLayerLoader : INNWBlockV2V
    {
        /// <summary>
        /// Веса нейронов
        /// </summary>
        [JsonPropertyName("w")]
        public double[][] Neurons { get; set; }

        /// <summary>
        /// Веса смещения
        /// </summary>
        [JsonPropertyName("bias")]
        public double[] Bias { get; set; }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        public Vector Forward(double[] input)
        {
            double[] output = new double[Neurons.Length];
            int lenInp = Neurons[0].Length;

            Parallel.For(0, Neurons.Length, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, neuronNum =>
            {
                output[neuronNum] = 0;

                for (int inpNum = 0; inpNum < lenInp; inpNum++)
                    output[neuronNum] += Neurons[neuronNum][inpNum] * input[inpNum];

                output[neuronNum] += Bias[neuronNum];

            });

            return output;
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        public Vector Forward(Vector input)
        {
            return Forward((double[])input);
        }

        /// <summary>
        /// Перевод структуры в обучаемый слой AIFramework
        /// </summary>
        /// <returns></returns>
        public ILayer ToLayer()
        {
            Random random = new Random();
            FeedForwardLayer linearLayer = new FeedForwardLayer(Neurons[0].Length, Neurons.Length, new LinearUnit(), random);
            var W = new NNValue(height: Neurons.Length, width: Neurons[0].Length);


            for (int h = 0; h < Neurons.Length; h++)
                for (int w = 0; w < Neurons[0].Length; w++)
                    W[h, w] = (float)Neurons[h][w];

            linearLayer.Bias = new NNValue((Vector)Bias);
            linearLayer.W = W;


            return linearLayer;
        }

        /// <summary>
        /// Загрузка из JSON
        /// </summary>
        public static LinearLayerLoader LoadFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            var tokens = JsonSerializer.Deserialize<LinearLayerLoader>(json);
            return tokens;
        }

        /// <summary>
        /// Загрузка из бинарного файла
        /// </summary>
        public static LinearLayerLoader LoadFromBinary(string path)
        {
            return BinarySerializer.Load<LinearLayerLoader>(path);
        }

        /// <summary>
        /// Сохранение в бинарный файл
        /// </summary>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
    }
}
