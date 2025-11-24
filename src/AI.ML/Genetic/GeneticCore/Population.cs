using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.ML.Genetic.GeneticCore
{
    /// <summary>
    /// Популяция клеток
    /// </summary>
    [Serializable]
    public class Population : List<Cell>
    {
        private readonly Vector[] X, Y;
        private readonly Random rnd = new Random(10);
        /// <summary>
        /// Вероятность мутации
        /// </summary>
        public double mutProb = 0.2;
        /// <summary>
        /// Скрещивание
        /// </summary>
        public double k = 0.5;
        private int top = 5;

        /// <summary>
        /// Число лидеров для размножения
        /// </summary>
        public int LiderCount
        {
            get => top;
            set => top = (0.75 * Count > value) ? value : (int)(0.75 * Count);
        }

        /// <summary>
        /// Величина мутации
        /// </summary>
        public double MutationValue
        {
            get; set;
        }

        /// <summary>
        /// Верхняя граница поиска (ГПСЧ)
        /// </summary>
        public double DValueUp
        {
            get;
            private set;
        }


        /// <summary>
        /// Нижняя граница поиска (ГПСЧ)
        /// </summary>
        public double DValueDown
        {
            get;
            private set;
        }

        /// <summary>
        /// Population
        /// </summary>
        /// <param name="countCell">Число клеток</param>
        /// <param name="countParam">Число параметров</param>
        /// <param name="function">Функция полезности</param>
        /// <param name="inp">Вектор входных данных</param>
        /// <param name="outp">Вектор выхода</param>
        /// <param name="valDown">Минимальное значение поиска параметров</param>
        /// <param name="valUp">Максимальное значение поиска параметра</param>
        public Population(int countCell, int countParam, Func<Vector, Vector, Vector> function, Vector[] inp, Vector[] outp, double valDown = -10, double valUp = 10)
        {
            DValueDown = valDown;
            DValueUp = valUp;
            MutationValue = 10;

            Random random = new Random();
            Cell[] cells = new Cell[countCell];

            for (int i = 0; i < countCell; i++)
            {
                cells[i] = new Cell(countParam, function, random, valDown, valUp);
            }

            AddRange(cells);

            X = inp;
            Y = outp;
        }

        /// <summary>
        /// Популяция
        /// </summary>
        /// <param name="countCell">Число клеток</param>
        /// <param name="countParam">Число параметров</param>
        /// <param name="function">Функция полезности</param>
        /// <param name="inp">Вектор входных данных</param>
        /// <param name="outp">Вектор выхода</param>
        /// <param name="valDown">Минимальное значение поиска параметров</param>
        /// <param name="valUp">Максимальное значение поиска параметра</param>
        public Population(int countCell, int countParam, Func<Vector, Vector, Vector> function, Vector inp, Vector outp, double valDown = -10, double valUp = 10)
        {
            DValueDown = valDown;
            DValueUp = valUp;
            MutationValue = 10;
            Random random = new Random();
            Cell[] cells = new Cell[countCell];

            for (int i = 0; i < countCell; i++)
            {
                cells[i] = new Cell(countParam, function, random, valDown, valUp);
            }

            AddRange(cells);

            X = new Vector[inp.Count];
            Y = new Vector[outp.Count];

            for (int i = 0; i < X.Length; i++)
            {
                X[i] = new Vector(1);
                Y[i] = new Vector(1);
                X[i][0] = inp[i];
                Y[i][0] = outp[i];
            }
        }

        /// <summary>
        ///  Передача функции от одной переменной
        /// </summary>
        /// <param name="inp">Вектор входов</param>
        /// <param name="indCell">Номер клетки</param>
        public Vector CellOut(Vector inp, int indCell = 0)
        {
            Vector outp = new Vector(inp.Count);

            for (int i = 0; i < inp.Count; i++)
            {
                outp[i] = this[indCell].Output(new Vector(inp[i]))[0];
            }

            return outp;
        }

        /// <summary>
        /// Эпоха для клеток
        /// </summary>
        /// <param name="count">Число потомков</param>
        public void Epoch(int count = 100)
        {
            Cell[] liders = new Cell[top];
            SortCells();
            Cell cellCentr = Central();
            Cell[] cells = new Cell[count];

            for (int i = 0; i < top; i++)
            {
                liders[i] = this[i];
            }

            #region Реализация браков
            int ind1 = rnd.Next(top), ind2 = rnd.Next(top);

            while (ind1 == ind2)
            {
                ind1 = rnd.Next(top);
                ind2 = rnd.Next(top);
            }
            #endregion

            for (int i = 0; i < count; i++)
            {
                cells[i] = Cross(liders[ind1], liders[rnd.Next(ind2)]);
            }

            for (int i = 0; i < count; i++)
            {
                if (rnd.NextDouble() < mutProb)
                {
                    cells[i] = Mutation(cells[i]);
                }
            }

            Clear();

            Add(cellCentr);
            AddRange(cells);
            AddRange(liders);
        }

        /// <summary>
        /// Баллы за один пример
        /// </summary>
        /// <param name="outp">Выход модели</param>
        /// <param name="targ">Целевые значения</param>
        private double Score(Vector outp, Vector targ)
        {
            return Distances.BaseDist.EuclideanDistance(outp, targ);
        }
        /// <summary>
        /// Сортировка
        /// </summary>
        public void SortCells()
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < X.Length; j++)
                {
                    this[i].Score += Score(this[i].Output(X[j]), Y[j]);
                }

                this[i].Score /= X.Length;
            }

            Sort((x, y) => x.Score.CompareTo(y.Score));
        }
        /// <summary>
        /// Мутация клетки
        /// </summary>
        /// <param name="input">Оригинальная клетка</param>
        public Cell Mutation(Cell input)
        {
            int pc = input.Parametrs.Count;
            int ind = rnd.Next(pc);
            Cell cell = new Cell(pc, input.Function, new Random(), DValueDown, DValueUp)
            {
                Parametrs = input.Parametrs.Clone()
            };
            cell.Parametrs[ind] += MutationValue * (rnd.NextDouble() - 0.5);
            return cell;
        }
        /// <summary>
        /// Cell crossing
        /// </summary>
        /// <param name="par1">First parent</param>
        /// <param name="par2">Second parent</param>
        public Cell Cross(Cell par1, Cell par2)
        {
            int pc = par1.Parametrs.Count;
            int hrK = (int)(k * pc);
            int ind = 0;
            Cell cell = new Cell(pc, par1.Function, new Random(), DValueDown, DValueUp);

            for (int i = 0; i < hrK; i++)
            {
                cell.Parametrs[ind++] = par1.Parametrs[i];
            }

            for (int i = hrK; i < pc; i++)
            {
                cell.Parametrs[ind++] = par2.Parametrs[i];
            }

            return cell;
        }

        private Cell Central()
        {
            double h = this[top - 1].Score;
            double w = 0, sumW = 0;
            Vector s = new Vector(this[0].Parametrs.Count);

            for (int i = 0; i < top; i++)
            {
                w = Rbf(this[i].Score, h);
                s += w * this[i].Parametrs;
                sumW += w;
            }

            Cell ret = new Cell(s.Count, this[0].Function, new Random(), DValueDown, DValueUp)
            {
                Parametrs = s / sumW
            };
            return ret;
        }


        private double Rbf(double dist, double h)
        {
            return Math.Exp(-(0.4 * dist * dist / h));
        }
    }


}
