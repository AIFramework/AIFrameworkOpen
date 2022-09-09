using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.DataSets;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork.CoreNNW.Train;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork
{
    /// <summary>
    /// Neural network manager (helper)
    /// </summary>
    [Serializable]
    public class NeuralNetworkManager
    {
        #region Cвойства
        /// <summary>
        /// Neural Network
        /// </summary>
        public INetwork Model { get; set; }
        /// <summary>
        /// Graph of automatic differentiation
        /// </summary>
        public INNWGraph Graph { get; set; } = new NNWGraphCPU(false);
        /// <summary>
        /// Optimizer
        /// </summary>
        public IOptimizer Optimizer { get; set; } = new Adam();
        /// <summary>
        /// Loss function
        /// </summary>
        public ILoss Loss { get; set; } = new LossMSE();
        /// <summary>
        /// Learning rate
        /// </summary>
        public float LearningRate { get; set; } = 0.001f;
        /// <summary>
        /// L1 регуляризация
        /// </summary>
        public float L1Regularization { get; set; } = 0;
        /// <summary>
        /// L2 регуляризация
        /// </summary>
        public float L2Regularization { get; set; } = 0;
        /// <summary>
        /// Ограничение градиентов
        /// </summary>
        public float GradientClipValue { get; set; } = 3;
        /// <summary>
        /// Какая часть выборки идет на валидацию модели
        /// </summary>
        public float ValSplit { get; set; } = 0.1f;
        /// <summary>
        /// Размер батча
        /// </summary>
        public int BatchSize { get; set; } = 1;
        /// <summary>
        /// Number of epochs
        /// </summary>
        public int EpochesToPass { get; set; } = 10;
        /// <summary>
        /// Минимальная ошибка после которой останавливается обучение
        /// </summary>
        public float MinLoss { get; set; } = 0.0f;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Neural network
        /// </summary>
        public NeuralNetworkManager()
        {
            Model = new NNW();
        }
        /// <summary>
        /// Neural network
        /// </summary>
        /// <param name="path">Path to the saved network</param>
        public NeuralNetworkManager(string path)
        {
            Model = NNW.Load(path);
        }
        /// <summary>
        /// Neural network
        /// </summary>
        public NeuralNetworkManager(NNW nnw)
        {
            Model = nnw;
        }
        #endregion

        #region Forward pass
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="input"></param>
        public NNValue Forward(NNValue input)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(input, Graph);
            Graph.IsBackward = graphPrevState;
            return output;
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="algebraicStructure"></param>
        /// <returns></returns>
        public NNValue Forward(IAlgebraicStructure algebraicStructure)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(new NNValue(algebraicStructure), Graph);
            Graph.IsBackward = graphPrevState;
            return output;
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vector ForwardVector(NNValue input)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(input, Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToVector();
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="algebraicStructure"></param>
        /// <returns></returns>
        public Vector ForwardVector(IAlgebraicStructure algebraicStructure)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(new NNValue(algebraicStructure), Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToVector();
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Matrix ForwardMatrix(NNValue input)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(input, Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToMatrix();
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="algebraicStructure"></param>
        /// <returns></returns>
        public Matrix ForwardMatrix(IAlgebraicStructure algebraicStructure)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(new NNValue(algebraicStructure), Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToMatrix();
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Tensor ForwardTensor(NNValue input)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(input, Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToTensor();
        }
        /// <summary>
        /// Neural network forward pass
        /// </summary>
        /// <param name="algebraicStructure"></param>
        /// <returns></returns>
        public Tensor ForwardTensor(IAlgebraicStructure algebraicStructure)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            NNValue output = Model.Forward(new NNValue(algebraicStructure), Graph);
            Graph.IsBackward = graphPrevState;
            return output.ToTensor();
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public NNValue[] ForwardRecurrent(NNValue[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            NNValue[] outputs = new NNValue[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(inputs[i], Graph);
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public NNValue[] ForwardRecurrent(IAlgebraicStructure[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            NNValue[] outputs = new NNValue[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(new NNValue(inputs[i]), Graph);
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Vector[] ForwardRecurrentVector(NNValue[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Vector[] outputs = new Vector[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(inputs[i], Graph).ToVector();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Vector[] ForwardRecurrentVector(IAlgebraicStructure[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Vector[] outputs = new Vector[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(new NNValue(inputs[i]), Graph).ToVector();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Matrix[] ForwardRecurrentMatrix(NNValue[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Matrix[] outputs = new Matrix[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(inputs[i], Graph).ToMatrix();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Matrix[] ForwardRecurrentMatrix(IAlgebraicStructure[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Matrix[] outputs = new Matrix[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(new NNValue(inputs[i]), Graph).ToMatrix();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Tensor[] ForwardRecurrentTensor(NNValue[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Tensor[] outputs = new Tensor[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(inputs[i], Graph).ToTensor();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        /// <summary>
        /// Neural network forward reccurent pass
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public Tensor[] ForwardRecurrentTensor(IAlgebraicStructure[] inputs)
        {
            Model.ResetState();
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;

            Tensor[] outputs = new Tensor[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Model.Forward(new NNValue(inputs[i]), Graph).ToTensor();
            }

            Graph.IsBackward = graphPrevState;
            return outputs;
        }
        #endregion

        #region Обучение
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public TrainInfo TrainNet(NNValue[] inputs, NNValue[] outputs)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetNoRecurrent dsnr = new DataSetNoRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            trainer.Train(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TrainInfo> TrainNetAsync(NNValue[] inputs, NNValue[] outputs, CancellationToken cancellationToken = default)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetNoRecurrent dsnr = new DataSetNoRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            await trainer.TrainAsync(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss, cancellationToken);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public TrainInfo TrainNet(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetNoRecurrent dsnr = new DataSetNoRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            trainer.Train(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <returns></returns>
        public async Task<TrainInfo> TrainNetAsync(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs, CancellationToken cancellationToken = default)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetNoRecurrent dsnr = new DataSetNoRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            await trainer.TrainAsync(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss, cancellationToken);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public TrainInfo TrainNetSignal(Vector[] inputs, Vector[] outputs)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            SignalProcessingDataSet dsnr = new SignalProcessingDataSet(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            trainer.Train(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network async training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public async Task<TrainInfo> TrainNetSignalAsync(Vector[] inputs, Vector[] outputs, CancellationToken cancellationToken = default)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            SignalProcessingDataSet dsnr = new SignalProcessingDataSet(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            await trainer.TrainAsync(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss, cancellationToken);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public TrainInfo TrainNet(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetRecurrent dsnr = new DataSetRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            trainer.Train(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network async training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public async Task<TrainInfo> TrainNetAsync(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs, CancellationToken cancellationToken = default)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetRecurrent dsnr = new DataSetRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            await trainer.TrainAsync(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss, cancellationToken);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public TrainInfo TrainNet(IReadOnlyList<IAlgebraicStructure>[] inputs, IReadOnlyList<IAlgebraicStructure>[] outputs)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetRecurrent dsnr = new DataSetRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            trainer.Train(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        /// <summary>
        /// Neural network async training
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        public async Task<TrainInfo> TrainNetAsync(IReadOnlyList<IAlgebraicStructure>[] inputs, IReadOnlyList<IAlgebraicStructure>[] outputs, CancellationToken cancellationToken = default)
        {
            bool graphPrevState = Graph.IsBackward;
            Graph.IsBackward = false;
            DataSetRecurrent dsnr = new DataSetRecurrent(inputs, outputs, Loss, ValSplit);
            Trainer trainer = new Trainer(Graph, Optimizer)
            {
                L1Regularization = L1Regularization,
                L2Regularization = L2Regularization,
                GradientClipValue = GradientClipValue,
            };
            await trainer.TrainAsync(EpochesToPass, BatchSize, LearningRate, Model, dsnr, MinLoss, cancellationToken);
            Graph.IsBackward = graphPrevState;
            return trainer.Info;
        }
        #endregion
    }
}