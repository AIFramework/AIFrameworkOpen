using AI.DataStructs.Algebraic;
using System;

namespace AI.Charts.Data
{
    [Serializable]
    internal class VectorBasedData : IData
    {
        private Vector _x = new Vector(0), _y = new Vector(0);

        public int Count { get; set; }

        public Vector GetRegionX(int a, int b)
        {
            try
            {
                return _x.GetInterval(a, b + 1);
            }
            catch
            {
                return _x.GetInterval(a, b);
            }
        }

        public Vector GetRegionY(int a, int b)
        {
            try
            {
                return _y.GetInterval(a, b + 1);
            }
            catch
            {
                return _y.GetInterval(a, b);
            }
        }

        public Vector GetX()
        {
            return _x;
        }

        public Vector GetY()
        {
            return _y;
        }

        public int IndexValueNeighborhoodMin(double value)
        {
            return _x.IndexValueNeighborhoodMin(value);
        }

        public void LoadData(Vector x, Vector y)
        {
            _x = x.Clone();
            _y = y.Clone();
            Count = _x.Count;
        }




        public double MaxX()
        {
            return _x.Max();
        }

        public double MaxY()
        {
            return _y.Max();
        }

        public double MinX()
        {
            return _x.Min();
        }

        public double MinY()
        {
            return _y.Min();
        }
    }
}
