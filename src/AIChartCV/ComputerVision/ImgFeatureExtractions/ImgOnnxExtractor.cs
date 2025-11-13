using AI.DataPrepaire.FeatureExtractors;
using AI.DataStructs.Algebraic;
using AI.ONNX;
using System;
using System.Drawing;

namespace AI.ComputerVision.ImgFeatureExtractions
{
    /// <summary>
    /// Извлечение признаков на базе onnx модели
    /// </summary>
    [Serializable]
    public class ImgOnnxExtractor : FeaturesExtractor<Bitmap>
    {
        private readonly Tensor2Tensor emb;
        private readonly Vector _mean, _std;
        private readonly int _h, _w;

        /// <summary>
        /// Извлечение признаков на базе onnx модели
        /// </summary>
        public ImgOnnxExtractor(string pathToOnnxModel, Vector mean, Vector std, int inpH, int inpW, LibType back)
        {
            emb = new Tensor2Tensor(pathToOnnxModel, back);
            _w = inpW;
            _h = inpH;
            _mean = mean;
            _std = std;
        }

        /// <summary>
        /// Получение признаков из модели
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override Vector GetFeatures(Bitmap data)
        {
            var inpTensor = ImageMatrixConverter.BmpToTensor(new Bitmap(data, _w, _h)) / 255;
            var transformTensor = new Tensor(_h, _w, inpTensor.Depth);

            for (int i = 0; i < inpTensor.Height; i++)
                for (int j = 0; j < inpTensor.Width; j++)
                {
                    transformTensor[i, j, 0] = (inpTensor[i, j, 2] - _mean[2]) / _std[2];
                    transformTensor[i, j, 1] = (inpTensor[i, j, 1] - _mean[1]) / _std[1];
                    transformTensor[i, j, 2] = (inpTensor[i, j, 0] - _mean[0]) / _std[0];
                }

            return emb.Transform(inpTensor).Data;
        }
    }
}
