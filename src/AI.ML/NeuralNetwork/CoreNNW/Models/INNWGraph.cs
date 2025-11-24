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
        /// Обратный проход (дифференцирование)
        /// </summary>
        void Backward();
        /// <summary>
        /// Перезапуск
        /// </summary>
        /// <param name="isBacward">Выполнять ли обратны проход</param>
        void Restart(bool isBacward);
        /// <summary>
        /// Копирование тензора
        /// </summary>
        /// <param name="value">Тензор</param>
        /// <param name="count">Число копий</param>
        NNValue Copyist(NNValue value, int count);
        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        /// <param name="v1">Первый вектор</param>
        /// <param name="v2">Второй вектор</param>
        NNValue ScalarProduct(NNValue v1, NNValue v2);
        /// <summary>
        /// Конкатенация двух векторов
        /// </summary>
        /// <param name="v1">Первый вектор</param>
        /// <param name="v2">Второй вектор</param>
        NNValue ConcatinateVectors(NNValue v1, NNValue v2);
        /// <summary>
        /// Конкатенация (последовательное соединение) векторов
        /// </summary>
        NNValue ConcatinateVectors(NNValue[] m);
        /// <summary>
        /// Нарезка вектора по одному элементу
        /// </summary>
        NNValue[] DeConcatinateOne(NNValue vector);
        /// <summary>
        /// Активационная функция
        /// </summary>
        /// <param name="function">Активационная, нелинейная функция</param>
        /// <param name="t">Тензор, на котором применить активацию</param>
        NNValue Activate(IActivation function, NNValue t);
        /// <summary>
        /// Добавить элемент
        /// </summary>
        /// <param name="old">Буфер старых значений</param>
        /// <param name="inp">Добавляемый элемент</param>
        /// <param name="len">Емкость буфера</param>
        NNValue AddCicleBuff(NNValue old, NNValue inp, int len);
        /// <summary>
        /// Перемножение матриц
        /// </summary>
        NNValue Mul(NNValue m1, NNValue m2);
        /// <summary>
        /// Умножение матрицы на вектор-столбец
        /// </summary>
        NNValue MulMV(NNValue matrix, NNValue vect);
        /// <summary>
        /// Сумма 2х тензоров
        /// </summary>
        NNValue Add(NNValue t1, NNValue t2);
        /// <summary>
        /// Сумма тензора и числа
        /// </summary>
        /// <param name="tensor">Тензор</param>
        /// <param name="number">Число</param>
        NNValue AddN(NNValue tensor, NNValue number);
        /// <summary>
        /// Сумма 3х тензоров
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
        /// Вычитание
        /// </summary>
        NNValue Subtract(NNValue tensor1, NNValue tensor2);
        /// <summary>
        /// Умножение на число
        /// </summary>
        NNValue MulMatrixByNumber(NNValue tensor, float s);
        /// <summary>
        /// Умножение на число
        /// </summary>
        NNValue MulMatrixByNumber(float s, NNValue m);
        /// <summary>
        /// Инверсия -m
        /// </summary>
        NNValue Invers(NNValue tensor);
        /// <summary>
        /// Поэлементное(адамарово) произведение тензоров
        /// </summary>
        NNValue AdamarMul(NNValue tensor1, NNValue tensor2);
        /// <summary>
        /// Свертка без нейрона смещения
        /// </summary>
        NNValue Convolution(NNValue input, NNValue[] filters, int padX, int padY, int strideX, int strideY);
        /// <summary>
        /// Свертка
        /// </summary>
        NNValue Convolution(NNValue input, NNValue[] filters, NNValue bias, int padX, int padY, int strideX, int strideY);
        /// <summary>
        /// Подвыборка 2D
        /// </summary>
        NNValue MaxPooling(NNValue inp, int kh, int kw);
        /// <summary>
        /// Изменение формы тензора
        /// </summary>
        NNValue ReShape(NNValue input, Shape3D newShape, float gain);
        /// <summary>
        /// UnPooling 2D
        /// </summary>
        NNValue UnPooling(NNValue inp, int h, int w);
        /// <summary>
        /// Апсемплинг с бикубической интерполяцией
        /// </summary>
        NNValue Upsampling2DBicubic(NNValue inp, int h, int w);
        /// <summary>
        /// Dropout
        /// </summary>
        NNValue DropOut(NNValue input, float q, float n, Random random);
        /// <summary>
        /// Смешивание реальной и мнимой частей для создания новой реальной и мнимой частей
        /// </summary>
        NNValue[] ImRealCross(NNValue real, NNValue im, NNValue alpha1, NNValue beta1, NNValue gama1, NNValue alpha2, NNValue beta2, NNValue gama2);
        /// <summary>
        /// Соединение глубины тензора
        /// </summary>
        NNValue DeepJoin(NNValue[] values);
        /// <summary>
        /// Разделение тензора по глубине
        /// </summary>
        NNValue[] DeepSplit(NNValue data, int countLayersInSlice);
        /// <summary>
        /// Полносвязный слой
        /// </summary>
        NNValue FeedForwardLayer(NNValue input, NNValue W, NNValue bias, IActivation activation);
        /// <summary>
        /// Линейный полносвязный слой
        /// </summary>
        NNValue FeedforwardLinLayer(NNValue input, NNValue W, NNValue bias);
        /// <summary>
        /// Слой gru
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