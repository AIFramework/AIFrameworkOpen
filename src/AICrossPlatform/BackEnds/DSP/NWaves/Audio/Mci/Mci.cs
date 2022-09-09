using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AI.BackEnds.DSP.NWaves.Audio.Mci
{
    /// <summary>
    /// Статический класс, содержащий функции MCI, импортированные из winmm.dll.
    /// </summary>
    [Serializable]
    public static class Mci
    {
        /// <summary>
        /// Посылает команду
        /// </summary>
        /// <param name="command">Команда</param>
        /// <param name="returnValue"></param>
        /// <param name="returnLength"></param>
        /// <param name="winHandle"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciSendString")]
        public static extern int SendString(
                string command,
                StringBuilder returnValue,
                int returnLength,
                int winHandle);

        /// <summary>
        /// Получает ошибку
        /// </summary>
        /// <param name="dwError"></param>
        /// <param name="lpstrBuffer"></param>
        /// <param name="wLength"></param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciGetErrorString")]
        public static extern uint GetErrorString(
                int dwError,
                StringBuilder lpstrBuffer,
                uint wLength);

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="command">Команда</param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciExecute")]
        public static extern int Execute(string command);
    }
}
