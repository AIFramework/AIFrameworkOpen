using AI.BackEnds.DSP.NWaves.Transforms;
using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Обертка для ДКП-2
    /// </summary>
    [Serializable]
    public class DCT2NWaveWrapper
    {
        private readonly Dct2 dct2;
        private readonly FastDct2 fastDct;

        /// <summary>
        /// Число отсчетов ДКП
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Испольльзовать ли быструю реализацию
        /// </summary>
        public bool IsFast { get; private set; }

        /// <summary>
        /// Обертка для ДКП-2
        /// </summary>
        public DCT2NWaveWrapper(int countElements)
        {
            Count = countElements;
            if (IsPow2(countElements))
            {
                fastDct = new FastDct2(Count);
                IsFast = true;
            }
            else
            {
                dct2 = new Dct2(Count);
                IsFast = false;
            }
        }

        #region Вспомогательные методы
        private static bool IsPow2(int n)
        {
            int pow = (int)Math.Log(n, 2);
            return n == 1 << pow;
        }

        /// <summary>
        /// Прямое преобразование
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public Vector DirectDCT(Vector st)
        {
            float[] input = (float[])st;
            float[] dctArr = new float[Count];
            if (IsFast) fastDct.Direct(input, dctArr);
            else dct2.Direct(input, dctArr);

            return dctArr;
        }

        /// <summary>
        /// Прямое нормированное преобразование
        /// </summary>
        public Vector DirectDCTNorm(Vector st)
        {
            float[] input = (float[])st;
            float[] dctArr = new float[Count];
            if (IsFast) fastDct.DirectNorm(input, dctArr);
            else dct2.DirectNorm(input, dctArr);
            return dctArr;
        }

        /// <summary>
        /// Обратное преобразование
        /// </summary>
        public Vector InversDCT(Vector dct)
        {
            float[] input = (float[])dct;
            float[] dctArr = new float[Count];
            if (IsFast) fastDct.Inverse(input, dctArr);
            else dct2.Inverse(input, dctArr);
            return dctArr;
        }

        private Vector InversDCTNorm(Vector dct)
        {
            float[] input = (float[])dct;
            float[] dctArr = new float[Count];
            //ToDo: Реализовать быстрое обратное преобразоние
            dct2.InverseNorm(input, dctArr);
            return dctArr;
        }

        #endregion

    }
}
