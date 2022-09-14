using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using AI.ML.NeuralNetwork.CoreNNW.Events;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints;
using AI.ML.NeuralNetwork.CoreNNW.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Train
{
    /// <summary>
    /// Class for training neural networks
    /// </summary>
    [Serializable]
    public class Trainer : ITrainer, IReporter
    {
        #region Поля и свойства
        private INNWGraph _graph;

        /// <summary>
        /// Regularization coefficient L1, if it is equal to 0 L1 no regularization
        /// </summary>
        public float L1Regularization { get; set; }
        /// <summary>
        /// Regularization coefficient L2, if it is equal to 0 L2 regularization no
        /// </summary>
        public float L2Regularization { get; set; }
        /// <summary>
        /// Learning method SGD, Adam, etc.
        /// </summary>
        public IOptimizer Optimizer { get; set; }
        /// <summary>
        /// Gradient clipping to avoid gradient explosion, default 3
        /// </summary>
        public float GradientClipValue { get; set; } = 3;
        /// <summary>
        /// Обучение нейронной сети information
        /// </summary>
        public TrainInfo Info { get; set; }
        /// <summary>
        /// True if currently training another model
        /// </summary>
        public bool IsBusy { get; private set; } = false;
        /// <summary>
        /// Whether to stop learning when overfitting
        /// </summary>
        public bool DoOverfitStop { get; set; } = false;
        /// <summary>
        /// Whether to make intermediate saves
        /// </summary>
        public bool DoCheckPointSave { get; set; } = false;
        /// <summary>
        /// Checkpoint saver
        /// </summary>
        public ICheckPointSaver CheckPointSaver { get; set; } = new FileCheckPointSaver("checkpoint.nn");
        /// <summary>
        /// The minimum value for metrics that decrease as the quality of the model increases
        /// </summary>
        public double MetricsMin { get; set; } = double.NaN;
        /// <summary>
        /// The maximum value for metrics that increase with increasing model quality
        /// </summary>
        public double MetricsMax { get; set; } = double.NaN;
        /// <summary>
        /// Metric
        /// </summary>
        public Metrics Metrics { get; set; } = Metrics.R2;
        /// <summary>
        /// Report type
        /// </summary>
        public ReportType ReportType { get; set; } = ReportType.ConsoleReport;
        /// <summary>
        /// Function that controls the training of the neural network.If it evaluates to "true", training will be stopped.
        /// The function of the following form "bool Function (INetwork network, TrainInfo info, float bestVal)"
        /// </summary>
        public Func<INetwork, TrainInfo, float, bool> ControlFunction { get; set; } = (net, inf, bestVal) => false;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Creating a trainer for a neural network
        /// </summary> 
        public Trainer()
        {
            Init();
        }
        /// <summary>
        /// Creating a trainer for a neural network
        /// </summary> 
        /// <param name="graph">Граф автоматического дифференцирования</param>
        public Trainer(INNWGraph graph)
        {
            Init(graph);
        }
        /// <summary>
        /// Creating a trainer for a neural network
        /// </summary> 
        /// <param name="graph">Граф автоматического дифференцирования</param>
        /// <param name="trainType">Train type</param>
        /// <param name="optimizer">Optimizer training method</param>
        public Trainer(INNWGraph graph, IOptimizer optimizer)
        {
            Init(graph, optimizer);
        }
        /// <summary>
        /// Creating a trainer for a neural network
        /// </summary> 
        public Trainer(INNWGraph graph = null, IOptimizer optimizer = null, int randSeed = 12)
        {
            Init(graph, optimizer);
        }
        #endregion

        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="epochesToPass">Число эпох</param>
        /// <param name="batchSize">Размер подвыборки</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="data">Выборка</param>
        /// <param name="minLoss">Минимальное значение ошибки</param>
        public void Train(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss = 0.0f)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (IsBusy)
            {
                return;
            }

            if (network.InputShape != data.InputShape)
            {
                throw new InvalidOperationException("Input shapes of network and dataset mismatche");
            }

            if (network.OutputShape != data.OutputShape)
            {
                throw new InvalidOperationException("Output shapes of network and dataset mismatche");
            }

            IsBusy = true; // Бронирование трейнера
            Info = new TrainInfo(); // Информация
            float bestVal = float.MaxValue;
            bool contTr = true, hasValidation = data.HasValidationData;

            TrainingEventArgs trainingEventArgs = new TrainingEventArgs(epochesToPass, batchSize, learningRate, network, data, minLoss);
            TrainingStarted?.Invoke(this, trainingEventArgs);

            Clear(network);
            Report(ReportElementType.TrainingStarted, TrainerMessages.TrainingStarted);

            ModelQualityMetrics modelInfo = new ModelQualityMetrics();

            for (int epoch = 0; epoch < epochesToPass; epoch++)
            {
                PassEpoch(epochesToPass, batchSize, network, data, minLoss, learningRate, out contTr, hasValidation, out modelInfo, epoch); // Запуск одной эпохи обучения
                EpochPassedEventArgs epochPassedEventArgs = new EpochPassedEventArgs
                {
                    TrainingArgs = trainingEventArgs,
                    CurrentEpochNumber = epoch + 1,
                    TrainingLoss = modelInfo.TrainLoss
                };
                if (hasValidation)
                {
                    epochPassedEventArgs.ValidationLoss = modelInfo.ValLoss;
                    epochPassedEventArgs.ValidationMetrics = modelInfo.ValMetric;
                    epochPassedEventArgs.Metrics = (Metrics?)Metrics;
                }
                Report(ReportElementType.EpochPassed, modelInfo.Report);
                EpochPassed?.Invoke(this, epochPassedEventArgs);
                if (bestVal > modelInfo.ValLoss && DoCheckPointSave && hasValidation && CheckPointSaver != null)
                {
                    bestVal = modelInfo.ValLoss;
                    CheckPointSaver.Save(network, bestVal);
                    Report(ReportElementType.CheckPointSaved, string.Format("Saving model using {0}, val. {1:N3}", CheckPointSaver, bestVal));
                }
                // Функция контроля обучения
                if (ControlFunction(network, Info, bestVal) || !contTr)
                {
                    IsBusy = false;
                    Report(ReportElementType.TrainingStopped, TrainerMessages.TrainingStopped);
                    TrainingStopped?.Invoke(this, trainingEventArgs);
                    return;
                }
            }
            Report(ReportElementType.CheckPointSaved, TrainerMessages.TrainingFinished);
            IsBusy = false;
            TrainingFinished?.Invoke(this, trainingEventArgs);
        }
        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="epochesToPass">Число эпох</param>
        /// <param name="batchSize">Размер подвыборки</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="data">Выборка</param>
        /// <param name="minLoss">Минимальное значение ошибки</param>
        /// <param name="cancellationToken">Token for cancelling asynchronous operation</param>
        public async Task TrainAsync(int epochesToPass, int batchSize, float learningRate, INetwork network, IDataSet data, float minLoss = 0.0f, CancellationToken cancellationToken = default)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (IsBusy)
            {
                return;
            }

            if (network.InputShape != data.InputShape)
            {
                throw new InvalidOperationException("Input shapes of network and dataset mismatche");
            }

            if (network.OutputShape != data.OutputShape)
            {
                throw new InvalidOperationException("Output shapes of network and dataset mismatche");
            }

            IsBusy = true; // Бронирование трейнера
            Info = new TrainInfo(); // Информация
            float bestVal = float.MaxValue;
            bool contTr = true, hasValidation = data.HasValidationData;

            TrainingEventArgs trainingEventArgs = new TrainingEventArgs(epochesToPass, batchSize, learningRate, network, data, minLoss);
            TrainingStarted?.Invoke(this, trainingEventArgs);

            Clear(network);
            Report(ReportElementType.TrainingStarted, TrainerMessages.TrainingStarted);

            ModelQualityMetrics modelInfo = new ModelQualityMetrics();

            for (int epoch = 0; epoch < epochesToPass; epoch++)
            {
                await Task.Run(() =>
                {
                    PassEpoch(epochesToPass, batchSize, network, data, minLoss, learningRate, out contTr, hasValidation, out modelInfo, epoch, cancellationToken);
                });
                if (cancellationToken.IsCancellationRequested)
                {
                    Report(ReportElementType.TrainingCancelled, TrainerMessages.TrainingCancelled);
                    IsBusy = false;
                    TrainingCancelled?.Invoke(this, trainingEventArgs);
                    return;
                }
                EpochPassedEventArgs epochPassedEventArgs = new EpochPassedEventArgs
                {
                    TrainingArgs = trainingEventArgs,
                    CurrentEpochNumber = epoch + 1,
                    TrainingLoss = modelInfo.TrainLoss
                };
                if (hasValidation)
                {
                    epochPassedEventArgs.ValidationLoss = modelInfo.ValLoss;
                    epochPassedEventArgs.ValidationMetrics = modelInfo.ValMetric;
                    epochPassedEventArgs.Metrics = (Metrics?)Metrics;
                }
                Report(ReportElementType.EpochPassed, modelInfo.Report);
                EpochPassed?.Invoke(this, epochPassedEventArgs);
                if (bestVal > modelInfo.ValLoss && DoCheckPointSave && hasValidation && CheckPointSaver != null)
                {
                    bestVal = modelInfo.ValLoss;
                    CheckPointSaver.Save(network, bestVal);
                    Report(ReportElementType.CheckPointSaved, string.Format("Saving model using {0}, val. {1:N3}", CheckPointSaver, bestVal));
                }
                // Функция контроля обучения
                if (ControlFunction(network, Info, bestVal) || !contTr)
                {
                    Report(ReportElementType.TrainingStopped, TrainerMessages.TrainingStopped);
                    IsBusy = false;
                    TrainingStopped?.Invoke(this, trainingEventArgs);
                    return;
                }
            }
            Report(ReportElementType.CheckPointSaved, TrainerMessages.TrainingFinished);
            IsBusy = false;
            TrainingFinished?.Invoke(this, trainingEventArgs);
        }

        private void PassEpoch(int trainingEpochs, int batchSize, INetwork network,
            IDataSet data, float minLoss, float lr, out bool contTr, bool hasValidation,
            out ModelQualityMetrics modelInfo, int epoch, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<DataSequence> sequences = data.Training;
            int len = sequences.Count,
                passes = (len % batchSize == 0) ? len / batchSize : (len / batchSize) + 1;

            for (int j = 0; j < passes; j++)
            {
                _graph.Restart(true); // Перезапуск графа
                Random r = new Random(j);
                for (int i = 0; i < batchSize; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        modelInfo = null;
                        contTr = false;
                        return;
                    }

                    StepsPass(sequences[r.Next(len)], network, data.LossFunction, cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    modelInfo = null;
                    contTr = false;
                    return;
                }

                Backward(network, batchSize, lr);

                if (cancellationToken.IsCancellationRequested)
                {
                    modelInfo = null;
                    contTr = false;
                    return;
                }
            }
            modelInfo = CalculationQualityMetrics(network, data, epoch, trainingEpochs, hasValidation); // Измерение качества модели
            contTr = DoContinueTraining(modelInfo.ValMetric, modelInfo.TrainLoss, modelInfo.ValLoss, minLoss, hasValidation); // Стоит ли продолжать обучение
        }

        #region Вспомогательные методы
        // Очищение
        private void Clear(INetwork network)
        {
            foreach (ILayer layer in network.Layers)
            {
                if (layer is ILearningLayer)
                {
                    List<NNValue> param = (layer as ILearningLayer)!.GetParameters();

                    for (int j = 0; j < param.Count; j++)
                    {
                        param[j].InitTrainData();
                    }
                }
            }
        }
        //Инициализация
        private void Init(INNWGraph graph = null, IOptimizer optimizer = null)
        {
            _graph = graph ?? new NNWGraphCPU(false);
            Optimizer = optimizer ?? new Adam();
            L1Regularization = 0;
            L2Regularization = 0;
        }
        //Проход по всем шагам
        private void StepsPass(DataSequence seq, INetwork network, ILoss lossTraining, CancellationToken cancellationToken)
        {
            network.ResetState(); //Сброс состояния рекуррентных сетей
            float loss = 0;

            foreach (DataStep step in seq.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                NNValue output = network.Forward(step.Input, _graph);
                if (step.TargetOutput != null)
                {
                    loss += lossTraining.Measure(output, step.TargetOutput);
                    if (float.IsNaN(loss) || float.IsInfinity(loss))
                    {
                        return;
                    }

                    lossTraining.Backward(output, step.TargetOutput);
                }
            }

          //  Console.WriteLine($"Реальный лосс {loss/ seq.Steps.Count}");

        }
        // Обратный проход по графу и обновление коэффииентов модели
        private void Backward(INetwork network, int batchSize, float learningRate)
        {
            _graph.Backward(); //backprop dw values
            float gradGain = 1.0f / batchSize; //update model params
            Optimizer.UpdateModelParams(network, learningRate, GradientClipValue, L1Regularization, L2Regularization, gradGain);
        }
        //Формирование отчета
        private void Report(ReportElementType type, string message)
        {
            switch (ReportType)
            {
                case ReportType.ConsoleReport:
                    Console.WriteLine(message);
                    break;
                case ReportType.EventReport:
                    ReportElementCreated?.Invoke(this, new ReportElementEventArgs(type, message));
                    break;
            }
        }
        // Данные об ошибке модели на одном наборе
        private float GetLoss(INetwork network, IReadOnlyCollection<DataSequence> dataSequences, ILoss loss)
        {
            float nom = 0, denom = 0;
            int ind = 0;

            foreach (DataSequence seq in dataSequences)
            {
                ind++;
                network.ResetState(); // Сброс состояния при измерении ошибки

                foreach (DataStep step in seq.Steps)
                {
                    NNValue output = network.Forward(step.Input, _graph);
                    if (step.TargetOutput != null)
                    {
                        nom += loss.Measure(output, step.TargetOutput);
                        denom++;
                    }
                }
            }

            return nom / denom;
        }
        // Получение данных качества модели(ошибка обучени, валидации и метрика валидации)
        private ModelQualityMetrics CalculationQualityMetrics(INetwork network, IDataSet dataSet, int epoch, int epochs, bool hasValidation)
        {
            float trLoss, valLoss = 0, metric = 0, testLoss;
            trLoss = GetLoss(network, dataSet.Training, dataSet.LossFunction);
            string report = $"epoch[{epoch + 1}/{epochs}]  tr. loss = {trLoss:N3}";
            Info.TrainLoss.Add(trLoss);

            if (hasValidation)
            {
                valLoss = GetLoss(network, dataSet.Validation, dataSet.LossFunction);
                metric = (float)Tester.Test(_graph, network, dataSet.Validation, Metrics);
                report += $"  val. loss = {valLoss:N3}  val. {Metrics} = {metric:N3}";
                Info.ValidationLoss.Add(valLoss);
            }

            if (dataSet.HasTestingData && epoch + 1 == epochs)
            {
                testLoss = GetLoss(network, dataSet.Testing, dataSet.LossFunction);
                report += $"  test loss  = {testLoss:N3}";
                Info.TestLoss = testLoss;
            }

            return new ModelQualityMetrics()
            {
                TrainLoss = trLoss,
                ValLoss = valLoss,
                ValMetric = metric,
                Report = report
            };
        }
        // Продолжать ли обучение
        private bool DoContinueTraining(float valMetric, float trLoss, float valLoss, float minLoss, bool hasValidation)
        {
            if (hasValidation)
            {
                bool metrMin = !double.IsNaN(MetricsMin) && valMetric <= MetricsMin;
                bool metrMax = !double.IsNaN(MetricsMax) && valMetric >= MetricsMax;
                bool convergence = trLoss < minLoss && valLoss < minLoss;
                bool overfit = false;
                bool isStop;

                if (DoOverfitStop)
                {
                    overfit = OverfitDetector.IsOverfit(Info.ValidationLoss, Info.TrainLoss);
                }

                isStop = metrMax || metrMin || convergence || overfit;

                return !isStop;
            }
            else
            {
                bool convergence = trLoss < minLoss;
                bool isStop = convergence;
                return !isStop;
            }
        }
        #endregion

        #region События
        /// <summary>
        /// Calls when new report element is created
        /// </summary>
        public event EventHandler<ReportElementEventArgs> ReportElementCreated;
        /// <summary>
        /// Calls when epoch is passed
        /// </summary>
        public event EventHandler<EpochPassedEventArgs> EpochPassed;
        /// <summary>
        /// Calls when training is started
        /// </summary>
        public event EventHandler<TrainingEventArgs> TrainingStarted;
        /// <summary>
        /// Calls when training is finished
        /// </summary>
        public event EventHandler<TrainingEventArgs> TrainingFinished;
        /// <summary>
        /// Calls when network training is stopped without finishing all epoches
        /// </summary>
        public event EventHandler<TrainingEventArgs> TrainingStopped;
        /// <summary>
        /// Calls when network training is cancelled by the token
        /// </summary>
        public event EventHandler<TrainingEventArgs> TrainingCancelled;
        #endregion
    }
}