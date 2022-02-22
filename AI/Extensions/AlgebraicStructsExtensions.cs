using AI.DataStructs.Algebraic;

namespace AI.Extensions
{
    public static class AlgebraicStructsExtensions
    {
        #region ToTensor
        //ToDo: Optim
        public static Tensor MatricesToTensor(this Matrix[] matrices)
        {
            Tensor tensor = new Tensor(matrices[0].Height, matrices[0].Width, matrices.Length);

            for (int i = 0; i < tensor.Depth; i++)
            {
                for (int j = 0; j < tensor.Height; j++)
                {
                    for (int k = 0; k < tensor.Width; k++)
                    {
                        tensor[j, k, i] = matrices[i][j, k];
                    }
                }

            }

            return tensor;
        }
        #endregion

        #region ToVector
        public static Vector ToVector(this double[] array)
        {
            return new Vector(array);
        }

        public static Vector ToVector(this float[] array)
        {
            return new Vector(array);
        }

        public static Vector ToVector(this int[] array)
        {
            return new Vector(array.ToDoubleArray());
        }

        public static Vector ToVector(this long[] array)
        {
            return new Vector(array.ToDoubleArray());
        }

        public static Vector ToVector(this decimal[] array)
        {
            return new Vector(array.ToDoubleArray());
        }

        public static Vector ToVector(this short[] array)
        {
            return new Vector(array.ToDoubleArray());
        }

        public static Vector ToVector(this byte[] array)
        {
            return new Vector(array.ToDoubleArray());
        }
        #endregion

        #region ToMatrix
        public static Matrix ToMatrix(this double[] matrixData, int H, int W)
        {
            Matrix matrix = new Matrix(H, W);

            for (int i = 0, k = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    matrix[i, j] = matrixData[k++];
                }
            }

            return matrix;
        }

        public static Matrix ToMatrix(this float[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this int[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this short[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this decimal[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this byte[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this long[] matrixData, int H, int W)
        {
            return matrixData.ToDoubleArray().ToMatrix(H, W);
        }

        public static Matrix ToMatrix(this double[,] matrixData)
        {
            return new Matrix(matrixData);
        }

        public static Matrix ToMatrix(this int[,] matrixData)
        {
            return new Matrix(matrixData.ToDoubleArray2D());
        }

        public static Matrix ToMatrix(this long[,] matrixData)
        {
            return new Matrix(matrixData.ToDoubleArray2D());
        }

        public static Matrix ToMatrix(this short[,] matrixData)
        {
            return new Matrix(matrixData.ToDoubleArray2D());
        }

        public static Matrix ToMatrix(this float[,] matrixData)
        {
            return new Matrix(matrixData.ToDoubleArray2D());
        }

        public static Matrix ToMatrix(this byte[,] matrixData)
        {
            return new Matrix(matrixData.ToDoubArray2D());
        }
        #endregion

        #region Vector Array
        /// <summary>
        /// Getting the vector of the desired coordinate from the array of vectors
        /// </summary>
        /// <param name="array"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static Vector GetDimention(this Vector[] array, int dim)
        {
            int n = array.Length;
            Vector vector = new Vector(n);

            for (int i = 0; i < n; i++)
            {
                vector[i] = array[i][dim];
            }

            return vector;
        }
        #endregion
    }
}