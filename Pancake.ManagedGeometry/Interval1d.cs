using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("({From}, {To})")]
    public struct Interval1d
    {
        public double From;
        public double To;

        public Interval1d(double from, double to)
        {
            From = from;
            To = to;
        }

        public bool Contains(double t) => t.BetweenRange(From, To);
        public bool Contains(double t, double tolerance) => t.BetweenRange(From, To, tolerance);
    }
}