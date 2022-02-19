
using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;
using System.IO;


namespace AI.NLP
{
    /// <summary>
    /// Bag-of-Words model
    /// </summary>
    [Serializable]
    public class BoWModel
    {
        private readonly string[] model;
        /// <summary>
        /// A vector where all 0's except for the word position
        /// </summary>
		public Vector vector;
        /// <summary>
        /// Are stop words analyzed
        /// </summary>
		public bool isStop { get; set; }
        /// <summary>
        /// Are numbers being skipped
        /// </summary>
		public bool isDig { get; set; }
        /// <summary>
        /// Vector / Dictionary Length
        /// </summary>
		public int Len;
        /// <summary>
        /// Is the vector normalization
        /// </summary>
		public bool IsNormalise { get; set; }

        /// <summary>
        ///  Bag-of-Words model
        /// </summary>
        public BoWModel(string pathModel)
        {
            model = File.ReadAllText(pathModel).Split(" .,!\t\n".ToCharArray());
            Len = model.Length;
            vector = new Vector(model.Length);
            isStop = false;
            IsNormalise = false;
            isDig = false;
        }

        /// <summary>
        /// Getting a vector from text
        /// </summary>
        /// <param name="text">Text</param>
        public Vector GetVector(string text)
        {
            ProbabilityDictionary prob = new ProbabilityDictionary(isStop, isDig);
            ProbabilityDictionaryData[] pds = prob.Run(text);

            vector = new Vector(vector.Count);

            for (int i = 0; i < pds.Length; i++)
            {
                for (int j = 0; j < model.Length; j++)
                {
                    if (pds[i].Word == model[j].Trim('\r'))
                    {
                        vector[j]++;
                    }
                }
            }

            if (IsNormalise)
            {
                vector /= Statistic.MaximalValue(vector) + 1e-6;
                vector -= Statistic.ExpectedValue(vector);
            }

            return vector;
        }

        /// <summary>
        /// Generating a model
        /// </summary>
        public static void ModelGen(string text, string path, bool isStop = false)
        {
            ProbabilityDictionary prob = new ProbabilityDictionary(isStop);
            ProbabilityDictionaryData[] pb = prob.Run(text);
            int len = pb.Length;
            string[] newModel = new string[len];

            for (int i = 0; i < len; i++)
            {
                newModel[i] = pb[i].Word;
            }

            File.WriteAllLines(path, newModel);
        }
    }
}
