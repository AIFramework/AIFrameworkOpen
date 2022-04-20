using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Matrix = AI.DataStructs.Algebraic.Matrix;

namespace AI.ComputerVision
{
    // ToDo: Перевести
    //ToDo: Оптимизировать матрицы
    /// <summary>
    /// Конвертирование изображений
    /// в разные математические типы
    /// и обратно
    /// </summary>
    public static class ImageMatrixConverter
    {

        /// <summary>
        /// Загрузка картинки
        /// </summary>
        /// <param name="path">Имя</param>
        /// <returns>изображение</returns>
        public static Bitmap GetBitmap(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            return LoadImage(fs);
        }

        public static Matrix LoadAsMatrix(string path)
        {
            Bitmap bmp = GetBitmap(path);
            return BmpToMatr(bmp);
        }

        public static Tensor LoadAsTensor(string path)
        {
            Bitmap bmp = GetBitmap(path);
            return BmpToTensor(bmp);
        }

        //Загрузка изображения
        private static Bitmap LoadImage(Stream stream)
        {
            Bitmap retval = null;

            using (Bitmap b = new Bitmap(stream))
            {
                retval = new Bitmap(b.Width, b.Height, b.PixelFormat);
                using (Graphics g = Graphics.FromImage(retval))
                {
                    g.DrawImage(b, Point.Empty);
                    g.Flush();
                }
            }
            stream.Close();

            return retval;
        }

        /// <summary>
        /// Получение массива байт, для сохранения или передачи по сети
        /// </summary>
        /// <param name="bitmap">Изображение</param>
        public static byte[] ImgToByteArray(Bitmap bitmap)
        {
            Image img = bitmap;
            ImageConverter _imageConverter = new ImageConverter();
            byte[] buffer = (byte[])_imageConverter.ConvertTo(img, typeof(byte[]));
            return buffer;
        }

        private static unsafe double[,,] BaseTransformBmp(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;

            double[,,] outp = new double[3, height, width];

            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);


            try
            {
                byte* curpos;
                fixed (double* _outp = outp)
                {
                    double* r = _outp, g = _outp + width * height, b = _outp + 2 * width * height;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bmData.Scan0) + h * bmData.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *r = *(curpos++); ++r;
                            *g = *(curpos++); ++g;
                            *b = *(curpos++); ++b;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmData);
            }

            return outp;
        }

        private static unsafe Bitmap TensorToBmp(Tensor data)
        {

            int width = data.Width,
               height = data.Height;
            double[,,] outp = new double[3, height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    outp[0, i, j] = data[i, j, 0];
                    outp[1, i, j] = data[i, j, 1];
                    outp[2, i, j] = data[i, j, 2];
                }
            }

            return DbsToBitmap(outp);
        }

        // Code from https://archive.codeplex.com/?p=rasterconversion
        private static unsafe Bitmap DbsToBitmap(double[,,] rgb)
        {

            int width = rgb.GetLength(2),
                height = rgb.GetLength(1);

            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bd = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                byte* curpos;
                fixed (double* _rgb = rgb)
                {
                    double* _r = _rgb, _g = _rgb + width * height, _b = _rgb + 2 * width * height;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *(curpos++) = Limit(*_b); _b++;
                            *(curpos++) = Limit(*_g); _g++;
                            *(curpos++) = Limit(*_r); _r++;
                        }
                    }
                }
            }
            finally
            {
                result.UnlockBits(bd);
            }

            return result;
        }


        // Code from https://archive.codeplex.com/?p=rasterconversion
        private static unsafe Bitmap Dbs2DToBitmap(double[,] gray)
        {

            int width = gray.GetLength(1),
                height = gray.GetLength(0);

            Bitmap result = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bd = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                byte* curpos;
                fixed (double* _gray = gray)
                {
                    double* c = _gray;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            byte color = Limit(*c);
                            *(curpos++) = color;
                            *(curpos++) = color;
                            *(curpos++) = color;
                            c++;
                        }
                    }
                }
            }
            finally
            {
                result.UnlockBits(bd);
            }

            return result;
        }

        // Code from https://archive.codeplex.com/?p=rasterconversion
        private static byte Limit(double x)
        {
            if (x < 0)
            {
                return 0;
            }

            if (x > 255)
            {
                return 255;
            }

            return (byte)x;
        }

        /// <summary>
        /// Преобразование изображения в тензор 3-го ранга(нормировка на 1)
        /// </summary>
        /// <param name="Bmp">Изображение</param>
        public static Tensor BmpToTensor(Bitmap Bmp)
        {
            Tensor Out = new Tensor(Bmp.Height, Bmp.Width, 3);

            double[,,] d = BaseTransformBmp(Bmp);

            for (int i = 0; i < Bmp.Height; i++)
            {
                for (int j = 0; j < Bmp.Width; j++)
                {
                    Out[i, j, 2] = d[0, i, j];
                    Out[i, j, 1] = d[1, i, j];
                    Out[i, j, 0] = d[2, i, j];
                }
            }

            return Out;

        }

        /// <summary>
        /// Изображение в полутоновую матрицу
        /// </summary>
        /// <param name="Bmp">Изображение</param>
        public static Matrix BmpToMatr(Bitmap Bmp)
        {

            int W = Bmp.Width;
            int H = Bmp.Height; ;
            Matrix Out = new Matrix(H, W);

            double[,,] b = BaseTransformBmp(Bmp);

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Out[j, i] = (b[0, j, i] + b[1, j, i] + b[2, j, i]) / 3.0;
                }
            }

            return Out;

        }


        /// <summary>
        /// Изображение в матрицу синего канала
        /// </summary>
        /// <param name="Bmp">Изображение</param>
        public static Matrix BmpToMatrBlue(Bitmap Bmp)
        {

            int W = Bmp.Width;
            int H = Bmp.Height; ;
            Matrix Out = new Matrix(H, W);

            double[,,] b = BaseTransformBmp(Bmp);

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Out[j, i] = (b[2, j, i]);
                }
            }

            return Out;

        }

        /// <summary>
        /// Изображение в матрицу зеленого канала
        /// </summary>
        /// <param name="Bmp">Изображение</param>
        public static Matrix BmpToMatrGreen(Bitmap Bmp)
        {

            int W = Bmp.Width;
            int H = Bmp.Height; ;
            Matrix Out = new Matrix(H, W);

            double[,,] b = BaseTransformBmp(Bmp);

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Out[j, i] = (b[1, j, i]);
                }
            }

            return Out;

        }

        /// <summary>
        /// Изображение в матрицу красного канала
        /// </summary>
        /// <param name="Bmp">Изображение</param>
        public static Matrix BmpToMatrRed(Bitmap Bmp)
        {

            int W = Bmp.Width;
            int H = Bmp.Height; ;
            Matrix Out = new Matrix(H, W);

            double[,,] b = BaseTransformBmp(Bmp);

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Out[j, i] = b[0, j, i];
                }
            }

            return Out;

        }
        /// <summary>
        /// Преобразование картинки в матрицу H компонент
        /// H принадлежит интервалу [0,1]
        /// </summary>
        /// <param name="Bmp">Картинка</param>
        /// <returns></returns>
        public static Matrix BmpToHMatr(Bitmap Bmp)
        {
            int W = Bmp.Width;
            int H = Bmp.Height; ;
            Matrix Out = new Matrix(H, W);

            Tensor tensor = BmpToTensor(Bmp);

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    Out[j, i] = HComponent(new int[]
                        {
                            (int)(tensor[j,i,0]*255.0),
                            (int)(tensor[j,i,1]*255.0),
                            (int)(tensor[j,i,2]*255.0)
                        });
                }
            }


            return Out;
        }
        /// <summary>
        /// Поворот изображения на заданный угол
        /// </summary>
        /// <param name="bmp">Исходное изображение</param>
        /// <param name="angleRotate">Угол поворота</param>
        /// <returns>Изображение повернутое на заданный угол</returns>
        public static Bitmap RotateBitmap(Bitmap bmp, float angleRotate)
        {

            angleRotate %= 360;
            if (angleRotate > 180)
            {
                angleRotate -= 360;
            }
            PixelFormat pf = bmp.PixelFormat;

            float sin = (float)Math.Abs(Math.Sin(angleRotate * Math.PI / 180.0));
            float cos = (float)Math.Abs(Math.Cos(angleRotate * Math.PI / 180.0));
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;

            if (angleRotate > 0)
            {
                if (angleRotate <= 90)
                {
                    originX = sin * bmp.Height;
                }
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angleRotate >= -90)
                {
                    originY = sin * bmp.Width;
                }
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
            Graphics g = Graphics.FromImage(newImg);
            g.Clear(Color.White);
            g.TranslateTransform(originX, originY);
            g.RotateTransform(angleRotate);
            g.InterpolationMode = InterpolationMode.Bicubic;
            g.DrawImageUnscaled(bmp, 0, 0);
            g.Dispose();

            return newImg;
        }

        /// <summary>
        /// Вертикальное зеркальное отображение
        /// </summary>
        /// <param name="bmp">Изображение</param>
        /// <returns></returns>
        public static Bitmap VerticalReflectionBitmap(Bitmap bmp)
        {
            Image image = bmp;
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return (Bitmap)image;
        }

        /// <summary>
        /// Горизонтальное зеркальное отображение
        /// </summary>
        /// <param name="bmp">Изображение</param>
        public static Bitmap HorizontalReflectionBitmap(Bitmap bmp)
        {
            Image image = bmp;
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return (Bitmap)image;
        }



        /// <summary>
        /// Пропорционально изменение размеров с помощью явного указания ширины
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <param name="newW">Новая ширина</param>
        /// <returns></returns>
        public static Bitmap BmpResizeW(Bitmap bitmap, int newW)
        {
            double oldW = bitmap.Width, k = newW / oldW;
            int newH = (int)(bitmap.Height * k);

            return new Bitmap(bitmap, newW, newH);
        }

        /// <summary>
        /// Пропорционально изменение размеров с помощью явного указания высоты
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <param name="newH">Новая высота</param>
        /// <returns></returns>
        public static Bitmap BmpResizeH(Bitmap bitmap, int newH)
        {
            double oldH = bitmap.Height, k = newH / oldH;
            int newW = (int)(bitmap.Width * k);

            return new Bitmap(bitmap, newW, newH);
        }

        /// <summary>
        /// Пропорционально изменение размеров с помощью явного указания размера максимальной стороны
        /// </summary>
        /// <param name="bitmap">Исходное изображение</param>
        /// <param name="newM">Размер максимальной стороны</param>
        /// <returns></returns>
        public static Bitmap BmpResizeM(Bitmap bitmap, int newM)
        {
            if (bitmap.Height > bitmap.Width)
            {
                return BmpResizeH(bitmap, newM);
            }
            else
            {
                return BmpResizeW(bitmap, newM);
            }
        }




        // Вычисление H
        private static double HComponent(int[] rgb)
        {
            int max = rgb.Max();
            int min = rgb.Min();
            int indexMax = -1;
            double d = max - min;

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] == max)
                {
                    indexMax = i;
                    break;
                }
            }

            double H;
            if (d == 0)
            {
                H = 0;
            }


            else if (indexMax == 0)
            {
                d = 60.0 / d;
                if (rgb[1] >= rgb[2])
                {
                    H = d * (rgb[1] - rgb[2]);
                }
                else
                {
                    H = d * (rgb[1] - rgb[2]) + 360;
                }
            }

            else if (indexMax == 1)
            {
                d = 60.0 / d;
                H = d * (rgb[2] - rgb[0]) + 120;
            }

            else
            {
                d = 60.0 / d;
                H = d * (rgb[0] - rgb[1]) + 240;
            }

            return H / 360.0;
        }


        /// <summary>
        /// Вычисление H компоненты
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        static public double HComponent(Color rgb)
        {
            int[] rgbInt = { rgb.R, rgb.G, rgb.B };
            return HComponent(rgbInt);
        }

        private static int BiueInt(double intensiv)
        {
            return 120 / ((int)intensiv + 1);
        }

        private static int RedInt(double intensiv)
        {
            try
            {
                return (int)(intensiv) / 220;
            }
            catch { return 0; }
        }

        /// <summary>
        /// Визуализация матрицы
        /// </summary>
        public static Bitmap Visualization(Matrix matr)
        {
            Bitmap bmp = new Bitmap(matr.Width, matr.Height);
            Color color;
            Vector a = matr.Data;
            double max = new Statistic(FunctionsForEachElements.Abs(a)).MaxValue;
            double k = 250.0 / max;
            double intensiv;


            for (int i = 0; i < matr.Height; i++)
            {
                for (int j = 0; j < matr.Width; j++)
                {
                    intensiv = Math.Abs(k * matr[i, j]);
                    try
                    {
                        color = Color.FromArgb((int)(RedInt(intensiv) * intensiv), (int)(0.2 * intensiv), (int)(BiueInt(intensiv) * intensiv));
                    }
                    catch { color = Color.Coral; }
                    bmp.SetPixel(j, i, color);
                }
            }


            return bmp;
        }



        /// <summary>
        /// Перевод матрицы в полутоновое изображение
        /// </summary>
        public static Bitmap ToBitmap(Matrix matrix)
        {


            int width = matrix.Width,
               height = matrix.Height;

            double[,] gray = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    gray[i, j] = matrix[i, j];
                }
            }

            return Dbs2DToBitmap(gray);
        }

        /// <summary>
        /// Тензор в картинку
        /// </summary>
        /// <param name="tensor">Тензор</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ToBitmap(Tensor tensor)
        {
            return TensorToBmp(tensor);
        }



    }
}
