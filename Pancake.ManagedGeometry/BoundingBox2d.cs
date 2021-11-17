using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public struct BoundingBox2d
    {
        public double MinX;
        public double MaxX;

        public double MinY;
        public double MaxY;

        public BoundingBox2d(IEnumerable<Coord2d> pts)
        {
            var unset = true;

            MinX = MaxX = MinY = MaxY = 0;

            foreach (var it in pts)
            {
                if (unset)
                {
                    MinX = MaxX = it.X;
                    MinY = MaxY = it.Y;
                    unset = false;
                }
                else
                {
                    if (it.X > MaxX) MaxX = it.X;
                    if (it.X < MinX) MinX = it.X;

                    if (it.Y > MaxY) MaxY = it.Y;
                    if (it.Y < MinY) MinY = it.Y;
                }
            }
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
    }
}
