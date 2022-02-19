using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using System.Text;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    internal static class LayerHelper
    {
        public static string GetLayerDescription(string layerName, Shape3D inputShape, Shape3D outputShape, IActivation activationFunction, int trainableParams)
        {
            return GetLayerDescription(layerName, inputShape, outputShape, activationFunction.ToString(), trainableParams);
        }

        public static string GetLayerDescription(string layerName, Shape3D inputShape, Shape3D outputShape, string activationFunctionDescription, int trainableParams)
        {
            StringBuilder builder = new StringBuilder(layerName);
            while (builder.Length < 24)
            {
                builder.Append(" ");
            }

            return string.Format("{0}|Inp: {1} |Outp: {2} |Activation: {3} |Train params: {4}", builder.ToString(), inputShape, outputShape, activationFunctionDescription, trainableParams);
        }
    }
}