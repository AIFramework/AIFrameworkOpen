using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.ML.SeqAnalyze
{
    /// <summary>
    /// Классификатор на базе алгоритма S2V
    /// </summary>
    [Serializable]
    public class ClassifierS2V
    {
        /// <summary>
        /// Порог принадлежности к классу для многоклассового классификатора (по умолчанию 0.5)
        /// </summary>
        public double Treshold { get; set; } = 0.5;

        /// <summary>
        /// Добавление/просмотр правила
        /// </summary>
        /// <param name="ints">Вход</param>
        /// <returns></returns>
        public Vector this[int[] ints] 
        {
            get { return _s2v[ints]; }
            set { _s2v[ints] = value; }
        }


        /// <summary>
        /// Число правил
        /// </summary>
        public int CountRules => _s2v.CountRules;

        /// <summary>
        /// Регрессор
        /// </summary>
        public States2Vector States2Vector => _s2v;

        // S2V регрессор
        private States2Vector _s2v;
        // Число классов
        private int _n_cl;
        private S2VClType _s2VClType;


        /// <summary>
        /// Классификатор на базе алгоритма S2V
        /// </summary>
        /// <param name="nCl">Число классов</param>
        /// <param name="max_n_gramm">Максимальная длинна n-граммы</param>
        /// <param name="top_p">Значение до которого должна дойти интегральное значение важности после сортировки</param>
        /// <param name="s2VClType">Тип классификатора, одноклассовый/многоклассовый</param>
        public ClassifierS2V(int nCl, int max_n_gramm, double top_p = 0.75, S2VClType s2VClType = S2VClType.OneClassPredict) 
        {
            _s2v = new States2Vector(max_n_gramm, top_p);
            _n_cl = nCl;
            _s2VClType = s2VClType;

            if (s2VClType == S2VClType.OneClassPredict) _s2v.Activation = SoftMax;
            else _s2v.Activation = Sigmoid;

        }

        /// <summary>
        /// Обучение классификатора
        /// </summary>
        public void Train(IEnumerable<int[]> states, IEnumerable<int> classes) 
        {
            Vector[] targets = new Vector[classes.Count()];
            int[][] statesArray = states.ToArray();
            int i = 0;

            foreach (var item in classes)
                targets[i++] = Vector.OneHotPol(item, _n_cl-1);

            _s2v.Train(statesArray, targets);
        }

        /// <summary>
        /// Создание классификатора 
        /// (cls - массив правил, каждый элемент массива содержит список индикаторов класса, 
        /// число классов равно длинне массива)
        /// </summary>
        /// <param name="cls">массив правил, каждый элемент массива содержит список индикаторов класса, 
        /// число классов равно длинне массива</param>
        /// <param name="max_n_gramm">Максимальная длинна n-граммы</param>
        /// <param name="membership_cl_coef"></param>
        /// <param name="top_p">Значение до которого должна дойти интегральное значение важности после сортировки</param>
        /// <param name="s2VClType">Тип классификатора, одноклассовый/многоклассовый</param>
        public ClassifierS2V CreateClassifierS2V(List<int[]>[] cls, int max_n_gramm, double membership_cl_coef = 300, double top_p = 0.75, S2VClType s2VClType = S2VClType.OneClassPredict) 
        {
            ClassifierS2V classifier = new ClassifierS2V(cls.Length, max_n_gramm, top_p, s2VClType);
            var data = new List<Tuple<int[], Vector>>();

            for (int i = 0; i < cls.Length; i++)
            {
                // Вектор элемента класса
                Vector cl = Vector.OneHotBePol(i, cls.Length-1)*membership_cl_coef;

                for (int j = 0; j < cls[i].Count; j++)
                    data.Add(new Tuple<int[], Vector>(cls[i][j], cl.Clone())); // Добавление данных класса
            }

            classifier._s2v = States2Vector.CreateS2V(data, max_n_gramm, top_p);

            return classifier;
        }

        /// <summary>
        /// Получение вектора вероятностей принадлежности к классу
        /// </summary>
        /// <param name="input">Вход (Последовательность состояний)</param>
        public Vector ClassifyProbVector(IEnumerable<int> input) 
        {
            var inp = input.ToArray();
            return _s2v.Predict(inp);
        }

        /// <summary>
        /// Классификация, -1 означает, что запрос аномальный и классификатор не может его классифицировать 
        /// </summary>
        /// <param name="input">Вход (Последовательность состояний)</param>
        /// <returns></returns>
        public int[] Classify(IEnumerable<int> input) 
        {
            var probs = ClassifyProbVector(input);

            if (probs.ContainsNan()) return new[] { -1 };

            if (_s2VClType == S2VClType.MultyClassPredict)
            {
                List<int> output = new List<int>();
                for (int i = 0; i < probs.Count; i++)
                    if (probs[i] >= Treshold) output.Add(i);

                var retArr = output.ToArray();

                return retArr.Length > 0? retArr: new[] { -1};
            }
            else 
            {
                int ind = probs.MaxElementIndex();
                ind = probs[ind] >= Treshold ? ind : -1;
                return new[] { ind};
            }
        }

        // Активация
        private Vector SoftMax(Vector inp)
        {
            Vector outp = inp.Transform(Math.Exp);
            return outp / outp.Sum();
        }

        // Активация
        private Vector Sigmoid(Vector inp)
        {
            return inp.Transform(x => 1.0 / (1 + Math.Exp(-x)));
        }

        /// <summary>
        /// Добавление правила для классификатора
        /// </summary>
        /// <param name="states"></param>
        /// <param name="cl_mark"></param>
        /// <param name="gain"></param>
        public void AddRuleCl(int[] states, int cl_mark, double gain = 100) 
        {
            var dat = gain*Vector.OneHotPol(cl_mark, _n_cl - 1);
            this[states] = dat;
        }
    }

    /// <summary>
    /// Тип классификатора s2v
    /// </summary>
    public enum S2VClType 
    {
        /// <summary>
        /// Предсказание одного класса по входу
        /// </summary>
        OneClassPredict,
        /// <summary>
        /// Предсказание многих классов по входу
        /// </summary>
        MultyClassPredict
    }
}
