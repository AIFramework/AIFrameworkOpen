using AI.DataStructs.Algebraic;

namespace AI.DSP.Multiray
{
    public class GeometrySignalObject 
    {
        public Vector Coordinates { get; set; }

        public GeometrySignalObject()
        {
            Coordinates = new Vector(2);
        }

        public GeometrySignalObject(params double[] coord)
        {
            Coordinates = coord;
        }
    }
}
