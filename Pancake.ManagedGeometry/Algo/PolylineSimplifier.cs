using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public static class PolylineSimplifier
    {
        public static bool TrySimplify(IEnumerable<Coord2d> coords, bool closedPolyline, out List<Coord2d> outCoords)
        {
            var list = new List<Coord2d>(coords);
            var originalLength = list.Count;
            var index = 0;

            while (TryRemovePairFromList(list, closedPolyline, ref index)) ;

            outCoords = list;
            return list.Count < originalLength;
        }

        private static bool TryRemovePairFromList(List<Coord2d> coords, bool closedPolyline, ref int latestScannedIndex)
        {
            for (; ; )
            {
                if (coords.Count < 3)
                    throw new InvalidOperationException("Polygon has fewer than 3 vertices. Is it a degenerate polygon?");

                if (latestScannedIndex >= coords.Count)
                    return false;

                if (latestScannedIndex == coords.Count - 1)
                {
                    // 最后一个点

                    if (!closedPolyline)
                        return false;

                    if (Coord2d.IsColinear(coords[coords.Count - 1], coords[0], coords[1]))
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

                    if (Coord2d.IsColinear(coords[coords.Count - 2], coords[coords.Count - 1], coords[0]))
                    {
                        coords.RemoveAt(coords.Count - 1);
                        continue;
                    }
                }
                else
                {
                    if (Coord2d.IsColinear(coords[latestScannedIndex], coords[latestScannedIndex + 1], coords[latestScannedIndex + 2]))
                    {
                        coords.RemoveAt(latestScannedIndex + 1);
                        continue;
                    }
                }

                ++latestScannedIndex;
            }
        }
        public static bool TrySimplify(IEnumerable<Coord> coords, bool closedPolyline, out List<Coord> outCoords)
        {
            var list = new List<Coord>(coords);
            var originalLength = list.Count;
            var index = 0;

            while (TryRemovePairFromList(list, closedPolyline, ref index)) ;

            outCoords = list;
            return list.Count < originalLength;
        }

        private static bool TryRemovePairFromList(List<Coord> coords, bool closedPolyline, ref int latestScannedIndex)
        {
            for (; ; )
            {
                if (coords.Count < 3)
                    throw new InvalidOperationException("Polygon has fewer than 3 vertices. Is it a degenerate polygon?");

                if (latestScannedIndex >= coords.Count)
                    return false;

                if (latestScannedIndex == coords.Count - 1)
                {
                    // 最后一个点

                    if (!closedPolyline)
                        return false;

                    if (Coord.IsColinear(coords[coords.Count - 1], coords[0], coords[1]))
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

                    if (Coord.IsColinear(coords[coords.Count - 2], coords[coords.Count - 1], coords[0]))
                    {
                        coords.RemoveAt(coords.Count - 1);
                        continue;
                    }
                }
                else
                {
                    if (Coord.IsColinear(coords[latestScannedIndex], coords[latestScannedIndex + 1], coords[latestScannedIndex + 2]))
                    {
                        coords.RemoveAt(latestScannedIndex + 1);
                        continue;
                    }
                }

                ++latestScannedIndex;
            }
        }
    }
}
