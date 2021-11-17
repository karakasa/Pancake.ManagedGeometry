using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pancake.ManagedGeometry.Utility
{
    public static class MathUtils
    {
        public const double ZeroTolerance = 1e-9;
        public const double OneDegree = Math.PI / 180;
        public const double OneTenthDegree = OneDegree / 10.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegreeToRadian(double degree)
        {
            return degree / 180 * Math.PI;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DMSToRadian(double d, double m)
        {
            if (d < 0)
                return -DegreeToRadian(-d + m / 60);
            return DegreeToRadian(d + m / 60);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReverseSinCos(double oneValue)
        {
            return Math.Sqrt(1 - oneValue * oneValue);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clip(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static Coord BipolarToVector(double zenith, double azimuth)
        {
            return new Coord(
                +Math.Cos(zenith) * Math.Sin(azimuth),
                -Math.Cos(zenith) * Math.Cos(azimuth),
                +Math.Sin(zenith)
                );
        }
        public static Coord Average(this IEnumerable<Coord> pts)
        {
            var cnt = 0;
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            foreach (var it in pts)
            {
                x += it.X;
                y += it.Y;
                z += it.Z;
                ++cnt;
            }

            if (cnt == 0)
                return Coord.Unset;

            return new Coord(x / cnt, y / cnt, z / cnt);
        }
        public static void MinMax(this IEnumerable<double> nums, out double min, out double max)
        {
            var _min = double.MaxValue;
            var _max = double.MinValue;

            foreach (var it in nums)
            {
                if (it < _min) _min = it;
                if (it > _max) _max = it;
            }

            min = _min;
            max = _max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CloseToZero(this double v)
        {
            return v > -ZeroTolerance && v < ZeroTolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenRange(this double v, double from, double to)
        {
            return from - ZeroTolerance < v && v < to + ZeroTolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTinyAngle(this double v)
        {
            return v < OneTenthDegree;
        }

        public static int UnboundIndex(this int index, int length)
        {
            if (index < 0)
            {
                index = length - (-index) % length;
            }
            else
            {
                index %= length;
            }

            return index;
        }
        public static double PiAngleToAcuteAngle(double angle)
        {
            if (angle > Math.PI / 2) return Math.PI - angle;
            return angle;
        }
    }
}
