using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AI.BackEnds.DSP.NWaves.Audio.Mci
{
    /// <summary>
    /// Static class containing MCI functions imported from winmm.dll
    /// </summary>
    [Serializable]
    public static class Mci
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        public static extern int SendString(
                string command,
                StringBuilder returnValue,
                int returnLength,
                int winHandle);

        [DllImport("winmm.dll", EntryPoint = "mciGetErrorString")]
        public static extern uint GetErrorString(
                int dwError,
                StringBuilder lpstrBuffer,
                uint wLength);

        [DllImport("winmm.dll", EntryPoint = "mciExecute")]
        public static extern int Execute(string command);
    }
}
