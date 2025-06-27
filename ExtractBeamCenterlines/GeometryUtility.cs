using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractBeamCenterlines;

internal class LineWithTraits
{
    public Line2d Line { get; }
    public int Tag { get; }
    public double Angle { get; }
    public double Intercept { get; }

    public LineWithTraits(in Line2d line, int tag = -1)
    {
        var l = line;
        l.EnsurePositiveDirection();

        Line = l;
        Tag = tag;
        Angle = l.Direction.AngleFromAxisX();
        Intercept = GetIntercept(l);
    }

    private static double GetIntercept(in Line2d line)
    {
        const double XTolerance = 0.0001;
        if (Math.Abs(line.Direction.X) < XTolerance)
        {
            return line.From.X;
        }

        return line.From.Y - line.From.X * line.Direction.Y / line.Direction.X;
    }
}

internal static class LineTraitsUtility
{
    public static double AngleTolerance { get; set; } = 0.1 / 180.0 * Math.PI;
    public static double DistanceTolerance { get; set; } = 0.01;
    public static readonly LineTraitsComparer ComparerInstance = new();
    public static readonly LineTraitsEqualityComparer EqualityComparerInstance = new();
    public static readonly AngleOnlyComparer AngleOnlyComparerInstance = new();
    internal sealed class AngleOnlyComparer : IComparer<LineWithTraits>
    {
        public int Compare(LineWithTraits? x, LineWithTraits? y)
        {
            if (Math.Abs(x.Angle - y.Angle) < AngleTolerance) return 0;
            return x.Angle.CompareTo(y.Angle);
        }
    }
    internal sealed class LineTraitsEqualityComparer : IEqualityComparer<LineWithTraits>
    {
        public bool Equals(LineWithTraits? x, LineWithTraits? y)
        {
            return Math.Abs(x.Angle - y.Angle) < AngleTolerance &&
                Math.Abs(x.Intercept - y.Intercept) < DistanceTolerance;
        }

        public int GetHashCode([DisallowNull] LineWithTraits obj)
        {
            throw new NotSupportedException(); 
            // return HashCode.Combine(obj.Angle, obj.Intercept);
        }
    }
    internal sealed class LineTraitsComparer : IComparer<LineWithTraits>
    {
        static int Compare(double x, double y, double tol)
        {
            var val = x - y;
            if (val < -tol)
            {
                return -1;
            }
            else if (val > tol)
            {
                return 1;
            }
            return 0;
        }
        public int Compare(LineWithTraits? x, LineWithTraits? y)
        {
            var val = Compare(x.Angle, y.Angle, AngleTolerance);
            if (val != 0) return val;

            val = Compare(x.Intercept, y.Intercept, DistanceTolerance);
            if (val != 0) return val;

            if (Math.Abs(x.Line.Direction.X) < DistanceTolerance)
            {
                val = Compare(x.Line.From.Y, y.Line.From.Y, DistanceTolerance);
                if (val != 0) return val;

                val = Compare(x.Line.To.Y, y.Line.To.Y, DistanceTolerance);
                return val;
            }
            else
            {
                val = Compare(x.Line.From.X, y.Line.From.X, DistanceTolerance);
                if (val != 0) return val;

                val = Compare(x.Line.To.X, y.Line.To.X, DistanceTolerance);
                return val;
            }
        }
    }
}

internal static class GeometryUtility
{
    public static void GetByLine(List<LineWithTraits> list)
    {
        list.Sort(LineTraitsUtility.ComparerInstance);
    }
}
