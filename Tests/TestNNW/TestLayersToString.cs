using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Layers.ComplexLayers;
using AI.ML.NeuralNetwork.CoreNNW.Layers.ConvDeconv;
using System;

namespace TestNNW
{
    public static class TestLayersToString
    {
        public static void Execute()
        {
            FeedComplexLayer feedComplexLayer = new FeedComplexLayer(new Shape3D(), 10, new LinearUnit(), new Random());
            Console.WriteLine(feedComplexLayer.ToString());

            Conv1D conv1D = new Conv1D(new Shape3D(10), 10, 10, new Random());
            Console.WriteLine(conv1D);

            ConvolutionalLayer convolutionLayer = new ConvolutionalLayer(new FilterStruct(), new LinearUnit());
            convolutionLayer.InputShape = new Shape3D(3, 3);
            convolutionLayer.InitWeights(new Random());
            Console.WriteLine(convolutionLayer);

            Flatten flatten = new Flatten();
            Console.WriteLine(flatten);

            MaxPool1D maxPool1D = new MaxPool1D(new Shape3D(), 1);
            Console.WriteLine(maxPool1D);

            MaxPooling maxPooling = new MaxPooling();
            Console.WriteLine(maxPooling);

            ReShape reShape = new ReShape(new Shape3D(), new Shape3D(9));
            Console.WriteLine(reShape);

            UpSampling1D upSampling1D = new UpSampling1D();
            Console.WriteLine(upSampling1D);

            UpSampling2DBicubic upSampling2DBicubic = new UpSampling2DBicubic(new Shape3D());
            Console.WriteLine(upSampling2DBicubic);

            EmbedingLayer embedingLayer = new EmbedingLayer(new[] { new Vector(3.0), new Vector(1.0), new Vector(-9.0) });
            Console.WriteLine(embedingLayer);

            FilterCell filterCell = new FilterCell();
            Console.WriteLine(filterCell);

            FilterLayer filterLayer = new FilterLayer();
            Console.WriteLine(filterLayer);

            GRULayer gRULayer = new GRULayer(10, 10, new Random());
            Console.WriteLine(gRULayer);

            GRURegression gRURegression = new GRURegression(10, 10, new Random());
            Console.WriteLine(gRURegression);

            LSTMLayer lSTMLayer = new LSTMLayer(10, 10, 1.5, new Random());
            Console.WriteLine(lSTMLayer);

            LSTMLayerL1 lSTMLayerL1 = new LSTMLayerL1(10, 10, 1.5, new Random());
            Console.WriteLine(lSTMLayerL1);

            LSTMLayerPeepholeConnection lSTMLayerPeepholeConnection = new LSTMLayerPeepholeConnection(10, 10, 1.5, new Random());
            Console.WriteLine(lSTMLayerPeepholeConnection);

            RNNLayer rNNLayer = new RNNLayer(10, 10, new LinearUnit(), 1.5, new Random());
            Console.WriteLine(rNNLayer);

            ActivationLayer activation = new ActivationLayer(new LinearUnit());
            Console.WriteLine(activation);

            Agregate agregate = new Agregate(new Shape3D());
            Console.WriteLine(agregate);

            BatchReNormalization batchReNormalization = new BatchReNormalization();
            Console.WriteLine(batchReNormalization);

            CopyistLayer copyistLayer = new CopyistLayer(10);
            Console.WriteLine(copyistLayer);

            DropOut dropOut = new DropOut();
            Console.WriteLine(dropOut);

            FeedForwardLayer feedForwardLayer = new FeedForwardLayer(10, 10, new LinearUnit(), new Random());
            Console.WriteLine(feedForwardLayer);

            LinearLayer linearLayer = new LinearLayer(10, 10, 1.5, new Random());
            Console.WriteLine(linearLayer);
        }
    }
}