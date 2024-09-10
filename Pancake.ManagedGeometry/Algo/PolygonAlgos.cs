using Pancake.ManagedGeometry.DataModel;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo;

public static class PolygonAlgos
{
    public static double CalculateArea<TPolygon>(this TPolygon ply) where TPolygon : IPolygon
        => Math.Abs(CalculateDirectionalArea(ply));

    public static ClockwiseDirection CalculateDirection<TPolygon>(this TPolygon ply) where TPolygon : IPolygon
    {
        var sum = 0.0;

        for (var i = 0; i < ply.VertexCount - 1; i++)
            sum += (ply[i + 1].X - ply[i].X) * (ply[i].Y + ply[i + 1].Y);

        sum += (ply[0].X - ply[ply.VertexCount - 1].X) * (ply[ply.VertexCount - 1].Y + ply[0].Y);

        return sum < 0 ? ClockwiseDirection.CounterClockwise : ClockwiseDirection.Clockwise;
    }

    /// <summary>
    /// Get the area of the polygon.
    /// The result is negative for counter-clockwise polygon and positive for clockwise polygon.
    /// </summary>
    /// <returns>Directional area</returns>
    public static double CalculateDirectionalArea<TPolygon>(this TPolygon ply) where TPolygon : IPolygon
    {
        var area1 = 0.0;
        var area2 = 0.0;

        for (var i = 0; i < ply.VertexCount - 1; i++)
        {
            area1 += ply[i].X * ply[i + 1].Y;
            area2 += ply[i].Y * ply[i + 1].X;
        }

        area1 += ply[ply.VertexCount - 1].X * ply[0].Y;
        area2 += ply[ply.VertexCount - 1].Y * ply[0].X;

        var area = (area1 - area2) / 2;
        return area;
    }

    public static double ClosestDistanceTo<TPolygon>(this TPolygon ply, in Coord2d pt) where TPolygon : IPolygon
    {
        var minDistance = double.MaxValue;
        var thisEdgeCnt = ply.VertexCount;

        for (var i = 0; i < thisEdgeCnt; i++)
        {
            var dist = ply.EdgeAt(i).SquareDistanceToPoint(pt);
            if (dist < minDistance)
                minDistance = dist;
        }

        return Math.Sqrt(minDistance);
    }
    public static double ClosestDistanceTo<TPolygon1, TPolygon2>(this TPolygon1 ply, TPolygon2 another,
        out Coord2d thisPt, out Coord2d anotherPt,
        out int edgeIndexOnThis, out int edgeIndexOnAnother)
        where TPolygon1 : IPolygon
        where TPolygon2 : IPolygon
    {
        var minDist = double.MaxValue;
        thisPt = default;
        anotherPt = default;
        var thisEdgeId = -1;
        var thatEdgeId = -1;

        for (var i = 0; i < ply.VertexCount; i++)
        {
            var line1 = ply.EdgeAt(i);

            for (var j = 0; j < another.VertexCount; j++)
            {
                var line2 = another.EdgeAt(j);

                var dist = line1.NearestPtToAnotherLine(line2, out var ptOnThis, out var outsidePt);
                if (dist < minDist)
                {
                    thisEdgeId = i;
                    thatEdgeId = j;

                    minDist = dist;
                    thisPt = ptOnThis;
                    anotherPt = outsidePt;
                }
            }
        }

        edgeIndexOnThis = thisEdgeId;
        edgeIndexOnAnother = thatEdgeId;

        return minDist;
    }

    public static bool Contains<TPolygon>(this TPolygon ply, in Coord2d pt) where TPolygon : IPolygon
    {
        return PointInsidePolygon.Contains(ply, pt) != PointInsidePolygon.PointContainment.Outside;
    }

    /*public static bool Contains<TPolygon1, TPolygon2>(this TPolygon1 ply, TPolygon2 another)
        where TPolygon1 : IPolygon
        where TPolygon2 : IPolygon
    {
        return ContainsAllPoint(another) && !IntersectWith(another);
    }*/

    /*public bool DoesSelfIntersect()
    {
        var edgeCnt = ply.VertexCount;
        for (var i = 0; i < edgeCnt; i++)
        {
            var l1 = EdgeAt(i);
            for (var j = i + 2; j <= edgeCnt; j++)
            {
                var l2 = EdgeAt(j.UnboundIndex(edgeCnt));

                var result = l1.DoesIntersectWith(l2);
                if (result == LineRelation.Intersected || result == LineRelation.Collinear)
                    return true;
            }
        }

        return false;
    }*/

    /*public bool IntersectWith(Polygon b)
    {
        throw new NotImplementedException();
    }*/

    public static int OnWhichEdge<TPolygon>(this TPolygon ply, in Coord2d pt, out PointOnEdgeRelation relation)
        where TPolygon : IPolygon
    {
        for (var i = 0; i < ply.VertexCount; i++)
        {
            var line = ply.EdgeAt(i);

            var t = line.NearestPoint(pt);
            var closetPt = line.PointAt(t);

            if ((pt - closetPt).Length > MathUtils.ZeroTolerance)
                continue;

            if (t.CloseToZero())
            {
                relation = PointOnEdgeRelation.AtStart;
            }
            else if ((t - 1).CloseToZero())
            {
                relation = PointOnEdgeRelation.AtEnd;
            }
            else
            {
                relation = PointOnEdgeRelation.InMiddle;
            }

            return i;
        }

        relation = PointOnEdgeRelation.NotOnEdge;
        return -1;
    }
}
