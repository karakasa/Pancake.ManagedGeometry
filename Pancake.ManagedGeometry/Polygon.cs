using Pancake.ManagedGeometry.Algo;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Pancake.ManagedGeometry
{
    public enum PolygonShape
    {
        SelfIntersecting,
        Convex,
        Concave,
        Degenerate
    }
    /// <summary>
    /// XY 平面上的多边形。第一个点不需要重复一遍。
    /// </summary>
    public class Polygon : ICloneable
    {
        private Coord2d[] _v;
        
        /// <summary>
        /// Do not use the property unless you know what you are doing.
        /// </summary>
        public Coord2d[] InternalVerticeArray
        {
            get => _v;
            internal set => _v = value;
        }

        public int VerticeCount => _v.Length;
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
        public void Translate(Coord2d vec)
        {
            for (var i = 0; i < VerticeCount; i++)
                _v[i] += vec;
        }
        public void Transform(Matrix44 xform)
        {
            for (var i = 0; i < VerticeCount; i++)
                _v[i] = _v[i].Transform(xform);
        }
        public void Transform(Func<Coord2d, Coord2d> func)
        {
            for (var i = 0; i < VerticeCount; i++)
                _v[i] = func(_v[i]);
        }

        public Polygon TransformDuplicate(Matrix44 xform)
        {
            var ply = Duplicate();
            ply.Transform(xform);
            return ply;
        }
        /// <summary>
        /// Creates an empty polygon for further use. Array uninitialized.
        /// </summary>
        internal Polygon()
        {
        }

        public static Polygon CreateByRef(Coord2d[] vertices)
        {
            return new Polygon { _v = vertices };
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

        public Polygon(Coord2d basePoint, double width, double height) 
            : this(basePoint - (width / 2, 0), basePoint + (width / 2, height))
        {
        }

        public void Rotate(Coord2d center, double angle)
        {
            for (var i = 0; i < VerticeCount; i++)
                _v[i].Rotate(center, angle);
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

        public double CalculateArea()
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

            var area = (area1 - area2) * 0.5;
            return (area > 0) ? area : -area;
        }
        private static bool HasSeparatingAxis(Polygon a, Polygon b)
        {
            // test each side of a in turn:
            for (var i = 0; i < a.VerticeCount; i++)
            {
                var normal_x = a._v[(i + 1) % a.VerticeCount].Y - a._v[i].Y;
                var normal_y = a._v[i].X - a._v[(i + 1) % a.VerticeCount].X;

                for (var j = 0; j < b.VerticeCount; j++)
                {
                    var dot_product = ((b._v[j].X - a._v[i].X) * normal_x) +
                        ((b._v[j].Y - a._v[i].Y) * normal_y);
                    if (dot_product <= MathUtils.ZeroTolerance) // change sign of test based on winding order
                        break;
                    if (j == b.VerticeCount - 1)
                        return true; // all dots were +ve, we found a separating axis
                }
            }
            return false;
        }

        public Line2d[] ToLine2dArray()
        {
            var line = new Line2d[_v.Length];
            for (var i = 0; i < line.Length; i++)
                line[i] = LineAt(i);

            return line;
        }

        public bool IntersectWith(Polygon b)
        {
            // https://stackoverflow.com/questions/42464399/2d-rotated-rectangle-collision

            return !HasSeparatingAxis(this, b) && !HasSeparatingAxis(b, this);
        }

        public double ClosestDistToAnother(Polygon another, out Coord2d thisPt, out Coord2d anotherPt)
        {
            var minDist = double.MaxValue;
            thisPt = default;
            anotherPt = default;

            for (var i = 0; i < _v.Length; i++)
            {
                var line1 = LineAt(i);

                for (var j = 0; j < another._v.Length; j++)
                {
                    var line2 = another.LineAt(j);

                    var dist = line1.NearestPtToAnotherLine(line2, out var ptOnThis, out var outsidePt);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        thisPt = ptOnThis;
                        anotherPt = outsidePt;
                    }
                }
            }

            return minDist;
        }
       
        public enum PointOnEdgeRelation : int
        {
            NotOnEdge = 0,
            AtStart = 1,
            InMiddle = 2,
            AtEnd = 3
        }
        public int OnWhichEdge(Coord2d pt, out PointOnEdgeRelation relation)
        {
            for (var i = 0; i < _v.Length; i++)
            {
                var line = LineAt(i);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line2d LineAt(int startPtId)
        {
            return new Line2d(_v[startPtId], _v[(startPtId + 1) % _v.Length]);
        }
        public Line2d LineAtUnbounded(int startPtId)
        {
            startPtId = startPtId.UnboundIndex(_v.Length);

            return new Line2d(_v[startPtId], _v[(startPtId + 1) % _v.Length]);
        }

        object ICloneable.Clone() => Duplicate();


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

        public bool DoesSelfIntersect()
        {
            var edgeCnt = _v.Length;
            for (var i = 0; i < edgeCnt; i++)
            {
                var l1 = LineAt(i);
                for (var j = i + 2; j <= edgeCnt; j++)
                {
                    var l2 = LineAtUnbounded(j);

                    var result = l1.DoesIntersectWith(l2);
                    if (result == LineRelation.Intersected || result == LineRelation.Collinear)
                        return true;
                }
            }

            return false;
        }
        public PolygonShape CalculateShape()
        {
            if (_v.Length < 3) return PolygonShape.Degenerate;
            if (DoesSelfIntersect()) return PolygonShape.SelfIntersecting;

            return CalculateSimpleShape();
        }
        public double ClosestDistanceTo(Coord2d pt)
        {
            var minDistance = double.MaxValue;
            var thisEdgeCnt = _v.Length;

            for (var i = 0; i < thisEdgeCnt; i++)
            {
                var dist = LineAt(i).SquareDistanceToPoint(pt);
                if (dist < minDistance)
                    minDistance = dist;
            }

            return Math.Sqrt(minDistance);
        }
        public BoundingBox2d GetBoundingbox() => new(_v);
        private bool ContainsAllPoint(Polygon another)
        {
            foreach (var pt in another._v)
                if (!Contains(pt)) return false;
            return true;
        }
        public bool Contains(Coord2d pt)
        {
            return PointInsidePolygon.Contains(_v, pt) != PointInsidePolygon.PointContainment.Outside;
        }
        public bool Contains(Polygon another)
        {
            return ContainsAllPoint(another) && !IntersectWith(another);
        }

        public enum PolygonRelation : int
        {
            Unset = 0,
            Intersected,
            ContainsAnother,
            InsideAnother,
            OutsideAnother
        }

        public PolygonRelation RelationTo(Polygon another)
        {
            if (IntersectWith(another)) return PolygonRelation.Intersected;
            if (ContainsAllPoint(another)) return PolygonRelation.ContainsAnother;
            if (another.ContainsAllPoint(this)) return PolygonRelation.InsideAnother;
            return PolygonRelation.OutsideAnother;
        }
        public double CalculatePerimeter()
        {
            var sum = 0.0;
            for (var i = 0; i < _v.Length; i++)
                sum += LineAt(i).Length;

            return sum;
        }
    }
}
