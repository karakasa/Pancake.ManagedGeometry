using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public class EpsilonComparer : IComparer<double>
    {
        private double _tolerance;
        public EpsilonComparer() : this(MathUtils.ZeroTolerance) { }
        public EpsilonComparer(double tolerance)
        {
            _tolerance = tolerance;
        }
        public int Compare(double x, double y)
        {
            if ((x - y).CloseToZero(_tolerance)) return 0;
            return (x < y) ? -1 : 1;
        }
    }
}
