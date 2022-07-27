using AI.DataStructs.Algebraic;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ONNX.Classifiers
{
    public class GrayScaleClassifier
    {
        public InferenceSession Session { get; protected set; }
        public string InputName { get; private set; }
        public string OutputName { get; private set; }
        public int H { get; private set; }
        public int W { get; private set; }
        private int _iH, _iW;
        int[] _shape;

        public GrayScaleClassifier(string path) 
        {
            Session = new InferenceSession(path);
            InputName = Session.InputMetadata.Keys.ToArray()[0];
            OutputName = Session.OutputMetadata.Keys.ToArray()[0];
            _shape = Session.InputMetadata[InputName].Dimensions;
            bool isH = false;

            for (int i = 0; i < _shape.Length; i++)
            {
                if(_shape[i] > 1) 
                {
                    if (isH) 
                    {
                        _iW = i;
                        W = _shape[i];
                        break;
                    }
                    else 
                    {
                        _iH = i;
                        H = _shape[i];
                        isH = true;
                    }
                }
            }

        }


        public Vector Classify(Matrix img)
        {
            _shape[0] = 1;
            Tensor<float> tensorF = new DenseTensor<float>(_shape);
            
            if (_iH == 1 && _iW == 2) 
            {
                for (int i = 0; i < H; i++)
                    for (int j = 0; j < H; j++)
                        tensorF[0, i, j, 0] = (float)img[i, j];
            }
            else // iH = 2, iW = 3
            {
                for (int i = 0; i < H; i++)
                    for (int j = 0; j < H; j++)
                        tensorF[0, 0, i, j] = (float)img[i, j];
            }


            List<NamedOnnxValue> input = new List<NamedOnnxValue>();
            input.Add(NamedOnnxValue.CreateFromTensor<float>(InputName, tensorF));

            Vector vect = new Vector();

            using (var results = Session.Run(input))
            {
                var dat = results.First().AsEnumerable<float>().ToArray();
                vect = new Vector(dat.Length);

                for (int i = 0; i < vect.Count; i++)
                {
                    vect[i] = dat[i];
                }
            }

            return vect;
        }
    }
}
