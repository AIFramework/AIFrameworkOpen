using AI.DataStructs.Algebraic;
using AI.Fuzzy;
using AI.Fuzzy.Fuzzyficators.FVector;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Нечеткий классификатор
    /// </summary>
    [Serializable]
    public class FuzzyClassifier : BaseClassifier<FuzzyClassifier>
    {
        /// <summary>
        /// Фаззификатор входов
        /// </summary>
        public IFuzzyficatorVector FuzzyficatorVectorInp { get; set; } = new SigmoidVectorFuzzyficator(3);
        /// <summary>
        /// Фаззификатор выходов
        /// </summary>
        public IFuzzyficatorVector FuzzyficatorVectorOutp { get; set; } = new SigmoidVectorFuzzyficator(2);
        /// <summary>
        /// Матрица импликаций
        /// </summary>
        public Matrix ImplMatrix { get; protected set; }

        /// <summary>
        /// Распознать вектор
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <returns>Метка класса</returns>
        public override int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }

        /// <summary>
        /// Распознать вектор
        /// </summary>
        /// <param name="inp">Вход</param>
        /// <returns>Вектор принадлежностей</returns>
        public override Vector ClassifyProbVector(Vector inp)
        {
            Vector fV = FuzzyficatorVectorInp.Fuzzyfication(inp);
            Vector outF = FuzzyAnalogyInference.Inference(ImplMatrix, fV);
            Vector outp = FuzzyficatorVectorOutp.DeFuzzyfication(outF);
            return outp/outp.Sum();
        }
      

        /// <summary>
        /// Обучить
        /// </summary>
        public override void Train(Vector[] features, int[] classes)
        {
            int max = classes.Max();
            Vector[] inp = features.Select(x => FuzzyficatorVectorInp.Fuzzyfication(x)).ToArray();
            Vector[] outp = classes.Select(x => FuzzyficatorVectorOutp.Fuzzyfication(Vector.OneHotBePol(x, max))).ToArray();
            ImplMatrix = FuzzyAnalogyInference.GetImplicationMatrixG(inp, outp);
        }
    }
}
