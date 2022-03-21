using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pancake.ManagedGeometry.Algo
{
    public class LineMerger
    {
        public double Tolerance { get; set; } = MathUtils.ZeroTolerance;

        public bool SplitAtOriginalEndPoints { get; set; } = true;

        private List<Line2d> _linesSorted;
        private List<List<Line2d>> _lines;
        public LineMerger(IEnumerable<Line2d> lines)
        {
            _linesSorted = lines.Select(EnsureLineDirection).ToList();
        }

        public void Calculate()
        {
            _lines = UnionFindData.CategorizeData(_linesSorted, DoesLineOverlap).ToNestedLists();
            _linesSorted.Clear();
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
        private bool DoesLineOverlap(Line2d a, Line2d b)
        {
            if (!a.IsValid() || !b.IsValid()) return false;

            var relation = a.IsParallelOrColinear(b, Tolerance);
            if (relation != LineRelation.Collinear) return false;

            return IsPtIncluded(a, b);
        }
        private bool IsPtIncluded(Line2d a, Line2d b)
        {
            if (a.IsOnLine(b.From, Tolerance)
                || a.IsOnLine(b.To, Tolerance)
                || b.IsOnLine(a.From, Tolerance)
                || b.IsOnLine(a.To, Tolerance))
            {
                return true;
            }

            return false;
        }
        private bool IsLineIncluded(Line2d smaller, Line2d larger)
        {
            if (larger.IsOnLine(smaller.From, Tolerance)
                && larger.IsOnLine(smaller.To, Tolerance))
            {
                return true;
            }

            return false;
        }
    }
}
