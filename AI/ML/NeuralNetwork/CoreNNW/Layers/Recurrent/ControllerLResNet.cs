using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Long short-term memory(LSTM) layer with command line
    /// </summary>
    [Serializable]
    public class ControllerLResNet : ILearningLayer, IRecurrentLayer
    {
        /// <summary>
        /// Adding to the denominator
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation Function { get; set; }
        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => 3;

        #region поля
        private NNValue outpGate;
        private NNValue forgetG;
        private NNValue writeG;
        private NNValue inpG;
        private NNValue commandGet;

        private NNValue forgetBias;
        private NNValue outputBias;
        private NNValue cellWriteBias;
        private NNValue inputBias;

        private NNValue _hiddenContext;
        private NNValue _cellContext;

        private readonly IActivation forgetGateActivation = new SigmoidUnit();
        private readonly IActivation outputGateActivation = new SigmoidUnit();
        private readonly IActivation inpGateActivation = new SigmoidUnit();
        private readonly IActivation cellInputActivation = new TanhUnit();
        private readonly IActivation cellOutputActivation = new TanhUnit();
        #endregion

        /// <summary>
        /// Long short-term memory(LSTM) layer with command line
        /// </summary>
        /// <param name="inputDimension">Input dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public ControllerLResNet(int inputDimension, double initParamsStdDev, Random rnd)
        {
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(inputDimension);
            Init(InputShape, inputDimension, initParamsStdDev, rnd);
            ResetState();
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer with command line
        /// </summary>
        /// <param name="inputShape">Input dimension</param>
        /// <param name="outputDimension">Output dimension</param>
        /// <param name="initParamsStdDev">Standard deviation</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public ControllerLResNet(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            Init(inputShape, outputDimension, initParamsStdDev, rnd);
            ResetState(); // Запуск НС
        }
        /// <summary>
        /// Long short-term memory(LSTM) layer with command line
        /// </summary>
        /// <param name="outputDimension">Output dimension</param>
        public ControllerLResNet(int outputDimension)
        {
            OutputShape = new Shape3D(outputDimension);
        }

        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            NNValue conc = g.ConcatinateVectors(input, _hiddenContext);
            NNValue commandLine = g.MulMV(commandGet, conc);


            //forget gate
            NNValue fSum = g.AdamarMul(forgetG, commandLine);
            NNValue forgetGate = g.Activate(forgetGateActivation, g.Add(fSum, forgetBias));

            //input gate
            NNValue iSum = g.AdamarMul(forgetG, commandLine);
            NNValue i = g.Activate(cellInputActivation, g.Add(iSum, inputBias));

            //output gate
            NNValue oSum = g.AdamarMul(outpGate, commandLine);
            NNValue outputGate = g.Activate(outputGateActivation, g.Add(oSum, outputBias));

            //write operation on cells
            NNValue cSum = g.MulMV(writeG, conc);
            NNValue cellInput = g.Activate(cellInputActivation, g.Add(cSum, cellWriteBias));

            //compute new cell activation
            NNValue retainCell = g.AdamarMul(forgetGate, _cellContext);
            NNValue writeCell = g.AdamarMul(i, cellInput);
            NNValue cellAct = g.Add(retainCell, writeCell);

            //compute hidden state as gated, saturated cell activations
            NNValue output = g.Add(
                g.AdamarMul(outputGate, 
                g.Activate(cellOutputActivation, cellAct)), input);

            //rollover activations for next iteration
            _hiddenContext = output;
            _cellContext = cellAct;

            return output;
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            _hiddenContext = new NNValue(OutputShape.Height);
            _cellContext = new NNValue(OutputShape.Height);
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>
            {
                forgetG,
                outpGate,
                writeG,
                forgetBias,
                outputBias,
                cellWriteBias,
                commandGet,
                inputBias,
                inpG
            };
            return result;
        }
        /// <summary>
        /// Generating weight coefficients of a neural network layer
        /// </summary>
        /// <param name="inpShape"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public void Generate(Shape3D inpShape, Random random)
        {
            InputShape = inpShape;
            double std = 1.0 / Math.Sqrt(OutputShape.Count * InputShape.Count);
            Init(inpShape, OutputShape.Height, std, random);
        }
        private void Init(Shape3D inputShape, int outputDimension, double initParamsStdDev, Random rnd)
        {
            //set forget bias to 1.0, as described here: http://jmlr.org/proceedings/papers/v37/jozefowicz15.pdf
            int inputDimension = inputShape.Height;
            int con = inputDimension + outputDimension;
            InputShape = new Shape3D(inputDimension);
            OutputShape = new Shape3D(outputDimension);

            commandGet = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);
            outpGate = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            forgetG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            inpG = NNValue.Random(outputDimension, 1, initParamsStdDev, rnd);
            writeG = NNValue.Random(outputDimension, con, initParamsStdDev, rnd);

            forgetBias = new NNValue(outputDimension);
            outputBias = new NNValue(outputDimension);
            cellWriteBias = new NNValue(outputDimension);
            inputBias = new NNValue(outputDimension);

            ResetState();
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
            outpGate.OnlyUse();
            forgetG.OnlyUse();
            writeG.OnlyUse();
            commandGet.OnlyUse();
            inpG.OnlyUse();

            forgetBias.OnlyUse();
            outputBias.OnlyUse();
            cellWriteBias.OnlyUse();
            inputBias.OnlyUse();
        }

        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("LstmLayerL1     \t|inp: {0} |outp: {1} |Non lin. activate: {3} |TrainParams: {2}", InputShape, OutputShape, TrainableParameters, "sigm/tanh");
        }

        public void InitWeights(Random random)
        {
            double std = 1.0 / Math.Sqrt(OutputShape.Volume * InputShape.Volume);
            Init(InputShape, OutputShape.Height, std, random);
        }
    }
}

