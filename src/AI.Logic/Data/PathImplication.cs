using System;

namespace AI.Logic.Data
{
    /// <summary>
    /// Путь для импликации
    /// </summary>
    [Serializable]
    public class PathImplication 
    {
        public uint ID { get; set; }
        public double Prob { get; set; }
    }
}
