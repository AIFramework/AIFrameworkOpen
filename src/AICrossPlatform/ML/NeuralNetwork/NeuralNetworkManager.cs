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
    /// Менеджер нейронных сетей, класс для облегчения работы с нейросетями 
    /// </summary>
    [Serializable]
    public class NeuralNetworkManager
    {
        #region Cвойства
        /// <summary>
        /// Нейросеть
        /// </summary>
        public INetwork Model { get; set; }
        /// <summary>
        /// Граф атоматического дифференцирования
        /// </summary>
        public INNWGraph Graph { get; set; } = new NNWGraphCPU(false);
        /// <summary>
        /// Метод оптимизации
        /// </summary>
        public IOptimizer Optimizer { get; set; } = new Adam();
        /// <summary>
        /// Функция ошибки
        /// </summary>
        public ILoss Loss { get; set; } = new LossMSE();
        /// <summary>
        /// Скорость обучения
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
        /// Число эпох
        /// </summary>
        public int EpochesToPass { get; set; } = 10;
        /// <summary>
        /// Минимальная ошибка после которой останавливается обучение
        /// </summary>
        public float MinLoss { get; set; } = 0.0f;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Менеджер нейронных сетей, класс для облегчения работы с нейросетями 
        /// </summary>
        public NeuralNetworkManager()
        {
            Model = new NNW();
        }
        /// <summary>
        /// Менеджер нейронных сетей, класс для облегчения работы с нейросетями 
        /// </summary>
        /// <param name="path">Путь до сохраненной нейронной сети</param>
        public NeuralNetworkManager(string path)
        {
            Model = NNW.Load(path);
        }
        /// <summary>
        /// Менеджер нейронных сетей, класс для облегчения работы с нейросетями 
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
        /// Обучение нейронной сети
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
        /// Обучение нейронной сети
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
        /// Обучение нерекуррентной сети в асинхронном режиме
        /// </summary>
        /// <param name="inputs">Данные входа</param>
        /// <param name="outputs">Целевые переменные</param>
        /// <param name="cancellationToken">Окончание обучения </param>
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
        /// Обучение нейронной сети для работы с сигналами
        /// </summary>
        /// <param name="inputs">Массив входных одномерных сигналов</param>
        /// <param name="outputs">Массив входных одномерных сигналов(Целевые значения)</param>
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
        /// Асинхронное обучение нейронной сети для работы с сигналами
        /// </summary>
        /// <param name="inputs">Массив входных одномерных сигналов</param>
        /// <param name="outputs">Массив входных одномерных сигналов(Целевые значения)</param>
        /// <param name="cancellationToken">Окончание обучения</param>
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
        /// Обучение рекуррентной нейронной сети для работы с сигналами
        /// </summary>
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
        /// Асинхронное обучение рекуррентной нейронной сети для работы с сигналами
        /// </summary>
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
        /// Обучение нейронной сети
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
        /// Асинхронное обучение рекуррентной нейронной сети для работы с сигналами
        /// </summary>
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