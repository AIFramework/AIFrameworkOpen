using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW
{
    /// <summary>
    /// Граф автоматического дифференцирования реализация для ЦП
    /// </summary>
    [Serializable]
    public class NNWGraphCPU : INNWGraph
    {
        /// <summary>
        /// Просчитывать ли обратный проход со взятием производных
        /// </summary>
        public bool IsBackward { get; set; }
        /// <summary>
        /// Элементы (собранные делегаты) для обратного прохода (функции расчета градиента)
        /// </summary>
        public List<IBackwardRun> Backprop { get; set; }
        /// <summary>
        /// Граф автоматического дифференцирования реализация для ЦП
        /// </summary>
        public NNWGraphCPU() : this(false)
        {
        }
        /// <summary>
        /// Граф автоматического дифференцирования реализация для ЦП
        /// </summary>
        public NNWGraphCPU(bool isBakward)
        {
            IsBackward = isBakward;
            Backprop = new List<IBackwardRun>();
        }
        /// <summary>
        /// Обратный проход (дифференцирование)
        /// </summary>
        public virtual void Backward()
        {
            for (int i = Backprop.Count - 1; i >= 0; i--)
            {
                Backprop[i].StartCalc();
            }
        }
        /// <summary>
        /// Перезапуск графа
        /// </summary>
        /// <param name="isBacward">Просчитывать ли обратный проход со взятием производных</param>
        public virtual void Restart(bool isBacward)
        {
            IsBackward = isBacward;
            Backprop.Clear();
        }
        /// <summary>
        /// Копирование тензора
        /// </summary>
        /// <param name="value">Тензор</param>
        /// <param name="count">Число копий</param>
        public virtual NNValue Copyist(NNValue value, int count)
        {
            int len = value.Shape.Count * count;
            NNValue returnObj = new NNValue(len);

            for (int i = 0, ind = 0; i < count; i++)
            {
                for (int j = 0; j < value.Shape.Count; j++)
                {
                    returnObj[ind] = value[j];
                    returnObj.DifData[ind] = value.DifData[j];
                    returnObj.StepCache[ind] = value.StepCache[j];
                    returnObj.StepCache2[ind] = value.StepCache2[j];
                    ind++;
                }
            }
            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0, ind = 0; i < count; i++)
                        {
                            for (int j = 0; j < value.Shape.Count; j++)
                            {
                                value.DifData[j] += returnObj.DifData[ind];
                            }
                        }

                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        /// <param name="v1">Первый вектор</param>
        /// <param name="v2">Второй вектор</param>
        public virtual NNValue ScalarProduct(NNValue v1, NNValue v2)
        {
            if (v1.Shape.Count != v2.Shape.Count)
            {
                throw new InvalidOperationException("NNValues dimensions mismatche");
            }

            NNValue returnObj = new NNValue(1);

            _ = Parallel.For(0, v1.Shape.Count, i =>
            {
                returnObj[0] += v1[i] * v2[i];
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        float d = returnObj.DifData[0];

                        _ = Parallel.For(0, v1.Shape.Count, i =>
                        {
                            v2.DifData[i] += v1[i] * d;
                            v1.DifData[i] += v2[i] * d;
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Добавить элемент
        /// </summary>
        /// <param name="old">Буфер старых значений</param>
        /// <param name="inp">Добавляемый элемент</param>
        /// <param name="len">Емкость буфера</param>
        public virtual NNValue AddCicleBuff(NNValue old, NNValue inp, int len)
        {
            NNValue data = new NNValue(len);

            _ = Parallel.For(1, len, i =>
            {
                data[i] = old[i - 1];
                data.DifData[i] = old.DifData[i - 1];
                data.StepCache[i] = old.StepCache[i - 1];
                data.StepCache2[i] = old.StepCache2[i - 1];
            });

            data[0] = inp[0];

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        inp.DifData[0] += data.DifData[0];

                        _ = Parallel.For(1, len, i =>
                        {
                            old.DifData[i - 1] = data.DifData[i];
                        });
                    }
                };

                Backprop.Add(bp);
            }
            return data;

        }
        /// <summary>
        /// Конкатенация (последовательное соединение) векторов
        /// </summary>
        public virtual NNValue ConcatinateVectors(NNValue[] m)
        {
            int len = 0;

            for (int i = 0; i < m.Length; i++)
            {
                len += m[i].Shape.Count;
            }

            NNValue returnObj = new NNValue(len);

            int loc = 0;

            for (int j = 0; j < m.Length; j++)
            {
                for (int i = 0; i < m[j].Shape.Count; i++)
                {
                    returnObj[loc] = m[j][i];
                    returnObj.DifData[loc] = m[j].DifData[i];
                    returnObj.StepCache[loc] = m[j].StepCache[i];
                    returnObj.StepCache2[loc] = m[j].StepCache2[i];
                    loc++;
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        int index0 = 0;
                        for (int j = 0; j < m.Length; j++)
                        {

                            for (int i = 0; i < m[j].Shape.Count; i++)
                            {
                                m[j][i] = returnObj[index0];
                                m[j].DifData[i] = returnObj.DifData[index0];
                                m[j].StepCache[i] = returnObj.StepCache[index0];
                                m[j].StepCache2[i] = returnObj.StepCache2[index0];
                                index0++;
                            }
                        }
                    }
                };

                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Нарезка вектора по одному элементу
        /// </summary>
        public virtual NNValue[] DeConcatinateOne(NNValue vector)
        {
            int len = vector.Shape.Count;

            NNValue[] returnObj = new NNValue[len];

            for (int i = 0; i < len; i++)
            {
                returnObj[i] = new NNValue(1);
                returnObj[i][0] = vector[i];
                returnObj[i].DifData[0] = vector.DifData[i];
                returnObj[i].StepCache[0] = vector.StepCache[i];
                returnObj[i].StepCache2[0] = vector.StepCache2[i];
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        int index0 = 0;

                        _ = Parallel.For(0, len, i =>
                        {
                            vector[i] = returnObj[i][index0];
                            vector.DifData[i] = returnObj[i].DifData[index0];
                            vector.StepCache[i] = returnObj[i].StepCache[index0];
                            vector.StepCache2[i] = returnObj[i].StepCache2[index0];
                        });
                    }
                };

                Backprop.Add(bp);
            }

            return returnObj;
        }
        /// <summary>
        /// Конкатенация двух векторов
        /// </summary>
        /// <param name="v1">Первый вектор</param>
        /// <param name="v2">Второй вектор</param>
        public virtual NNValue ConcatinateVectors(NNValue v1, NNValue v2)
        {
            if (v1.Shape.Width > 1 || v2.Shape.Width > 1)
            {
                throw new InvalidOperationException("Expected column vectors");
            }

            NNValue returnObj = new NNValue(v1.Shape.Height + v2.Shape.Height);

            int loc = 0;
            if (v1.DifData != null)
            {
                for (int i = 0; i < v1.Shape.Count; i++)
                {
                    returnObj[loc] = v1[i];
                    returnObj.DifData[loc] = v1.DifData[i];
                    returnObj.StepCache[loc] = v1.StepCache[i];
                    returnObj.StepCache2[loc] = v1.StepCache2[i];
                    loc++;
                }
                for (int i = 0; i < v2.Shape.Count; i++)
                {
                    returnObj[loc] = v2[i];
                    returnObj.DifData[loc] = v2.DifData[i];
                    returnObj.StepCache[loc] = v2.StepCache[i];
                    returnObj.StepCache2[loc] = v2.StepCache2[i];
                    loc++;
                }
            }
            else // Если используется модификация "только использование"
            {
                loc = 0;
                for (int i = 0; i < v1.Shape.Count; i++)
                {
                    returnObj[loc++] = v1[i];
                }
                for (int i = 0; i < v2.Shape.Count; i++)
                {
                    returnObj[loc++] = v2[i];
                }
            }
            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        int index0 = 0;
                        for (int i = 0; i < v1.Shape.Count; i++)
                        {
                            v1.Data[i] = returnObj[index0];
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                            v1.DifData[i] = returnObj.DifData[index0];
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                            v1.StepCache[i] = returnObj.StepCache[index0];
                            v1.StepCache2[i] = returnObj.StepCache2[index0];
                            index0++;
                        }
                        for (int i = 0; i < v2.Shape.Count; i++)
                        {
                            v2.Data[i] = returnObj[index0];
                            v2.DifData[i] = returnObj.DifData[index0];
                            v2.StepCache[i] = returnObj.StepCache[index0];
                            v2.StepCache2[i] = returnObj.StepCache2[index0];
                            index0++;
                        }
                    }
                };

                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Активационная функция
        /// </summary>
        /// <param name="function">Function activation</param>
        /// <param name="t">Тензор, на котором применить активацию</param>
        public virtual unsafe NNValue Activate(IActivation function, NNValue t)
        {
            NNValue returnObj = new NNValue(t.Shape.Height, t.Shape.Width);
            int n = t.Shape.Count;
            returnObj = function.Forward(t);

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        NNValue data = function.Backward(t);

                        //for (int i = 0; i < data.Data.Length; i++)
                        //    t.DifData[i] += data[i]*returnObj.DifData[i];


                        fixed (float* inpDiff = t.DifData)
                        {
                            fixed (float* dat = data.Data)
                            {
                                fixed (float* outpDiff = returnObj.DifData)
                                {
                                    float* pInDif = inpDiff, pDat = dat, pOutp = outpDiff;

                                    for (int i = 0; i < n; i++)
                                    {
                                        *pInDif += *pDat * *pOutp;
                                        pDat++;
                                        pInDif++;
                                        pOutp++;
                                    }
                                }
                            }
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Перемножение матриц
        /// </summary>
        public virtual NNValue Mul(NNValue m1, NNValue m2)
        {
            if (m1.Shape.Width != m2.Shape.Height)
            {
                throw new ArgumentException("NNValues dimensions mismatche");
            }

            int m1Rows = m1.Shape.Height;
            int m1Cols = m1.Shape.Width;
            int m2Cols = m2.Shape.Width;
            NNValue returnObj = new NNValue(m1Rows, m2Cols);
            int outcols = m2Cols;
            for (int i = 0; i < m1Rows; i++)
            {
                int m1Col = m1Cols * i;
                for (int j = 0; j < m2Cols; j++)
                {
                    float dot = 0;
                    for (int k = 0; k < m1Cols; k++)
                    {
                        dot += m1[m1Col + k] * m2[(m2Cols * k) + j];
                    }
                    returnObj[(outcols * i) + j] = dot;
                }
            }
            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0; i < m1.Shape.Height; i++)
                        {
                            int outcol = outcols * i;
                            for (int j = 0; j < m2.Shape.Width; j++)
                            {
                                float b = returnObj.DifData[outcol + j];
                                for (int k = 0; k < m1.Shape.Width; k++)
                                {
                                    m1.DifData[(m1Cols * i) + k] += m2[(m2Cols * k) + j] * b;
                                    m2.DifData[(m2Cols * k) + j] += m1[(m1Cols * i) + k] * b;
                                }
                            }
                        }

                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Умножение матрицы на вектор-столбец
        /// </summary>
        public virtual unsafe NNValue MulMV(NNValue m1, NNValue vect)
        {
            int m1Rows = m1.Shape.Height;
            int m1Cols = m1.Shape.Width;
            NNValue returnObj = new NNValue(m1Rows, 1);

            fixed (float* rV = returnObj.Data)
            {
                float* pRV = rV; // Указатель на выходной массив

                fixed (float* v = vect.Data)
                {

                    for (int i = 0; i < m1Rows; i++)
                    {
                        float* pV = v;
                        float dot = 0;
                        for (int j = 0; j < m1.Shape.Width; j++)
                        {
                            dot += m1[i, j] * *pV;
                            pV++;
                        }

                        *pRV = dot;
                        pRV++;
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        fixed (float* rVDif = returnObj.DifData)
                        {
                            fixed (float* v = vect.Data)
                            {
                                fixed (float* vDif = vect.DifData)
                                {
                                    float* pRVD = rVDif;

                                    for (int i = 0; i < m1.Shape.Height; i++)
                                    {
                                        float b = *pRVD;
                                        float* pV = v;
                                        float* pVDif = vDif;

                                        for (int j = 0; j < m1.Shape.Width; j++)
                                        {
                                            m1.DifData[(m1Cols * i) + j] += *pV * b;
                                            *pVDif += m1[(m1Cols * i) + j] * b;
                                            pV++; // Следующий элемент входного вектора
                                            pVDif++;
                                        }

                                        pRVD++; //Следующий элемент выходного вектора
                                    }
                                }
                            }
                        }

                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Сумма 2х тензоров
        /// </summary>
        public virtual NNValue Add(NNValue m1, NNValue m2)
        {
            NNValue returnObj = new NNValue(m1.Shape);

            _ = Parallel.For(0, m1.Shape.Count, i =>
            {
                returnObj[i] = m1[i] + m2[i];
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, m1.Shape.Count, i =>
                        {
                            m1.DifData[i] += returnObj.DifData[i];
                            m2.DifData[i] += returnObj.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Сумма 3х тензоров
        /// </summary>
        /// <param name="t1">Tensor #1</param>
        /// <param name="t2">Tensor #2</param>
        /// <param name="t3">Tensor #3</param>
        public virtual unsafe NNValue Add(NNValue t1, NNValue t2, NNValue t3)
        {
            NNValue returnObj = new NNValue(t1.Shape);
            int len = t1.Shape.Count;

            fixed (float* ret = returnObj.Data)
            {
                fixed (float* t1p = t1.Data)
                {
                    fixed (float* t2p = t2.Data)
                    {
                        fixed (float* t3p = t3.Data)
                        {
                            float* pRet = ret, pT1 = t1p, pT2 = t2p, pT3 = t3p;

                            for (int i = 0; i < len; i++)
                            {
                                *pRet = *pT1 + *pT2 + *pT3;

                                pT1++;
                                pT2++;
                                pT3++;
                                pRet++;
                            }
                        }
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        fixed (float* ret = returnObj.DifData)
                        {
                            fixed (float* t1p = t1.DifData)
                            {
                                fixed (float* t2p = t2.DifData)
                                {
                                    fixed (float* t3p = t3.DifData)
                                    {
                                        float* pRet = ret, pT1 = t1p, pT2 = t2p, pT3 = t3p;
                                        for (int i = 0; i < len; i++)
                                        {
                                            *pT1 += *pRet;
                                            *pT2 += *pRet;
                                            *pT3 += *pRet;
                                            pT1++;
                                            pT2++;
                                            pT3++;
                                            pRet++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// 1-m
        /// </summary>
        /// <param name="m">Тензор входных данных</param>
        public virtual NNValue OneMinus(NNValue m)
        {
            NNValue returnObj = new NNValue(m.Shape);

            _ = Parallel.For(0, m.Data.Length, i =>
            {
                returnObj.Data[i] = 1 - m.Data[i];
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, m.Data.Length, i =>
                        {
                            m.DifData[i] -= returnObj.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;

        }
        /// <summary>
        /// Вычитание
        /// </summary>
        public virtual unsafe NNValue Subtract(NNValue m1, NNValue m2)
        {
            NNValue returnObj = new NNValue(m1.Shape);

            fixed (float* ret = returnObj.Data)
            {
                fixed (float* m_1 = m1.Data)
                {
                    fixed (float* m_2 = m2.Data)
                    {
                        float* pR = ret, pM1 = m_1, pM2 = m_2;
                        for (int i = 0; i < m1.Data.Length; i++)
                        {
                            *pR = *pM1 - *pM2;
                            pR++;
                            pM1++;
                            pM2++;
                        }
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, m1.Data.Length, i =>
                        {
                            m1.DifData[i] += returnObj.DifData[i];
                            m2.DifData[i] -= returnObj.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Умножение на число
        /// </summary>
        public virtual NNValue MulMatrixByNumber(NNValue tensor, float s)
        {
            NNValue returnObj = new NNValue(tensor.Shape);

            _ = Parallel.For(0, tensor.Shape.Count, i =>
            {
                returnObj[i] = tensor[i] * s;
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, tensor.Shape.Count, i =>
                        {
                            tensor.DifData[i] += tensor[i] * returnObj.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Умножение на число
        /// </summary>
        public virtual NNValue MulMatrixByNumber(float s, NNValue m)
        {
            NNValue returnObj = MulMatrixByNumber(m, s);
            return returnObj;
        }
        //TODO: Оптимизировать
        /// <summary>
        /// Инверсия -m
        /// </summary>
        public virtual NNValue Invers(NNValue m)
        {
            NNValue returnObj = new NNValue(m.Shape);

            _ = Parallel.For(0, m.Shape.Count, i =>
            {
                returnObj[i] = -m[i];
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, m.Shape.Count, i =>
                        {
                            m.DifData[i] -= returnObj.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Поэлементное(адамарово) произведение тензоров
        /// </summary>
        public virtual unsafe NNValue AdamarMul(NNValue m1, NNValue m2)
        {
            NNValue returnObj = new NNValue(m1.Shape);
            fixed (float* ret = returnObj.Data)
            {
                fixed (float* m_1 = m1.Data)
                {
                    fixed (float* m_2 = m2.Data)
                    {
                        float* pR = ret, pM1 = m_1, pM2 = m_2;
                        for (int i = 0; i < m1.Data.Length; i++)
                        {
                            *pR = *pM1 * *pM2;
                            pR++;
                            pM1++;
                            pM2++;
                        }
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        fixed (float* ret = returnObj.DifData)
                        {
                            fixed (float* m_1 = m1.Data)
                            {
                                fixed (float* m_2 = m2.Data)
                                {
                                    fixed (float* m2Dif = m2.DifData)
                                    {
                                        fixed (float* m1Dif = m1.DifData)
                                        {
                                            float* pR = ret, pM1 = m_1, pM2 = m_2, pM1Dif = m1Dif, pM2Dif = m2Dif;
                                            for (int i = 0; i < m1.Data.Length; i++)
                                            {
                                                *pM2Dif += *pM1 * *pR;
                                                *pM1Dif += *pM2 * *pR;
                                                pR++;
                                                pM1++;
                                                pM2++;
                                                pM2Dif++;
                                                pM1Dif++;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Свертка без нейрона смещения
        /// </summary>
        public virtual NNValue Convolution(NNValue input, NNValue[] filters, int padX, int padY, int strideX, int strideY)
        {
            int outpH, outpW, outpD = filters.Length;

            outpH = ((input.Shape.Height - filters[0].Shape.Height + padY) / strideY) + 1;
            outpW = ((input.Shape.Width - filters[0].Shape.Width + padX) / strideX) + 1;

            if ((outpW < 1) || (outpH < 1))
            {
                throw new Exception("Недостаточная размерность выхода");
            }

            NNValue returnObj = new NNValue(outpH, outpW, outpD);

            _ = Parallel.For(0, outpD, d =>
            {
                NNValue filter = filters[d];
                int y = -padY;

                for (int y1 = 0; y1 < outpH; y1++, y += strideY)
                {
                    int x = -padX;
                    for (int x1 = 0; x1 < outpW; x1++, x += strideX)
                    {
                        float newPixel = 0.0f;
                        for (int dy = 0; dy < filter.Shape.Height; dy++)
                        {
                            int y2 = y + dy;
                            for (int dx = 0; dx < filter.Shape.Width; dx++)
                            {
                                int ox = x + dx;
                                if (y2 >= 0 && y2 < input.Shape.Height && ox >= 0 && ox < input.Shape.Width)
                                {
                                    for (int fd = 0; fd < filter.Shape.Depth; fd++)
                                    {
                                        newPixel += filter[dy, dx, fd] * input[y2, ox, fd];
                                    }
                                }
                            }
                        }

                        returnObj[y1, x1, d] = newPixel;
                    }
                }
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, outpD, d =>
                        {
                            NNValue filter = filters[d];
                            int y = -padY;
                            for (int y1 = 0; y1 < outpH; y1++, y += strideY)
                            {

                                int x = -padX;
                                for (int x1 = 0; x1 < outpW; x1++, x += strideX)
                                {

                                    float delt = returnObj.DifData[x1 + (outpW * y1) + (outpW * outpH * d)];
                                    for (int dy = 0; dy < filter.Shape.Height; dy++)
                                    {
                                        int y2 = y + dy;
                                        for (int dx = 0; dx < filter.Shape.Width; dx++)
                                        {
                                            int ox = x + dx;
                                            if (y2 >= 0 && y2 < input.Shape.Height && ox >= 0 && ox < input.Shape.Width)
                                            {
                                                for (int fd = 0; fd < filter.Shape.Depth; fd++)
                                                {
                                                    filter.DifData[(filter.Shape.Width * dy) + dx + (fd * filter.Shape.Area)] += input[y2, ox, fd] * delt;
                                                    input.DifData[ox + (input.Shape.Width * y2) + (input.Shape.Area * fd)] += filter[dy, dx, fd] * delt;

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Свертка
        /// </summary>
        public virtual NNValue Convolution(NNValue input, NNValue[] filters, NNValue bias, int padX, int padY, int strideX, int strideY)
        {
            int outpH, outpW, outpD = filters.Length;

            outpH = ((input.Shape.Height - filters[0].Shape.Height + padY) / strideY) + 1;
            outpW = ((input.Shape.Width - filters[0].Shape.Width + padX) / strideX) + 1;
            float balanser = outpH * outpD;
            balanser = 1.0f / balanser;

            if ((outpW < 1) || (outpH < 1))
            {
                throw new Exception("Недостаточная размерность выхода");
            }

            NNValue returnObj = new NNValue(outpH, outpW, outpD);

            _ = Parallel.For(0, outpD, d =>
            {
                NNValue filter = filters[d];
                int y = -padY;

                for (int y1 = 0; y1 < outpH; y1++, y += strideY)
                {
                    int x = -padX;
                    for (int x1 = 0; x1 < outpW; x1++, x += strideX)
                    {
                        float newPixel = 0.0f;
                        for (int dy = 0; dy < filter.Shape.Height; dy++)
                        {
                            int y2 = y + dy;
                            for (int dx = 0; dx < filter.Shape.Width; dx++)
                            {
                                int ox = x + dx;
                                if (y2 >= 0 && y2 < input.Shape.Height && ox >= 0 && ox < input.Shape.Width)
                                {
                                    for (int fd = 0; fd < filter.Shape.Depth; fd++)
                                    {
                                        newPixel += filter[dy, dx, fd] * input[y2, ox, fd];
                                    }
                                }
                            }
                        }

                        returnObj[y1, x1, d] = newPixel + bias[d];
                    }
                }
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, outpD, d =>
                        {
                            NNValue filter = filters[d];
                            int y = -padY;
                            for (int y1 = 0; y1 < outpH; y1++, y += strideY)
                            {

                                int x = -padX;
                                for (int x1 = 0; x1 < outpW; x1++, x += strideX)
                                {
                                    float delt = returnObj.DifData[x1 + (outpW * y1) + (outpW * outpH * d)]; // Расчет дельт и балансировка
                                    for (int dy = 0; dy < filter.Shape.Height; dy++)
                                    {
                                        int y2 = y + dy;
                                        for (int dx = 0; dx < filter.Shape.Width; dx++)
                                        {
                                            int ox = x + dx;
                                            if (y2 >= 0 && y2 < input.Shape.Height && ox >= 0 && ox < input.Shape.Width)
                                            {
                                                for (int fd = 0; fd < filter.Shape.Depth; fd++)
                                                {
                                                    filter.DifData[(filter.Shape.Width * dy) + dx + (fd * filter.Shape.Area)] += input[y2, ox, fd] * delt;
                                                    input.DifData[ox + (input.Shape.Width * y2) + (input.Shape.Area * fd)] += filter[dy, dx, fd] * delt;

                                                }
                                            }
                                        }
                                    }

                                    bias.DifData[d] += delt;
                                }
                            }
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Подвыборка 2D
        /// </summary>
        public virtual NNValue MaxPooling(NNValue input, int h, int w)
        {
            int outpH = input.Shape.Height / h, outpW = input.Shape.Width / w, outpD = input.Shape.Depth;
            float[] data = new float[h * w];

            if ((outpW < 1) || (outpH < 1))
            {
                throw new Exception("Недостаточная размерность выхода");
            }

            NNValue returnObj = new NNValue(outpH, outpW, outpD);
            int[] maximusX = new int[returnObj.Shape.Count];
            int[] maximusY = new int[returnObj.Shape.Count];

            for (int s = 0; s < outpD; s++)
            {
                int k = s * returnObj.Shape.Area;

                for (int y = 0, y2 = 0; y2 < outpH; y += h, y2++)
                {
                    for (int x = 0, x2 = 0; x2 < outpW; x2++, x += w)
                    {

                        int i = y, j = x;

                        for (int dy = 0; dy < h; dy++)
                        {

                            for (int dx = 0; dx < w; dx++)
                            {
                                int x1 = x + dx;
                                int y1 = y + dy;

                                if (input[i, j, s] < input[y1, x1, s])
                                {
                                    i = y1;
                                    j = x1;
                                }
                            }
                        }

                        float max = input[i, j, s];
                        returnObj[y2, x2, s] = max;
                        maximusX[k] = j;
                        maximusY[k] = i;
                        k++;
                    }
                }
            };

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int n = 0; n < input.Shape.Depth; n++)
                        {
                            int k = n * returnObj.Shape.Area;

                            for (int y = 0; y < outpH; y++)
                            {
                                for (int x = 0; x < outpW; x++)
                                {
                                    input.DifData[(input.Shape.Width * maximusY[k]) + maximusX[k] + (input.Shape.Area * n)] = returnObj.DifData[(outpW * y) + x + (n * returnObj.Shape.Area)];
                                    k++;
                                }
                            }
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;

        }
        /// <summary>
        /// Изменение формы тензора
        /// </summary>
        public virtual NNValue ReShape(NNValue input, Shape3D newShape, float gain)
        {
            NNValue outp = input.Clone();

            outp.Shape = newShape;
            // Обратный проход
            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, input.Shape.Count, i =>
                        {
                            input.DifData[i] = gain * outp.DifData[i];
                        });
                    }
                };
                Backprop.Add(bp);
            }

            return outp;
        }
        /// <summary>
        /// UnPooling 2D
        /// </summary>
        public virtual NNValue UnPooling(NNValue inp, int h, int w)
        {
            int outpH = (inp.Shape.Height * h) + 1, outpW = (inp.Shape.Width * w) + 1, outpD = inp.Shape.Depth;
            float sq = 1.0f / (h * w);

            if ((outpW < 1) || (outpH < 1))
            {
                throw new Exception("Недостаточная размерность выхода");
            }

            NNValue returnObj = new NNValue(outpH, outpW, outpD);

            for (int s = 0; s < outpD; s++)
            {
                for (int y = 1, y1 = 0; y < outpH; y += h, y1++)
                {
                    for (int x = 1, x1 = 0; x < outpW; x += w, x1++)
                    {
                        returnObj[y, x, s] = inp[y1, x1, s];
                    }
                }
            };

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {

                        for (int n = 0; n < outpD; n++)
                        {
                            for (int y = 0, y2 = 1; y < inp.Shape.Height; y++, y2 += h)
                            {
                                for (int x = 0, x2 = 1; x < inp.Shape.Width; x++, x2 += w)
                                {
                                    inp.DifData[(inp.Shape.Width * y) + x + (inp.Shape.Area * n)] = returnObj.DifData[(outpW * y2) + x2 + (returnObj.Shape.Area * n)];
                                }
                            }
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Апсемплинг с бикубической интерполяцией
        /// </summary>
        public NNValue Upsampling2DBicubic(NNValue inp, int h, int w)
        {
            int outpH = inp.Shape.Height * h, outpW = inp.Shape.Width * w, outpD = inp.Shape.Depth;

            if ((outpW < 1) || (outpH < 1))
            {
                throw new Exception("Недостаточная размерность выхода");
            }

            NNValue returnObj = new NNValue(outpH, outpW, outpD);
            float stepH = 1.0f / h;
            float stepW = 1.0f / w;
            _ = Parallel.For(0, outpD, s =>//for (int s = 0; s < outpD; s++)
            {
                float y1 = 0.0f;
                for (int y = 0; y < outpH; y++, y1 += stepH)
                {
                    float x1 = 0.0f;
                    for (int x = 0; x < outpW; x++, x1 += stepW)
                    {
                        returnObj[y, x, s] = Bicubic(inp, x1, y1, s);
                    }
                }
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, outpD, s =>//for (int s = 0; s < outpD; s++)
                        {
                            float y1 = 0.0f;
                            for (int y_ = 0; y_ < outpH; y_++, y1 += stepH)
                            {
                                float x1 = 0.0f;
                                for (int x_ = 0; x_ < outpW; x_++, x1 += stepW)
                                {
                                    int coordX = (int)x1;
                                    int coordY = (int)y1;

                                    bool x1w = coordX + 1 < inp.Shape.Width;
                                    bool y1h = coordY + 1 < inp.Shape.Width;
                                    bool x10 = coordX - 1 > -1;
                                    bool y10 = coordY - 1 > -1;
                                    bool x2w = coordX + 2 < inp.Shape.Width;
                                    bool y2w = coordY + 2 < inp.Shape.Height;

                                    float dif = returnObj.DifData[(y_ * returnObj.Shape.Width) + x_ + (s * returnObj.Shape.Area)];
                                    int index = (inp.Shape.Width * (coordY + 0)) + coordX + 0 + (inp.Shape.Area * s);

                                    float[] b = CalcB(x1, y1);

                                    inp.DifData[index] += dif * b[0];

                                    if (x1w)
                                    {
                                        inp.DifData[index + 1] += dif * b[1];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[1];
                                    }

                                    if (y1h)
                                    {
                                        inp.DifData[index + inp.Shape.Width] += dif * b[2];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[2];
                                    }

                                    if (x1w && y1h)
                                    {
                                        inp.DifData[index + inp.Shape.Width + 1] += dif * b[3];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[3];
                                    }

                                    if (x10)
                                    {
                                        inp.DifData[index - 1] += dif * b[4];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[4];
                                    }

                                    if (y10)
                                    {
                                        inp.DifData[index - inp.Shape.Width] += dif * b[5];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[5];
                                    }

                                    if (x10 && y1h)
                                    {
                                        inp.DifData[index + inp.Shape.Width - 1] += dif * b[6];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[6];
                                    }

                                    if (y10 && x1w)
                                    {
                                        inp.DifData[index - inp.Shape.Width + 1] += dif * b[7];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[7];
                                    }

                                    if (x2w)
                                    {
                                        inp.DifData[index + 2] += dif * b[8];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[8];
                                    }

                                    if (y2w)
                                    {
                                        inp.DifData[index + (2 * inp.Shape.Width)] += dif * b[9];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[9];
                                    }

                                    if (y10 && x10)
                                    {
                                        inp.DifData[index - inp.Shape.Width - 1] += dif * b[10];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[10];
                                    }

                                    if (y1h && x2w)
                                    {
                                        inp.DifData[index + inp.Shape.Width + 2] += dif * b[11];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[11];
                                    }

                                    if (y2w && x1w)
                                    {
                                        inp.DifData[index + (2 * inp.Shape.Width) + 1] += dif * b[12];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[12];
                                    }

                                    if (y10 && x2w)
                                    {
                                        inp.DifData[index - inp.Shape.Width + 2] += dif * b[13];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[13];
                                    }

                                    if (y2w && x10)
                                    {
                                        inp.DifData[index + (2 * inp.Shape.Width) - 1] += dif * b[14];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[14];
                                    }

                                    if (y2w && x2w)
                                    {
                                        inp.DifData[index + (2 * inp.Shape.Width) + 2] += dif * b[15];
                                    }
                                    else
                                    {
                                        inp.DifData[index] += dif * b[15];
                                    }
                                }
                            }
                        });
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Сумма тензора и числа
        /// </summary>
        /// <param name="tensor">Тензор</param>
        /// <param name="number">Число</param>
        public virtual NNValue AddN(NNValue tensor, NNValue number)
        {
            NNValue returnObj = new NNValue(tensor.Shape);
            float val = number[0];

            _ = Parallel.For(0, tensor.Shape.Count, i =>
            {
                returnObj[i] = tensor[i] + val;
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0; i < tensor.Shape.Count; i++)
                        {
                            tensor.DifData[i] += returnObj.DifData[i];
                            number.DifData[0] += returnObj.DifData[i];
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Dropout
        /// </summary>
        public NNValue DropOut(NNValue input, float q, float n, Random random)
        {
            if (IsBackward)
            {
                NNValue retObj = new NNValue(input.Shape);

                bool[] mask = new bool[input.Shape.Count];


                for (int i = 0; i < input.Shape.Count; i++)
                {
                    mask[i] = random.NextDouble() < q;
                    retObj[i] = mask[i] ? input[i] * n : 0;
                }

                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0; i < input.Shape.Count; i++)
                        {
                            input.DifData[i] = mask[i] ? 0 : retObj.DifData[i] * n;
                        }
                    }
                };
                Backprop.Add(bp);

                return retObj;
            }

            return input;
        }
        /// <summary>
        /// Разделение тензора на несколько тензоров по глубине
        /// </summary>
        /// <param name="data">Тензор входа</param>
        /// <param name="countLayersInSlice">Количество слоев в одном срезе</param>
        public virtual NNValue[] DeepSplit(NNValue data, int countLayersInSlice)
        {
            int count = data.Shape.Depth / countLayersInSlice;

            NNValue[] returnObj = new NNValue[count];
            int H = data.Shape.Height;
            int W = data.Shape.Width;
            int S = H * W;

            for (int i = 0, k = 0; i < count; i++)
            {
                returnObj[i] = new NNValue(data.Shape.Height, data.Shape.Width, countLayersInSlice);

                for (int d = 0; d < countLayersInSlice; d++, k++)
                {
                    for (int h = 0; h < data.Shape.Height; h++)
                    {
                        for (int w = 0; w < data.Shape.Width; w++)
                        {
                            int indData = (W * h) + w + (S * k);
                            int indRet = (W * h) + w + (S * d);

                            returnObj[i].Data[indRet] = data.Data[indData];
                        }
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0, k = 0; i < count; i++)
                        {
                            for (int d = 0; d < countLayersInSlice; d++, k++)
                            {
                                for (int h = 0; h < data.Shape.Height; h++)
                                {
                                    for (int w = 0; w < data.Shape.Width; w++)
                                    {
                                        int indData = (W * h) + w + (S * k);
                                        int indRet = (W * h) + w + (S * d);
                                        data.DifData[indData] = returnObj[i].DifData[indRet];
                                    }
                                }
                            }
                        }
                    }
                };
                Backprop.Add(bp);
            }
            return returnObj;
        }
        /// <summary>
        /// Смешивание реальной и мнимой частей для создания новой реальной и мнимой частей
        /// </summary>
        public virtual NNValue[] ImRealCross(NNValue real, NNValue im, NNValue alpha1, NNValue beta1, NNValue gama1, NNValue alpha2, NNValue beta2, NNValue gama2)
        {
            NNValue[] returnObj = new NNValue[2];

            returnObj[0] = new NNValue(real.Shape);// real outp
            returnObj[1] = new NNValue(real.Shape); // Im outp

            _ = Parallel.For(0, real.Shape.Count, i =>
            {
                returnObj[0][i] = (real[i] * alpha1[0]) + (im[i] * beta1[0]) + (im[i] * real[i] * gama1[0]);
                returnObj[1][i] = (real[i] * alpha2[0]) + (im[i] * beta2[0]) + (im[i] * real[i] * gama2[0]);
            });

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        _ = Parallel.For(0, real.Shape.Count, i =>
                        {
                            float dR = returnObj[0].DifData[i];// производные реальной части
                            float dI = returnObj[1].DifData[i];// производные мнимой части

                            real.DifData[i] += ((alpha1[0] + (gama1[0] * im[i])) * dR) + ((alpha2[0] + (gama2[0] * im[i])) * dI);
                            im.DifData[i] += ((beta1[0] + (gama1[0] * real[i])) * dR) + ((beta2[0] + (gama2[0] * real[i])) * dI);

                            alpha1.DifData[0] += real[i] * dR;
                            alpha2.DifData[0] += real[i] * dI;

                            beta1.DifData[0] += im[i] * dR;
                            beta2.DifData[0] += im[i] * dI;

                            gama1.DifData[0] += im[i] * real[i] * dR;
                            gama2.DifData[0] += im[i] * real[i] * dI;
                        });
                    }
                };
                Backprop.Add(bp);
            }

            return returnObj;
        }
        /// <summary>
        /// Соединение глубины тензора
        /// </summary>
        public virtual NNValue DeepJoin(NNValue[] values)
        {
            int deepRet = 0;
            for (int i = 0; i < values.Length; i++)
            {
                deepRet += values[i].Shape.Depth;
            }

            int H = values[0].Shape.Height, W = values[0].Shape.Width, S = H * W;

            NNValue returnObj = new NNValue(H, W, deepRet);

            for (int i = 0, deep = 0; i < values.Length; i++)
            {
                for (int d = 0; d < values[i].Shape.Depth; d++, deep++)
                {
                    for (int h = 0; h < H; h++)
                    {
                        for (int w = 0; w < W; w++)
                        {
                            returnObj[h, w, deep] = values[i][h, w, d];
                        }
                    }
                }
            }

            if (IsBackward)
            {
                Runnable bp = new Runnable
                {
                    StartCalc = delegate ()
                    {
                        for (int i = 0, deep = 0; i < values.Length; i++)
                        {
                            for (int d = 0; d < values[i].Shape.Depth; d++, deep++)
                            {
                                for (int h = 0; h < H; h++)
                                {
                                    for (int w = 0; w < W; w++)
                                    {
                                        int indData = (W * h) + w + (S * d);
                                        int indRet = (W * h) + w + (S * deep);

                                        values[i].DifData[indData] = returnObj.DifData[indRet];
                                    }
                                }
                            }
                        }

                    }
                };
                Backprop.Add(bp);
            }

            return returnObj;
        }
        /// <summary>
        /// Полносвязный слой
        /// </summary>
        public virtual NNValue FeedForwardLayer(NNValue input, NNValue W, NNValue bias, IActivation activation)
        {
            NNValue sum = Add(MulMV(W, input), bias);
            NNValue returnObj = Activate(activation, sum);
            return returnObj;
        }
        /// <summary>
        /// Слой gru
        /// </summary>
        public virtual NNValue GRULayer(NNValue input, NNValue hmix, NNValue hHmix, NNValue bmix, NNValue hnew, NNValue hHnew, NNValue bnew, NNValue hreset, NNValue hHreset, NNValue breset, NNValue context, SigmoidUnit fMix,
            SigmoidUnit fReset,
            TanhUnit fNew)
        {

            NNValue actMix = Activate(fMix,
                Add(
                    MulMV(hmix, input),
                    MulMV(hHmix, context),
                    bmix));


            NNValue actReset = Activate(fReset,
                Add(
                    MulMV(hreset, input),
                    MulMV(hHreset, context),
                    breset));

            NNValue gatedContext = AdamarMul(actReset, context);
            NNValue actNewPlusGatedContext =
                Activate(fNew,
                Add(
                    MulMV(hnew, input),
                    MulMV(hHnew, gatedContext),
                    bnew));

            NNValue memvals = AdamarMul(actMix, context);

            NNValue newvals = AdamarMul(OneMinus(actMix), actNewPlusGatedContext);

            NNValue output = Add(memvals, newvals);

            return output;
        }
        /// <summary>
        /// Линейный полносвязный слой
        /// </summary>
        public virtual NNValue FeedforwardLinLayer(NNValue input, NNValue W, NNValue bias)
        {
            return Add(MulMV(W, input), bias);
        }

        private float Bicubic(NNValue tensor, float pointX, float pointY, int d)
        {
            float f = 0.0f;
            int w = tensor.Shape.Width;
            int h = tensor.Shape.Height;
            int coordX = (int)pointX;
            int coordY = (int)pointY;

            float[] b = CalcB(pointX, pointY);

            float[] a = new float[b.Length];

            _ = Parallel.For(0, a.Length, i =>
            {
                a[i] = tensor[coordY, coordX, d];
            });

            bool x1w = coordX + 1 < w;
            bool y1h = coordY + 1 < h;
            bool x10 = coordX - 1 > -1;
            bool y10 = coordY - 1 > -1;
            bool x2w = coordX + 2 < w;
            bool y2w = coordY + 2 < h;

            if (x1w)
            {
                a[1] = tensor[coordY, coordX + 1, d];
            }

            if (y1h)
            {
                a[2] = tensor[coordY + 1, coordX, d];
            }

            if (x1w && y1h)
            {
                a[3] = tensor[coordY + 1, coordX + 1, d];
            }

            if (x10)
            {
                a[4] = tensor[coordY, coordX - 1, d];
            }

            if (y10)
            {
                a[5] = tensor[coordY - 1, coordX, d];
            }

            if (x10 && y1h)
            {
                a[6] = tensor[coordY + 1, coordX - 1, d];
            }

            if (y10 && x1w)
            {
                a[7] = tensor[coordY - 1, coordX + 1, d];
            }

            if (x2w)
            {
                a[8] = tensor[coordY, coordX + 2, d];
            }

            if (y2w)
            {
                a[9] = tensor[coordY + 2, coordX, d];
            }

            if (y10 && x10)
            {
                a[10] = tensor[coordY - 1, coordX - 1, d];
            }

            if (y1h && x2w)
            {
                a[11] = tensor[coordY + 1, coordX + 2, d];
            }

            if (y2w && x1w)
            {
                a[12] = tensor[coordY + 2, coordX + 1, d];
            }

            if (y10 && x2w)
            {
                a[13] = tensor[coordY - 1, coordX + 2, d];
            }

            if (y2w && x10)
            {
                a[14] = tensor[coordY + 2, coordX - 1, d];
            }

            if (y2w && x2w)
            {
                a[15] = tensor[coordY + 2, coordX + 2, d];
            }

            for (int i = 0; i < b.Length; i++)
            {
                f += a[i] * b[i];
            }

            return f;
        }

        private float[] CalcB(float x1, float y1)
        {
            float x = (float)(x1 - Math.Truncate(x1));
            float y = (float)(y1 - Math.Truncate(y1));
            #region b coefs
            float[] b = new float[16];
            b[0] = 1.0f / 4.0f * (x - 1.0f) * (x - 2.0f) * (x + 1.0f) * (y - 1.0f) * (y - 2.0f) * (y + 1.0f);
            b[1] = -1.0f / 4.0f * x * (x + 1.0f) * (x - 2.0f) * (y - 1.0f) * (y - 2.0f) * (y + 1.0f);
            b[2] = -1.0f / 4.0f * y * (x - 1.0f) * (x - 2.0f) * (x + 1.0f) * (y + 1.0f) * (y - 2.0f);
            b[3] = 1.0f / 4.0f * x * y * (x + 1.0f) * (x - 2.0f) * (y + 1.0f) * (y - 2.0f);
            b[4] = -1.0f / 12.0f * x * (x - 1.0f) * (x - 2.0f) * (y - 1.0f) * (y - 2.0f) * (y + 1.0f);
            b[5] = -1.0f / 12.0f * y * (x - 1.0f) * (x - 2.0f) * (x + 1.0f) * (y - 1.0f) * (y - 2.0f);
            b[6] = 1.0f / 12.0f * x * y * (x - 1.0f) * (x - 2.0f) * (y + 1.0f) * (y - 2.0f);
            b[7] = 1.0f / 12.0f * x * y * (x + 1.0f) * (x - 2.0f) * (y - 1.0f) * (y - 2.0f);
            b[8] = 1.0f / 12.0f * x * (x - 1.0f) * (x + 1.0f) * (y - 1.0f) * (y - 2.0f) * (y + 1.0f);
            b[9] = 1.0f / 12.0f * y * (x - 1.0f) * (x - 2.0f) * (x + 1.0f) * (y - 1.0f) * (y + 1.0f);
            b[10] = 1.0f / 36.0f * x * y * (x - 1.0f) * (x - 2.0f) * (y - 1.0f) * (y - 2.0f);
            b[11] = -1.0f / 12.0f * x * y * (x - 1.0f) * (x + 1.0f) * (y + 1.0f) * (y - 2.0f);
            b[12] = -1.0f / 12.0f * x * y * (x + 1.0f) * (x - 2.0f) * (y - 1.0f) * (y + 1.0f);
            b[13] = -1.0f / 36.0f * x * y * (x - 1.0f) * (x + 1.0f) * (y - 1.0f) * (y - 2.0f);
            b[14] = -1.0f / 36.0f * x * y * (x - 1.0f) * (x - 2.0f) * (y - 1.0f) * (y + 1.0f);
            b[15] = 1.0f / 36.0f * x * y * (x - 1.0f) * (x + 1.0f) * (y - 1.0f) * (y + 1.0f);
            #endregion

            return b;
        }
    }
}
