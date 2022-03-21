using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("[{From}, {To}]")]
    public struct Interval1d : IEquatable<Interval1d>
    {
        public double From;
        public double To;

        public Interval1d(double from, double to)
        {
            From = from;
            To = to;
        }

        public Interval1d EnsureOrder()
        {
            if (From < To) return this;
            return (To, From);
        }

        public static Interval1d Unset = (double.NaN, double.NaN);
        public static Interval1d Infinity = (double.NegativeInfinity, double.PositiveInfinity);
        public double Length => To - From;
        public bool IsFiniteValid => From < To && From.IsFinite() && To.IsFinite();
        public bool IsValid => From < To && !IsUnset;
        public bool IsUnset => double.IsNaN(From) || double.IsNaN(To);
        public bool Contains(double t) => t.BetweenRange(From, To);
        public bool Contains(double t, double tolerance) => t.BetweenRange(From, To, tolerance);
        public bool ContainsOpen(double t, double tolerance) => t.BetweenRangeOpen(From, To, tolerance);
        public bool Contains(Interval1d another, double tolerance)
            => Contains(another.From, tolerance) && Contains(another.To, tolerance);

        public bool Equals(Interval1d other)
        {
            if (IsUnset)
                return other.IsUnset;

            return other.From == From && other.To == To;
        }

        public override bool Equals(object obj)
        {
            if (obj is Interval1d another) return Equals(another, this);
            return false;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() * (-125487) + To.GetHashCode();
        }

        public static implicit operator Interval1d((double, double) d) => new Interval1d(d.Item1, d.Item2);

        public static bool operator ==(Interval1d left, Interval1d right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Interval1d left, Interval1d right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            return $"[{From}, {To}]";
        }
    }
}