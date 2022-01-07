using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public static class PointInsidePolygon
    {
        public enum PointContainment
        {
            Unset = 0,
            Inside = 1,
            Outside = 2,
            Coincident = 3
        }

        public static double Tolerance = 1e-8;

        private static int LineSide(double x1, double x2, double y1, double y2, Coord2d pt)
        {
            return Math.Sign((pt.X - x1) * (y2 - y1) - (pt.Y - y1) * (x2 - x1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointContainment Contains(Polygon polygon, Coord2d ptr)
            => Contains(polygon.InternalVerticeArray, ptr);
        public static PointContainment Contains(Coord2d[] polygon, Coord2d ptr)
        {
            var crossing = 0;
            var len = polygon.Length;

            for (var i = 0; i < len; i++)
            {
                var j = i + 1;
                if (j == len) j = 0;

                var p1 = polygon[i];
                var p2 = polygon[j];

                var y1 = p1.Y;
                var y2 = p2.Y;

                var x1 = p1.X;
                var x2 = p2.X;

                if (Math.Abs(x1 - x2) < Tolerance && Math.Abs(y1 - y2) < Tolerance)
                    continue;

                var minY = Math.Min(y1, y2);
                var maxY = Math.Max(y1, y2);

                if (ptr.Y < minY || ptr.Y > maxY)
                    continue;

                if (Math.Abs(minY - maxY) < Tolerance)
                {
                    // 平行于 X 轴的边且 Y 坐标和点一致
                    var minX = Math.Min(x1, x2);
                    var maxX = Math.Max(x1, x2);

                    if (ptr.X >= minX && ptr.X <= maxX)
                    {
                        return PointContainment.Coincident;
                    }
                    else
                    {
                        // 2022/01/08：这里有问题。要写单元测试。
                        // 考虑是否退化到 Winding 方法

                        if (ptr.X < minX)
                        {
                            if (DetermineCrossingState(x1, x2, y1, y2, i, j, polygon))
                                ++crossing;
                        }
                    }
                }
                else
                {
                    // 不平行于 X 轴的边

                    var x = (x2 - x1) * (ptr.Y - y1) / (y2 - y1) + x1;
                    if (Math.Abs(x - ptr.X) <= Tolerance)
                        return PointContainment.Coincident;

                    if (ptr.X < x)
                    {
                        ++crossing;
                    }
                }
            }

            return ((crossing & 1) == 0) ? PointContainment.Outside : PointContainment.Inside;
        }

        private const bool RAISE_ERROR_ON_INVALID_SHAPE = false;
        private static bool DetermineCrossingState(
            double x1, double x2, double y1, double y2,
            int i, int j, Coord2d[] polygon)
        {
            var len = polygon.Length;

            var prevPtId = i;
            var nextPtId = j;

            var prevSide = 0;
            var nextSide = 0;

            for (; ; )
            {
                prevPtId = (prevPtId - 1).UnboundIndex(len);
                if (prevPtId == i)
                {
                    if (RAISE_ERROR_ON_INVALID_SHAPE)
                        throw new InvalidOperationException();
                    else
                        return false;
                }

                var prevPt = polygon[prevPtId];
                prevSide = LineSide(x1, x2, y1, y2, prevPt);
                if (prevSide != 0)
                    break;
            }

            for (; ; )
            {
                nextPtId = (nextPtId + 1).UnboundIndex(len);
                if (nextPtId == j)
                {
                    if (RAISE_ERROR_ON_INVALID_SHAPE)
                        throw new InvalidOperationException();
                    else
                        return false;
                }

                var nextPt = polygon[nextPtId];
                nextSide = LineSide(x1, x2, y1, y2, nextPt);
                if (nextSide != 0)
                    break;
            }

            return prevSide != nextSide;
        }
    }
}
