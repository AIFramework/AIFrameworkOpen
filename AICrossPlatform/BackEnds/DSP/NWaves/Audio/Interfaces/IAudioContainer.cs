using AI.BackEnds.DSP.NWaves.Signals;
using System.Collections.Generic;

namespace AI.BackEnds.DSP.NWaves.Audio.Interfaces
{
    /// <summary>
    /// Interface for sound containers
    /// </summary>
    public interface IAudioContainer
    {
        /// <summary>
        /// Discrete signals contained in container's channels
        /// </summary>
        List<DiscreteSignal> Signals { get; }

        /// <summary>
        /// Indexing based on channel type
        /// </summary>
        /// <param name="channel">channel type (left, right or interleave)</param>
        /// <returns></returns>
        DiscreteSignal this[Channels channel] { get; }
    }
}
