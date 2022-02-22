// <copyright file="LazyObjectiveFunction.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2017 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;
using System;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.ObjectiveFunctions
{
    internal class LazyObjectiveFunction : IObjectiveFunction
    {
        private readonly Func<VectorMathNet<double>, double> _function;
        private readonly Func<VectorMathNet<double>, VectorMathNet<double>> _gradient;
        private readonly Func<VectorMathNet<double>, MatrixMathNet<double>> _hessian;
        private VectorMathNet<double> _point;
        private bool _hasFunctionValue;
        private double _functionValue;
        private bool _hasGradientValue;
        private VectorMathNet<double> _gradientValue;
        private bool _hasHessianValue;
        private MatrixMathNet<double> _hessianValue;

        public LazyObjectiveFunction(Func<VectorMathNet<double>, double> function, Func<VectorMathNet<double>, VectorMathNet<double>> gradient = null, Func<VectorMathNet<double>, MatrixMathNet<double>> hessian = null)
        {
            _function = function;
            _gradient = gradient;
            _hessian = hessian;

            IsGradientSupported = gradient != null;
            IsHessianSupported = hessian != null;
        }

        public IObjectiveFunction CreateNew()
        {
            return new LazyObjectiveFunction(_function, _gradient, _hessian);
        }

        public IObjectiveFunction Fork()
        {
            // no need to deep-clone values since they are replaced on evaluation
            return new LazyObjectiveFunction(_function, _gradient, _hessian)
            {
                _point = _point,
                _hasFunctionValue = _hasFunctionValue,
                _functionValue = _functionValue,
                _hasGradientValue = _hasGradientValue,
                _gradientValue = _gradientValue,
                _hasHessianValue = _hasHessianValue,
                _hessianValue = _hessianValue
            };
        }

        public bool IsGradientSupported { get; }
        public bool IsHessianSupported { get; }

        public void EvaluateAt(VectorMathNet<double> point)
        {
            _point = point;
            _hasFunctionValue = false;
            _hasGradientValue = false;
            _hasHessianValue = false;

            // don't keep references unnecessarily
            _gradientValue = null;
            _hessianValue = null;
        }

        public VectorMathNet<double> Point => _point;

        public double Value
        {
            get
            {
                if (!_hasFunctionValue)
                {
                    _functionValue = _function(_point);
                    _hasFunctionValue = true;
                }
                return _functionValue;
            }
        }

        public VectorMathNet<double> Gradient
        {
            get
            {
                if (!_hasGradientValue)
                {
                    _gradientValue = _gradient(_point);
                    _hasGradientValue = true;
                }
                return _gradientValue;
            }
        }

        public MatrixMathNet<double> Hessian
        {
            get
            {
                if (!_hasHessianValue)
                {
                    _hessianValue = _hessian(_point);
                    _hasHessianValue = true;
                }
                return _hessianValue;
            }
        }
    }
}
