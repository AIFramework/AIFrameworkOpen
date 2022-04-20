/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 07.02.2016
 * Time: 18:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;

namespace AI.HightLevelFunctions
{
    /// <summary>
    /// Analytic geometry
    /// </summary>
    public static class AnalyticGeometryFunctions
    {
        /// <summary>
        /// Cosine of the angle between two vectors
        /// </summary>
        static public double Cos(Vector vector1, Vector vector2)
        {
            return Dot(vector1, vector2) / Math.Sqrt(Dot(vector2, vector2) * Dot(vector1, vector1));
        }
        /// <summary>
        /// Calculates the Euclidean norm of a vector
        /// </summary>
        static public double NormVect(Vector vector)
        {
            return Math.Sqrt(Functions.Summ(vector * vector));
        }
        /// <summary>
        /// Dot product of 2 vectors
        /// </summary>
        static public double Dot(Vector vector, Vector vector2)
        {
            double dot = 0;

            for (int i = 0; i < vector.Count; i++)
            {
                dot += vector[i] * vector2[i];
            }

            return dot;
        }
        /// <summary>
        /// Projection of vector A onto vector B
        /// </summary>
        static public Vector ProectionAtoB(Vector A, Vector B)
        {
            double k = Dot(A, B) / Dot(B, B);
            return k * B;
        }
        /// <summary>
        /// Angle between vectors
        /// </summary>
        /// <returns>Returns the angle in radians</returns>
        static public double AngleVect(Vector vector, Vector vector2)
        {
            double a = Dot(vector, vector2), b = NormVect(vector) * NormVect(vector2);
            return Math.Acos(a / b);
        }
        /// <summary>
        /// Calculates the vector connecting point A to point B
        /// </summary>
        static public Vector VectorFromAToB(Vector pointA, Vector pointB)
        {
            if (pointA.Count != pointB.Count)
            {
                throw new ArgumentException("The dimensions of the points do not match");
            }

            return pointB - pointA;
        }
        /// <summary>
        /// Calculates the distance from point A to B
        /// </summary>
        static public double DistanceFromAToB(Vector pointA, Vector pointB)
        {
            return NormVect(VectorFromAToB(pointA, pointB));
        }
        /// <summary>
        /// Rotate a vector by specified angles
        /// </summary>
        static public Vector VectorRotate(Vector inp, double angl, int indAx1, int indAx2)
        {
            Matrix rotateMatr = new Matrix(inp.Count, inp.Count);

            for (int i = 0; i < inp.Count; i++)
            {
                rotateMatr[i, i] = 1;
            }

            rotateMatr[indAx1, indAx1] = Math.Cos(angl);
            rotateMatr[indAx2, indAx2] = Math.Cos(angl);
            rotateMatr[indAx1, indAx2] = -Math.Sin(angl);
            rotateMatr[indAx1, indAx1] = Math.Sin(angl);

            Matrix vectorInp = inp.ToMatrix().Transpose();

            return (rotateMatr * vectorInp).Transpose().LikeVector();
        }
    }
}