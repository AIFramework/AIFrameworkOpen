using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;

namespace AI.ML.Regression
{
    /// <summary>
    /// Description of PolynomialRegression.
    /// </summary>
    [Serializable]
    public class PolynomialRegression
    {
        private readonly MultipleRegression mR;
        private readonly int _nPoly;
        /// <summary>
        /// Полиномиальная регрессия
        /// </summary>
        public PolynomialRegression(Vector inp, Vector outp, int nPoly = 3)
        {
            _nPoly = nPoly;
            Vector[] vects = new Vector[inp.Count];

            for (int i = 0; i < inp.Count; i++)
            {
                vects[i] = ExtensionOfFeatureSpace.Polinomial(inp[i], nPoly);
            }

            mR = new MultipleRegression(true);
            mR.Train(vects, outp);
        }


        /// <summary>
        /// Прогноз
        /// </summary>
        /// <param name="inp">Значение незав. переменной</param>
        public double Predict(double inp)
        {
            Vector X = ExtensionOfFeatureSpace.Polinomial(inp, _nPoly);
            return mR.Predict(X);
        }

        /// <summary>
        /// Прогноз
        /// </summary>
        /// <param name="vect">Значения незав. переменных</param>
        public Vector Predict(Vector vect)
        {
            Vector outp = new Vector(vect.Count);

            for (int i = 0; i < vect.Count; i++)
            {
                outp[i] = Predict(vect[i]);
            }

            return outp;
        }

    }
}
