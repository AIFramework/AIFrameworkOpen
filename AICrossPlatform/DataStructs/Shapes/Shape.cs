using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Представляет собой многомерную форму
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Rank = {Rank}")]
    public class Shape : IEquatable<Shape>
    {
        #region Поля и свойства
        private readonly int[] _values;
        private readonly int _count = 1;

        /// <summary>
        /// Получить длину в определенном измерении
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public int this[int dimension] => _values[dimension];
        /// <summary>
        /// Ранг тензора (количество его измерений)
        /// </summary>
        public int Rank => _values.Length;
        /// <summary>
        /// Количество элементов в форме (произведение длин всех измерений)
        /// </summary>
        public int Count => _count;
        #endregion

        /// <summary>
        ///Создает многомерную форму
        /// </summary>
        /// <param name="values"></param>
        public Shape(params int[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length == 0)
            {
                throw new ArgumentException("Shape can't be empty", nameof(values));
            }

            _values = new int[values.Length];
            Array.Copy(values, _values, values.Length);

            foreach (int val in _values)
            {
                _count *= val;
            }
        }

        #region Операторы
        public static bool operator ==(Shape shape1, Shape shape2)
        {
            bool sh1N = Equals(shape1, null);
            bool sh2N = Equals(shape2, null);

            if (sh1N && sh2N)
            {
                return true;
            }
            else if ((sh1N && !sh2N) || (!sh1N && sh2N))
            {
                return false;
            }

            if (shape1.Rank != shape2.Rank || shape1.Count != shape2.Count)
            {
                return false;
            }

            for (int i = 0; i < shape1.Rank; i++)
            {
                if (shape1[i] != shape2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Shape shape1, Shape shape2)
        {
            if (shape1.Rank != shape2.Rank || shape1.Count != shape2.Count)
            {
                return true;
            }

            for (int i = 0; i < shape1.Rank; i++)
            {
                if (shape1[i] == shape2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Методы
        /// <summary>
        /// Returns shape with 1 less dimension
        /// </summary>
        /// <returns></returns>
        public virtual Shape Shrink()
        {
            if (Rank == 1)
            {
                throw new InvalidOperationException("Can't shrink a shape of 1 dimension");
            }

            return new Shape(_values.Take(_values.Length - 1).ToArray());
        }
        /// <summary>
        /// Expands shape with 1 more dimension
        /// </summary>
        /// <param name="newDimensionLength"></param>
        /// <returns></returns>
        public virtual Shape Expand(int newDimensionLength)
        {
            System.Collections.Generic.List<int> newShape = _values.ToList();
            newShape.Add(newDimensionLength);
            return new Shape(newShape.ToArray());
        }
        /// <summary>
        /// Returns true if shapes has equal element count and only differs by dimenson count 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool FuzzyEquals(Shape other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            int lowerRank = Rank < other.Rank ? Rank : other.Rank;

            for (int i = 0; i < lowerRank; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Returns current object data copy as an array
        /// </summary>
        /// <returns></returns>
        public int[] GetDataCopy()
        {
            int[] data = new int[_values.Length];
            Array.Copy(_values, data, _values.Length);
            return data;
        }
        #endregion

        #region Технические методы
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < Rank; i++)
            {
                sb.Append($"D{i}: {_values[i]}, ");
            }

            sb.Length -= 2;
            sb.Append("]");

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Shape shape)
            {
                return shape == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Shape other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (int val in _values)
                {
                    hash = (hash * 23) + val.GetHashCode();
                }
                return hash;
            }
        }
        #endregion
    }
}