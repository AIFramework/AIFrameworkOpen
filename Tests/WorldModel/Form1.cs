using AI.Charts.Control;
using AI.DataStructs.Algebraic;
using AI.Dog.Tools;
using AI.ML.HMM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace WorldModel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Matrix matrixState = new Matrix(512, 512);
            int[] states = new int[512];

            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    double p = 1;
                    if (i != j)
                        p = 0.9;
                    matrixState[i, j] = p*Math.Max(0, Simillary.CorrelationIntInt(i, j));
                    if (Simillary.Bools2Vect(i.DecimalToGrayBits(9)).Sum() != Simillary.Bools2Vect(j.DecimalToGrayBits(9)).Sum())
                        matrixState[i, j] *= 0.0002;
                }

                states[i] = i;
            }


           
            //matrixState = matrixState.AdamarProduct(matrixState);
            //matrixState = matrixState.AdamarProduct(matrixState);

            hmm.stateMatrix = matrixState;
            hmm.stateAlter = 1 - matrixState - 0.000001;
            hmm.states = states;
            colums = Matrix.GetColumns(matrixState);
        }

        HMM hmm = new HMM();
        int ind = 2, ind2 = 2;
        Vector[] colums;


        private void ShowH(int state, HeatMapControl heatMapControl) 
        {
            Vector doubles = Simillary.Bools2Vect(state.DecimalToGrayBits(9));
            Matrix matrix = new Matrix(3, 3);
            matrix.Data = doubles;
            heatMapControl.CalculateHeatMap(matrix);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            chartVisual1.BarBlack(colums[ind] / colums[ind].Sum());
            chartVisual2.BarBlack(colums[ind2] / colums[ind2].Sum());

            ind = hmm.Generate(2, ind)[1];
            ind2 = hmm.Generate(2, ind2)[1];


            ShowH(ind, heatMapControl1);
            ShowH(ind2, heatMapControl2);
        }
    }
}
