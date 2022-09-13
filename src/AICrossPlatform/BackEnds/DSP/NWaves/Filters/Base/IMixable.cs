namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMixable
    {
        /// <summary>
        /// Wet mix
        /// </summary>
        float Wet { get; set; }

        /// <summary>
        /// Dry mix
        /// </summary>
        float Dry { get; set; }
    }
}
