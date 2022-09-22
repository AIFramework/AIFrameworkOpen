using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Models
{
    /// <summary>
    /// Граф автоматического дифференцирования
    /// </summary>
    public interface INNWGraph
    {
        /// <summary>
        /// Просчитывать ли обратный проход со взятием производных
        /// </summary>
        bool IsBackward { get; set; }
        /// <summary>
        /// Элементы (собранные делегаты) для обратного прохода
        /// </summary>
        List<IBackwardRun> Backprop { get; set; }

        /// <summary>
        /// Backward pass (differentiation)
        /// </summary>
        void Backward();
        /// <summary>
        /// Restart
        /// </summary>
        /// <param name="isBacward">  Is there a backward pass</param>
        void Restart(bool isBacward);
        /// <summary>
        /// Copy tensor
        /// </summary>
        /// <param name="value">Tensor</param>
        /// <param name="count">Number of copies</param>
        NNValue Copyist(NNValue value, int count);
        /// <summary>
        /// Dot product of two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        NNValue ScalarProduct(NNValue v1, NNValue v2);
        /// <summary>
        /// Concatenate two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        NNValue ConcatinateVectors(NNValue v1, NNValue v2);
        /// <summary>
        /// Конкатенация (последовательное соединение) векторов
        /// </summary>
        NNValue ConcatinateVectors(NNValue[] m);
        /// <summary>
        /// Cutting a vector into vectors by 1 element
        /// </summary>
        NNValue[] DeConcatinateOne(NNValue vector);
        /// <summary>
        /// Активационная функция
        /// </summary>
        /// <param name="function">Активационная, нелинейная функция</param>
        /// <param name="t">The tensor to which it is applied</param>
        NNValue Activate(IActivation function, NNValue t);
        /// <summary>
        /// Adding an item
        /// </summary>
        /// <param name="old">Buffer old state</param>
        /// <param name="inp">Added element</param>
        /// <param name="len">Buffer capacity</param>
        NNValue AddCicleBuff(NNValue old, NNValue inp, int len);
        /// <summary>
        /// Matrix multiplication
        /// </summary>
        NNValue Mul(NNValue m1, NNValue m2);
        /// <summary>
        /// Matrix-vector multiplication
        /// </summary>
        NNValue MulMV(NNValue matrix, NNValue vect);
        /// <summary>
        /// Addition of two tensors
        /// </summary>
        NNValue Add(NNValue t1, NNValue t2);
        /// <summary>
        /// Adding a tensor to a number
        /// </summary>
        /// <param name="tensor">Tensor</param>
        /// <param name="number">Number</param>
        NNValue AddN(NNValue tensor, NNValue number);
        /// <summary>
        /// Addition of three tensors
        /// </summary>
        /// <param name="t1">Tensor #1</param>
        /// <param name="t2">Tensor #2</param>
        /// <param name="t3">Tensor #3</param>
        NNValue Add(NNValue t1, NNValue t2, NNValue t3);
        /// <summary>
        /// 1-m
        /// </summary>
        NNValue OneMinus(NNValue tensor);
        /// <summary>
        /// Subtraction
        /// </summary>
        NNValue Subtract(NNValue tensor1, NNValue tensor2);
        /// <summary>
        /// Multiplication by a number
        /// </summary>
        NNValue MulMatrixByNumber(NNValue tensor, float s);
        /// <summary>
        /// Multiplication by a number
        /// </summary>
        NNValue MulMatrixByNumber(float s, NNValue m);
        /// <summary>
        /// Tensor inversion -m
        /// </summary>
        NNValue Invers(NNValue tensor);
        /// <summary>
        /// Adamar product of tensors
        /// </summary>
        NNValue AdamarMul(NNValue tensor1, NNValue tensor2);
        /// <summary>
        /// Convolution without bias
        /// </summary>
        NNValue Convolution(NNValue input, NNValue[] filters, int padX, int padY, int strideX, int strideY);
        /// <summary>
        /// Convolution
        /// </summary>
        NNValue Convolution(NNValue input, NNValue[] filters, NNValue bias, int padX, int padY, int strideX, int strideY);
        /// <summary>
        /// Max pooling 2D
        /// </summary>
        NNValue MaxPooling(NNValue inp, int kh, int kw);
        /// <summary>
        /// Tensor shape transformation
        /// </summary>
        NNValue ReShape(NNValue input, Shape3D newShape, float gain);
        /// <summary>
        /// UnPooling 2D
        /// </summary>
        NNValue UnPooling(NNValue inp, int h, int w);
        /// <summary>
        /// Upsampling with bicubic interpolation
        /// </summary>
        NNValue Upsampling2DBicubic(NNValue inp, int h, int w);
        /// <summary>
        /// Dropout
        /// </summary>
        NNValue DropOut(NNValue input, float q, float n, Random random);
        /// <summary>
        /// Mixing real and imaginary parts to create a new real and imaginary part
        /// </summary>
        NNValue[] ImRealCross(NNValue real, NNValue im, NNValue alpha1, NNValue beta1, NNValue gama1, NNValue alpha2, NNValue beta2, NNValue gama2);
        /// <summary>
        /// Tensor depth join
        /// </summary>
        NNValue DeepJoin(NNValue[] values);
        /// <summary>
        /// Splitting a tensor by depth
        /// </summary>
        NNValue[] DeepSplit(NNValue data, int countLayersInSlice);
        /// <summary>
        /// Feedforward layer
        /// </summary>
        NNValue FeedForwardLayer(NNValue input, NNValue W, NNValue bias, IActivation activation);
        /// <summary>
        /// Feedforward linear layer
        /// </summary>
        NNValue FeedforwardLinLayer(NNValue input, NNValue W, NNValue bias);
        /// <summary>
        /// GRU layer
        /// </summary>
        NNValue GRULayer(
            NNValue input,
            NNValue hmix,
            NNValue hHmix,
            NNValue bmix,
            NNValue hnew,
            NNValue hHnew,
            NNValue bnew,
            NNValue hreset,
            NNValue hHreset,
            NNValue breset,
            NNValue context,
            SigmoidUnit fMix,
            SigmoidUnit fReset,
            TanhUnit fNew
            );
    }
}