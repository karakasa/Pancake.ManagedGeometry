// #define HIDE_BBOX_2D_IENUMERABLE_INTERFACE

using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("({Min}), ({Max})")]
    public struct BoundingBox3d : IEnumerable<Coord>
    {
        public double MinX;
        public double MaxX;

        public double MinY;
        public double MaxY;

        public double MinZ;
        public double MaxZ;

        public double SpanX => MaxX - MinX;
        public double SpanY => MaxY - MinY;
        public double SpanZ => MaxZ - MinZ;
        public BoundingBox3d(Coord corner1, Coord corner2)
        {
            MinX = corner1.X;
            MaxX = corner2.X;
            MinY = corner1.Y;
            MaxY = corner2.Y;
            MinZ = corner1.Z;
            MaxZ = corner2.Z;
            MakeValid();
        }
        public BoundingBox3d(BoundingBox2d bbox2d, double elevation)
        {
            MinX = bbox2d.MinX;
            MaxX = bbox2d.MaxX;
            MinY = bbox2d.MinY;
            MaxY = bbox2d.MaxY;
            MinZ = MaxZ = elevation;
        }
        public BoundingBox3d(IEnumerable<Coord> pts)
        {
            var unset = true;

            MinX = MaxX = MinY = MaxY = MinZ = MaxZ = 0;

            foreach (var it in pts)
                ExpandToContainWithUnset(it, ref unset);
        }

        public BoundingBox3d(Coord[] pts)
        {
            var unset = true;

            MinX = MaxX = MinY = MaxY = MinZ = MaxZ = 0;

            foreach (var it in pts)
                ExpandToContainWithUnset(it, ref unset);
        }
        private void ExpandToContainWithUnset(Coord pt, ref bool unset)
        {
            if (unset)
            {
                MinX = MaxX = pt.X;
                MinY = MaxY = pt.Y;
                MinZ = MaxZ = pt.Z;
                unset = false;
            }
            else
            {
                ExpandToContain(pt);
            }
        }
        public void ExpandToContain(Coord pt)
        {
            if (pt.X > MaxX) MaxX = pt.X;
            if (pt.X < MinX) MinX = pt.X;

            if (pt.Y > MaxY) MaxY = pt.Y;
            if (pt.Y < MinY) MinY = pt.Y;

            if (pt.Z > MaxZ) MaxZ = pt.Z;
            if (pt.Z < MinZ) MinZ = pt.Z;
        }
        private void MakeValid()
        {
            if (MinX > MaxX) LanguageExtensions.Swap(ref MinX, ref MaxX);
            if (MinY > MaxY) LanguageExtensions.Swap(ref MinY, ref MaxY);
            if (MinZ > MaxZ) LanguageExtensions.Swap(ref MinZ, ref MaxZ);
        }
        public bool Contains(Coord ptr)
        {
            if (ptr.X < MinX || ptr.X > MaxX || ptr.Y < MinY || ptr.Y > MaxY
                || ptr.Z < MinZ || ptr.Z > MaxZ)
                return false;

            return true;
        }

        public bool IntersectsWith(BoundingBox3d another)
        {
            return MinX < another.MaxX
                && MaxX > another.MinX
                && MaxY > another.MinY
                && MinY < another.MaxY
                && MinZ < another.MaxZ
                && MaxZ > another.MinZ;
        }

        private struct BBoxEnumerator : IEnumerator<Coord>
        {
            public int Index;
            public BoundingBox3d BBox;
            public Coord Current
                => Index switch
                {
                    0 => (BBox.MinX, BBox.MinY, BBox.MinZ),
                    1 => (BBox.MaxX, BBox.MinY, BBox.MinZ),
                    2 => (BBox.MaxX, BBox.MaxY, BBox.MinZ),
                    3 => (BBox.MinX, BBox.MaxY, BBox.MinZ),
                    4 => (BBox.MinX, BBox.MinY, BBox.MaxZ),
                    5 => (BBox.MaxX, BBox.MinY, BBox.MaxZ),
                    6 => (BBox.MaxX, BBox.MaxY, BBox.MaxZ),
                    7 => (BBox.MinX, BBox.MaxY, BBox.MaxZ),
                    _ => (0, 0)
                };

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
                => (++Index) <= 7;

            public void Reset()
                => Index = -1;
        }
        public IEnumerator<Coord> GetEnumerator()
            => new BBoxEnumerator { Index = -1, BBox = this };

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public Coord Center => ((MinX + MaxX) / 2, (MinY + MaxY) / 2, (MinZ + MaxZ) / 2);
        public Coord Min => (MinX, MinY, MinZ);
        public Coord Max => (MaxX, MaxY, MaxZ);
        public Coord this[int index]
            => index switch
            {
                0 => (MinX, MinY, MinZ),
                1 => (MaxX, MinY, MinZ),
                2 => (MaxX, MaxY, MinZ),
                3 => (MinX, MaxY, MinZ),
                4 => (MinX, MinY, MaxZ),
                5 => (MaxX, MinY, MaxZ),
                6 => (MaxX, MaxY, MaxZ),
                7 => (MinX, MaxY, MaxZ),
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };
    }
}
