using AI.DataStructs.Shapes;
using AI.Extensions;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace AI.DataStructs.Algebraic
{
    /// <summary>
    /// ������ 3-�� �����
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}, Depth = {Depth}")]
    public class Tensor : IAlgebraicStructure<double> , IEquatable<Tensor>, ISavable, IByteConvertable
    {
        #region ���� � ��������
        /// <summary>
        /// ������
        /// </summary>
        public int Height => Shape[1];
        /// <summary>
        /// ������
        /// </summary>
        public int Width => Shape[0];
        /// <summary>
        /// �������
        /// </summary>
        public int Depth => Shape[2];
        /// <summary>
        /// ����� �������
        /// </summary>
        public Shape Shape { get; }
        /// <summary>
        /// ������
        /// </summary>
        public double[] Data { get; set; }
        /// <summary>
        /// ������ � �������� �� �������
        /// </summary>
        /// <param name="i">������</param>
        /// <param name="j">������</param>
        /// <param name="k">�������</param>
        public double this[int i, int j, int k]
        {
            get => Data[GetByIndex(i, j, k)];
            set => Data[GetByIndex(i, j, k)] = value;
        }
        #endregion

        #region ������������
        /// <summary>
        /// ������������� 3�-������ ��������
        /// </summary>
        public Tensor(double[,,] data)
        {
            Shape = new Shape3D(data.GetLength(0), Data!.GetLength(1), data.GetLength(2));

            Data = new double[Height * Width * Depth];

            //ToDo: ��������������
            for (int i = 0; i < Depth; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    for (int k = 0; k < Width; k++)
                    {
                        this[j, k, i] = data[j, k, i];
                    }
                }

            }
        }
        /// <summary>
        /// ������� ������ ����������� ������
        /// </summary>
        public Tensor(Shape3D shape)
        {
            if (shape.Rank > 3)
            {
                throw new ArgumentException("������������ ����(�����������) ����� = 3", nameof(shape));
            }

            switch (shape.Rank)
            {
                case 1:
                    Shape = new Shape3D(1, shape[0]);
                    break;
                case 2:
                    Shape = new Shape3D(shape[1], shape[0]);
                    break;
                case 3:
                    Shape = new Shape3D(shape[1], shape[0], shape[2]);
                    break;
            }

            Data = new double[Shape!.Count];
        }
        /// <summary>
        /// ������� ������ ������������������ ������
        /// </summary>
        /// <param name="height">������</param>
        /// <param name="width">������</param>
        /// <param name="depth">�������</param>
        public Tensor(int height, int width, int depth) : this(new Shape3D(height, width, depth)) { }
        /// <summary>
        /// ������������� ��������
        /// </summary>
        public Tensor(double[] data)
        {
            Shape = new Shape3D(1, 1, data.Length);
            Data = new double[Depth];
            Buffer.BlockCopy(data, 0, Data, 0, 8 * Shape.Count);
        }
        #endregion

        #region ���������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Tensor operator +(Tensor A, double b)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] + b;
            }

            return newTensor;
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Tensor operator +(Tensor A, Tensor B)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);

            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] + B.Data[i];
            }

            return newTensor;
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Tensor operator +(double b, Tensor A)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] + b;
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public static Tensor operator *(Tensor A, double K)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] * K;
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public static Tensor operator *(double K, Tensor A)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] * K;
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Tensor operator *(Tensor A, Tensor B)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] * B.Data[i];
            }

            return newTensor;
        }
        /// <summary>
		/// �������
		/// </summary>
		public static Tensor operator /(Tensor A, double b)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] / b;
            }

            return newTensor;
        }
        /// <summary>
        /// �������
        /// </summary>
        public static Tensor operator /(double b, Tensor A)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = b / A.Data[i];
            }

            return newTensor;
        }
        /// <summary>
        /// �������
        /// </summary>
        public static Tensor operator /(Tensor A, Tensor B)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] / B.Data[i];
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        public static Tensor operator -(Tensor A, double b)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = A.Data[i] - b;
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        public static Tensor operator -(double b, Tensor A)
        {
            Tensor newTensor = new Tensor(A.Width, A.Height, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = b - A.Data[i];
            }

            return newTensor;
        }
        /// <summary>
        /// ���������
        /// </summary>
        public static Tensor operator -(Tensor B, Tensor A)
        {
            Tensor newTensor = new Tensor(A.Height, A.Width, A.Depth);
            int len = A.Shape.Count;

            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = B.Data[i] - A.Data[i];
            }

            return newTensor;
        }
        /// <summary>
        /// �������� ���������
        /// </summary>
        public static bool operator ==(Tensor left, Tensor right)
        {
            return left.Shape == right.Shape && left.Data.ElementWiseEqual(right.Data);
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        public static bool operator !=(Tensor left, Tensor right)
        {
            return left.Shape != right.Shape || !left.Data.ElementWiseEqual(right.Data);
        }
        #endregion

        /// <summary>
        /// �����������
        /// </summary>
        public Tensor Copy()
        {
            Tensor tensor3 = new Tensor(Height, Width, Depth);
            Buffer.BlockCopy(Data, 0, tensor3.Data, 0, 8 * Shape.Count);
            return tensor3;
        }
        /// <summary>
        /// ������������ �������������� �������
        /// </summary>
        /// <param name="transform">������� ��������������</param>
        public Tensor Transform(Func<double, double> transform)
        {
            Tensor newTensor = new Tensor(Height, Width, Depth);
            int len = Shape.Count;
            for (int i = 0; i < len; i++)
            {
                newTensor.Data[i] = transform(Data[i]);
            }

            return newTensor;
        }

        /// <summary>
        /// ����������� ������ � ������ ������
        /// </summary>
        public Matrix[] ToMatrices()
        {
            Matrix[] matrix = new Matrix[Depth];

            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new Matrix(Height, Width);
            }

            for (int k = 0; k < Depth; k++)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        matrix[k][i, j] = this[i, j, k];
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// ����������� ������� ������ � ������
        /// </summary>
        public static Tensor FromMatrices(Matrix[] matrices)
        {
            Tensor tensor = new Tensor(matrices[0].Height, matrices[0].Width, matrices.Length);


            for (int k = 0; k < matrices.Length; k++)
            {
                for (int i = 0; i < matrices[0].Height; i++)
                {
                    for (int j = 0; j < matrices[0].Width; j++)
                    {
                        tensor[i, j, k] = matrices[k][i, j];
                    }
                }
            }

            return tensor;
        }
        /// <summary>
        /// ����������� ������� � ������
        /// </summary>
        public static Tensor VectorToTensor(Vector data, int h, int w)
        {
            int d = data.Count / (h * w);

            Tensor tensor = new Tensor(h, w, d);

            for (int i = 0, l = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    for (int k = 0; k < d; k++)
                    {
                        tensor[i, j, k] = data[l++];
                    }
                }
            }

            return tensor;
        }
        /// <summary>
        /// ������������ ��������� �� ������� �� ������� TNew[i,j,k] = T[i,j,k] - V[k]
        /// </summary>
        /// <returns></returns>
        public Tensor SubtractingDepth(Vector minus)
        {
            Tensor tensor = new Tensor(Height, Width, Depth);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        tensor[i, j, k] = this[i, j, k] - minus[k];
                    }
                }
            }

            return tensor;
        }
        /// <summary>
        /// ������������ ����������� � ������� �� ������� TNew[i,j,k] = T[i,j,k] + V[k]
        /// </summary>
        public Tensor PlusD(Vector ps)
        {
            Tensor tensor = new Tensor(Height, Width, Depth);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    for (int k = 0; k < Depth; k++)
                    {
                        tensor[i, j, k] = this[i, j, k] + ps[k];
                    }
                }
            }

            return tensor;
        }

        #region ����������

        /// <summary>
        /// ����� ���� ��������� �������
        /// </summary>
        public double Sum()
        {
            double summ = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                summ += Data[i];
            }

            return summ;
        }

        /// <summary>
        /// ������� ���� ��������� �������
        /// </summary>
        public double Mean()
        {
            return Sum() / Shape.Count;
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="mean">������������ �������</param>
        public double Dispersion(double mean)
        {
            double summ = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                summ += Math.Pow(Data[i] - mean, 2);
            }

            return summ / (Shape.Count - 1);
        }

        /// <summary>
        /// ���������
        /// </summary>
        public double Dispersion()
        {
            double mean = Mean();
            double summ = 0;
            int len = Shape.Count;

            for (int i = 0; i < len; i++)
            {
                summ += Math.Pow(Data[i] - mean, 2);
            }

            return summ / (Shape.Count - 1);
        }

        /// <summary>
        /// ������������������ ����������
        /// </summary>
        /// <param name="mean">������������ �������</param>
        public double Std(double mean)
        {
            return Math.Sqrt(Dispersion(mean));
        }

        /// <summary>
        /// ������������������ ����������
        /// </summary>
        public double Std()
        {
            return Math.Sqrt(Dispersion());
        }

        #endregion

        #region ����������� ������

        /// <summary>
        /// ������� ������� � ������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(AISettings.GetProvider());
        }

        /// <summary>
        /// ������� ������� � ������
        /// </summary>
        public string ToString(NumberFormatInfo provider)
        {
            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < Depth; k++)
            {
                sb.Append("Deep #");
                sb.Append(k + 1);
                sb.AppendLine(":");

                for (int i = 0; i < Height; i++)
                {
                    sb.Append("[");
                    for (int j = 0; j < Width; j++)
                    {
                        sb.Append(this[i, j, k].ToString(provider));
                        sb.Append(" ");
                    }
                    sb.Length--;
                    sb.AppendLine("]");
                }
            }

            sb.Length -= Environment.NewLine.Length;
            return sb.ToString();
        }

        /// <summary>
        /// ��������� � ��������
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Tensor tensor)
            {
                return tensor == this;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ��������� � ������ ��������
        /// </summary>
        public bool Equals(Tensor other)
        {
            return this == other;
        }

        /// <summary>
        /// ������� ���-���
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = ((Vector)Data).GetHashCode();
                hash = (hash * 13) + Height;
                hash = (hash * 13) + Width;
                hash = (hash * 13) + Depth;
                return hash;
            }
        }


        #endregion

        #region ������������

        #region ����������
        /// <summary>
        /// ��������� � ����
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// ��������� �  �����
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// ����������� � ���� ������� ����
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return InMemoryDataStream.Create().Write(KeyWords.Tensor).Write(Height).Write(Width).Write(Depth).Write(Data).AsByteArray();
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��������� �� �����
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Tensor Load(string path)
        {
            return BinarySerializer.Load<Tensor>(path);
        }
        /// <summary>
        /// ��������� �� ������
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Tensor Load(Stream stream)
        {
            return BinarySerializer.Load<Tensor>(stream);
        }
        /// <summary>
        /// ��������� �� ������� ����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Tensor FromBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return FromDataStream(InMemoryDataStream.FromByteArray(data));
        }
        /// <summary>
        /// ��������� �� ������� �������  InMemoryDataStream
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public static Tensor FromDataStream(InMemoryDataStream dataStream)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            dataStream.SkipIfEqual(KeyWords.Tensor).ReadInt(out int height).ReadInt(out int width).ReadInt(out int depth).ReadDoubles(out double[] tData);
            Tensor result = new Tensor(height, width, depth)
            {
                Data = tData
            };
            return result;
        }
        #endregion

        #endregion

        #region ��������� ������
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetByIndex(int h, int w, int d)
        {
            return (Width * h) + w + (Height * Width * d);
        }
        #endregion
    }
}