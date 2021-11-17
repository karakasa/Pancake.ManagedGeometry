using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pancake.ManagedGeometry.Utility
{
    internal static class MathUtils
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
        public static double PiAngleToAcuteAngle(double angle)
        {
            if (angle > Math.PI / 2) return Math.PI - angle;
            return angle;
        }
    }
}
