using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pancake.ManagedGeometry.Algo
{
    public class LineMerger
    {
        // TODO: Reduce memory use.
        public double Tolerance { get; set; } = MathUtils.ZeroTolerance;

        public bool SplitAtOriginalEndPoints { get; set; } = true;

        public List<Line2d> Calculate(IEnumerable<Line2d> lines)
        {
            var linesSorted = lines
                .Select(EnsureLineDirection)
                .Where(l => l.IsValid(Tolerance))
                .ToList();

            var ptComparer = new Coord2dComparer(Tolerance);
            var epsComparer = new EpsilonComparerStruct(Tolerance);

            // The algorithm here is generally O(n^2).
            // There's an O(nlogn) algorithm, using directional angle & cross product as hashes,
            // which, though, is difficult to handle tolerance and may overflow.
            var lineGroups = UnionFindData.CategorizeData(linesSorted,
                (a, b) => Line2d.DoesOverlap(a, b, Tolerance)
                ).ToNestedLists();

            var listOfLines = new List<Line2d>(linesSorted.Count);

            var listOfEnds = new List<double>();

            foreach (var colinearLines in lineGroups)
            {
                if (colinearLines.Count == 0)
                {
                    // Shouldn't happen
                    throw new InvalidOperationException();
                }

                if (colinearLines.Count == 1)
                {
                    // 只有一个元素
                    listOfLines.Add(colinearLines[0]);
                    continue;
                }

                var sortedStartPt = colinearLines.OrderBy(l => l.From, ptComparer).First().From;
                var unitVector = colinearLines[0].Direction.Unitize();

                var set = new Interval1dSet(Tolerance);

                listOfEnds.Clear();

                foreach (var it in colinearLines)
                {
                    var lenAtStart = (it.From - sortedStartPt) * unitVector;
                    var lenAtEnd = (it.To - sortedStartPt) * unitVector;

                    listOfEnds.Add(lenAtStart);
                    listOfEnds.Add(lenAtEnd);

                    set.UnionWith((lenAtStart, lenAtEnd));
                }

                if (!SplitAtOriginalEndPoints)
                    set.Compact();

                var intervals = set.Intervals;

                if (SplitAtOriginalEndPoints)
                {
                    intervals = intervals.SplitAt(
                        listOfEnds.DistinctByComparerInline(epsComparer), Tolerance);
                }

                foreach (var it in intervals)
                {
                    var ptStart = sortedStartPt + unitVector * it.From;
                    var ptEnd = sortedStartPt + unitVector * it.To;

                    var line = new Line2d(ptStart, ptEnd);
                    listOfLines.Add(line);
                }
            }

            return listOfLines;
        }

        private Line2d EnsureLineDirection(Line2d a)
        {
            return CreateFromTwoPt(a.From, a.To);
        }
        private Line2d CreateFromTwoPt(Coord2d a, Coord2d b)
        {
            return IsPtSwapped(a, b) ? new Line2d(b, a) : new Line2d(a, b);
        }

        private bool IsPtSwapped(Coord2d a, Coord2d b)
        {
            if ((a.X - b.X).CloseToZero(Tolerance)) return a.Y > b.Y;
            return a.X > b.X;
        }
    }
}
