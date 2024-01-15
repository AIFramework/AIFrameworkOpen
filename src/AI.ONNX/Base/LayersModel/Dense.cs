using AI.DataStructs.Algebraic;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;

namespace AI.ONNX.Base.LayersModel
{
    /// <summary>
    /// Полносвязный слой 
    /// </summary>
    [Serializable]
    public class Dense
    {
        InferenceSession _session;
        public DataType DType { get; set; }

        /// <summary>
        /// Полносвязный слой 
        /// </summary>
        public Dense(string modelPath)
        {
            _session = new InferenceSession(modelPath);
        }


        public Vector ForwardNoBatch(Vector inputVector)
        {
            RunOptions runOptions = new RunOptions();
            var input = CreateInput(inputVector);
            var resultTensors = _session.Run(runOptions, input, _session.OutputNames);

            switch (DType) 
            {
                case DataType.float32:
                    return resultTensors[0].GetTensorDataAsSpan<float>().ToArray();
                case DataType.float64:
                    return resultTensors[0].GetTensorDataAsSpan<double>().ToArray();
                case DataType.int32:
                    return resultTensors[0].GetTensorDataAsSpan<int>().ToArray();
            }
            throw new Exception();
        }

        public Dictionary<string, OrtValue> CreateInput(Vector inpVect, DataType dtype = DataType.float32)
        {
            var shape = new long[] { 1, inpVect.Count };

            OrtValue inputIdsOrtValue;

            switch (dtype) 
            {
                case DataType.float32:
                    inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory((float[])inpVect, shape);
                    break;
                case DataType.float64:
                    inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory((double[])inpVect, shape);
                    break;
               default: throw new InvalidOperationException();
            }

            return new Dictionary<string, OrtValue>
              {
                  { "input", inputIdsOrtValue }
              };
        }
    }


    public enum DataType
    {
        float32,
        float64,
        int32,
        int64,
    }
}
