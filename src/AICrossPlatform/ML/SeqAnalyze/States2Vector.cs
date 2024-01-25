using AI.DataStructs.Algebraic;
using AI.DataStructs.Data;
using AI.Dog.Tools;
using AI.ML.Regression;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.ML.SeqAnalyze
{
    /// <summary>
    /// Регрессор, превращающий состояния в вектор
    /// </summary>
    [Serializable]
    public class States2Vector : IMultyRegression<int[]>
    {

        /// <summary>
        /// Число правил
        /// </summary>
        public int CountRules => _data_marks.Count;

        /// <summary>
        /// Размерность вектора ассоциаций
        /// </summary>
        public int VectorDimention => _vectorDimention;

        /// <summary>
        /// Установка / чтение правила
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Vector this[int[] state]
        {
            get
            {
                var stStr = state;
                return _data_marks.ContainsKey(stStr)? _data_marks[stStr].TargetValue : null;
            }

            set
            {
                if (_data_marks.ContainsKey(state))
                {
                    _data_marks[state].TargetValue = value;
                    _data_marks[state].CountActiv = 1;
                }
                else
                {
                    ValueDictRegressor valueDict = new ValueDictRegressor()
                    {
                        CountActiv = 1,
                        NGram = (short)state.Length,
                        TargetValue = value
                    };

                    _data_marks.Add(state, valueDict);
                }
                _data_marks[state].GetKImportance();

            }
        }

        /// <summary>
        /// Максимальный размер контекста
        /// </summary>
        public int MaxNGramm;
        /// <summary>
        /// Значение до которого должна дойти интегральное значение важности после сортировки
        /// </summary>
        public double TopP;
        // Размерность вектора
        private int _vectorDimention = -1;
        // Данные о векторах и состояниях
        private Dictionary<int[], ValueDictRegressor> _data_marks; // знания модели
        // Среднее и СКО выходов
        private Vector _y_mean, _y_std;

        /// <summary>
        /// Активационная функция
        /// </summary>
        public Func<Vector, Vector> Activation { get; set; }

        /// <summary>
        /// Регрессор, превращающий состояния в вектор
        /// </summary>
        public States2Vector(int max_n_gram, double top_p)
        {
            MaxNGramm = max_n_gram;
            TopP = top_p;
            Activation = Lin;
        }

        /// <summary>
        /// Прогнозирование вектора по массиву состояний
        /// </summary>
        /// <param name="data">Масив состояний</param>
        public Vector Predict(int[] data)
        {
            return PredictWithListCount(data).Item1;
        }

        /// <summary>
        /// Прогнозирование вектора по массиву состояний
        /// </summary>
        /// <param name="inp">Масив состояний</param>
        public Tuple<Vector, List<ValueDictRegressor>, int> PredictWithListCount(int[] inp)
        {
            int[] data = (int[])inp.Clone();
            List<ValueDictRegressor> list = new List<ValueDictRegressor>();

            if (VectorDimention == -1)
                throw new Exception("Обучите модель");

            Vector outp = new Vector(VectorDimention);
            double sumW = 0; // Число срабатываний
            int n = 0;

            // Проход по всем длиннам буфера
            for (int i = MaxNGramm; i > 0; i--)
            {
                RingBuffer<int> ringBuffer = new RingBuffer<int>(i);
                List<int[]> used = new List<int[]>();

                // Проход по последовательности
                for (int j = 0; j < data.Length; j++)
                {
                    ringBuffer.AddElement(data[j]);
                    var key = CopyKey(ringBuffer.Data);


                    if (_data_marks.ContainsKey(key))
                    {
                        used.Add(ringBuffer.Data); // Добавление n-gramm
                        var dat = _data_marks[key];
                        outp += dat.GetVector();
                        sumW += dat.GetKImportance();
                        list.Add(dat);
                        n++;
                    }
                }

                // Удаление групп
                foreach (var item in used)
                    data = ArrayUtils<int>.DeleteSubArray(data, item);

            }

            Vector outpVector = Activation(outp / (sumW * n));

            return new Tuple<Vector, List<ValueDictRegressor>, int>(outpVector, list, n);
        }



        /// <summary>
        /// Кодирование одного массива состояний в укрупненный
        /// </summary>
        /// <param name="inp">Масив состояний</param>
        public int[] Encode(int[] inp)
        {
            int[] data = (int[])inp.Clone();
            List<int> gTokens = new List<int> ();

            if (VectorDimention == -1)
                throw new Exception("Обучите модель");

           


            // Проход по всем длиннам буфера
            for (int i = MaxNGramm; i > 0; i--)
            {
                RingBuffer<int> ringBuffer = new RingBuffer<int>(i);
                List<int[]> used = new List<int[]>();

                // Проход по последовательности
                for (int j = 0; j < data.Length; j++)
                {
                    ringBuffer.AddElement(data[j]);
                    var key = CopyKey(ringBuffer.Data);


                    if (_data_marks.ContainsKey(key))
                    {
                        used.Add(ringBuffer.Data); // Добавление n-gramm
                        var dat = _data_marks[key];
                        gTokens.Add(dat.Index);
                    }
                }

                // Удаление групп (выбранной n-граммы)
                foreach (var item in used)
                    data = ArrayUtils<int>.DeleteSubArray(data, item);

            }


            return gTokens.ToArray();
        }


        /// <summary>
        /// Обучение 
        /// ToDo: Добавить рассчет индекса
        /// </summary>
        /// <param name="data">Массивы состояний</param>
        /// <param name="y">Целевые векторы</param>
        public void Train(int[][] data, Vector[] y)
        {
            // Число эпох
            int ep = (int)(600 / Math.Sqrt(data.Length));
            ep = Math.Max(ep, 10);

            _y_std = Statistics.Statistic.EnsembleStd(y)+double.Epsilon;
            _y_mean = Statistics.Statistic.MeanVector(y);

            Vector[] targets = new Vector[y.Length];

            for (int i = 0; i < y.Length; i++)
                targets[i] = (y[i] - _y_mean) / _y_std;

            if (targets == null)
                throw new Exception("Массив целевых векторов равен null");

            if (targets.Length != data.Length)
                throw new Exception("Размерность массива целевых векторов неравна размерности массива последовательностей состояний");
            
            if (targets.Length == 0)
                throw new Exception("Обучающая выборка пуста");

            _data_marks = new Dictionary<int[], ValueDictRegressor>(new IntArrayEqualityComparer());
            _vectorDimention = targets[0].Count;

            TrainCandidateSearch(data, targets); // Обучение
            Tuning(data, y, (int)(ep*0.5), 2); // Согласование
            Optimize2(TopP* TopP);
            Tuning(data, y, ep, 1); // Согласование
        }

        /// <summary>
        /// Создает регрессор на базе пользовательских правил (списка)
        /// </summary>
        /// <param name="data">Список кортежей (n-грамма, вектор)</param>
        /// <param name="max_n_gramm">Максимальная длинна n-граммы</param>
        /// <param name="top_p">Значение до которого должна дойти интегральное значение важности после сортировки</param>
        public static States2Vector CreateS2V(List<Tuple<int[], Vector>> data, int max_n_gramm, double top_p) 
        {
            States2Vector states2Vector = new States2Vector(max_n_gramm, top_p);
            states2Vector._data_marks = new Dictionary<int[], ValueDictRegressor>(new IntArrayEqualityComparer());

            foreach (var item in data)
            {
                var key = item.Item1;
                if (states2Vector._data_marks.ContainsKey(key))
                {
                    var el = states2Vector._data_marks[key];
                    el.CountActiv++;
                    el.TargetValue += item.Item2;
                }
                else 
                {
                    var el = new ValueDictRegressor();
                    el.CountActiv = 1;
                    el.TargetValue = item.Item2;
                    el.NGram = (short)item.Item1.Length;
                    states2Vector._data_marks.Add(key, el);
                }
            }

            // Расчет важностей
            foreach (var item in states2Vector._data_marks) item.Value.GetKImportance();

            return states2Vector;
        } 

        /// <summary>
        /// Тюнинг (градиентный спуск)
        /// </summary>
        public void Tuning(int[][] seqs, Vector[] targets, int epoch = 10, double lr = 0.01, double l2 = 0.0001, double l1 = 0.004)
        {
            for (int j = 0; j < epoch; j++)
            {
                List<ValueDictRegressor> regressors = new List<ValueDictRegressor>();

                for (int i = 0; i < seqs.Length; i++)
                {
                    var d = PredictWithListCount(seqs[i]);
                    var list = d.Item2;
                    Vector dif = d.Item1 - targets[i];
                    double denom = 0;

                    // Вычисление знаменателя
                    foreach (var item in list) denom += item.Importance;

                    // Вычисление производных
                    foreach (var item in list)
                        item.AddGrad(item.Importance * dif * d.Item3 / denom);

                    regressors.AddRange(list);
                }

                // Обучение
                foreach (var item in regressors) item.Upd(lr, l1, l2);

            }
        }

        // Линейная активация
        private Vector Lin(Vector inp)
        {
            return inp;
        }

        // Обучение на одной последовательности с вектором меток
        private void TrainCandidateSearch(int[][] seqs, Vector[] targets) 
        {
            for (int i = 1; i < MaxNGramm+1; i++)
                TrainCalcNG(seqs, targets, (short)i);
            Optimize2(Math.Pow(TopP, 0.2));
        }

        // Обучение с определенной длинной n-граммы
        private void TrainCalcNG(int[][] seqs, Vector[] targets, short nGram)
        {
            // Промежуточный словарь (избыточный) 
            Dictionary<int[], ValueDictRegressor> data_marks_intem = new Dictionary<int[], ValueDictRegressor>(new IntArrayEqualityComparer());

            for (int i = 0; i < seqs.Length; i++)
            {
                int[] seq = seqs[i];
                Vector target = targets[i];

                if (seq.Length < nGram) return; // Если последовательность меньше анализируемой n-граммы выходим из метода
                RingBuffer<int> ringBuffer = new RingBuffer<int>(nGram);
                int ngOfset = 0; // Смещение, чтобы в буффере не было 0

                for (int j = 0; j < seq.Length; j++)
                {
                    ringBuffer.AddElement(seq[j]);
                    if (ngOfset >= nGram) // Добавление nGramm
                    {
                        int[] d = CopyKey(ringBuffer.Data);

                        if (data_marks_intem.ContainsKey(d))
                        {
                            data_marks_intem[d].TargetValue += target;
                            data_marks_intem[d].CountActiv++;
                        }
                        else
                        {
                            data_marks_intem[d] = new ValueDictRegressor()
                            {
                                CountActiv = 1,
                                NGram = nGram,
                                TargetValue = target
                            };
                        }
                    }

                    ngOfset++;
                }
            }

            Optimize1(data_marks_intem);
        }


        // Первый уровень оптимизации, по частоте
        private void Optimize1(Dictionary<int[], ValueDictRegressor> keyValues, int min_count = 1)
        {
            foreach (var item in keyValues)
                if (item.Value.CountActiv > min_count)
                    _data_marks.Add(item.Key, item.Value);
        }

        // Второй уровень оптимизации, top-p
        private void Optimize2(double top_p)
        {
            Dictionary<int[], ValueDictRegressor> keyValues = new Dictionary<int[], ValueDictRegressor>(new IntArrayEqualityComparer());
            List<Tuple<int[], ValueDictRegressor>> data = new List<Tuple<int[], ValueDictRegressor>>(_data_marks.Count);

            // Заполнение списка
            foreach (var item in _data_marks) 
                data.Add(new Tuple<int[], ValueDictRegressor>(item.Key, item.Value));

            
            double p = 0;
            double denom = 0;
            int index = 0;

            // Сумма важностей
            foreach (var item in _data_marks) 
                denom += item.Value.GetImportance();

            data.Sort((x, y) => y.Item2.GetImportance().CompareTo(x.Item2.GetImportance())); // Сортировка для top-p

            while (p <= top_p) 
            {
                var l = data[index++];
                keyValues.Add(l.Item1, l.Item2);
                p += l.Item2.GetImportance() / denom;
            }

            _data_marks = keyValues;
        }

        // Копирует ключ
        private int[] CopyKey(int[] key)
        {
            int[] keyCopy = new int[key.Length];
            Array.Copy(key, 0, keyCopy, 0, key.Length);
            return keyCopy;
        }
    }
}