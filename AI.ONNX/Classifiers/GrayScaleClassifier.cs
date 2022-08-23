using AI.DataStructs.Algebraic;
//using Microsoft.ML.OnnxRuntime;
//using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ONNX.Classifiers
{
    [Serializable]
    public class GrayScaleClassifier
    {
        Tensor2Tensor t2t;
        public GrayScaleClassifier(string path, LibType libType = LibType.Keras)
        {
           t2t = new Tensor2Tensor(path, libType);
        }


        public Vector Classify(Matrix img)
        {
            Tensor tensor = Tensor.FromMatrices(new[] { img });
            return t2t.Transform(tensor).Data;
        }
    }
}
