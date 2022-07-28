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
        private static int LineSide(double x1, double x2, double y1, double y2, Coord2d pt, double tolerance)
        {
            return ((pt.X - x1) * (y2 - y1) - (pt.Y - y1) * (x2 - x1)).SignWithTolerance(tolerance);
        }

        private static int LineSide(Coord2d p0, Coord2d p1, Coord2d ptTest, double tolerance)
            => LineSide(p0.X, p1.X, p0.Y, p1.Y, ptTest, tolerance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointContainment Contains(Polygon polygon, Coord2d ptr)
            => Contains(polygon.InternalVerticeArray, ptr);
        public static PointContainment Contains(Coord2d[] polygon, Coord2d ptr)
            => ContainsRaycastingMethod(polygon, ptr, Tolerance);
        public static PointContainment ContainsRaycastingMethod(Coord2d[] polygon, Coord2d ptr, double tolerance)
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

                if (Math.Abs(x1 - x2) < tolerance && Math.Abs(y1 - y2) < tolerance)
                    continue;

                var minY = Math.Min(y1, y2);
                var maxY = Math.Max(y1, y2);

                // Bug20220728
                // 这里记得也要加上/扣掉公差
                if (ptr.Y < minY - tolerance || ptr.Y > maxY + tolerance)
                    continue;

                if (Math.Abs(minY - maxY) < tolerance)
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
                    if (Math.Abs(x - ptr.X) <= tolerance)
                        return PointContainment.Coincident;

                    if (ptr.X < x)
                    {
                        ++crossing;
                    }
                }
            }

            return ((crossing & 1) == 0) ? PointContainment.Outside : PointContainment.Inside;
        }
        public static bool ContainsWindingNumberMethod(Coord2d[] coords, Coord2d ptToTest)
            => ContainsWindingNumberMethod(coords, ptToTest, Tolerance);
        public static bool ContainsWindingNumberMethod(Coord2d[] coords, Coord2d ptToTest, double tolerance)
        {
            // https://stackoverflow.com/questions/924171/geo-fencing-point-inside-outside-polygon
            // originally from Manuel Castro
            // modified to avoid extra allocation

            var windingNumber = 0;    // the winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < coords.Length; i++)
            {
                var pt1 = coords[i];
                Coord2d pt2;

                if (i == coords.Length - 1)
                {
                    pt2 = coords[0];
                }
                else
                {
                    pt2 = coords[i + 1];
                }

                // edge from V[i] to V[i+1]
                if (pt1.X <= ptToTest.X + tolerance)
                {         // start y <= P.y
                    if (pt2.X > ptToTest.X - tolerance)      // an upward crossing
                        if (LineSide(pt1, pt2, ptToTest, tolerance) > 0)  // P left of edge
                            ++windingNumber;            // have a valid up intersect
                }
                else
                {                       // start y > P.y (no test needed)
                    if (pt2.X <= ptToTest.X + tolerance)     // a downward crossing
                        if (LineSide(pt1, pt2, ptToTest, tolerance) < 0)  // P right of edge
                            --windingNumber;            // have a valid down intersect
                }
            }

            return windingNumber != 0;
        }

        private const bool RAISE_ERROR_ON_INVALID_SHAPE = false;
        private static bool DetermineCrossingState(
            double x1, double x2, double y1, double y2,
            int i, int j, Coord2d[] polygon)
        {
            var len = polygon.Length;

            var prevPtId = i;
            var nextPtId = j;
            int prevSide;
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

            int nextSide;
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
