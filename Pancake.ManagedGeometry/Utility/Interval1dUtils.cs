using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public static class Interval1dUtils
    {
        public static ICollection<Interval1d> SplitAt(
            this IEnumerable<Interval1d> src,
            IEnumerable<double> points,
            double tolerance = MathUtils.ZeroTolerance)
        {
            return SplitAtSorted(src, points.OrderBy(p => p), tolerance);
        }

        public static ICollection<Interval1d> SplitAtSorted(
            this IEnumerable<Interval1d> src,
            IEnumerable<double> sorted,
            double tolerance = MathUtils.ZeroTolerance)
        {
            var pts = sorted.ToArray();
            var result = new List<Interval1d>();

            var ptsTemp = new List<double>();

            foreach (var interval in src)
            {
                ptsTemp.Clear();

                foreach (var num in pts)
                {
                    if (interval.ContainsOpen(num, tolerance))
                        ptsTemp.Add(num);
                }

                if (ptsTemp.Count == 0)
                {
                    AddToResult(result, tolerance, interval);
                    continue;
                }

                AddToResult(result, tolerance, (interval.From, ptsTemp[0]));

                for (var i = 0; i < ptsTemp.Count - 1; i++)
                    AddToResult(result, tolerance, (ptsTemp[i], ptsTemp[i + 1]));

                AddToResult(result, tolerance, (ptsTemp[ptsTemp.Count - 1], interval.To));
            }

            return result;
        }

        private static void AddToResult(List<Interval1d> result, double tolerance, Interval1d iv)
        {
            if (iv.Length <= tolerance)
                return;
            result.Add(iv);
        }
    }
}
