﻿using AI.BackEnds.DSP.NWaves.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Operations
{
    /// <summary>
    /// Class for reconstructing signal from a given power / magnitude spectrogram
    /// based on Griffin-Lim iterative algorithm.
    /// </summary>
    public class GriffinLimReconstructor
    {
        /// <summary>
        /// STFT transformer
        /// </summary>
        private readonly Stft _stft;

        /// <summary>
        /// Magnitude part of the spectrogram
        /// </summary>
        private readonly List<float[]> _magnitudes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spectrogram"></param>
        /// <param name="stft"></param>
        /// <param name="power"></param>
        public GriffinLimReconstructor(List<float[]> spectrogram, Stft stft, int power = 2)
        {
            _stft = stft;

            _magnitudes = spectrogram;

            if (power == 2)
            {
                for (var i = 0; i < _magnitudes.Count; i++)
                {
                    for (var j = 0; j < _magnitudes[i].Length; j++)
                    {
                        _magnitudes[i][j] = (float)Math.Sqrt(_magnitudes[i][j]);
                    }
                }
            }

            for (var i = 0; i < _magnitudes.Count; i++)
            {
                for (var j = 0; j < _magnitudes[i].Length; j++)
                {
                    _magnitudes[i][j] *= 20; // how to calculate this compensation?
                }
            }
        }

        /// <summary>
        /// One iteration of reconstruction
        /// </summary>
        /// <param name="signal">Signal reconstructed at previous iteration</param>
        /// <returns>Reconstructed signal</returns>
        public float[] Iterate(float[] signal = null)
        {
            var magPhase = new MagnitudePhaseList() { Magnitudes = _magnitudes };

            if (signal == null)
            {
                var spectrumSize = _magnitudes[0].Length;

                var r = new Random();
                var randomPhases = new List<float[]>();

                for (var i = 0; i < _magnitudes.Count; i++)
                {
                    randomPhases.Add(Enumerable.Range(0, spectrumSize)
                                               .Select(s => (float)(2 * Math.PI * r.NextDouble()))
                                               .ToArray());
                }

                magPhase.Phases = randomPhases;
            }
            else
            {
                magPhase.Phases = _stft.MagnitudePhaseSpectrogram(signal).Phases;
            }

            return _stft.ReconstructMagnitudePhase(magPhase);
        }

        /// <summary>
        /// Reconstruct iteratively
        /// </summary>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public float[] Reconstruct(int iterations = 20)
        {
            var reconstructed = Iterate();

            for (var i = 0; i < iterations - 1; i++)
            {
                reconstructed = Iterate(reconstructed);
            }

            return reconstructed;
        }
    }
}
