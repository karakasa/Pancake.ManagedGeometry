using Pancake.ManagedGeometry.Utility;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("({X}, {Y})")]
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 16)]
    public readonly struct Coord2d : IEquatable<Coord2d>
    {
        public readonly double X;
        public readonly double Y;
    
        public Coord2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Rotate the point around another point.
        /// </summary>
        /// <param name="center">Rotation center</param>
        /// <param name="angle">Angle. Counter-clockwise.</param>
        /// <returns>Rotated point</returns>
        public Coord2d Rotate(Coord2d center, double angle)
        {
            var x0 = X - center.X;
            var y0 = Y - center.Y;
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            var x = x0 * cos - y0 * sin + center.X;
            var y = x0 * sin + y0 * cos + center.Y;

            return new(x, y);
        }
        /// <summary>
        /// Rotate the point around the origin point (0,0).
        /// </summary>
        /// <param name="angle">Angle. Counter-clockwise.</param>
        /// <returns>Rotated point</returns>
        public Coord2d Rotate(double angle)
        {
            var x0 = X;
            var y0 = Y;
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            var x = x0 * cos - y0 * sin;
            var y = x0 * sin + y0 * cos;

            return new(x, y);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Coord2d ThrowHelper_OutOfRange()
        {
            throw new ArgumentOutOfRangeException("dir");
        }
        public Coord2d Rotate(ClockwiseDirection dir)
        {
            return dir switch
            {
                ClockwiseDirection.Clockwise => new(Y, -X),
                ClockwiseDirection.CounterClockwise => new(-Y, X),
                _ => ThrowHelper_OutOfRange()
            };
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
        public static bool operator ==(Coord2d a, Coord2d b) => a.Equals(b);
        public static bool operator !=(Coord2d a, Coord2d b) => !a.Equals(b);
        public static implicit operator Coord2d((double, double) d) => new Coord2d(d.Item1, d.Item2);
        public double Length => Math.Sqrt(X * X + Y * Y);
        public double SquareLength => X * X + Y * Y;
        public static Coord2d Zero => default;
        public static Coord2d Unset => new(double.NaN, double.NaN);
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
        public bool AlmostEqualTo(Coord2d another, double tolerance)
        {
            return
                (X - another.X).CloseToZero(tolerance)
                && (Y - another.Y).CloseToZero(tolerance);
        }

        public static bool IsColinear(Coord2d a, Coord2d b, Coord2d c)
        {
            return CrossProductLength((b - a).Unitize(), (c - b).Unitize()).CloseToZero();
        }
        public static bool IsColinear(Coord2d a, Coord2d b, Coord2d c, double tolerance)
        {
            return CrossProductLength((b - a).Unitize(), (c - b).Unitize()).CloseToZero(tolerance);
        }
        public bool IsValid => X.IsFinite() && Y.IsFinite();
        public Coord2d Unitize()
        {
            var length = Length;

            if (length.CloseToZero()) return Coord2d.Unset;

            return (X / length, Y / length);
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var baseHs = -145785563;
                baseHs += X.GetHashCode() * 77761841;
                baseHs += Y.GetHashCode() * 77761841;
                return baseHs;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is Coord2d c2d) return this.Equals(c2d);
            return false;
        }

        public bool Equals(Coord2d other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
        public Coord Escalate()
        {
            return (X, Y, 0);
        }
    }
}
