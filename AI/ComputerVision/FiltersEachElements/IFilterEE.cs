using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.ComputerVision.FiltersEachElements
{
    public interface IFilterEE
    {
        Matrix Filtration(Matrix input);
        Bitmap Filtration(Bitmap input);
        Bitmap Filtration(string path);
    }
}
