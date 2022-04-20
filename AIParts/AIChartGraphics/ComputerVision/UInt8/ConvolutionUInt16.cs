namespace AI.ComputerVision.UInt8
{
    /// <summary>
    /// Свертка изображений UInt8
    /// </summary>
    public static class ConvolutionUInt16
    {
        /// <summary>
        /// Свертка чб изобржений
        /// </summary>
        /// <param name="img"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static ImgUInt16Gray Conv16Gray(ImgUInt16Gray img, ImgUInt16Gray filter)
        {

            int h = img.Height, w = img.Width;
            ImgUInt16Gray Ret16Gray = new ImgUInt16Gray(img.Height, img.Width);

            int sX = 1 - filter.Width;
            int y = 1 - filter.Height;

            for (int y1 = 0; y1 < h; y1++, y++)
            {
                int x = sX;
                for (int x1 = 0; x1 < w; x1++, x++)
                {
                    for (int dy = 0; dy < filter.Height; dy++)
                    {
                        int y2 = y + dy;
                        for (int dx = 0; dx < filter.Width; dx++)
                        {
                            int ox = x + dx;
                            if (y2 >= 0 && y2 < h && ox >= 0 && ox < w)
                            {
                                Ret16Gray[y1, x1] += (short)(filter[dy, dx] * img[y2, ox]);
                            }
                        }
                    }

                }
            }

            return Ret16Gray;
        }
    }
}