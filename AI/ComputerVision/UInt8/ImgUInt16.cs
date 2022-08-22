using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AI.ComputerVision.UInt8
{
    /// <summary>
    /// Изображение серое UInt16
    /// </summary>
    [Serializable]
    public class ImgUInt16Gray
    {
        /// <summary>
        /// Изображение
        /// </summary>
        public short[,] img;
        /// <summary>
        /// Ширина
        /// </summary>
        public readonly int Width;
        /// <summary>
        /// Высота
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Доступ к пикселам
        /// </summary>
        /// <param name="i">Index высоты</param>
        /// <param name="j">Index ширины</param>
        /// <returns></returns>
        public short this[int i, int j]
        {
            get => img[i, j];
            set => img[i, j] = value;
        }
        /// <summary>
        /// Создание черного изображения указанных размеров
        /// </summary>
        /// <param name="h"></param>
        /// <param name="w"></param>
        public ImgUInt16Gray(int h, int w)
        {
            img = new short[h, w];
            Height = h;
            Width = w;
        }

        /// <summary>
        /// Загрузка картинки(с переводом в чб)
        /// </summary>
        /// <param name="bitmap">Изображение</param>
        public ImgUInt16Gray(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;

            img = new short[Height, Width];

            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            int dim = bmData.Stride * bmData.Height;
            System.IntPtr pointer = bmData.Scan0;
            byte[] array = new byte[dim];
            Marshal.Copy(pointer, array, 0, dim);
            bitmap.UnlockBits(bmData);

            for (int j = 0; j < Height; j++)
            {
                int p = bmData.Stride * j;
                for (int k = 0; k < Width; k++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        img[j, k] += array[p++];
                    }

                    img[j, k] /= 3;
                }
            }

        }



        /// <summary>
        /// Сумма
        /// </summary>
        /// <param name="img"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static ImgUInt16Gray operator +(ImgUInt16Gray img, int k)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] + k);
                }
            }

            return outp;
        }

        /// <summary>
        /// Разность
        /// </summary>
        public static ImgUInt16Gray operator -(ImgUInt16Gray img, int k)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] - k);
                }
            }

            return outp;
        }

        /// <summary>
        /// Сумма
        /// </summary>
        public static ImgUInt16Gray operator +(int k, ImgUInt16Gray img)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] + k);
                }
            }

            return outp;
        }

        /// <summary>
        /// Разность
        /// </summary>
        public static ImgUInt16Gray operator -(int k, ImgUInt16Gray img)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(k - img[i, j]);
                }
            }

            return outp;
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImgUInt16Gray operator *(ImgUInt16Gray img, double k)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] * k);
                }
            }

            return outp;
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImgUInt16Gray operator /(ImgUInt16Gray img, double k)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] / k);
                }
            }

            return outp;
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImgUInt16Gray operator /(ImgUInt16Gray img, int k)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] / k);
                }
            }

            return outp;
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImgUInt16Gray operator *(double k, ImgUInt16Gray img)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(img[i, j] * k);
                }
            }

            return outp;
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImgUInt16Gray operator /(double k, ImgUInt16Gray img)
        {

            ImgUInt16Gray outp = new ImgUInt16Gray(img.Height, img.Width);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    outp[i, j] = (short)(k / img[i, j]);
                }
            }

            return outp;
        }



        /// <summary>
        /// Перевод изображения в Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {

            Bitmap bmp = new Bitmap(Width, Height);
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite,
               PixelFormat.Format24bppRgb);
            int dim = bitmapData.Stride * bitmapData.Height;
            System.IntPtr pointer = bitmapData.Scan0;
            byte[] array = new byte[dim];

            for (int i = 0; i < Height; i++)
            {
                int p = bitmapData.Stride * i;
                for (int j = 0; j < Width; j++)
                {
                    short d = img[i, j];
                    if (d > 255)
                    {
                        d = 255;
                    }
                    else if (d < 0)
                    {
                        d = 0;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        array[p++] = (byte)d;
                    }
                }
            }

            Marshal.Copy(array, 0, pointer, dim);
            bmp.UnlockBits(bitmapData);

            return bmp;
        }
    }
}

