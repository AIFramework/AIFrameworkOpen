using System;
using System.Drawing;

namespace AI.ComputerVision
{
    /// <summary>
    /// Подсчет объектов
    /// </summary>
    [Serializable]
    public class CalculateBinaryEl
    {
        /// <summary>
        /// Изображение
        /// </summary>
        public BinaryImg img;
        private readonly bool[][,] masksE = new bool[4][,];
        private readonly bool[][,] masksI = new bool[4][,];
        private int countE = 0, countI = 0;

        /// <summary>
        /// Подсчет объектов
        /// </summary>
        public CalculateBinaryEl()
        {
        }


        /// <summary>
        /// Подсчет объектов
        /// </summary>
        /// <param name="bmp">Изображение</param>
        /// <returns>Кол-во объектов</returns>
        public int CalculateBinElements(Bitmap bmp)
        {
            Mascs();
            img = new BinaryImg(bmp);
            int m = img.M, n = img.Count;

            countE = 0;
            countI = 0;

            // Проход по всему изображению фильтрами с подсчетом углов
            for (int i = 0; i < m - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    Filter(j, i);
                }
            }


            return (int)((countE - countI) / 4.0 + 0.999);// кол-во объектов


        }







        /// <summary>
        /// Проход одного шага по фильтрам внутренних углов, с подсчетом углов
        /// </summary>
        /// <param name="dx">Смещение по x</param>
        /// <param name="dy">Смещение по y</param>
        private void FilterI(int dx, int dy)
        {
            bool akkum = true;

            for (int k = 0; k < 4; k++)
            {
                akkum = true;


                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        akkum = akkum && img[dy + i, dx + j] == masksI[k][i, j];
                    }
                }

                if (akkum)
                {
                    countI++;
                    break;
                }

            }
        }

        /// <summary>
        /// Проход одного шага по фильтрам внешних углов, с подсчетом углов
        /// </summary>
        /// <param name="dx">Смещение по x</param>
        /// <param name="dy">Смещение по y</param>
        private void FilterE(int dx, int dy)
        {
            bool akkum = true;

            for (int k = 0; k < 4; k++)
            {
                akkum = true;


                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        akkum = akkum && img[dy + i, dx + j] == masksE[k][i, j];
                    }
                }

                if (akkum)
                {
                    countE++;
                    break;
                }

            }
        }

        /// <summary>
        /// Проход одного шага по внешним и внутренним углам
        /// </summary>
        /// <param name="dx">Смещение х</param>
        /// <param name="dy">Смещение у</param>
        private void Filter(int dx, int dy)
        {
            FilterE(dx, dy);
            FilterI(dx, dy);
        }

        /// <summary>
        /// Создание масок для фильтров внеш. и внутр. углов
        /// </summary>
        private void Mascs()
        {
            // Внешние
            masksE[0] = new bool[,]
            {
                {true, true},
                {true, false}
            };

            masksE[1] = new bool[,]
            {
                {true, true},
                {false, true}
            };

            masksE[2] = new bool[,]
            {
                {false, true},
                {true, true}
            };

            masksE[3] = new bool[,]
            {
                {true, false},
                {true, true}
            };

            //Внутренние
            masksI[0] = new bool[,]
            {
                {true, false},
                {false, false}
            };

            masksI[1] = new bool[,]
            {
                {false, true},
                {false, false}
            };

            masksI[2] = new bool[,]
            {
                {false, false},
                {true, false}
            };

            masksI[3] = new bool[,]
            {
                {false, false},
                {false, true}
            };

        }




    }
}
