using AI.Charts.Data;
using AI.DataStructs.Algebraic;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AI.Charts.ChartElements
{
    [Serializable]
    internal class BaseChart : IChartElement
    {
        public string Name { get; private set; }
        public Series Series { get; private set; }
        private readonly Chart _chart;

        protected IData data;
        public IData Data => data;



        public BaseChart(Chart chart)
        {
            Name = string.Empty;
            Series = new Series();
            chart.Legends[0].Enabled = false;
            chart.Series.Add(Series);
            _chart = chart;
        }

        public BaseChart(Chart chart, string name)
        {
            Name = name;
            Series = new Series(name);

            if (name == string.Empty)
            {
                chart.Legends[0].Enabled = false;
            }
            else
            {
                chart.Legends[0].Enabled = true;
            }

            chart.Series.Add(Series);
            _chart = chart;
        }

        public virtual void SetColor(Color color)
        {
            Series.Color = color;
        }

        public virtual void SetWidth(int width)
        {
            Series.BorderWidth = width;
        }

        public virtual void LoadData(Vector x, Vector y)
        {
            //if (x.Count < 70000)
            //{
            data = new VectorBasedData();
            data.LoadData(x, y);
            //}
        }

        public void LoadData(IData data)
        {
            LoadData(data.GetX(), data.GetY());
        }

        public virtual void Recalc(double min, double max)
        {
            if (_chart.InvokeRequired)
            {
                _chart.BeginInvoke((MethodInvoker)(() =>
                {
                    Series.Points.Clear();

                    int minI = data.IndexValueNeighborhoodMin(min);
                    int maxI = data.IndexValueNeighborhoodMin(max);

                    if (minI != 0)
                    {
                        minI--;
                    }

                    if (maxI != data.Count - 1)
                    {
                        maxI++;
                    }

                    Vector xN = data.GetRegionX(minI, maxI);
                    Vector yN = data.GetRegionY(minI, maxI);

                    Tuple<Vector, Vector> dat = ReducMethod(xN, yN);
                    xN = dat.Item1;
                    yN = dat.Item2;

                    for (int j = 0; j < xN.Count; j++)
                    {
                        Series.Points.AddXY(xN[j], yN[j]);
                    }
                }));
            }
            else 
            {
                Series.Points.Clear();

                int minI = data.IndexValueNeighborhoodMin(min);
                int maxI = data.IndexValueNeighborhoodMin(max);

                if (minI != 0)
                {
                    minI--;
                }

                if (maxI != data.Count - 1)
                {
                    maxI++;
                }

                Vector xN = data.GetRegionX(minI, maxI);
                Vector yN = data.GetRegionY(minI, maxI);

                Tuple<Vector, Vector> dat = ReducMethod(xN, yN);
                xN = dat.Item1;
                yN = dat.Item2;

                for (int j = 0; j < xN.Count; j++)
                {
                    Series.Points.AddXY(xN[j], yN[j]);
                }
            }
        }


        public virtual Tuple<Vector, Vector> ReducMethod(Vector xN, Vector yN)
        {
            return DataMethods.ReducDataPlot(xN, yN);
        }

        public double GetXMin()
        {
            return data.MinX();
        }

        public double GetXMax()
        {
            return data.MaxX();
        }

        public double GetYMin()
        {
            return data.MinY();
        }

        public double GetYMax()
        {
            return data.MaxY();
        }
    }
}
