using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public struct NonOverlappingInterval1dComparer : IComparer<Interval1d>
    {
        private readonly double _tolerance;
        public NonOverlappingInterval1dComparer(double tolerance)
        {
            _tolerance = tolerance;
        }
        public int Compare(Interval1d x, Interval1d y)
        {
            var deltaFrom = x.From - y.From;
            var deltaTo = x.To - y.To;

            if (deltaFrom.CloseToZero(_tolerance))
            {
                if (deltaTo.CloseToZero(_tolerance))
                    return 0;

                deltaFrom = deltaTo;
            }

            return (deltaFrom < 0) ? -1 : 1;
        }
    }
}
