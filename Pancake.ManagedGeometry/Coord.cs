using Pancake.ManagedGeometry.Utility;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("({X}, {Y}, {Z})")]
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
    public readonly struct Coord : IEquatable<Coord>
    {
        public Coord(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        internal const double Tolerance = 1e-7;
        public bool IdenticalTo(Coord b)
        {
            return IdenticalTo(b, Tolerance);
        }
        public bool IdenticalTo(Coord b, double tol)
        {
            return Math.Abs(X - b.X) < tol && Math.Abs(Y - b.Y) < tol && Math.Abs(Z - b.Z) < tol;
        }

        public Coord(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static Coord Unset =>
            new(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
        public static Coord Origin => default;

        public override bool Equals(object obj)
        {
            return (obj is Coord another && this.Equals(another));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var num = -14849965;

                num += X.GetHashCode() * 7748113;
                num += Y.GetHashCode() * 7748113;
                num += Z.GetHashCode() * 7748113;

                return num;
            }
        }

        public bool Equals(Coord other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public static bool operator ==(Coord left, Coord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coord left, Coord right)
        {
            return !(left == right);
        }
        public static Coord CrossProduct(Coord a, Coord b)
        {
            return new Coord(a.Y * b.Z - b.Y * a.Z, a.Z * b.X - b.Z * a.X, a.X * b.Y - b.X * a.Y);
        }
        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Coord operator- (Coord a)
        {
            return new Coord(-a.X, -a.Y, -a.Z);
        }
        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Coord operator /(Coord a, double ratio)
        {
            return new Coord(a.X / ratio, a.Y / ratio, a.Z / ratio);
        }
        public static Coord operator *(Coord a, double ratio)
        {
            return new Coord(a.X * ratio, a.Y * ratio, a.Z * ratio);
        }
        public static Coord operator *(double ratio, Coord a)
        {
            return new Coord(a.X * ratio, a.Y * ratio, a.Z * ratio);
        }
        public static double operator *(Coord a, Coord b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
        public double SquareLength => (X * X + Y * Y + Z * Z);

        public static double CrossProductLength(Coord v, Coord w)
        {
            return v.X * w.Y - v.Y * w.X;
        }

        public Coord RotateInXY(Coord center, double angle)
        {
            var x0 = X - center.X;
            var y0 = Y - center.Y;
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            var x = x0 * cos - y0 * sin + center.X;
            var y = x0 * sin + y0 * cos + center.Y;

            return (x, y, Z);
        }

        public static implicit operator Coord((double, double) d) => new Coord(d.Item1, d.Item2);
        public static implicit operator Coord((double, double, double) d) => new Coord(d.Item1, d.Item2, d.Item3);

        public static Coord BipolarToVector(double zenith, double azimuth)
        {
            return new Coord(
                +Math.Cos(zenith) * Math.Sin(azimuth),
                -Math.Cos(zenith) * Math.Cos(azimuth),
                +Math.Sin(zenith)
                );
        }
        public Coord Transform(Matrix44 xform)
        {
            return (xform * this).ThreeDPart;
        }

        public static bool IsColinear(Coord a, Coord b, Coord c)
        {
            var vec = Coord.CrossProduct((b - a), (c - a));
            return vec.X.CloseToZero() && vec.Y.CloseToZero() && vec.Z.CloseToZero();
        }
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
        public Coord Unitize()
        {
            var len = 1 / Length;
            return (X * len, Y * len, Z * len);
        }
        public Coord2d StripDown()
        {
            return (X, Y);
        }
    }
}
