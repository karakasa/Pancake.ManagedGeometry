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

namespace Pancake.ManagedGeometry
{
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
    public class Polygon : ICloneable, IPolygon
    {

        internal Coord2d[] _v;
        public void CopyVerticesTo(Coord2d[] array, int startIndex)
        {
            Array.Copy(InternalVerticeArray, 0, array, startIndex, InternalVerticeArray.Length);
        }
        internal static bool HasSeparatingAxis(Polygon a, Polygon b)
        {
            // test each side of a in turn:
            for (var i = 0; i < a.VertexCount; i++)
            {
                var ax = a._v[i].X;
                var ay = a._v[i].Y;

                var nextId = (i + 1) % a.VertexCount;

                var normal_x = a._v[nextId].Y - ay;
                var normal_y = ax - a._v[nextId].X;

                var comparision = ax * normal_x + ay * normal_y + MathUtils.ZeroTolerance;

                for (var j = 0; j < b.VertexCount; j++)
                {
                    var dot_product = ((b._v[j].X) * normal_x) +
                        ((b._v[j].Y) * normal_y);
                    if (dot_product <= comparision) // change sign of test based on winding order
                        break;
                    if (j == b.VertexCount - 1)
                        return true; // all dots were +ve, we found a separating axis
                }
            }
            return false;
        }
        internal static bool HasSeparatingAxis(Polygon a, ref BoundingBox2d b)
        {
            // test each side of a in turn:
            for (var i = 0; i < a.VertexCount; i++)
            {
                var ax = a._v[i].X;
                var ay = a._v[i].Y;

                var nextId = (i + 1) % a.VertexCount;

                var normal_x = a._v[nextId].Y - ay;
                var normal_y = ax - a._v[nextId].X;

                double dot_product;
                var comparision = ax * normal_x + ay * normal_y + MathUtils.ZeroTolerance;

                dot_product = ((b[0].X ) * normal_x) + ((b[0].Y ) * normal_y);
                if (dot_product <= comparision)
                    goto endOfFirstLoop;

                dot_product = ((b[1].X ) * normal_x) + ((b[1].Y ) * normal_y);
                if (dot_product <= comparision)
                    goto endOfFirstLoop;

                dot_product = ((b[2].X ) * normal_x) + ((b[2].Y ) * normal_y);
                if (dot_product <= comparision)
                    goto endOfFirstLoop;

                dot_product = ((b[3].X ) * normal_x) + ((b[3].Y ) * normal_y);
                if (dot_product <= comparision)
                    goto endOfFirstLoop;

                return true;

endOfFirstLoop:
                ;
            }
            return false;
        }

        internal static bool HasSeparatingAxis(ref BoundingBox2d a, Polygon b)
        {
            double normal_x, normal_y;

            normal_x = a[1].Y - a[0].Y;
            normal_y = a[0].X - a[1].X;

            var comparision = a[0].X * normal_x + a[0].Y * normal_x + MathUtils.ZeroTolerance;

            for (var j = 0; j < b.VertexCount; j++)
            {
                var dot_product = ((b._v[j].X ) * normal_x) +
                    ((b._v[j].Y) * normal_y);
                if (dot_product <= comparision) // change sign of test based on winding order
                    break;
                if (j == b.VertexCount - 1)
                    return true; // all dots were +ve, we found a separating axis
            }

            normal_x = a[2].Y - a[1].Y;
            normal_y = a[1].X - a[2].X;

            comparision = a[1].X * normal_x + a[1].Y * normal_x + MathUtils.ZeroTolerance;

            for (var j = 0; j < b.VertexCount; j++)
            {
                var dot_product = ((b._v[j].X) * normal_x) +
                    ((b._v[j].Y) * normal_y);
                if (dot_product <= comparision) // change sign of test based on winding order
                    break;
                if (j == b.VertexCount - 1)
                    return true; // all dots were +ve, we found a separating axis
            }

            normal_x = a[3].Y - a[2].Y;
            normal_y = a[2].X - a[3].X;

            comparision = a[2].X * normal_x + a[2].Y * normal_x + MathUtils.ZeroTolerance;

            for (var j = 0; j < b.VertexCount; j++)
            {
                var dot_product = ((b._v[j].X) * normal_x) +
                    ((b._v[j].Y) * normal_y);
                if (dot_product <= comparision) // change sign of test based on winding order
                    break;
                if (j == b.VertexCount - 1)
                    return true; // all dots were +ve, we found a separating axis
            }

            normal_x = a[0].Y - a[3].Y;
            normal_y = a[3].X - a[0].X;

            comparision = a[3].X * normal_x + a[3].Y * normal_x + MathUtils.ZeroTolerance;

            for (var j = 0; j < b.VertexCount; j++)
            {
                var dot_product = ((b._v[j].X) * normal_x) +
                    ((b._v[j].Y) * normal_y);
                if (dot_product <= comparision) // change sign of test based on winding order
                    break;
                if (j == b.VertexCount - 1)
                    return true; // all dots were +ve, we found a separating axis
            }

            return false;
        }
        internal static bool HasSeparatingAxisGeneric<T1, T2>(T1 a, T2 b)
            where T1 : IPolygon
            where T2 : IPolygon
        {
            var lenA = a.VertexCount;
            var lenB = b.VertexCount;

            // test each side of a in turn:
            for (var i = 0; i < lenA; i++)
            {
                var normal_x = a[(i + 1) % lenA].Y - a[i].Y;
                var normal_y = a[i].X - a[(i + 1) % lenA].X;

                for (var j = 0; j < lenB; j++)
                {
                    var dot_product = ((b[j].X - a[i].X) * normal_x) +
                        ((b[j].Y - a[i].Y) * normal_y);
                    if (dot_product <= MathUtils.ZeroTolerance) // change sign of test based on winding order
                        break;
                    if (j == lenB - 1)
                        return true; // all dots were +ve, we found a separating axis
                }
            }
            return false;
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

        /// <summary>
        /// Creates an empty polygon for further use. Array uninitialized.
        /// </summary>
        internal Polygon()
        {
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

        public Polygon(Coord2d min, Coord2d max)
        {
            _v = new Coord2d[4];
            _v[0] = min;
            _v[1] = new Coord2d(max.X, min.Y);
            _v[2] = max;
            _v[3] = new Coord2d(min.X, max.Y);
        }

        [Obsolete]
        public Polygon(Coord2d basePoint, double width, double height)
                    : this(basePoint - (width / 2, 0), basePoint + (width / 2, height))
        {
        }
        /// <summary>
        /// For faster access of the internal vertice array
        /// </summary>
        public Coord2d[] InternalVerticeArray
        {
            get => _v;
            internal set => _v = value;
        }

        public int VertexCount => _v.Length;

        public static Polygon CreateByCoords(params Coord2d[] vertices)
                    => CreateByRef(vertices);

        /// <summary>
        /// Similar to C++'s std::move semantic, create a polygon using existing vertice array to reduce unnecessary allocation.
        /// </summary>
        /// <param name="vertices">Vertice array to use. You shouldn't operate on the array afterwards.</param>
        /// <returns></returns>
        public static Polygon CreateByRef(Coord2d[] vertices)
        {
            return new Polygon { _v = vertices };
        }

        /// <summary>
        /// Get the area of the polygon. The result is always positive.
        /// </summary>
        /// <returns>Positive result of the area</returns>
        public double CalculateArea()
            => CalculateDirectionalArea().FastAbs();

        public ClockwiseDirection CalculateDirection()
        {
            var sum = 0.0;

            for (var i = 0; i < _v.Length - 1; i++)
                sum += (_v[i + 1].X - _v[i].X) * (_v[i].Y + _v[i + 1].Y);

            sum += (_v[0].X - _v[_v.Length - 1].X) * (_v[_v.Length - 1].Y + _v[0].Y);

            return sum < 0 ? ClockwiseDirection.CounterClockwise : ClockwiseDirection.Clockwise;
        }

        /// <summary>
        /// Get the area of the polygon.
        /// The result is negative for counter-clockwise polygon and positive for clockwise polygon.
        /// </summary>
        /// <returns>Directional area</returns>
        public double CalculateDirectionalArea()
        {
            var area1 = 0.0;
            var area2 = 0.0;

            for (var i = 0; i < _v.Length - 1; i++)
            {
                area1 += _v[i].X * _v[i + 1].Y;
                area2 += _v[i].Y * _v[i + 1].X;
            }

            area1 += _v[_v.Length - 1].X * _v[0].Y;
            area2 += _v[_v.Length - 1].Y * _v[0].X;

            var area = (area1 - area2) / 2;
            return area;
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

        public double ClosestDistanceTo(Coord2d pt)
        {
            var minDistance = double.MaxValue;
            var thisEdgeCnt = _v.Length;

            for (var i = 0; i < thisEdgeCnt; i++)
            {
                var dist = EdgeAt(i).SquareDistanceToPoint(pt);
                if (dist < minDistance)
                    minDistance = dist;
            }

            return Math.Sqrt(minDistance);
        }

        public double ClosestDistToAnother(Polygon another,
            out Coord2d thisPt, out Coord2d anotherPt)
            => ClosestDistanceTo(another, out thisPt, out anotherPt, out _, out _);
        public double ClosestDistanceTo(Polygon another, 
            out Coord2d thisPt, out Coord2d anotherPt,
            out int edgeIndexOnThis, out int edgeIndexOnAnother)
        {
            var minDist = double.MaxValue;
            thisPt = default;
            anotherPt = default;
            var thisEdgeId = -1;
            var thatEdgeId = -1;

            for (var i = 0; i < _v.Length; i++)
            {
                var line1 = EdgeAt(i);

                for (var j = 0; j < another._v.Length; j++)
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

        public bool Contains(Coord2d pt)
        {
            return PointInsidePolygon.Contains(_v, pt) != PointInsidePolygon.PointContainment.Outside;
        }

        public bool Contains(Polygon another)
        {
            return ContainsAllPoint(another) && !IntersectWith(another);
        }
        public bool Contains(BoundingBox2d another)
        {
            return ContainsAllPoint(another) && !IntersectWith(another);
        }

        private LineInsidePolygon _lineSideSolver;

        public bool Contains(Line2d line)
        {
            _lineSideSolver ??= new();
            return _lineSideSolver.IsInside(this, line);
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
        public Polygon Duplicate()
        {
            var ply = new Polygon();
            ply._v = (Coord2d[])_v.Clone();
            return ply;
        }
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
            return !HasSeparatingAxis(this, b) && !HasSeparatingAxis(b, this);
        }

        public bool IntersectWith(BoundingBox2d b)
        {
            return !HasSeparatingAxis(this, ref b) && !HasSeparatingAxis(ref b, this);
        }

        public static bool IntersectsWith<T1, T2>(T1 a, T2 b)
            where T1 : IPolygon
            where T2 : IPolygon
        {
            // https://stackoverflow.com/questions/42464399/2d-rotated-rectangle-collision

            return !HasSeparatingAxisGeneric(a, b) && !HasSeparatingAxisGeneric(b, a);
        }

        [Obsolete("The method is renamed to EdgeAt", true)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line2d LineAt(int startPtId) => EdgeAt(startPtId);
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

        private struct PolygonEdgeEnumerator : IEnumerator<Line2d>
        {
            private readonly Polygon _ply;
            private int _index;
            private Coord2d lastPt;
            private Coord2d currentPt;
            public PolygonEdgeEnumerator(Polygon ply)
            {
                if (ply._v.Length <= 1)
                    throw new ArgumentException("Invalid polygon.");

                lastPt = default;
                currentPt = ply._v[0];
                _index = -1;
                _ply = ply;
            }

            public Line2d Current => (lastPt, currentPt);

            object IEnumerator.Current => Current;

            public void Dispose()
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

        public IEnumerable<Line2d> Edges 
            => BasedEnumerable.Create<Line2d, PolygonEdgeEnumerator>(new(this));
        public IEnumerable<Coord2d> Vertices => InternalVerticeArray;

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

        public bool TrySimplifyInplace(double tolerance = MathUtils.ZeroTolerance)
        {
            if (PolylineSimplifier.TrySimplify(_v, true, out var coords, tolerance))
            {
                _v = coords.ToArray();
                return true;
            }

            return false;
        }
    }
}


