using Pancake.ManagedGeometry.DataModel;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Algo;

public static partial class PointInsidePolygon
{
    private const bool RAISE_ERROR_ON_INVALID_SHAPE = false;
    public static PointContainment ContainsRaycastingMethod<TPolygon>(TPolygon polygon, in Coord2d ptr)
        where TPolygon : IPolygon
    {
        return ContainsRaycastingMethod(polygon, ptr, Tolerance);
    }
    public static PointContainment ContainsRaycastingMethod<TPolygon>(TPolygon polygon, in Coord2d ptr, double tolerance)
        where TPolygon : IPolygon
    {
        var crossing = 0;
        var len = polygon.VertexCount;

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
    public static bool ContainsWindingNumberMethod<TPolygon>(TPolygon coords, in Coord2d ptToTest)
        where TPolygon : IPolygon
    {
        return ContainsWindingNumberMethod(coords, ptToTest, Tolerance);
    }
    public static bool ContainsWindingNumberMethod<TPolygon>(TPolygon coords, in Coord2d ptToTest, double tolerance)
        where TPolygon : IPolygon
    {
        // https://stackoverflow.com/questions/924171/geo-fencing-point-inside-outside-polygon
        // originally from Manuel Castro
        // modified to avoid extra allocation

        var windingNumber = 0;    // the winding number counter

        // loop through all edges of the polygon
        for (int i = 0; i < coords.VertexCount; i++)
        {
            var pt1 = coords[i];
            Coord2d pt2;

            if (i == coords.VertexCount - 1)
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

    private static bool DetermineCrossingState<TPolygon>(
        double x1, double x2, double y1, double y2,
        int i, int j, TPolygon polygon)
        where TPolygon : IPolygon
    {
        var len = polygon.VertexCount;

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
