/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 13.04.2018
 * Время: 22:13
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.ML.HMM
{
    /// <summary>
    /// Марковская цепь
    /// </summary>
    [Serializable]
    public class MChWords
    {
        /// <summary>
        /// Матрица состояний
        /// </summary>
        public Matrix stateMatrix;
        /// <summary>
        /// Обратная матрица состояний (1-matr_state)
        /// </summary>
        public Matrix stateAlter;
        private string[] stateNames;
        private double len;
        private readonly Random rnd = new Random();


        /// <summary>
        /// Марковская цепь
        /// </summary>
        public MChWords()
        {

        }

        /// <summary>
        /// Обучение
        /// </summary>
        /// <param name="TrainText">Training text</param>
        public void Train(string TrainText)
        {

            double[,] _stateMatrix, _stateAlter;
            string[] trainText = TrainText.ToLower().Split();
            stateNames = GetWords(trainText);


            _stateMatrix = new double[stateNames.Length, stateNames.Length];
            _stateAlter = new double[stateNames.Length, stateNames.Length];
            len = stateNames.Length * stateNames.Length;


            for (int i = 0; i < trainText.Length - 1; i++)
            {
                for (int j = 0; j < stateNames.Length; j++)
                {
                    for (int k = 0; k < stateNames.Length; k++)
                    {
                        if (trainText[i] == stateNames[j]
                        && trainText[i + 1] == stateNames[k])
                        {
                            _stateMatrix[j, k]++;
                            _stateAlter[j, k]++;
                            break;
                        }
                    }
                }
            }


            double max = GetMax(_stateAlter);

            for (int j = 0; j < stateNames.Length; j++)
            {
                for (int k = 0; k < stateNames.Length; k++)
                {
                    _stateMatrix[j, k] /= trainText.Length;
                    _stateAlter[j, k] /= max;
                    _stateAlter[j, k] = (1 - _stateAlter[j, k]) * 0.9999;
                }
            }

            stateAlter = new Matrix(_stateAlter);
            stateMatrix = new Matrix(_stateMatrix);
        }




        /// <summary>
        /// Maximum transition probability
        /// </summary>
        /// <param name="matrix">State Matrix</param>
        private double GetMax(double[,] matrix)
        {
            double max = matrix[0, 0];

            for (int j = 0; j < stateNames.Length; j++)
            {
                for (int k = 0; k < stateNames.Length; k++)
                {
                    if (matrix[j, k] > max)
                    {
                        max = matrix[j, k];
                    }
                }
            }

            return max;
        }


        /// <summary>
        /// Generating text
        /// </summary>
        /// <param name="num">How many words</param>
        /// <param name="begin">The first word</param>
        public string Generate(int num, string begin)
        {
            string[] chs = new string[num];
            int ch;
            chs[0] = begin;
            string outp = begin + " ";


            for (int i = 1; i < num; i++)
            {

                while (true)
                {
                    ch = rnd.Next(stateNames.Length);

                    if (rnd.NextDouble() > stateAlter[GetInd(chs[i - 1]), ch])
                    {
                        chs[i] = stateNames[ch];
                        outp += chs[i] + " ";
                        break;
                    }

                }
            }


            return outp;
        }

        // <summary>
        // Поиск индекса
        // </summary>
        // <param name="stateName">Имя состояния</param>
        // <returns></returns>
        private int GetInd(string stateName)
        {
            int ind = 0;

            for (int i = 0; i < stateNames.Length; i++)
            {
                if (stateName == stateNames[i])
                {
                    ind = i;
                    break;
                }
            }

            return ind;
        }



        // <summary>
        //  Получение слов
        // </summary>
        // <param name="strs">Строки входа</param>
        // <returns></returns>
        private string[] GetWords(string[] strs)
        {
            List<string> words = new List<string>
            {
                strs[0]
            };
            bool flag = true;

            for (int i = 0; i < strs.Length; i++)
            {
                flag = true;
                for (int j = 0; j < words.Count; j++)
                {
                    if (strs[i] == words[j])
                    {
                        flag = false;
                        break;
                    }


                }

                if (flag)
                {
                    words.Add(strs[i]);
                }
            }

            return words.ToArray();

        }

    }
}
