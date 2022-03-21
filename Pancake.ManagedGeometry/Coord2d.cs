using Pancake.ManagedGeometry.Utility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("({X}, {Y})")]
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 16)]
    public struct Coord2d
    {
        public double X;
        public double Y;

        public Coord2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Rotate(Coord2d center, double angle)
        {
            var x0 = X - center.X;
            var y0 = Y - center.Y;
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            X = x0 * cos - y0 * sin + center.X;
            Y = x0 * sin + y0 * cos + center.Y;
        }

        public void Rotate(double angle)
        {
            var x0 = X;
            var y0 = Y;
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            X = x0 * cos - y0 * sin;
            Y = x0 * sin + y0 * cos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CrossProductLength(Coord2d v, Coord2d w)
        {
            return v.X * w.Y - v.Y * w.X;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coord2d ChopZero(double tolerance)
        {
            return new Coord2d(X.CloseToZero(tolerance) ? 0.0 : X, Y.CloseToZero(tolerance) ? 0.0 : Y);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double CrossProduct(Coord2d secondOperand) => CrossProductLength(this, secondOperand);
        public bool ParallelTo(Coord2d another)
        {
            return CrossProductLength(this, another).CloseToZero();
        }
        public double AngleWith(Coord2d another)
        {
            return Math.Acos(this * another / (another.Length * this.Length));
        }
        public double AcuteAngleWith(Coord2d another)
        {
            return MathUtils.PiAngleToAcuteAngle(AngleWith(another));
        }
        public bool AlmostParallelTo(Coord2d another)
        {
            return AcuteAngleWith(another).IsTinyAngle();
        }
        public bool SameDirection(Coord2d another) 
            => (this * another).CloseToZero() && ParallelTo(another);
        public static Coord2d operator +(Coord2d a, Coord2d b) => new Coord2d(a.X + b.X, a.Y + b.Y);
        public static Coord2d operator -(Coord2d a, Coord2d b) => new Coord2d(a.X - b.X, a.Y - b.Y);
        public static Coord2d operator -(Coord2d a) => new Coord2d(-a.X, -a.Y);
        public static double operator *(Coord2d a, Coord2d b) => a.X * b.X + a.Y * b.Y;
        public static Coord2d operator *(Coord2d a, double t) => new Coord2d(a.X * t, a.Y * t);
        public static Coord2d operator *(double t, Coord2d a) => a * t;
        public static Coord2d operator /(Coord2d a, double t) => a * (1 / t);
        public static implicit operator Coord2d((double, double) d) => new Coord2d(d.Item1, d.Item2);
        public double Length => Math.Sqrt(X * X + Y * Y);
        public double SquareLength => X * X + Y * Y;
        public static readonly Coord2d Zero = (0, 0);
        public static readonly Coord2d Unset = (double.NaN, double.NaN);
        public Coord2d Transform(Matrix44 xform)
        {
            return (xform * this).TwoDPart;
        }

        public bool AlmostEqualTo(Coord2d another)
        {
            return
                (X - another.X).CloseToZero()
                && (Y - another.Y).CloseToZero();
        }

        public static bool IsColinear(Coord2d a, Coord2d b, Coord2d c)
        {
            return CrossProductLength(b - a, c - a).CloseToZero();
        }
        public bool IsValid => X.IsFinite() && Y.IsFinite();
        public Coord2d Unitize()
        {
            var length = Length;

            if (length.CloseToZero()) return Coord2d.Unset;

            return (X / length, Y / length);
        }
    }
}
