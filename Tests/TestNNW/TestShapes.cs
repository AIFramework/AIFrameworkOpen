using AI.DataStructs.Shapes;
using System;

namespace TestNNW
{
    public static class TestShapes
    {
        public static void Execute()
        {
            Shape1D shape1D = new Shape1D(3);
            Console.WriteLine(shape1D);
            Console.WriteLine(shape1D.Count);
            Console.WriteLine((Shape2D)shape1D);
            Console.WriteLine((Shape3D)shape1D);
            Console.WriteLine(shape1D.Expand(7));

            Shape2D shape2D = new Shape2D(2, 5);
            Console.WriteLine(shape2D);
            Console.WriteLine(shape2D.Count);
            Console.WriteLine(shape2D.Area);
            Console.WriteLine((Shape3D)shape2D);
            Console.WriteLine(shape2D.Shrink());
            Console.WriteLine(shape2D.Expand(3));

            Shape3D shape3D = new Shape3D(4, 7, 3);
            Console.WriteLine(shape3D);
            Console.WriteLine(shape3D.Count);
            Console.WriteLine(shape3D.Volume);
            Console.WriteLine(shape3D.Shrink());
            Console.WriteLine(shape3D.Expand(5));

            Shape shape = new Shape(1, 2, 3, 4);
            Console.WriteLine(shape);
            Console.WriteLine(shape.Count);
            Console.WriteLine(shape.Shrink());
            Console.WriteLine(shape.Expand(5));

            Shape shape1 = new Shape(1, 2, 3);
            Console.WriteLine(shape.FuzzyEquals(shape1));
            Shape shape2 = new Shape(1, 2, 3, 4);
            Console.WriteLine(shape.FuzzyEquals(shape2));
            Shape shape3 = new Shape(1, 2, 3, 4, 1, 1);
            Console.WriteLine(shape.FuzzyEquals(shape3));
            Shape shape4 = new Shape(2, 1, 3, 4);
            Console.WriteLine(shape.FuzzyEquals(shape4));
        }
    }
}