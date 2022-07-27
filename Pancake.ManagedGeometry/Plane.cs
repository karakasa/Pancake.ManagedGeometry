using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public struct Plane : ICloneable
    {
        public Coord Origin { private set; get; }
        public Coord AxisX { private set; get; }
        public Coord AxisY { private set; get; }
        public Coord AxisZ { private set; get; }

        public static Plane CreateFromComponents(Coord origin, Coord x, Coord y)
        {
            var pl = new Plane();
            pl.Origin = origin;
            pl.AxisX = x;
            pl.AxisY = y;
            pl.AxisZ = Coord.CrossProduct(x, y);

            pl.NormalizeVectors();
            return pl;
        }
        public Plane(Coord a, Coord b, Coord c)
        {
            Origin = a;
            AxisX = b - a;
            AxisZ = Coord.CrossProduct(AxisX, c - a);
            AxisY = Coord.CrossProduct(AxisZ, AxisX);

            NormalizeVectors();
        }

        public Plane(Plane plane)
        {
            Origin = plane.Origin;
            AxisX = plane.AxisX;
            AxisY = plane.AxisY;
            AxisZ = plane.AxisZ;
        }

        public void NormalizeVectors()
        {
            AxisX /= AxisX.Length;
            AxisY /= AxisY.Length;
            AxisZ /= AxisZ.Length;
        }

        public void ProjectedParameter(Coord from, out double s, out double t)
        {
            var v = from - Origin;
            s = AxisX * v;
            t = AxisY * v;
        }

        public Coord2d ProjectedParameter(Coord from)
        {
            var v = from - Origin;
            var s = AxisX * v;
            var t = AxisY * v;

            return new Coord2d(s, t);
        }

        public Coord Projected(Coord from)
        {
            ProjectedParameter(from, out var s, out var t);
            return PointAt(s, t);
        }
        public Coord MapToParametricSpace(Coord from)
        {
            var v = from - Origin;
            var s = AxisX * v;
            var t = AxisY * v;
            var k = AxisZ * v;
            return new Coord(s, t, k);
        }

        public FastVector2d MapToParametricSpaceInternal(Coord vec)
        {
            var v = MapToParametricSpace(vec) - MapToParametricSpace(Coord.Origin);
            return new FastVector2d(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord PointAt(double s, double t)
        {
            return Origin + s * AxisX + t * AxisY;
        }

        public bool IntersectWithRay(Coord pt, Coord vec, out double s, out double t, out double k)
        {
            var X = pt.X - Origin.X;
            var Y = pt.Y - Origin.Y;
            var Z = pt.Z - Origin.Z;

            var denom = AxisX.Z * vec.Y * AxisY.X - AxisX.Y * vec.Z * AxisY.X - AxisX.Z * vec.X * AxisY.Y + AxisX.X * vec.Z * AxisY.Y + AxisX.Y * vec.X * AxisY.Z - AxisX.X * vec.Y * AxisY.Z;
            if (Math.Abs(denom) <= 1e-9)
            {
                s = t = k = 0;
                return false;
            }

            s = (vec.Z * AxisY.Y * X - vec.Y * AxisY.Z * X - vec.Z * AxisY.X * Y + vec.X * AxisY.Z * Y + vec.Y * AxisY.X * Z - vec.X * AxisY.Y * Z) / denom;
            t = (AxisX.Z * vec.Y * X - AxisX.Y * vec.Z * X - AxisX.Z * vec.X * Y + AxisX.X * vec.Z * Y + AxisX.Y * vec.X * Z - AxisX.X * vec.Y * Z) / denom;
            k = (AxisX.Z * AxisY.Y * X - AxisX.Y * AxisY.Z * X - AxisX.Z * AxisY.X * Y + AxisX.X * AxisY.Z * Y + AxisX.Y * AxisY.X * Z - AxisX.X * AxisY.Y * Z) / denom;

            if (k < 0 && Math.Abs(k) > 1e-9)
                return false;

            return true;
        }

        /// <summary>
        /// 快速求某退化向量表达的射线和 XY 平面交点的算法
        /// </summary>
        /// <param name="pt">出发点</param>
        /// <param name="vec">退化向量，可以用 <see cref="FastVector2d.FastVector2d(Coord)"/> 初始化。</param>
        /// <param name="s">X</param>
        /// <param name="t">Y</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastIntersectWorldXY(Coord pt, FastVector2d vec, ref double s, ref double t)
        {
            if (vec.Direction == 0)
                return false;

            if (vec.Direction > 0)
            {
                if (pt.Z > -1e-9)
                    return false;

                s = pt.X - vec.X * pt.Z;
                t = pt.Y - vec.Y * pt.Z;

                return true;
            }
            else
            {
                if (pt.Z < 1e-9)
                    return false;

                s = pt.X + vec.X * pt.Z;
                t = pt.Y + vec.Y * pt.Z;

                return true;
            }
        }

        public Plane Clone()
        {
            return new Plane(this);
        }
        object ICloneable.Clone()
        {
            return Clone();
        }

        public bool OnPlane(Coord ptr)
        {
            return (Projected(ptr) - ptr).SquareLength < MathUtils.ZeroTolerance;
        }
        public bool CoplanarWith(Plane other)
        {
            if (Math.Abs(other.AxisZ * AxisZ) > MathUtils.ZeroTolerance)
                return false;

            return OnPlane(other.Origin);
        }

        public static bool TryCreateFromPoints(IList<Coord> polygon, out Plane plane)
        {
            if (polygon.Count < 3)
            {
                plane = default;
                return false;
            }

            var ptr1 = polygon[0];
            var ptr2 = polygon[1];
            var vec = ptr2 - ptr1;

            var ptr3 = default(Coord);
            var found = false;
            for (var i = 2; i < polygon.Count; i++)
            {
                ptr3 = polygon[i];
                if (Math.Abs((ptr3 - ptr1) * vec) > MathUtils.ZeroTolerance)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                plane = default;
                return false;
            }

            plane = new Plane(ptr1, ptr2, ptr3);
            return true;
        }
    }
}
