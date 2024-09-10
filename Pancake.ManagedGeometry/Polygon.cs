﻿using Pancake.ManagedGeometry.Algo;
using Pancake.ManagedGeometry.DataModel;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

[assembly: InternalsVisibleTo("Pancake.ModernUtility")]

namespace Pancake.ManagedGeometry;

public enum ClockwiseDirection
{
    Clockwise,
    CounterClockwise
}

public enum PointOnEdgeRelation : int
{
    NotOnEdge = 0,
    AtStart = 1,
    InMiddle = 2,
    AtEnd = 3
}

public enum PolygonRelation : int
{
    Unset = 0,
    Intersected,
    ContainsAnother,
    InsideAnother,
    OutsideAnother
}

/// <summary>
/// Determine the category of polygon
/// </summary>
public enum PolygonShape
{
    /// <summary>
    /// The polygon is self-intersecting.
    /// </summary>
    SelfIntersecting,

    /// <summary>
    /// The polygon is convex.
    /// See <a href="https://en.wikipedia.org/wiki/Convex_polygon"/> for definition.
    /// </summary>
    Convex,

    /// <summary>
    /// The polygon is concave.
    /// See <a href="https://en.wikipedia.org/wiki/Concave_polygon"/> for definition.
    /// </summary>
    Concave,

    /// <summary>
    /// The polygon is a degenerate polygon, which means it is stored as a polygon but may represents a line actually.
    /// </summary>
    Degenerate
}

/// <summary>
/// XY 平面上的多边形。第一个点不需要重复一遍。
/// Immutable 2D Polygon on the XY plane. Stores n points for a n-sided polygon.
/// </summary>
public readonly struct Polygon : ICloneable, IPolygon
{
    private readonly Coord2d[] _v;
    public bool IsNull => _v is null;
    public void CopyVerticesTo(Coord2d[] array, int startIndex)
    {
        Array.Copy(InternalVerticeArray, 0, array, startIndex, InternalVerticeArray.Length);
    }
    public Coord2d GetCenter()
    {
        var sumX = 0.0;
        var sumY = 0.0;

        foreach (var pt in InternalVerticeArray)
        {
            sumX += pt.X;
            sumY += pt.Y;
        }

        return (sumX / VertexCount, sumY / VertexCount);
    }
    public bool ContainsAllPoint(Polygon another)
    {
        foreach (var pt in another._v)
            if (!Contains(pt)) return false;
        return true;
    }
    public bool ContainsAllPoint(BoundingBox2d another)
    {
        foreach (var pt in another.Vertices)
            if (!Contains(pt)) return false;
        return true;
    }

    internal Polygon(Coord2d[] vertices, bool noCopyFlag)
    {
        _v = vertices;
    }

    public Polygon(Coord2d[] vertices)
    {
        _v = new Coord2d[vertices.Length];
        vertices.CopyTo(_v, 0);
    }

    public Polygon(List<Coord2d> vertices)
    {
        _v = new Coord2d[vertices.Count];
        vertices.CopyTo(_v, 0);
    }

    public Polygon(ICollection<Coord2d> vertices)
    {
        _v = new Coord2d[vertices.Count];
        vertices.CopyTo(_v, 0);
    }

    /// <summary>
    /// For faster access of the internal vertice array
    /// </summary>
    public Coord2d[] InternalVerticeArray
    {
        get => _v;
    }

    public int VertexCount => _v.Length;
    public static Polygon CreateByCoords(params Coord2d[] vertices) => CreateByRef(vertices);

    /// <summary>
    /// Similar to C++'s std::move semantic, create a polygon using existing vertice array to reduce unnecessary allocation.
    /// </summary>
    /// <param name="vertices">Vertice array to use. You shouldn't operate on the array afterwards.</param>
    /// <returns></returns>
    public static Polygon CreateByRef(Coord2d[] vertices)
    {
        return new Polygon(vertices, noCopyFlag: true);
    }

    /// <summary>
    /// Get the area of the polygon. The result is always positive.
    /// </summary>
    /// <returns>Positive result of the area</returns>
    public double CalculateArea() => Math.Abs(CalculateDirectionalArea());

    public ClockwiseDirection CalculateDirection()
    {
        return this.CalculateDirection<Polygon>();
    }

    /// <summary>
    /// Get the area of the polygon.
    /// The result is negative for counter-clockwise polygon and positive for clockwise polygon.
    /// </summary>
    /// <returns>Directional area</returns>
    public double CalculateDirectionalArea()
    {
        return this.CalculateDirectionalArea<Polygon>();
    }

    public double CalculatePerimeter()
    {
        var sum = 0.0;
        for (var i = 0; i < _v.Length; i++)
            sum += EdgeAt(i).Length;

        return sum;
    }

    /// <summary>
    /// Get the shape category (convex, concanve, self-intersecting, etc.) of the polygon.
    /// You may need to simplify the polygon first to get an accurate result.
    /// </summary>
    /// <returns>Category of the polygon</returns>
    public PolygonShape CalculateShape()
    {
        if (_v.Length < 3) return PolygonShape.Degenerate;
        if (DoesSelfIntersect()) return PolygonShape.SelfIntersecting;

        return CalculateSimpleShape();
    }

    /// <summary>
    /// Get the shape category (convex, concanve etc.) of the polygon.
    /// Compared to <see cref="CalculateShape"/>, this method doesn't calculate self-intersection (which is slow).
    /// </summary>
    /// <returns>Category of the polygon</returns>
    public PolygonShape CalculateSimpleShape()
    {
        if (_v.Length < 3) return PolygonShape.Degenerate;

        Coord2d v1, v2;

        v1 = _v[1] - _v[0];
        v2 = _v[2] - _v[1];

        var sign = Math.Sign(Coord2d.CrossProductLength(v1, v2));

        for (var i = 1; i < _v.Length - 1; i++)
        {
            v1 = v2;
            v2 = _v[(i + 2) % _v.Length] - _v[i + 1];

            if (Math.Sign(Coord2d.CrossProductLength(v1, v2)) != sign)
                return PolygonShape.Concave;
        }

        v1 = v2;
        v2 = _v[1] - _v[0];

        if (Math.Sign(Coord2d.CrossProductLength(v1, v2)) != sign)
            return PolygonShape.Concave;

        return PolygonShape.Convex;
    }

    public void ChangeSeam(int index)
    {
        index = index.UnboundIndex(_v.Length);
        if (index == 0) return;

        var newVertices = new Coord2d[index];

        Array.Copy(_v, newVertices, index);
        Array.Copy(_v, index, _v, 0, _v.Length - index);
        Array.Copy(newVertices, 0, _v, _v.Length - index, index);
    }

    public unsafe void ChangeSeamSmallIndex(int index)
    {
        index = index.UnboundIndex(_v.Length);
        if (index == 0) return;

        var newVertices = stackalloc Coord2d[index];

        for (var i = 0; i < index; i++)
            newVertices[i] = _v[i];

        Array.Copy(_v, index, _v, 0, _v.Length - index);

        for (var i = 0; i < index; i++)
            _v[i + _v.Length - index] = newVertices[i];
    }

    object ICloneable.Clone() => Duplicate();

    public double ClosestDistanceTo(in Coord2d pt)
    {
        return this.ClosestDistanceTo<Polygon>(pt);
    }

    public double ClosestDistToAnother(Polygon another,
        out Coord2d thisPt, out Coord2d anotherPt)
        => ClosestDistanceTo(another, out thisPt, out anotherPt, out _, out _);
    public double ClosestDistanceTo(Polygon another, 
        out Coord2d thisPt, out Coord2d anotherPt,
        out int edgeIndexOnThis, out int edgeIndexOnAnother)
    {
        return this.ClosestDistanceTo<Polygon, Polygon>(another, out thisPt, out anotherPt, out edgeIndexOnThis, out edgeIndexOnAnother);
    }

    public bool Contains(Coord2d pt)
    {
        return PointInsidePolygon.Contains(this, pt) != PointInsidePolygon.PointContainment.Outside;
    }

    public bool Contains(Polygon another)
    {
        return ContainsAllPoint(another) && !IntersectWith(another);
    }

    public bool Contains(Line2d line)
    {
        var lineSideSolver = new LineInsidePolygon();
        return lineSideSolver.IsInside(this, line);
    }
    public bool DoesSelfIntersect()
    {
        var edgeCnt = _v.Length;
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
    }

    /// <summary>
    /// Creates a deepcopy of the current object.
    /// </summary>
    /// <returns></returns>
    public Polygon Duplicate() => CreateByRef((Coord2d[])_v.Clone());
    public bool EnsureDirection(ClockwiseDirection expected = ClockwiseDirection.CounterClockwise)
    {
        var dir = CalculateDirection();
        if (dir != expected)
        {
            Reverse();
            return true;
        }

        return false;
    }

    public BoundingBox2d GetBoundingbox() => new(_v);

    public bool IntersectWith(Polygon b)
    {
        throw new NotImplementedException();
    }

    public Line2d EdgeAt(int startPtId)
    {
        return new Line2d(_v[startPtId], _v[(startPtId + 1) % _v.Length]);
    }
    public Coord2d VertexAt(int verticeId)
    {
        if (verticeId < 0 || verticeId >= InternalVerticeArray.Length)
            return default;
            // ThrowHelperOutOfRange();

        return InternalVerticeArray[verticeId];
    }

    private static void ThrowHelperOutOfRange()
    {
        throw new ArgumentOutOfRangeException();
    }

    public int OnWhichEdge(Coord2d pt, out PointOnEdgeRelation relation)
    {
        for (var i = 0; i < _v.Length; i++)
        {
            var line = EdgeAt(i);

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

    public PolygonRelation RelationTo(Polygon another)
    {
        if (IntersectWith(another)) return PolygonRelation.Intersected;
        if (ContainsAllPoint(another)) return PolygonRelation.ContainsAnother;
        if (another.ContainsAllPoint(this)) return PolygonRelation.InsideAnother;
        return PolygonRelation.OutsideAnother;
    }

    /// <summary>
    /// Flip the direction of the polygon
    /// </summary>
    public void Reverse()
    {
        Array.Reverse(_v);
    }

    public void Rotate(Coord2d center, double angle)
    {
        for (var i = 0; i < VertexCount; i++)
            _v[i] = _v[i].Rotate(center, angle);
    }

    public bool SimilarTo(Polygon another, double sqrEps = 0.003)
    {
        var cnt = _v.Length;
        if (another._v.Length != cnt)
            return false;

        for (var startPos = 0; startPos < cnt; startPos++)
        {
            var j = 0;
            for (; j < cnt; j++)
            {
                if ((_v[j] - another._v[(startPos + j) % cnt]).SquareLength > sqrEps)
                    break;
            }

            if (j >= cnt)
                return true;
        }

        return false;
    }

    internal struct PolygonEdgeEnumeratorBoxed : IEnumerator<Line2d>
    {
        private readonly Polygon _ply;
        private int _index;
        private Coord2d lastPt;
        private Coord2d currentPt;
        public PolygonEdgeEnumeratorBoxed(Polygon ply)
        {
            if (ply._v.Length <= 1)
                throw new ArgumentException("Invalid polygon.");

            lastPt = default;
            currentPt = ply._v[0];
            _index = -1;
            _ply = ply;
        }

        public readonly Line2d Current => (lastPt, currentPt);

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            ++_index;

            if (_index >= _ply._v.Length)
                return false;

            if (_index == _ply._v.Length - 1)
            {
                lastPt = currentPt;
                currentPt = _ply._v[0];
                return true;
            }

            lastPt = currentPt;
            currentPt = _ply._v[_index + 1];

            return true;
        }

        public void Reset()
        {
            _index = -1;
        }
    }

    IEnumerable<Line2d> IPolygon.Edges => BasedEnumerable.Create<Line2d, PolygonEdgeEnumeratorBoxed>(new(this));
    IEnumerable<Coord2d> IPolygon.Vertices => InternalVerticeArray;

    public PolygonEdgeEnumerable Edges => new(this);
    public Coord2d this[int index] => InternalVerticeArray[index];

    public Line2d[] ToLine2dArray()
    {
        var line = new Line2d[_v.Length];
        for (var i = 0; i < line.Length; i++)
            line[i] = EdgeAt(i);

        return line;
    }

    public void Transform(Matrix44 xform)
    {
        for (var i = 0; i < VertexCount; i++)
            _v[i] = _v[i].Transform(xform);
    }

    public void Transform(Func<Coord2d, Coord2d> func)
    {
        for (var i = 0; i < VertexCount; i++)
            _v[i] = func(_v[i]);
    }

    public Polygon TransformDuplicate(Matrix44 xform)
    {
        var ply = Duplicate();
        ply.Transform(xform);
        return ply;
    }

    public void Translate(Coord2d vec)
    {
        for (var i = 0; i < VertexCount; i++)
            _v[i] += vec;
    }
    public bool TrySimplify(out Polygon simplified, double tolerance = MathUtils.ZeroTolerance)
    {
        if (PolylineSimplifier.TrySimplify(_v, true, out var coords, tolerance))
        {
            simplified = Polygon.CreateByRef(coords.ToArray());
            return true;
        }

        simplified = this;
        return false;
    }
}


