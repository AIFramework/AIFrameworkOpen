using AI.DataStructs.Algebraic;
using System;
using System.Drawing;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CustomFilter : ISpatialFilterGray
    {
        protected Matrix f;

        public CustomFilter(Matrix fMatrix)
        {
            f = fMatrix;
        }

        public CustomFilter()
        {
            f = new Matrix(3, 3);
            f[1, 1] = 1;
        }

        public Matrix Filtration(Matrix input)
        {
            return ImgFilters.SpatialFilter(input, f);
        }

        public Bitmap Filtration(Bitmap input)
        {
            Matrix matrix = ImageMatrixConverter.BmpToMatr(input);
            Matrix filtred = Filtration(matrix);
            return new Bitmap(ImageMatrixConverter.ToBitmap(filtred), input.Width, input.Height);
        }

        public Bitmap Filtration(string path)
        {
            Matrix matrix = ImageMatrixConverter.LoadAsMatrix(path);
            Matrix filtred = Filtration(matrix);
            return new Bitmap(ImageMatrixConverter.ToBitmap(filtred), matrix.Width, matrix.Height);
        }
    }
}
