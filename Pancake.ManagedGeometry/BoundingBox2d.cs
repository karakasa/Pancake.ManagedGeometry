// #define HIDE_BBOX_2D_IENUMERABLE_INTERFACE

using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public struct BoundingBox2d
#if !HIDE_BBOX_2D_IENUMERABLE_INTERFACE
    : IEnumerable<Coord2d>
#endif
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
            if (ptr.X < MinX || ptr.X > MaxX || ptr.Y < MinY || ptr.Y > MaxY)
                return false;

            return true;
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
        public IEnumerator<Coord2d> GetEnumerator()
            => new BBoxEnumerator2d { Index = -1, BBox = this };

#if !HIDE_BBOX_2D_IENUMERABLE_INTERFACE
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
#endif
    }
}
