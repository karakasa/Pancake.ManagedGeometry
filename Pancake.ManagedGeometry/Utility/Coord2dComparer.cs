using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public class Coord2dComparer : IComparer<Coord2d>
    {
        private readonly double _tolerance;
        public Coord2dComparer(double tolerance)
        {
            _tolerance = tolerance;
        }
        public int Compare(Coord2d x, Coord2d y)
        {
            var result = (x.X - y.X);

            if (result.CloseToZero(_tolerance))
            {
                result = (x.Y - y.Y);
                if (result.CloseToZero(_tolerance))
                    return 0;
            }

            return (result > 0) ? 1 : -1;
        }
    }

    public struct Coord2dComparerStruct : IComparer<Coord2d>
    {
        private readonly double _tolerance;
        public Coord2dComparerStruct(double tolerance)
        {
            _tolerance = tolerance;
        }
        public int Compare(Coord2d x, Coord2d y)
        {
            var result = (x.X - y.X);

            if (result.CloseToZero(_tolerance))
            {
                result = (x.Y - y.Y);
                if (result.CloseToZero(_tolerance))
                    return 0;
            }

            return (result > 0) ? 1 : -1;
        }
    }
}
