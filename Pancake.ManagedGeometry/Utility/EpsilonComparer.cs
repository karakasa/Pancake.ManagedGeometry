using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public class EpsilonComparer : IComparer<double>
    {
        public double Tolerance;
        public EpsilonComparer() : this(MathUtils.ZeroTolerance) { }
        public EpsilonComparer(double tolerance)
        {
            Tolerance = tolerance;
        }
        public int Compare(double x, double y)
        {
            if ((x - y).CloseToZero(Tolerance)) return 0;
            return (x < y) ? -1 : 1;
        }
    }
    public struct EpsilonComparerStruct : IComparer<double>
    {
        public double Tolerance;
        public EpsilonComparerStruct(double tolerance)
        {
            Tolerance = tolerance;
        }
        public int Compare(double x, double y)
        {
            if (x == y || x.CloseTo(y, Tolerance)) return 0;
            return (x < y) ? -1 : 1;
        }
    }
}
