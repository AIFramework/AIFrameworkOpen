using AI.DataStructs.Shapes;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using Tensor = AI.DataStructs.Algebraic.Tensor;

namespace AI.ONNX
{
    /// <summary>
    /// Нейронная сеть преобразующая тензор входа в тензор выхода
    /// </summary>
    [Serializable]
    public class Tensor2Tensor
    {
        /// <summary>
        /// Вычислительный граф
        /// </summary>
        public InferenceSession Session { get; protected set; }
        /// <summary>
        /// Имя входа
        /// </summary>
        public string InputName { get; private set; }
        /// <summary>
        /// Имя выхода
        /// </summary>
        public string OutputName { get; private set; }
        /// <summary>
        /// Высота входного тензора
        /// </summary>
        public int InputH { get; private set; }
        /// <summary>
        /// Ширина входного тензора
        /// </summary>
        public int InputW { get; private set; }
        /// <summary>
        /// Глубина входного тензора
        /// </summary>
        public int InputD { get; private set; }

        /// <summary>
        /// Высота выходного тензора
        /// </summary>
        public int OutpH { get; private set; }
        /// <summary>
        /// Ширина выходного тензора
        /// </summary>
        public int OutpW { get; private set; }
        /// <summary>
        /// Глубина выходного тензора
        /// </summary>
        public int OutpD { get; private set; }
        /// <summary>
        /// Библиотека в которой была создана нейронка
        /// </summary>
        public LibType LibType { get; private set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public int DimOut { get; private set; }

        private int _iH, _iW, _iD, _iHO, _iWO, _iDO;
        int[] _inputShape;
        int[] _outpShape;

        /// <summary>
        /// Нейронная сеть преобразующая тензор входа в тензор выхода
        /// </summary>
        public Tensor2Tensor(string path, LibType libType = LibType.Keras, LibType libTypeOut = LibType.Keras)
        {
            Session = new InferenceSession(path);
            InputName = Session.InputMetadata.Keys.ToArray()[0];
            OutputName = Session.OutputMetadata.Keys.ToArray()[0];
            _inputShape = Session.InputMetadata[InputName].Dimensions;
            _outpShape = Session.OutputMetadata[OutputName].Dimensions;
            LibType = libType;

            switch (LibType)
            {
                case LibType.Keras:
                    _iD = 3;
                    _iH = 1;
                    _iW = 2;
                    break;
                case LibType.PyTorch:
                    _iD = 0;
                    _iH = 2;
                    _iW = 3;
                    break;
                case LibType.InverseCh:
                    _iD = 1;
                    _iH = 2;
                    _iW = 3;
                    break;
            }

            switch (libTypeOut)
            {
                case LibType.Keras:
                    _iDO = 3;
                    _iHO = 1;
                    _iWO = 2;
                    break;
                case LibType.PyTorch:
                    _iD = 0;
                    _iH = 2;
                    _iW = 3;
                    break;
            }

            // Входная размерность
            InputH = _inputShape[_iH];
            InputW = _inputShape[_iW];
            InputD = _inputShape[_iD];
            DimOut = _outpShape.Length - 1;

            // Выходная размерность
            try
            {
                OutpH = _outpShape[_iHO];
            }
            catch { OutpH = 1; }

            try
            {
                OutpW = _outpShape[_iWO];
            }
            catch { OutpW = 1; }

            try
            {
                OutpD = _outpShape[_iDO]; 
            }
            catch { OutpD = 1; }

            
        }

        /// <summary>
        /// Преобразование из тензора в тензор
        /// </summary>
        public Tensor Transform(Tensor img)
        {
            _inputShape[0] = 1;
            DenseTensor<float> tensorF = new DenseTensor<float>(_inputShape);

            if (_iH == 1 && _iW == 2)
            {
                for (int i = 0; i < InputH; i++)
                    for (int j = 0; j < InputW; j++)
                        for (int k = 0; k < InputD; k++)
                            tensorF[0, i, j, k] = (float)img[i, j, k];
            }
            else // iH = 2, iW = 3
            {
                for (int i = 0; i < InputH; i++)
                    for (int j = 0; j < InputW; j++)
                        for (int k = 0; k < InputD; k++)
                            tensorF[0, k, i, j] = (float)img[i, j, k];
            }


            List<NamedOnnxValue> input = new List<NamedOnnxValue>();
            input.Add(NamedOnnxValue.CreateFromTensor<float>(InputName, tensorF));

            using (var results = Session.Run(input))
            {
                var dat = results.First().AsTensor<float>();

                switch (DimOut)
                {
                    case 1:
                        return OneD(dat);

                    case 2:
                        return TwoD(dat);

                    case 3:
                        return TreeD(dat);
                }

            }

            return new Tensor(1, 1, 1);
        }

        /// <summary>
        /// Одномерный тензор
        /// </summary>
        private Tensor OneD(Tensor<float> dat)
        {
            Shape3D outTensorShape = new Shape3D(OutpH, OutpW, OutpD); // Размерность выходного тензора
            Tensor outTensor = new Tensor(outTensorShape);

            for (int i = 0; i < OutpH; i++)
                outTensor[i, 0, 0] = (float)dat[0, i];

            return outTensor;
        }

        /// <summary>
        /// Двумерный тензор
        /// </summary>
        private Tensor TwoD(Tensor<float> dat)
        {
            Shape3D outTensorShape = new Shape3D(OutpH, OutpW, OutpD); // Размерность выходного тензора
            Tensor outTensor = new Tensor(outTensorShape);

            switch (LibType)
            {
                case LibType.Keras:
                    for (int i = 0; i < OutpH; i++)
                        for (int j = 0; j < OutpW; j++)
                                outTensor[i, j, 0] = (float)dat[0, i, j];
                    break;

                case LibType.PyTorch:
                    for (int i = 0; i < OutpH; i++)
                        for (int j = 0; j < OutpW; j++)
                                outTensor[i, j, 0] = (float)dat[0, 0, i, j];
                    break;
            }

            return outTensor;
        }

        /// <summary>
        /// Трехмерный тензор
        /// </summary>
        private Tensor TreeD(Tensor<float> dat)
        {
            Shape3D outTensorShape = new Shape3D(OutpH, OutpW, OutpD); // Размерность выходного тензора
            Tensor outTensor = new Tensor(outTensorShape);

            switch (LibType)
            {
                case LibType.Keras:
                    for (int i = 0; i < OutpH; i++)
                        for (int j = 0; j < OutpW; j++)
                            for (int k = 0; k < OutpD; k++)
                                outTensor[i, j, k] = (float)dat[0, i, j, k];
                    break;

                case LibType.PyTorch:
                    for (int i = 0; i < OutpH; i++)
                        for (int j = 0; j < OutpW; j++)
                            for (int k = 0; k < OutpD; k++)
                                outTensor[i, j, k] = (float)dat[0, k, i, j];
                    break;
            }

            return outTensor;
        }


    }

    /// <summary>
    /// Библиотека в которой была создана нейронка
    /// </summary>
    public enum LibType 
    {
        /// <summary>
        /// Кирас
        /// </summary>
        Keras,
        /// <summary>
        /// PyTorch
        /// </summary>
        PyTorch,
        /// <summary>
        /// Вначале глубина
        /// </summary>
        InverseCh
    }
}
