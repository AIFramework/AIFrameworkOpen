using AI.DataStructs.Algebraic;
using System.Drawing;

namespace AI.ComputerVision.SpatialFilters
{
    /// <summary>
    /// Grayscale spatial filter interface
    /// </summary>
    public interface ISpatialFilterGray
    {
        Matrix Filtration(Matrix input);
        Bitmap Filtration(Bitmap input);
        Bitmap Filtration(string path);
    }
}
