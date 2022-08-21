using AI.BackEnds.DSP.NWaves.Filters.Butterworth;
using AI.DataStructs.Algebraic;
using AI.DSP.IIR;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.FeatureExtractors.TimeSeq
{
    public class FilterExtractor : TSExtractor
    {
        /// <summary>
        /// Функция преобразование фильрованного сигнала в число
        /// </summary>
        public Func<Vector, double> Transformer { get; set; }

        /// <summary>
        /// Банк БИХ фильтров
        /// </summary>
        public IIRFilter[] IIRs { get; set; }

        /// <summary>
        /// Экстрактор признаков на базе фильтрации
        /// </summary>
        /// <param name="fCutDown">Нижние частоты среза</param>
        /// <param name="fCutUp">Верхние частоты среза</param>
        /// <param name="sr">Частота дискретизации</param>
        /// <param name="order">Порядок фильтров</param>
        public FilterExtractor(double[] fCutDown, double[] fCutUp, double sr, int order = 5) 
        {

            Transformer = Statistic.СalcVariance;

            IIRs = new IIRFilter[fCutDown.Length];

            for (int i = 0; i < fCutDown.Length; i++)
            {
                // Расчет фильтров
                double fCNormD = fCutDown[i] / sr;
                double fCNormU = fCutUp[i] / sr;
                var filter = new BandPassFilter(fCNormD, fCNormU, order);
                Vector a1 = new Vector(filter._a);
                Vector b1 = new Vector(filter._b);
                Vector b = b1.CutAndZero(b1.Count / 2);
                Vector a = a1.CutAndZero(a1.Count / 2);
                IIRs[i] = new IIRFilter(a, b);
            }
        }



        /// <summary>
        /// Возвращает признаки после банка фильтров
        /// </summary>
        public override Vector GetFeatures(Vector crop)
        {
            Vector features = new Vector(IIRs.Length);
            
            for (int i = 0; i < features.Count; i++)
                features[i] = Transformer(IIRs[i].FilterOutp(crop));
            
            return features;
        }
    }
}
