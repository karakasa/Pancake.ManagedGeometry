using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public static class PolylineSimplifier
    {
        public static bool TrySimplify(IEnumerable<Coord2d> coords, bool closedPolyline, out List<Coord2d> outCoords, double tolerance = MathUtils.ZeroTolerance)
        {
            var list = new List<Coord2d>(coords);
            var originalLength = list.Count;
            var index = 0;

            while (TryMergeIdenticalPoints(list, closedPolyline, ref index, tolerance)) ;

            index = 0;

            while (TryRemovePairFromList(list, closedPolyline, ref index, tolerance)) ;

            outCoords = list;
            return list.Count < originalLength;
        }
        private static bool TryMergeIdenticalPoints(List<Coord2d> coords, bool closedPolyline, ref int latestScannedIndex, double tolerance)
        {
            for (; ; )
            {
                ThrowForDegeneratedPolygon(coords);

                if (latestScannedIndex >= coords.Count)
                    return false;

                if (latestScannedIndex == coords.Count - 1)
                {
                    // 最后一个点

                    if (!closedPolyline)
                        return false;

                    if (coords[latestScannedIndex].AlmostEqualTo(coords[0]))
                    {
                        coords.RemoveAt(latestScannedIndex);
                        continue;
                    }
                }
                else
                {
                    if (coords[latestScannedIndex].AlmostEqualTo(coords[latestScannedIndex + 1]))
                    {
                        coords.RemoveAt(latestScannedIndex + 1);
                        continue;
                    }
                }

                ++latestScannedIndex;
            }
        }
        private static bool TryRemovePairFromList(List<Coord2d> coords, bool closedPolyline, ref int latestScannedIndex, double tolerance)
        {
            for (; ; )
            {
                ThrowForDegeneratedPolygon(coords);

                if (latestScannedIndex >= coords.Count)
                    return false;

                if (latestScannedIndex == coords.Count - 1)
                {
                    // 最后一个点

                    if (!closedPolyline)
                        return false;

                    if (Coord2d.IsColinear(coords[coords.Count - 1], coords[0], coords[1], tolerance))
                    {
                        coords.RemoveAt(0);
                    }

                    return false;
                }
                else if (latestScannedIndex == coords.Count - 2)
                {
                    // 倒数第二个点

                    if (!closedPolyline)
                        return false;

                    if (Coord2d.IsColinear(coords[coords.Count - 2], coords[coords.Count - 1], coords[0], tolerance))
                    {
                        coords.RemoveAt(coords.Count - 1);
                        continue;
                    }
                }
                else
                {
                    if (Coord2d.IsColinear(coords[latestScannedIndex], coords[latestScannedIndex + 1], coords[latestScannedIndex + 2], tolerance))
                    {
                        coords.RemoveAt(latestScannedIndex + 1);
                        continue;
                    }
                }

                ++latestScannedIndex;
            }
        }
        private static void ThrowForDegeneratedPolygon(List<Coord2d> coords)
        {
            if (coords.Count < 3)
                throw new InvalidOperationException("多段线顶点数小于 3，有可能是退化的多段线。");
        }
    }

}
