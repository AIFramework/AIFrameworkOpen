using AI.DataStructs.Algebraic;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace AI.ONNX.NLP.Bert
{
    /// <summary>
    /// Работа с моделью Bert
    /// </summary>
    [Serializable]
    public class BertInfer
    {
        
        InferenceSession _session;
        //int _dim = 384; // Сделать чтобы автоматически читалось из доков

        /// <summary>
        /// Работа с моделью Bert
        /// </summary>
        public BertInfer(string modelPath) 
        {
            _session = new InferenceSession(modelPath);
        }


        public Vector[] Forward(IEnumerable<int> inpIds, IEnumerable<int> attentionMask, IEnumerable<int> types) 
        {
            RunOptions runOptions = new RunOptions();
            var input = CreateInput(I2L(inpIds), I2L(attentionMask), I2L(types));

            var resultTensors = _session.Run(runOptions, input, _session.OutputNames);
            
            // ToDo: Нормально реализовать, неоптимизированный метод для сбора эмбеддингов
            //double[] data = (Vector)resultTensors[0].GetTensorDataAsSpan<float>().ToArray();
            //Matrix matrix = new Matrix(inpIds.Count(), _dim);
            //matrix.Data = data;
            //Matrix.GetColumns(matrix.Transpose());


            var output = new Vector[resultTensors.Count];

            for (int i = 0; i < resultTensors.Count; i++)
                output[i] = resultTensors[i].GetTensorDataAsSpan<float>().ToArray();

            return output;
        }

        public Dictionary<string, OrtValue> CreateInput(long[] inpIds, long[] attentionMask, long[] types) 
        {
            var shape = new long[] { 1, inpIds.Length };

            var inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory(inpIds, shape);
            var attMaskOrtValue = OrtValue.CreateTensorValueFromMemory(attentionMask, shape);
            var typeIdsOrtValue = OrtValue.CreateTensorValueFromMemory(types, shape);

            return new Dictionary<string, OrtValue>
              {
                  { "input_ids", inputIdsOrtValue },
                  { "input_mask", attMaskOrtValue },
                  { "segment_ids", typeIdsOrtValue }
              };
        }

        // Конвертир из int arr в  long
        private long[] I2L(IEnumerable<int> ints)
        {
            var data = ints.ToArray();
            long[] ret = new long[data.Length];

            for (int i = 0; i < data.Length; i++)
                ret[i] = data[i];

            return ret;
        }
    }
}
