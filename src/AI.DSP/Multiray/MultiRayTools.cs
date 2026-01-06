using AI.DataStructs.Algebraic;
using AI.DSP.Multiray.Sources;
using AI.HightLevelFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DSP.Multiray
{
    public class MultiRayTools
    {

        /// <summary>
        /// Получение сигнала на рефлекторе или детекторе
        /// </summary>
        /// <param name="signals"></param>
        /// <param name="collectorCoords"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Vector CollectSignals(IEnumerable<Source> signals, GeometrySignalObject collectorCoords, double speed)
        {
            Vector signal = null;

            foreach (var source in signals)
            {
                if (signal == null)
                    signal = GetSignalOnDetector(collectorCoords, source, speed);
                else
                    signal += GetSignalOnDetector(collectorCoords, source, speed);
            }

            return signal;
        }

        private static Vector GetSignalOnDetector(GeometrySignalObject collectorCoords, Source source, double speed)
        {
            double d = GetDist(collectorCoords, source);
            return source.GetSignal(d, speed);
        }

        public static double GetDist(GeometrySignalObject go1, GeometrySignalObject go2) =>
            AnalyticGeometryFunctions.DistanceFromAToB(go1.Coordinates, go2.Coordinates);

        public static double GetDeltaT(GeometrySignalObject go1, GeometrySignalObject go2, GeometrySignalObject ancor, double v)
        {
            double t1 = GetDist(go1, ancor) / v;
            double t2 = GetDist(go2, ancor) / v;
            return t2 - t1;
        }
    }
}
