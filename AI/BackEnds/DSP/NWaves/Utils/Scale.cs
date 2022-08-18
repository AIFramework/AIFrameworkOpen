﻿using System;

namespace AI.BackEnds.DSP.NWaves.Utils
{
    /// <summary>
    /// Static class providing methods for 
    /// 
    /// 1) converting between different scales:
    ///     - decibel
    ///     - MIDI pitch
    ///     - mel (HTK)
    ///     - mel (Slaney)
    ///     - bark1 (Traunmueller)
    ///     - bark2 (Wang)
    ///     - ERB
    /// 
    /// 2) loudness weighting:
    ///     - A-weighting
    ///     - B-weighting
    ///     - C-weighting
    /// 
    /// </summary>
    public static class Scale
    {
        /// <summary>
        /// Method converts magnitude value to dB level
        /// </summary>
        /// <param name="value">Magnitude</param>
        /// <param name="valueReference">Reference magnitude</param>
        /// <returns>Decibel level</returns>
        public static double ToDecibel(double value, double valueReference = 1.0)
        {
            return 20 * Math.Log10((value / valueReference) + double.Epsilon);
        }

        /// <summary>
        /// Method converts power to dB level
        /// </summary>
        /// <param name="value">Power</param>
        /// <param name="valueReference">Reference power</param>
        /// <returns>Decibel level</returns>
        public static double ToDecibelPower(double value, double valueReference = 1.0)
        {
            return 10 * Math.Log10((value / valueReference) + double.Epsilon);
        }

        /// <summary>
        /// Method converts dB level to magnitude value
        /// </summary>
        /// <param name="level">dB level</param>
        /// <param name="valueReference">Reference magnitude</param>
        /// <returns>Magnitude value</returns>
        public static double FromDecibel(double level, double valueReference = 1.0)
        {
            return valueReference * Math.Pow(10, level / 20);
        }

        /// <summary>
        /// Method converts dB level to power
        /// </summary>
        /// <param name="level">dB level</param>
        /// <param name="valueReference">Reference power</param>
        /// <returns>Power</returns>
        public static double FromDecibelPower(double level, double valueReference = 1.0)
        {
            return valueReference * Math.Pow(10, level / 10);
        }

        /// <summary>
        /// Method converts MIDI pitch to frequency
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public static double PitchToFreq(int pitch)
        {
            return 440 * Math.Pow(2, (pitch - 69) / 12.0);
        }

        /// <summary>
        /// Method converts frequency to MIDI pitch
        /// </summary>
        /// <param name="freq"></param>
        /// <returns></returns>
        public static int FreqToPitch(double freq)
        {
            return (int)Math.Round(69 + (12 * Math.Log(freq / 440, 2)), MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Array of notes
        /// </summary>
        public static string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        /// <summary>
        /// Method converts note (in format ("G", 3), ("E", 5), etc.) to frequency in Hz
        /// </summary>
        /// <param name="note">Note (A-G#)</param>
        /// <param name="octave">Octave (0-8)</param>
        /// <returns>Frequency in Hz</returns>
        public static double NoteToFreq(string note, int octave)
        {
            int noteIndex = Array.IndexOf(Notes, note);

            if (noteIndex < 0)
            {
                throw new ArgumentException("Incorrect note. Valid notes are: " + string.Join(", ", Notes));
            }

            if (octave < 0 || octave > 8)
            {
                throw new ArgumentException("Incorrect octave. Valid octave range is [0, 8]");
            }

            return PitchToFreq(noteIndex + (12 * (octave + 1)));
        }

        /// <summary>
        /// Method converts frequency in Hz to note (in format ("G", 3), ("E", 5), etc.)
        /// </summary>
        /// <param name="freq">Frequency in Hz</param>
        /// <returns>Tuple (note, octave)</returns>
        public static Tuple<string, int> FreqToNote(double freq)
        {
            int pitch = FreqToPitch(freq);

            string note = Notes[pitch % 12];
            int octave = (pitch / 12) - 1;

            return new Tuple<string, int>(note, octave);
        }

        /// <summary>
        /// Method converts herz frequency to corresponding mel frequency
        /// </summary>
        /// <param name="herz">Herz frequency</param>
        /// <returns>Mel frequency</returns>
        public static double HerzToMel(double herz)
        {
            return 1127 * Math.Log((herz / 700) + 1); // actually, should be 1127.01048, but HTK and Kaldi seem to use 1127
        }

        /// <summary>
        /// Method converts mel frequency to corresponding herz frequency
        /// </summary>
        /// <param name="mel">Mel frequency</param>
        /// <returns>Herz frequency</returns>
        public static double MelToHerz(double mel)
        {
            return (Math.Exp(mel / 1127) - 1) * 700;
        }

        /// <summary>
        /// Method converts herz frequency to mel frequency (suggested by M.Slaney)
        /// </summary>
        /// <param name="herz">Herz frequency</param>
        /// <returns>Mel frequency</returns>
        public static double HerzToMelSlaney(double herz)
        {
            const double minHerz = 0.0;
            const double sp = 200.0 / 3;
            const double minLogHerz = 1000.0;
            const double minLogMel = (minLogHerz - minHerz) / sp;

            double logStep = Math.Log(6.4) / 27;

            return herz < minLogHerz ? (herz - minHerz) / sp : minLogMel + (Math.Log(herz / minLogHerz) / logStep);
        }

        /// <summary>
        /// Method converts mel frequency to herz frequency (suggested by M.Slaney)
        /// </summary>
        /// <param name="mel">Mel frequency</param>
        /// <returns>Herz frequency</returns>
        public static double MelToHerzSlaney(double mel)
        {
            const double minHerz = 0.0;
            const double sp = 200.0 / 3;
            const double minLogHerz = 1000.0;
            const double minLogMel = (minLogHerz - minHerz) / sp;

            double logStep = Math.Log(6.4) / 27;

            return mel < minLogMel ? minHerz + (sp * mel) : minLogHerz * Math.Exp(logStep * (mel - minLogMel));
        }

        /// <summary>
        /// Method #1 converts herz frequency to corresponding bark frequency
        /// (according to Traunmüller (1990))
        /// </summary>
        /// <param name="herz">Herz frequency</param>
        /// <returns>Bark frequency</returns>
        public static double HerzToBark(double herz)
        {
            return (26.81 * herz / (1960 + herz)) - 0.53;
        }

        /// <summary>
        /// Method #1 converts bark frequency to corresponding herz frequency
        /// (according to Traunmüller (1990))
        /// </summary>
        /// <param name="bark">Bark frequency</param>
        /// <returns>Herz frequency</returns>
        public static double BarkToHerz(double bark)
        {
            return 1960 / ((26.81 / (bark + 0.53)) - 1);
        }

        /// <summary>
        /// Method #2 converts herz frequency to corresponding bark frequency
        /// (according to Wang (1992)); used in M.Slaney's auditory toolbox
        /// </summary>
        /// <param name="herz">Herz frequency</param>
        /// <returns>Bark frequency</returns>
        public static double HerzToBarkSlaney(double herz)
        {
            return 6 * MathUtils.Asinh(herz / 600);
        }

        /// <summary>
        /// Method #2 converts bark frequency to corresponding herz frequency
        /// (according to Wang (1992)); used in M.Slaney's auditory toolbox
        /// </summary>
        /// <param name="bark">Bark frequency</param>
        /// <returns>Herz frequency</returns>
        public static double BarkToHerzSlaney(double bark)
        {
            return 600 * Math.Sinh(bark / 6);
        }

        /// <summary>
        /// Method converts herz frequency to corresponding ERB frequency
        /// </summary>
        /// <param name="herz">Herz frequency</param>
        /// <returns>ERB frequency</returns>
        public static double HerzToErb(double herz)
        {
            return 9.26449 * Math.Log(1.0 + herz) / (24.7 * 9.26449);
        }

        /// <summary>
        /// Method converts ERB frequency to corresponding herz frequency
        /// </summary>
        /// <param name="erb">ERB frequency</param>
        /// <returns>Herz frequency</returns>
        public static double ErbToHerz(double erb)
        {
            return (Math.Exp(erb / 9.26449) - 1.0) * (24.7 * 9.26449);
        }

        /// <summary>
        /// Method for obtaining a perceptual loudness weight
        /// </summary>
        /// <param name="freq">Frequency</param>
        /// <param name="weightingType">Weighting type (A, B, C)</param>
        /// <returns>Weight value in dB</returns>
        public static double LoudnessWeighting(double freq, string weightingType = "A")
        {
            double level2 = freq * freq;

            switch (weightingType.ToUpper())
            {
                case "B":
                    {
                        double r = level2 * freq * 148693636 /
                             (
                                (level2 + 424.36) *
                                 Math.Sqrt(level2 + 25122.25) *
                                (level2 + 148693636)
                             );
                        return (20 * Math.Log10(r)) + 0.17;
                    }

                case "C":
                    {
                        double r = level2 * 148693636 /
                             (
                                 (level2 + 424.36) *
                                 (level2 + 148693636)
                             );
                        return (20 * Math.Log10(r)) + 0.06;
                    }

                default:
                    {
                        double r = level2 * level2 * 148693636 /
                             (
                                 (level2 + 424.36) *
                                  Math.Sqrt((level2 + 11599.29) * (level2 + 544496.41)) *
                                 (level2 + 148693636)
                             );
                        return (20 * Math.Log10(r)) + 2.0;
                    }
            }
        }
    }
}
