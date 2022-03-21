using Pancake.ManagedGeometry.Algo.DataStructure;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Pancake.ManagedGeometry.Algo
{
    public class Interval1dSet
    {
        private readonly OrderedListBasicImpl<Interval1d> _orderedList;
        private readonly double _tolerance;
        private readonly NonOverlappingInterval1dComparer _comparer;
        private readonly EpsilonComparer _dblComparer;

        private readonly List<double> _tempListForSplitPoints = new();
        private Interval1d[] _tempArrayInterval = new Interval1d[1];
        public Interval1dSet() : this(MathUtils.ZeroTolerance)
        {

        }

        public Interval1dSet(double tolerance)
        {
            _tolerance = tolerance;
            _comparer = new(_tolerance);
            _dblComparer = new(_tolerance);
            _orderedList = new(null, _comparer);
        }

        public int Count => _orderedList.Count;
        public void Union(Interval1d interval)
        {
            if (interval.Length < _tolerance)
            {
                throw new ArgumentException("Interval smaller than tolerance.");
            }

            if (_orderedList.Count == 0)
            {
                _orderedList.Add(interval);
                return;
            }

            var list = _orderedList.UnderlyingList;
            _tempListForSplitPoints.Clear();

            for (var i = 0; i < list.Count; i++)
            {
                var pt = list[i].From;

                if (interval.ContainsOpen(pt, _tolerance))
                    _tempListForSplitPoints.Add(pt);

                pt = list[i].To;

                if (interval.ContainsOpen(pt, _tolerance))
                    _tempListForSplitPoints.Add(pt);
            }

            if (_tempListForSplitPoints.Count == 0)
            {
                _orderedList.Add(interval);
                return;
            }

            var splitPoints = _tempListForSplitPoints.DistinctByComparer(_dblComparer);

            _tempArrayInterval[0] = interval;
            foreach (var segment in _tempArrayInterval.SplitAtSorted(splitPoints))
            {
                if (list.All(iv => !iv.Contains(segment, _tolerance)))
                    _orderedList.Add(segment);
            }

            _tempListForSplitPoints.Clear();
        }

        public void Compact()
        {
            if (_orderedList.Count <= 1) return;
        }
        public IEnumerable<Interval1d> Intervals => _orderedList;
    }
}
