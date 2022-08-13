
using Pancake.ManagedGeometry.DataModel;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("{Min} -> {Max}")]
    public struct BoundingBox2d : IPolygon
    {
        public double MinX;
        public double MaxX;

        public double MinY;
        public double MaxY;

        public double SpanX => MaxX - MinX;
        public double SpanY => MaxY - MinY;

        public BoundingBox2d(Coord2d corner1, Coord2d corner2)
        {
            MinX = corner1.X;
            MaxX = corner2.X;
            MinY = corner1.Y;
            MaxY = corner2.Y;
            MakeValid();
        }
        public BoundingBox2d(IEnumerable<Coord2d> pts)
        {
            var unset = true;

            MinX = MaxX = MinY = MaxY = 0;

            foreach (var it in pts)
                ExpandToContainWithUnset(it, ref unset);
        }

        public BoundingBox2d(Coord2d[] pts)
        {
            var unset = true;

            MinX = MaxX = MinY = MaxY = 0;

            foreach (var it in pts)
                ExpandToContainWithUnset(it, ref unset);
        }
        private void ExpandToContainWithUnset(Coord2d pt, ref bool unset)
        {
            if (unset)
            {
                MinX = MaxX = pt.X;
                MinY = MaxY = pt.Y;
                unset = false;
            }
            else
            {
                ExpandToContain(pt);
            }
        }
        public void ExpandToContain(Coord2d pt)
        {
            if (pt.X > MaxX) MaxX = pt.X;
            if (pt.X < MinX) MinX = pt.X;

            if (pt.Y > MaxY) MaxY = pt.Y;
            if (pt.Y < MinY) MinY = pt.Y;
        }
        private void MakeValid()
        {
            if (MinX > MaxX) LanguageExtensions.Swap(ref MinX, ref MaxX);
            if (MinY > MaxY) LanguageExtensions.Swap(ref MinY, ref MaxY);
        }
        public bool Contains(Coord2d ptr)
        {
            return !(ptr.X < MinX || ptr.X > MaxX || ptr.Y < MinY || ptr.Y > MaxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(double ptrX, double ptrY)
        {
            if (ptrX < MinX || ptrX > MaxX || ptrY < MinY || ptrY > MaxY)
                return false;

            return true;
        }
        public Polygon ToPolygon()
        {
            return new Polygon
            {
                InternalVerticeArray = new Coord2d[] {
                (MinX, MinY),
                (MaxX, MinY),
                (MaxX, MaxY),
                (MinX, MaxY)
                }
            };
        }

        public bool IntersectsWith(BoundingBox2d another)
        {
            return MinX < another.MaxX
                && MaxX > another.MinX
                && MaxY > another.MinY
                && MinY < another.MaxY;
        }

        private struct BBoxEnumerator2d : IEnumerator<Coord2d>
        {
            public int Index;
            public BoundingBox2d BBox;
            public Coord2d Current
                => Index switch
                {
                    0 => (BBox.MinX, BBox.MinY),
                    1 => (BBox.MaxX, BBox.MinY),
                    2 => (BBox.MaxX, BBox.MaxY),
                    3 => (BBox.MinX, BBox.MaxY),
                    _ => (0, 0)
                };

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
                => (++Index) <= 3;

            public void Reset()
                => Index = -1;
        }
        private struct BBoxLineEnumerator2d : IEnumerator<Line2d>
        {
            public int Index;
            public BoundingBox2d BBox;
            public Line2d Current
                => Index switch
                {
                    0 => ((BBox.MinX, BBox.MinY), (BBox.MaxX, BBox.MinY)),
                    1 => ((BBox.MaxX, BBox.MinY), (BBox.MaxX, BBox.MaxY)),
                    2 => ((BBox.MaxX, BBox.MaxY), (BBox.MinX, BBox.MaxY)),
                    3 => ((BBox.MinX, BBox.MaxY), (BBox.MinX, BBox.MinY)),
                    _ => default
                };

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
                => (++Index) <= 3;

            public void Reset()
                => Index = -1;
        }

        private static readonly Direction[] DirectionSide = new Direction[] {
            Direction.LeftTop,
            Direction.Left,
            Direction.LeftBottom,
            Direction.Top,
            Direction.Center,
            Direction.Bottom,
            Direction.RightTop,
            Direction.Right,
            Direction.RightBottom
        };
        public Direction PointOnWhichSide(Coord2d ptToTest)
        {
            var xState = IntervalSign(ptToTest.X, MinX, MaxX);
            var yState = IntervalSign(ptToTest.Y, MinY, MaxY);

            var seq = (xState + 1) * 3 + 2 - (yState + 1);
            return DirectionSide[seq];
        }

        private static int IntervalSign(double pt, double min, double max)
        {
            if (pt < min - MathUtils.ZeroTolerance)
                return -1;

            if (pt > max + MathUtils.ZeroTolerance)
                return 1;

            return 0;
        }

        public Coord2d VertexAt(int verticeId)
        {
            return verticeId switch
            {
                0 => (MinX, MinY),
                1 => (MaxX, MinY),
                2 => (MaxX, MaxY),
                3 => (MinX, MaxY),
                _ => Coord2d.Unset
            };
        }

        public Line2d EdgeAt(int startPtId)
        {
            return startPtId switch
            {
                0 => ((MinX, MinY), (MaxX, MinY)),
                1 => ((MaxX, MinY), (MaxX, MaxY)),
                2 => ((MaxX, MaxY), (MinX, MaxY)),
                3 => ((MinX, MaxY), (MinX, MinY)),
                _ => default
            };
        }

        public void CopyVerticesTo(Coord2d[] array, int startIndex)
        {
            array[startIndex] = this[0];
            array[startIndex + 1] = this[1];
            array[startIndex + 2] = this[2];
            array[startIndex + 3] = this[3];
        }

        public Coord2d Center => ((MinX + MaxX) / 2, (MinY + MaxY) / 2);
        public Coord2d Min => (MinX, MinY);
        public Coord2d Max => (MaxX, MaxY);

        public IEnumerable<Line2d> Edges
            => BasedEnumerable.Create<Line2d, BBoxLineEnumerator2d>(new() { BBox = this, Index = -1 });

        public IEnumerable<Coord2d> Vertices
            => BasedEnumerable.Create<Coord2d, BBoxEnumerator2d>(new (){BBox = this, Index = -1});

        public int VertexCount => 4;

        public Coord2d this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => index switch
                {
                    0 => (MinX, MinY),
                    1 => (MaxX, MinY),
                    2 => (MaxX, MaxY),
                    3 => (MinX, MaxY),
                    _ => default
                };
        }
    }
}
