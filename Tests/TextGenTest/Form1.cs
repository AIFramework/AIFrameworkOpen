using AI.DataPrepaire.NLPUtils.TextGeneration;
using AI.DataPrepaire.Tokenizers.TextTokenizers;
using AI.DataStructs.Algebraic;
using AI.ML.HMM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextGenTest
{
    public partial class Form1 : Form
    {
        private readonly HMMFast _hmm = new HMMFast();
        private int _n;

        public Form1()
        {
            InitializeComponent();
        }

        private void nGr_ValueChanged(object sender, EventArgs e)
        {
            SetN();
        }

        private void train_Click(object sender, EventArgs e)
        {
            SetN();
            _hmm.NGram = _n;
            _hmm.Train(richTextBox1.Text, true);
        }

        private void SetN()
        {
            _n = (int)nGr.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] strArr = textBox1.Text.Split(' ');
            List<string> inp = new List<string>();
            int count = _n - strArr.Length - 1;

            for (int i = count; i > 0; i--)
            {
                inp.Add("<s>");
            }

            inp.AddRange(strArr);

            richTextBox2.Text = textBox1.Text + " " + _hmm.Generate(120, inp.ToArray());
        }
    }



  //  /// <summary>
  //  /// Быстрые марковские цепи
  //  /// </summary>
  //  [Serializable]
  //  public class HMMFast
  //  {
  //      #region Поля и свойства

  //      private MCFast _mc;
  //      private WordTokenizer _wordTokenizer;
  //      private int _nGramm = 3;

  //      /// <summary>
  //      /// N-граммы
  //      /// </summary>
		//public int NGram 
  //      {
  //          get { return _nGramm; }
  //          set { _nGramm = value; } 
  //      }
  //      /// <summary>
  //      /// Вектор вероятностей
  //      /// </summary>
		//public Vector ProbabilityVector => _mc.ProbabilityVector;
  //      /// <summary>
  //      /// 1- вектор вероятностей, полезен для установки квантелей
  //      /// </summary>
		//public Vector InvertedProbabilityVector => _mc.InvertedProbabilityVector;
  //      #endregion

  //      #region Конструкторы
  //      /// <summary>
  //      /// Быстрые марковские цепи
  //      /// </summary>
  //      /// <param name="tokenizer">Токенизатор</param>
  //      /// <param name="mc">Марковская цепь</param>
  //      public HMMFast(WordTokenizer tokenizer, MCFast mc)
  //      {
  //          _wordTokenizer = tokenizer;
  //          _mc = mc;
  //      }

  //      /// <summary>
  //      /// Быстрые марковские цепи
  //      /// </summary>
  //      public HMMFast()
  //      {
  //          _wordTokenizer = new WordTokenizer();
  //          _mc = new MCFast();
  //      }

  //      #endregion

  //      /// <summary>
  //      /// Обучение языковой модели
  //      /// </summary>
  //      /// <param name="trainText">Тренировочный текст</param>
  //      /// <param name="addStart">Добавлять ли старт вначале</param>
  //      public void Train(string trainText, bool addStart = false)
  //      {
  //          if (trainText == null)
  //              throw new ArgumentNullException(nameof(trainText));

  //          _wordTokenizer.TrainFromText(trainText);
  //          _mc.NGram = NGram;
  //          int[] tokens = _wordTokenizer.Encode(trainText);
  //          _mc.StartToken = _wordTokenizer.StartToken;
  //          _mc.EndToken = _wordTokenizer.EndToken;
  //          _mc.Train(tokens, addStart);

  //      }
        
  //      /// <summary>
		///// Генерация текста
		///// </summary>
		///// <param name="num">число слов</param>
		///// <returns>сгенерированная строка</returns>
		//public string Generate(int num)
  //      {
  //          return _wordTokenizer.DecodeObj(_mc.Generate(num));
  //      }
        
  //      /// <summary>
  //      /// Генерация текста
  //      /// </summary>
  //      /// <param name="num">число слов</param>
  //      /// <param name="strs">начальное состояние</param>
  //      /// <returns>сгенерированная строка</returns>
  //      public string Generate(int num, string[] strs)
  //      {
  //          int[] tokens = _wordTokenizer.Encode(strs);
  //          return _wordTokenizer.DecodeObj(_mc.Generate(num, tokens));
  //      }
        
  //      /// <summary>
  //      /// Генерация текста
  //      /// </summary>
  //      /// <param name="num">число слов</param>
  //      /// <param name="strs">начальное состояние</param>
  //      /// <param name="rnd">Генератор псевдо-случайных чисел</param>
  //      /// <returns>сгенерированная строка</returns>
  //      public string Generate(int num, string[] strs, Random rnd)
  //      {
  //          int[] tokens = _wordTokenizer.Encode(strs);
  //          return _wordTokenizer.DecodeObj(_mc.Generate(num, tokens, rnd));
  //      }

  //      /// <summary>
  //      /// Преобразование текста в вектор + изменение модели
  //      /// </summary>
  //      /// <param name="text">текст</param>
  //      /// <returns>вектор</returns>
  //      public Vector TextToVector(string text)
  //      {
  //          int[] tokens = _wordTokenizer.Encode(text);
  //          return _mc.SeqToVector(tokens);
  //      }

  //  }

}
