using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Pancake.ManagedGeometry.Utility.MathUtils;

namespace Pancake.ManagedGeometry.Utility
{
    public static class GeometricExtensions
    {
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
        public static bool CloseToZero(this double v, double tolerance)
        {
            return v > -tolerance && v < tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenRange(this double v, double from, double to)
        {
            return from - ZeroTolerance < v && v < to + ZeroTolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BetweenRange(this double v, double from, double to, double tolerance)
        {
            return from - tolerance < v && v < to + tolerance;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTinyAngle(this double v)
        {
            return v < OneTenthDegree;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool IsValid(this double number)
        {
            var val = (ulong)(*(long*)&number & 0x7FFFFFFFFFFFFFFFL);
            return val < 0x7FF0000000000000uL;

            // return !double.IsInfinity(number) && !double.IsNaN(number);
        }
    }
}
