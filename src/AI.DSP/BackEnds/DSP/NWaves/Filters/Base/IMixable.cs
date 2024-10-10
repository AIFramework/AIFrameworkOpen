namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMixable
    {
        /// <summary>
        /// Влажная смесь
        /// </summary>
        float Wet { get; set; }

        /// <summary>
        /// Сухая смесь
        /// </summary>
        float Dry { get; set; }
    }
}
