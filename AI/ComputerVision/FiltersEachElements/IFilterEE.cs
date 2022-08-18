using AI.DataStructs.Algebraic;
using System.Drawing;

namespace AI.ComputerVision.FiltersEachElements
{
    public interface IFilterEE
    {
        Matrix Filtration(Matrix input);
        Bitmap Filtration(Bitmap input);
        Bitmap Filtration(string path);
    }
}
