using AI.DataStructs.Algebraic;
using AI.DSP.Analyse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI.Charts.Control
{
    [Serializable]
    public partial class SpectrumWelchAnalyzer : UserControl
    {
        /// <summary>
        /// Тип представления СПМ (по умолчанию в дб)
        /// </summary>
        public WelchPSDType WelchPSDTypeData { get; set; } = WelchPSDType.Db;

        /// <summary>
        /// Смещение частоты
        /// </summary>
        public double FreqOffset { get; set; } = 0;

        /// <summary>
        /// Частота дискретизации
        /// </summary>
        public int SR { get; set; } = 80000;

        /// <summary>
        /// Размер блока БПФ преобразования
        /// </summary>
        public int FFTBlock { get; set; } = 1024;

        /// <summary>
        /// Веса окна
        /// </summary>
        public Vector WindowW => WindowFunc(FFTBlock);

        /// <summary>
        /// Оконная функция (По-умолчанию окно Блэкмана)
        /// </summary>
        public Func<int, Vector> WindowFunc = WindowForFFT.BlackmanWindow;

        /// <summary>
        /// Анализ спектра методом Уэлча
        /// </summary>
        public SpectrumWelchAnalyzer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Спектральный анализ сигнала
        /// </summary>
        /// <param name="signal"></param>
        public void Analyze(Vector signal) 
        {
            Vector fft = Welch.WelchRun(signal, FFTBlock, 0.5, WindowW)/FFTBlock;
            WelchData welchData = new WelchData(fft, SR, WelchPSDTypeData);
            chartVisual1.PlotBlack(welchData.HalfFreq + FreqOffset, welchData.HalfPSD);
        }
    }
}
